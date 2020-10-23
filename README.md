# Az-Lazy
Azure CLI tool for managing everything in azure storage

# Contents

1. Getting started
2. Command list
3. Contributing

# 1. Getting started

Use the following command to add a new connection

`azlazy addconnection --name "dinosaurStorage" --accessKey "<<azure access key>>"`

Select the connection you want to use

`azlazy connection --select "dinosaurStorage"`

You can check which connection is selected the the list command, by default you will always have **devStorage** which allows you to connect to a local azure emulator

`azlazy connection --list`

Output:

```
dinosaurStorage [*] - Added on 19/10/2020
animalStorage - Added on 19/10/2020
devStorage - Added on 19/10/2020
```

Once a connection has been added you can begin using all the other commands, eg

`azlazy queue --list`

Output:

```
importantqueue1
importantqueue2
importantqueue3
```

# 2. Command List

To view a list of commands through the CLI you can use `azlazy --help`

## Connection commands

| Command   |      Description      |
|----------|:-------------:|
| azlazy addconnection --name "name of connection" --connectionstring "connection string" |  Adds a new connection to the connection list. You can also select the connection with `--select true` |
| azlazy connection --help | Display a list of commends you can use for connections   |
| azlazy connection --list | Show a list of connections available, the selected connection will highlighted with a `[*]` symbol |
| azlazy connection --remove "name of connection" | Removes a connection from the connections list |
| azlazy connection --select "name of connection" | Selects a connection from the connections list |

## Queue commands

| Command   |      Description      |
|----------|:-------------:|
| azlazy addqueue --name |  Creates a new queue with the given name |
| azlazy queue --list |  View a list of queues in the storage account |

More comming soon !

# 3. Contributing

I havent writen any contributing guidelines yet but you can reach me [here](https://www.faesel.com/contact) 