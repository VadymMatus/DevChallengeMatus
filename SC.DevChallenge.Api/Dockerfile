# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY bin/Release/net5.0/publish/ App/
COPY Input/ App/Input
WORKDIR /App
ENTRYPOINT ["dotnet", "SC.DevChallenge.Api.dll"]
