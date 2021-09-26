# [Choice] .NET version: 5.0, 3.1, 2.1
ARG VARIANT="5.0"
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:0-${VARIANT}

RUN apt-get update && apt-get install -y build-essential g++ mariadb-client

ENV PATH $PATH:/home/vscode/.dotnet/tools
