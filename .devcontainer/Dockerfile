# syntax=docker/dockerfile:1

# =============================================================================
# Dev container for MiniZinc.Net
#
# Produces a .NET 8 SDK environment with the MiniZinc toolchain and the
# Google OR-Tools (CP-SAT) solver preinstalled, so the test suite and the
# library examples work out of the box. Builds for both amd64 and arm64.
# =============================================================================

ARG MINIZINC_VERSION=2.9.7
ARG MINIZINC_HOME=/usr/local/share/minizinc
ARG ORTOOLS_VERSION=9.15
ARG ORTOOLS_BUILD=6755
ARG ORTOOLS_HOME=/opt/ortools

# -----------------------------------------------------------------------------
# Stage 1: assemble MiniZinc + OR-Tools
#
# The official minizinc/minizinc image (multi-arch) ships gecode, chuffed,
# cbc and highs but NOT OR-Tools, so we drop the matching OR-Tools release in
# and register it as a MiniZinc solver.
# -----------------------------------------------------------------------------
FROM minizinc/minizinc:${MINIZINC_VERSION} AS minizinc-builder

ARG DEBIAN_FRONTEND=noninteractive
ARG MINIZINC_HOME
ARG ORTOOLS_VERSION
ARG ORTOOLS_BUILD
ARG ORTOOLS_HOME
# TARGETARCH is provided automatically by BuildKit (amd64 / arm64).
ARG TARGETARCH

RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    wget \
    && rm -rf /var/lib/apt/lists/*

# OR-Tools publishes a Debian build for amd64 but only an AlmaLinux build for
# aarch64. The AlmaLinux binary is built against glibc 2.28 and runs fine on
# Debian bookworm (glibc 2.36).
RUN set -eux; \
    version_build="${ORTOOLS_VERSION}.${ORTOOLS_BUILD}"; \
    case "${TARGETARCH}" in \
        amd64) asset="or-tools_amd64_debian-12_cpp_v${version_build}.tar.gz" ;; \
        arm64) asset="or-tools_aarch64_AlmaLinux-8.10_cpp_v${version_build}.tar.gz" ;; \
        *) echo "Unsupported TARGETARCH: ${TARGETARCH}" >&2; exit 1 ;; \
    esac; \
    url="https://github.com/google/or-tools/releases/download/v${ORTOOLS_VERSION}/${asset}"; \
    wget -q -O /tmp/ortools.tar.gz "${url}"; \
    mkdir -p "${ORTOOLS_HOME}"; \
    tar -xzf /tmp/ortools.tar.gz -C "${ORTOOLS_HOME}" --strip-components=1; \
    rm /tmp/ortools.tar.gz; \
    # Keep only what's needed to *run* the solver (.so libs, fzn binary, mzn
    # redefinitions). Drop headers, static archives and build-time metadata
    # (~167MB) that are useless at runtime.
    rm -rf "${ORTOOLS_HOME}/include" \
           "${ORTOOLS_HOME}/examples" \
           "${ORTOOLS_HOME}/man" \
           "${ORTOOLS_HOME}/README.md" \
           "${ORTOOLS_HOME}/Makefile" \
           "${ORTOOLS_HOME}/lib/cmake" \
           "${ORTOOLS_HOME}/lib/pkgconfig"; \
    find "${ORTOOLS_HOME}/lib" -name '*.a' -delete

# OR-Tools ships a self-contained MiniZinc integration: its cp-sat.msc points
# (via relative paths) at the fzn-cp-sat binary and the cp-sat redefinitions,
# and the binary finds its shared libs through an rpath of $ORIGIN/../lib. So we
# just expose that solver dir to MiniZinc rather than copying/symlinking pieces.
ENV MZN_SOLVER_PATH=${ORTOOLS_HOME}/share/minizinc/solvers

# Fail the build early if OR-Tools is not correctly registered.
RUN echo "var 1..9: x; constraint x > 5; solve satisfy;" \
    | minizinc --solver cp-sat --input-from-stdin

# -----------------------------------------------------------------------------
# Stage 2: development environment
#
# Microsoft's GA .NET 10 SDK image (the devcontainers/dotnet:1-10.0 image still
# ships a release-candidate SDK). The non-root `vscode` user and zsh/oh-my-zsh
# are layered on by the common-utils devcontainer feature (see devcontainer.json).
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0

ARG MINIZINC_HOME
ARG ORTOOLS_HOME

# MiniZinc + OR-Tools from the builder stage.
COPY --from=minizinc-builder /usr/local/bin/ /usr/local/bin/
COPY --from=minizinc-builder ${MINIZINC_HOME} ${MINIZINC_HOME}
COPY --from=minizinc-builder ${ORTOOLS_HOME} ${ORTOOLS_HOME}

ENV MZN_SOLVER_PATH=${ORTOOLS_HOME}/share/minizinc/solvers

# Confirm the toolchain survived the copy (catches missing runtime deps).
RUN echo "var 1..9: x; constraint x > 5; solve satisfy;" \
    | minizinc --solver cp-sat --input-from-stdin
