FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY MiriLanding.Api.csproj ./
RUN dotnet restore MiriLanding.Api.csproj
COPY . ./
RUN dotnet publish MiriLanding.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENTRYPOINT ["dotnet", "MiriLanding.Api.dll"]
