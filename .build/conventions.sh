FALLBACK_ARTIFACTS_DIRECTORY=./artifacts
PWD=$CDP_USER_SOURCE_FOLDER_CONTAINER_PATH
DOCKER_CONTEXT_ROOT=$PWD/MLS.Agent/obj/Docker/publish
WORKSPACES_ROOT=$DOCKER_CONTEXT_ROOT/workspaces
DEPENDENCIES_ROOT=$DOCKER_CONTEXT_ROOT/dependencies
NUGET_PACKAGES=$DOCKER_CONTEXT_ROOT/packages
DOTNET_TOOLS=$DEPENDENCIES_ROOT/.dotnet/tools
