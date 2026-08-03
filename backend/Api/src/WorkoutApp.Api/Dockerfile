FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Api/src/WorkoutApp.Api/WorkoutApp.Api.csproj", "Api/src/WorkoutApp.Api/"]
COPY ["Api/src/WorkoutApp.Application/WorkoutApp.Application.csproj", "Api/src/WorkoutApp.Application/"]
COPY ["Api/src/WorkoutApp.Domain/WorkoutApp.Domain.csproj", "Api/src/WorkoutApp.Domain/"]
COPY ["Api/src/WorkoutApp.Infrastructure/WorkoutApp.Infrastructure.csproj", "Api/src/WorkoutApp.Infrastructure/"]
COPY ["WorkoutApp.Contracts/WorkoutApp.Contracts.csproj", "WorkoutApp.Contracts/"]
RUN dotnet restore "Api/src/WorkoutApp.Api/WorkoutApp.Api.csproj"

COPY ["Api/src/WorkoutApp.Api/", "Api/src/WorkoutApp.Api/"]
COPY ["Api/src/WorkoutApp.Application/", "Api/src/WorkoutApp.Application/"]
COPY ["Api/src/WorkoutApp.Domain/", "Api/src/WorkoutApp.Domain/"]
COPY ["Api/src/WorkoutApp.Infrastructure/", "Api/src/WorkoutApp.Infrastructure/"]
COPY ["WorkoutApp.Contracts/", "WorkoutApp.Contracts/"]

WORKDIR /src/Api/src/WorkoutApp.Api
RUN dotnet publish "WorkoutApp.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "WorkoutApp.Api.dll"]