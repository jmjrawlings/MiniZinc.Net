#!/usr/bin/env bash

set -eu
set -o pipefail
dotnet tool restore

FAKE_DETAILED_ERRORS=true dotnet run --project ./build/build.fsproj -- -t "$@"
