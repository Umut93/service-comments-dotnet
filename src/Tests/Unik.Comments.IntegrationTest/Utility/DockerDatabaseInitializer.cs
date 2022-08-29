using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Unik.Comments.IntegrationTest.Utility;

public static class DockerDatabaseInitializer
{
    public const string DB_DATABASE_MASTER = "master";
    public const string DB_PASSWORD = "5ecretComments?!";
    public const string DB_DATABASE = "Comments";
    public const string DB_USER = "SA";
    public const string DB_IMAGE = "mcr.microsoft.com/mssql/server";
    public const string DB_IMAGE_TAG = "2019-latest";
    public const string DB_CONTAINER_NAME = "IntegrationTesting_Comments";
    public const string DB_VOLUME_NAME = "IntegrationTestingVolume_Accessioning";

    public static async Task<string> EnsureDockerStartedAndGetContainerIdAndPortAsync()
    {
        await CleanupRunningContainers();
        await CleanupRunningVolumes();
        var dockerClient = GetDockerClient();
        var freePort = GetFreePort();

        // This call ensures that the latest SQL Server Docker image is pulled
        await dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
        {
            FromImage = $"{DB_IMAGE}:{DB_IMAGE_TAG}"
        }, null, new Progress<JSONMessage>());

        // create a volume, if one doesn't already exist
        var volumeList = await dockerClient.Volumes.ListAsync();
        var volumeCount = volumeList.Volumes.Count(volume => volume.Name == DB_VOLUME_NAME);
        if (volumeCount <= 0)
        {
            await dockerClient.Volumes.CreateAsync(new VolumesCreateParameters
            {
                Name = DB_VOLUME_NAME,
            });
        }

        // create container, if one doesn't already exist
        var containerList = await dockerClient
            .Containers.ListContainersAsync(new ContainersListParameters { All = true });
        var existingContainer = containerList.FirstOrDefault(container => container.Names.Any(name => name.Contains(DB_CONTAINER_NAME)));

        if (existingContainer is null)
        {
            var sqlContainer = await CreateContainer(dockerClient, freePort);

            await dockerClient
                .Containers
                .StartContainerAsync(sqlContainer.ID, new ContainerStartParameters());

            await WaitUntilDatabaseAvailableAsync(freePort);
            return freePort;
        }

        return existingContainer.Ports[0].PublicPort.ToString();
    }

    private static async Task<CreateContainerResponse> CreateContainer(DockerClient dockerClient, string freePort)
    {
        return await dockerClient
            .Containers
            .CreateContainerAsync(new CreateContainerParameters
            {
                Name = DB_CONTAINER_NAME,
                Image = $"{DB_IMAGE}:{DB_IMAGE_TAG}",
                Env = new List<string>
                {
                    "ACCEPT_EULA=Y",
                    $"User={DB_USER}",
                    $"SA_PASSWORD={DB_PASSWORD}",
                    $"DB_PORT={freePort}",
                    "CATALOG=Comments"
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            "1433/tcp",
                            new PortBinding[]
                            {
                                new PortBinding
                                {
                                    HostPort = freePort
                                }
                            }
                        }
                    },
                    Binds = new List<string>
                    {
                        $"{DB_VOLUME_NAME}:/Accessioning_data"
                    }
                },
            });
    }

    private static bool IsRunningOnWindows()
    {
        return Environment.OSVersion.Platform == PlatformID.Win32NT;
    }

    private static DockerClient GetDockerClient()
    {
        var dockerUri = IsRunningOnWindows()
            ? "npipe://./pipe/docker_engine"
            : "unix:///var/run/docker.sock";
        return new DockerClientConfiguration(new Uri(dockerUri))
            .CreateClient();
    }

    private static async Task CleanupRunningContainers(int hoursTillExpiration = -24)
    {
        var dockerClient = GetDockerClient();

        var runningContainers = await dockerClient.Containers
            .ListContainersAsync(new ContainersListParameters());

        foreach (var runningContainer in runningContainers.Where(container => container.Names.Any(name => name.Contains(DB_CONTAINER_NAME))))
        {
            // Stopping all test containers that are older than 24 hours
            var expiration = hoursTillExpiration > 0
                ? hoursTillExpiration * -1
                : hoursTillExpiration;
            if (runningContainer.Created < DateTime.UtcNow.AddHours(expiration))
            {
                try
                {
                    await EnsureDockerContainersStoppedAndRemovedAsync(runningContainer.ID);
                }
                catch
                {
                    // Ignoring failures to stop running containers
                }
            }
        }
    }

    private static async Task CleanupRunningVolumes(int hoursTillExpiration = -24)
    {
        var dockerClient = GetDockerClient();

        var runningVolumes = await dockerClient.Volumes.ListAsync();

        foreach (var runningVolume in runningVolumes.Volumes.Where(volume => volume.Name == DB_VOLUME_NAME))
        {
            // Stopping all test volumes that are older than 24 hours
            var expiration = hoursTillExpiration > 0
                ? hoursTillExpiration * -1
                : hoursTillExpiration;
            if (DateTime.Parse(runningVolume.CreatedAt) < DateTime.UtcNow.AddHours(expiration))
            {
                try
                {
                    await EnsureDockerVolumesRemovedAsync(runningVolume.Name);
                }
                catch
                {
                    // Ignoring failures to stop running containers
                }
            }
        }
    }

    public static async Task EnsureDockerContainersStoppedAndRemovedAsync(string dockerContainerId)
    {
        var dockerClient = GetDockerClient();
        await dockerClient.Containers
            .StopContainerAsync(dockerContainerId, new ContainerStopParameters());
        await dockerClient.Containers
            .RemoveContainerAsync(dockerContainerId, new ContainerRemoveParameters());
    }

    public static async Task EnsureDockerVolumesRemovedAsync(string volumeName)
    {
        var dockerClient = GetDockerClient();
        await dockerClient.Volumes.RemoveAsync(volumeName);
    }

    private static async Task WaitUntilDatabaseAvailableAsync(string databasePort)
    {
        var start = DateTime.UtcNow;
        const int maxWaitTimeSeconds = 60;
        var connectionEstablished = false;
        while (!connectionEstablished && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
        {
            try
            {
                var sqlConnectionString = GetSqlConnectionString(databasePort, DB_DATABASE_MASTER);
                using var sqlConnection = new SqlConnection(sqlConnectionString);
                await sqlConnection.OpenAsync();
                connectionEstablished = true;
            }
            catch
            {
                // If opening the SQL connection fails, SQL Server is not ready yet
                await Task.Delay(500);
            }
        }

        if (!connectionEstablished)
        {
            throw new Exception($"Connection to the SQL docker database could not be established within {maxWaitTimeSeconds} seconds.");
        }
    }

    private static string GetFreePort()
    {
        // From https://stackoverflow.com/a/150974/4190785
        var tcpListener = new TcpListener(IPAddress.Loopback, 0);
        tcpListener.Start();
        var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
        tcpListener.Stop();
        return port.ToString();
    }

    public static string GetSqlConnectionString(string port, string database)
    {
        return $"Data Source=localhost,{port};" +
            $"Database={database};" +
            $"User ID={DB_USER};" +
            $"Password={DB_PASSWORD};" +
            "TrustServerCertificate=True;" +
            "Integrated Security=False;";
    }
}