# Introduction

Service for managing comments.

# Getting Started

## Multiorigin Git

Setup locally by running `setup-repos.ps1`.
At this time branches cannot use these reserved names [`master`, `develop`, `staging`] as these branches requires Pull requests on all commits.

## Testing

The unit tests in this Microservice, makes use of NSubstitute for easy mocking,\
to get an understanding of how NSubstitute works, you can visit: <https://nsubstitute.github.io/> \
Instead of the built in assertion functionality of xUnit, the project makes user of Fluent Assertions.\
this makes it the assertions much easy to read, for better readable code. and makes it easier to assert very specific issues. \
for more information visit: <https://fluentassertions.com/introduction>

## [Docker](https://dev.azure.com/logicmedia365/Logicmedia/_wiki/wikis/Logicmedia.wiki/1167/Docker)

Run docker compose through VS2022.

- VS2022 adds HTTPS credentials

### Add user credentials to Dockerfile

Replace "email" and "password" in the docker file  like [link](https://dev.azure.com/uniksystemdesign/Unik%20SaaS/_wiki/wikis/Unik-SaaS.wiki/88/Deploy-to-Local-How-To?anchor=deploy-using-docker)
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS {\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/uniksystemdesign/62c68d3c-ba53-41cd-9c35-cbf73647e95c/_packaging/unik.saas.components/nuget/v3/index.json\", \"username\":\"my-user@outlook.com\", \"password\":\"put-your-pat-here\"}]}

## Manually create EF Migration

Migration is created and run through the docker image, but you can manually create by running the following:

- `dotnet ef migrations add --startup-project "src/Unik.Comments.API/Unik.Comments.API.csproj" --project "src/Unik.Comments.Infrastructure/Unik.Comments.Infrastructure.csproj" <unique-migration-name>`
  - Replace `<unique-migration-name>` with unique migration name

## Microsoft SQL Server Managemenet Studio

Connect to the database like: `localhost,<port>` (Currently it would be: `localhost,1440`).

# Build and Test

Build image with the following command: `docker build --pull --rm -f "Unik.Comments.API\Dockerfile" -t servicecommentsdotnet:latest "."` inside the `src` folder.

# Contribute

- Team Rock It
- Architects
- Unik Bolig teams
