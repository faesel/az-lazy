![](az-lazy/icon.png)

AzLazy CLI tool is designed for developers, it provides a command line interface to quickly manage and make changes to azure storage queues, blobs and tables.

# Table of Contents
1. [Installation](#installation)
2. [Getting started](#gettingstarted)
3. [Command list](#commandlist)
    1. [Connection Commands](#connectioncommand)
    2. [Queue Commands](#queuecommand)
    3. [Container Commands](#containercommand)
    4. [Blob Commands](#blobcommand)
4. [Contributing](#contributing)
5. [Change Log](#changelog)

# 1. Installation <a name="installation"></a>

You can download the tool from the [Nuget Gallery](https://www.nuget.org/packages/az-lazy/), run the following installation command,

`dotnet tool install --global az-lazy`

# 2. Getting started <a name="gettingstarted"></a>

Use the following command to add a new connection

`azlazy addconnection --name "dinosaurStorage" --connectionString "<<azure access key>>"`

![](documentation/images/addconnection.png)

Select the connection you want to use

`azlazy connection --select "dinosaurStorage"`

![](documentation/images/selectconnection.png)

You can check which connection is selected the the list command, by default you will always have **devStorage** which allows you to connect to a local azure emulator

`azlazy connection --list`

![](/documentation/images/listconnection.png)

Once a connection has been added you can begin using all the other commands, eg

`azlazy queue --list`

![](/documentation/images/listqueues.png)

`azlazy queue --watch "process-carnivores"`

![](/documentation/images/watchqueue.png)

`azlazy queue --peek "process-carnivores" --peekCount 3`

![](/documentation/images/peekmessages.png)

`azlazy addcontainer --name "dinosaurpictures" --publicAccess "Blob"`

![](/documentation/images/addcontainer.png)

`azlazy container --tree "dinofiles" --detailed`

![](/documentation/images/containertree.png)

# 3. Command List <a name="commandlist"></a>

To view a list of commands through the CLI you can use `azlazy --help`, each command has an alias beginning with the first letter of the command, eg `azlazy connection --list` can be aliased to `azlazy connection -l`.

## i. Connection commands <a name="connectioncommand"></a>

| Command   |      Description      |
|--------------|:-------------|
| `azlazy connection --help` | Display a list of commends you can use for connections   |
| `azlazy addconnection --name "name of connection" --connectionstring "connection string"` |  Adds a new connection to the connection list. You can also select the connection with `--select true` |
| `azlazy connection --list` | Show a list of connections available, the selected connection will highlighted with a `[*]` symbol |
| `azlazy connection --remove "name of connection"` | Removes a connection from the connections list |
| `azlazy connection --select "name of connection"` | Selects a connection from the connections list |

## ii. Queue commands <a name="queuecommand"></a>

| Command   |      Description      |
|--------------|:-------------|
| `azlazy queue --help` |  Display a list of commends you can use for queues  |
| `azlazy addqueue --name "queue to add"` |  Creates a new queue with the given name |
| `azlazy queue --list` |  View a list of queues in the storage account along with the number of messages they are holding, poison queues are highlighted in red. You can also filter the list with `--contains` |
| `azlazy queue --remove "queue to remove"` | Removes the queue with the given name |
| `azlazy queue --cure "queue to move poison messages to"` | Moves poison queue messages back into the processing queue |
| `azlazy queue --clear "queue to clear"` | Removes all messages in the queue |
| `azlazy queue --addQueue "queue to add a new message" --addMessage '{ \"test\": true }'` | Adds a new message to the queue |
| `azlazy queue --watch "queue to watch"` | Watches a queue for new messages |
| `azlazy queue --peek "queue to peek messages"` | Views messages in the queue, note this function peeks messages so visibility is not changed for consuming applications. `--peekCount 10` can also be used to specify how many messages you want to view. The Maximum peek count available is 32.  |
| `azlazy queue --from "source queue name" --to "destination queue name"` | Moves queue messages from the source queue to a destination queue |
 
## iii. Container commands <a name="containercommand"></a>

| Command   |      Description      |
|--------------|:-------------|
| `azlazy container --help` | Displays a list of commands you can use for containers |
| `azlazy container --list` | View a list of containers in the storage account, along with whether or not its public and when it was last modified. You can also filter the list with `--contains` |
| `azlazy addcontainer --name "container to add" --publicAccess "Blob"` | Creates a new container with the given name, the containers public access level can be set using `--publicAccess`. Possible options are `None`, `Blob`, `BlobContainer`. By default any container created will not be publicly accessible and so will be set to `None` |
| `azlazy container --remove "container to remove"` | Removes a container with the given game |
| `azlazy container --tree "container to view"` | Returns a tree view of the container `--detailed` command can also be used view file sizes and last modified dates. For large containers `--depth 2` command can be used to limit how deep the folders are traversed |

## iv. Blob commands  <a name="blobcommand"></a>

| Command   |      Description      |
|--------------|:-------------|
| `azlazy blob --help` | Displays a list of commands you can use for blobs |
| `azlazy blob --container "container name" --remove "blob to remove"` | removes a blob from a given container, note the blob name needs to contain the full path |
| `azlazy blob --container "container name" --uploadFile "c:\dinofiles\safaripics\t-rex.png" --uploadPath "safaripics"` | Uploads a blob from a given path to the container, `--uploadPath` allows you to specify the location in blob storage. In the example given the file `t-rex.png` will be uploaded to `safaripics\t-rex.png` if no upload path is provided the file will be uploaded to the root of the container. |

More coming soon !

# 4. Contributing <a name="contributing"></a>

I haven't written any contributing guidelines yet but you can reach me here on [Faesel.com contact page](https://www.faesel.com/contact). [Development Notes](documentation/contributing.md) are also available.

# 5. Change Log <a name="changelog"></a>

For older versions check the change log [here](documentation/legacyversionhistory.md), for newer versions check out the [releases page](https://github.com/faesel/az-lazy/releases). 