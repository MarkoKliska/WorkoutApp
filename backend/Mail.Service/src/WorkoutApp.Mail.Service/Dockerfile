FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Mail.Service/src/WorkoutApp.Mail.Service/WorkoutApp.Mail.Service.csproj", "Mail.Service/src/WorkoutApp.Mail.Service/"]
COPY ["WorkoutApp.Contracts/WorkoutApp.Contracts.csproj", "WorkoutApp.Contracts/"]
RUN dotnet restore "Mail.Service/src/WorkoutApp.Mail.Service/WorkoutApp.Mail.Service.csproj"

COPY ["Mail.Service/src/WorkoutApp.Mail.Service/", "Mail.Service/src/WorkoutApp.Mail.Service/"]
COPY ["WorkoutApp.Contracts/", "WorkoutApp.Contracts/"]

WORKDIR /src/Mail.Service/src/WorkoutApp.Mail.Service
RUN dotnet publish "WorkoutApp.Mail.Service.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "WorkoutApp.Mail.Service.dll"]