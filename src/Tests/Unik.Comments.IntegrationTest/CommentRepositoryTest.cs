using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Unik.Comments.API.Models.V1;
using Unik.Comments.IntegrationTest.Utility;
using Xunit;

namespace Unik.Comments.IntegrationTest;

public class CommentRepositoryTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CommentRepositoryTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Test if a standard comment can be created in the docker container.
    /// </summary>
    [Trait("IntegrationTest", "CreateStandardComment")]
    [Fact]
    public async Task Post_CreateStandardComment_ReturnsOK()
    {
        // Arrange
        var client = _factory.CreateClient();
        var commentContract = new CommentCreateContract
        {
            Content = "Standard comment",
            ParentId = null
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/comments", commentContract);
        response.EnsureSuccessStatusCode();
        var content = JsonConvert.DeserializeObject<CommentContract>(await response.Content.ReadAsStringAsync());

        // Assert
        Assert.NotNull(content);
    }
}