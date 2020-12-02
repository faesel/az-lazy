# Development Notes

# Table of Contents
1. [Running the project](#runningproject)
2. [Running unit tests](#runningtests)
3. [Todo](#todo)
4. [Code expectations](#expectations)
5. [Dev Links](#devlinks)

# 1. Running the project <a name="runningproject"></a>

To install and run the project you can do the following:

1. `dotnet pack` the az-lazy project
2. cd into the `nupkg` folder
3. Run the following command `dotnet tool install --global --add-source ./ az-lazy`

Or alternatively

1. `dotnet build` the az-lazy project from the root repository
2. Cd into the az-lazy project > bin > .. > to the executable az-lazy.exe
3. Run commands off the exe produced, eg. `/az-lazy.exe connection --list`

# 2. Running tests locally <a name="runningtests"></a>

Unit tests are designed to run against the Azurite and are dependent on docker, steps are as follows,

1. Change directory to the root of the project
2. Run `docker-compose up` to create an Azurite instance
3. Run `dotnet test`

# 3. Todo <a name="todo"></a>

1. Add CI process with github

# 4. Code expectations <a name="expectations"></a>

1. Ensure there is a integration test for command
2. Ensure command names are easy to understand
3. Ensure commands have a valid alias when possible

# 5. Dev Links <a name="devlinks"></a>

Adding colour to documentation
https://stackoverflow.com/questions/11509830/how-to-add-color-to-githubs-readme-md-file

Spectre.Console
https://spectresystems.github.io/spectre.console/