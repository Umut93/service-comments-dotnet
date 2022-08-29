FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
ENV NUGET_CREDENTIALPROVIDER_SESSIONTOKENCACHE_ENABLED true
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS {\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/uniksystemdesign/62c68d3c-ba53-41cd-9c35-cbf73647e95c/_packaging/unik.saas.components/nuget/v3/index.json\", \"username\":\"my-user@outlook.com\", \"password\":\"put-your-pat-here\"}]}
RUN wget -qO- https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | bash

WORKDIR /src
COPY nuget.config .
COPY ["Unik.Comments.API/Unik.Comments.API.csproj", "Unik.Comments.API/"]
COPY ["Unik.Comments.Domain/Unik.Comments.Domain.csproj", "Unik.Comments.Domain/"]
COPY ["Unik.Comments.Infrastructure/Unik.Comments.Infrastructure.csproj", "Unik.Comments.Infrastructure/"]
RUN dotnet restore "Unik.Comments.API/Unik.Comments.API.csproj"

COPY . .
WORKDIR /src/Unik.Comments.API
RUN dotnet build "Unik.Comments.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Unik.Comments.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unik.Comments.API.dll"]