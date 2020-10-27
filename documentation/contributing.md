# Development Notes

# Table of Contents
1. [Running the project](#runningproject)
2. [Running unit tests](#runningtests)
3. [Todo](#todo)

# 1. Running the project <a name="runningproject"></a>

`dotnet build`

cd to the .exe /az-lazy.exe connection --list

# 2. Running unit tests <a name="runningtests"></a>

Unit tests are designed to run against the azure storage emulator, run them with 

`dotnet test`

TODO: change this to use azurite in docker
https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?toc=/azure/storage/blobs/toc.json

# 3. Todo <a name="todo"></a>

1. Make tests run through docker with azurite
2. Add CI process with github