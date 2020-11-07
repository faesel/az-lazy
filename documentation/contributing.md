# Development Notes

# Table of Contents
1. [Running the project](#runningproject)
2. [Running unit tests](#runningtests)
3. [Todo](#todo)
4. [Code expectations](#expectations)
5. [Dev Links](#devlinks)

# 1. Running the project <a name="runningproject"></a>

`dotnet build`

cd to the .exe /az-lazy.exe connection --list

To install and run the project you can do the following:

1. `dotnet pack` the az-lazy project
2. cd into the `nupkg` folder
3. Run the following command `dotnet tool install --global --add-source ./ az-lazy`

# 2. Running unit tests <a name="runningtests"></a>

Unit tests are designed to run against the azure storage emulator, run them with 

`dotnet test`

TODO: change this to use azurite in docker
https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?toc=/azure/storage/blobs/toc.json

# 3. Todo <a name="todo"></a>

1. Make tests run through docker with azurite
2. Add CI process with github

# 4. Code expectations <a name="expectations"></a>

1. Ensure there is a integration test for command
2. Ensure command names are easy to understand
3. Ensure commands have a valid alias when possible

# 5. Dev Links <a name="devlinks"></a>

https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-containers-list?tabs=dotnet

Change table to CsConsoleFormat as it supports word wrap

https://stackoverflow.com/questions/856845/how-to-best-way-to-draw-table-in-console-app-c
