# [Choice] .NET version: 5.0, 3.1, 2.1
ARG VARIANT="5.0"
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:0-${VARIANT}

RUN apt-get update && apt-get install -y build-essential g++ mariadb-client

ENV PATH $PATH:/home/vscode/.dotnet/tools

# [Option] Enable non-root Docker access in container
ARG ENABLE_NONROOT_DOCKER="true"
# [Option] Use the OSS Moby CLI instead of the licensed Docker CLI
ARG USE_MOBY="true"

ARG USERNAME=vscode
ARG GO_VERSION="1.17.2"
ARG GO_ARCH="amd64"

# Install needed packages and setup non-root user. Use a separate RUN statement to add your
# own dependencies. A user of "automatic" attempts to reuse an user ID if one already exists.
WORKDIR /home/$USERNAME
COPY library-scripts/docker-debian.sh /tmp/library-scripts/
RUN apt-get update \
    # Use Docker script from script library to set things up
    && /bin/bash /tmp/library-scripts/docker-debian.sh "${ENABLE_NONROOT_DOCKER}" "/var/run/docker-host.sock" "/var/run/docker.sock" "${USERNAME}"
# Install dapr cli
ADD https://raw.githubusercontent.com/dapr/cli/master/install/install.sh ./
RUN sudo chmod g+x install.sh
RUN su $USERNAME
RUN /bin/bash install.sh
# clean up
RUN apt-get autoremove -y && apt-get clean -y && rm -rf /var/lib/apt/lists/* /tmp/library-scripts/
# dapr setup
RUN sudo -u $USERNAME mkdir -p /home/$USERNAME/.dapr/components/
COPY dapr/components/* /home/$USERNAME/.dapr/components/
# Add daprd to the path for the VS Code Dapr extension.
ENV PATH "$PATH:/home/${USERNAME}/.dapr/bin"
# dapr initialize
RUN sudo -u $USERNAME dapr uninstall && sudo -u $USERNAME dapr init --slim

# install grpcurl
ADD "https://golang.org/dl/go${GO_VERSION}.linux-${GO_ARCH}.tar.gz" /go.tgz
RUN tar -C /usr/local/ -xzf /go.tgz
ENV PATH "/usr/local/go/bin:$PATH"
ENV PATH "/home/${USERNAME}/go/bin:$PATH"

# Setting the ENTRYPOINT to docker-init.sh will configure non-root access
# to the Docker socket. The script will also execute CMD as needed.
ENTRYPOINT [ "/usr/local/share/docker-init.sh" ]
CMD [ "sleep", "infinity" ]
