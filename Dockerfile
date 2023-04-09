ARG DOTNET_VERSION=6.0.407
ARG DEBIAN_VERSION=11
ARG DEBIAN_FRONTEND=noninteractive
ARG MINIZINC_VERSION=2.7.1
ARG MINIZINC_HOME=/usr/local/share/minizinc
ARG ORTOOLS_VERSION=9.6
ARG ORTOOLS_BUILD=2534
ARG ORTOOLS_HOME=/opt/ortools
ARG USER_NAME=harken
ARG USER_GID=1000
ARG USER_UID=1000

# ------------------------------------
# base
#
# Base operating system
# ------------------------------------
FROM debian:${DEBIAN_VERSION} as base

FROM base as non-root
ARG USER_NAME
ARG USER_GID
ARG USER_UID

# Create the user
RUN groupadd --gid ${USER_GID} ${USER_NAME} \
    && useradd --uid ${USER_UID} --gid ${USER_GID} -m ${USER_NAME} \
    && apt-get update \
    && apt-get install -y sudo \
    && echo ${USER_NAME} ALL=\(root\) NOPASSWD:ALL > /etc/sudoers.d/${USER_NAME} \
    && chmod 0440 /etc/sudoers.d/${USER_NAME}

# ********************************************************
# MiniZinc Builder
#
# This layer installs MiniZinc into the $MINIZINC_HOME
# directory which is later copied to other images.
#
# Google OR-Tools solver for MiniZinc is also installed
#
# ********************************************************
FROM minizinc/minizinc:${MINIZINC_VERSION} as minizinc-builder

# Install required packages
RUN apt-get update && apt-get install -y --no-install-recommends \
        ca-certificates \
        wget \
    && rm -rf /var/lib/apt/lists/*

# Install OR-Tools
ARG DEBIAN_VERSION
ARG MINIZINC_HOME
ARG ORTOOLS_VERSION
ARG ORTOOLS_BUILD
ARG ORTOOLS_HOME
ARG ORTOOLS_VERSION_BUILD=${ORTOOLS_VERSION}.${ORTOOLS_BUILD}
ARG ORTOOLS_TAR_NAME=or-tools_amd64_debian-${DEBIAN_VERSION}_cpp_v${ORTOOLS_VERSION_BUILD}.tar.gz
ARG ORTOOLS_TAR_URL=https://github.com/google/or-tools/releases/download/v${ORTOOLS_VERSION}/${ORTOOLS_TAR_NAME}
ARG ORTOOLS_DIR_NAME=or-tools_x86_64_Debian-${DEBIAN_VERSION}_cpp_v${ORTOOLS_VERSION_BUILD}

# Download and unpack the C++ build for this OS
RUN wget -c ${ORTOOLS_TAR_URL} && \
    tar -xzvf ${ORTOOLS_TAR_NAME}

# Move the files to the correct location
RUN mv ${ORTOOLS_DIR_NAME} ${ORTOOLS_HOME} && \
    cp ${ORTOOLS_HOME}/share/minizinc/solvers/* ${MINIZINC_HOME}/solvers \
    && cp -r ${ORTOOLS_HOME}/share/minizinc/ortools ${MINIZINC_HOME}/ortools \
    && ln -s ${ORTOOLS_HOME}/bin/fzn-ortools /usr/local/bin/fzn-ortools

# Test installation
RUN echo "var 1..9: x; constraint x > 5; solve satisfy;" \
  | minizinc --solver com.google.or-tools --input-from-stdin


# ------------------------------------
# dotnet-sdk
# ------------------------------------
FROM non-root as dotnet-sdk

ARG DOTNET_VERSION
ARG DEBIAN_FRONTEND
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_INSTALL_DIR=/usr/share/dotnet
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        curl \
        ca-certificates \
        libc6 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu67 \
        libssl1.1 \
        libstdc++6 \
        zlib1g \
    && rm -rf /var/lib/apt/lists/*

COPY global.json .

RUN curl -sSL https://dot.net/v1/dotnet-install.sh \
  | bash /dev/stdin \
     --jsonfile global.json \
     --install-dir ${DOTNET_INSTALL_DIR} \
 && ln -s \
     ${DOTNET_INSTALL_DIR}/dotnet \
     /usr/bin/dotnet

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        curl \
        ca-certificates \
        libc6 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu67 \
        libssl1.1 \
        libstdc++6 \
        zlib1g \
    && rm -rf /var/lib/apt/lists/*

# ------------------------------------
# dev
#
# Development environment
# ------------------------------------
FROM dotnet-sdk as dev

ARG USER_NAME
ARG USER_GID
ARG USER_UID
ARG DEBIAN_VERSION
ARG MINIZINC_HOME
ARG ORTOOLS_VERSION
ARG ORTOOLS_BUILD
ARG ORTOOLS_HOME

# Install core packages
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        gnupg2 \
        locales \
        lsb-release \
        wget \
    && rm -rf /var/lib/apt/lists/*

# Install Docker CE CLI
RUN curl -fsSL https://download.docker.com/linux/$(lsb_release -is | tr '[:upper:]' '[:lower:]')/gpg | apt-key add - 2>/dev/null \
    && echo "deb [arch=amd64] https://download.docker.com/linux/$(lsb_release -is | tr '[:upper:]' '[:lower:]') $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list \
    && apt-get update && apt-get install -y --no-install-recommends \
        docker-ce-cli \
    && rm -rf /var/lib/apt/lists/*

# Install Docker Compose
RUN LATEST_COMPOSE_VERSION=$(curl -sSL "https://api.github.com/repos/docker/compose/releases/latest" | grep -o -P '(?<="tag_name": ").+(?=")') \
    && curl -sSL "https://github.com/docker/compose/releases/download/${LATEST_COMPOSE_VERSION}/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose \
    && chmod +x /usr/local/bin/docker-compose

# Install Github CLI
RUN curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg \
    && chmod go+r /usr/share/keyrings/githubcli-archive-keyring.gpg \
    && echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
    && apt update \
    && apt install gh -y \
    && rm -rf /var/lib/apt/lists/*

# Install Developer packages
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        autojump \
        fonts-powerline \
        openssh-client \
        micro \
        less \
        inotify-tools \
        htop \                                                  
        git \    
        tree \
        zsh \
    && rm -rf /var/lib/apt/lists/*    

# # Install MiniZinc + ORTools from the build layer
COPY --from=minizinc-builder $MINIZINC_HOME $MINIZINC_HOME
COPY --from=minizinc-builder /usr/local/bin/ /usr/local/bin/
COPY --from=minizinc-builder $ORTOOLS_HOME $ORTOOLS_HOME

# Install ZSH
USER ${USER_NAME}
ENV HOME=/home/${USER_NAME}
WORKDIR $HOME
COPY .devcontainer/.p10k.zsh .p10k.zsh
RUN sh -c "$(wget -O- https://github.com/deluan/zsh-in-docker/releases/download/v1.1.5/zsh-in-docker.sh)" -- \
    -p git \
    -p docker \
    -p autojump \
    -p https://github.com/zsh-users/zsh-autosuggestions \
    -p https://github.com/zsh-users/zsh-completions \
 && echo "[[ ! -f ~/.p10k.zsh ]] || source ~/.p10k.zsh" >> .zshrc \
 && .oh-my-zsh/custom/themes/powerlevel10k/gitstatus/install
