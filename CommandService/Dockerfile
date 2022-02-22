FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 6000

ENV ASPNETCORE_URLS=http://+:6000

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["src/CommandService.csproj", "./"]
RUN dotnet restore "CommandService.csproj"
COPY src .
RUN dotnet publish "CommandService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CommandService.dll"]
