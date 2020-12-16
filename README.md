![](az-lazy/icon.png)

AzLazy CLI tool is designed for developers, it provides a command line interface to quickly manage and make changes to azure storage queues, blobs and tables. The inspiration for this project was to move away from using Azure Storage Manager and provide a faster CLI experience for developers.

# Table of Contents
1. [Installation](#installation)
2. [Getting started](#gettingstarted)
3. [Command list](#commandlist)
    1. [Connection Commands](#connectioncommand)
    2. [Queue Commands](#queuecommand)
    3. [Container Commands](#containercommand)
    4. [Blob Commands](#blobcommand)
    4. [Table Commands](#tablecommand)
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

You can check which connection is selected using the list command, by default you will always have **devStorage** which allows you to connect to a local Azure emulator

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

`azlazy blob --container "dinofiles" --uploadDirectory "C:\Users\faese\Desktop\dinofiles"`

![](/documentation/images/uploaddirectory.png)

# 3. Command List <a name="commandlist"></a>

To view a list of commands through the CLI you can use `azlazy --help`, each command has an alias beginning with the first letter of the command, eg `azlazy connection --list` can be aliased to `azlazy connection -l`.

## i. Connection commands <a name="connectioncommand"></a>

| Command   |      Description      |
|--------------|:-------------|
| `azlazy connection --help` | Display a list of commands you can use for connections |
| `azlazy addconnection --name "name of connection" --connectionstring "connection string"` | Adds a new connection to the connection list. <ul><li>`--select true` The select option allows you to select the connection when inserting</li></ul> |
| `azlazy connection --list` | Show a list of connections available, the selected connection will be highlighted with a `[*]` symbol |
| `azlazy connection --remove "name of connection"` | Removes a connection from the connections list |
| `azlazy connection --select "name of connection"` | Selects a connection from the connections list |
| `azlazy connection --wipe` | Removes all connections from the list, aside from the development connection |

## ii. Queue commands <a name="queuecommand"></a>

| Command   |      Description      |
|--------------|:-------------|
| `azlazy queue --help` |  Display a list of commands you can use for queues  |
| `azlazy addqueue --name "queue to add"` |  Creates a new queue with the given name |
| `azlazy queue --list` |  View a list of queues in the storage account along with the number of messages they are holding, poison queues are highlighted in red. <ul><li>`--contains` can  also be used to filter the list</li></ul> |
| `azlazy queue --remove "queue to remove"` | Removes the queue with the given name |
| `azlazy queue --cure "queue to move poison messages to"` | Moves poison queue messages back into the processing queue |
| `azlazy queue --clear "queue to clear"` | Removes all messages in the queue |
| `azlazy queue --addQueue "queue to add a new message" --addMessage '{ \"test\": true }'` | Adds a new message to the queue |
| `azlazy queue --watch "queue to watch"` | Watches a queue for new messages |
| `azlazy queue --peek "queue to peek messages"` | Views a messages in the queue, note this function peeks messages so visibility is not changed for consuming applications.<ul><li>`--peekCount 10` can also be used to specify how many messages you want to view. The Maximum peek count available is 32.</li></ul> |
| `azlazy queue --from "source queue name" --to "destination queue name"` | Moves queue messages from the source queue to a destination queue |
 
## iii. Container commands <a name="containercommand"></a>

| Command   |      Description      |
|--------------|:-------------|
| `azlazy container --help` | Displays a list of commands you can use for containers |
| `azlazy container --list` | View a list of containers in the storage account, along with whether or not its public and when it was last modified. <ul><li>`--contains` can  also be used to filter the list</li></ul> |
| `azlazy addcontainer --name "container to add" --publicAccess "Blob"` | Creates a new container with the given name. By default any container created will not be publicly accessible and so will have its public access level set to `None`, <ul><li>`--publicAccess` the containers public access level can be set using this command. Possible options are `None`, `Blob`, `BlobContainer`</li></ul>  |
| `azlazy container --remove "container to remove"` | Removes a container with the given game |
| `azlazy container --tree "container to view"` | Returns a tree view of the container, other options with this command include, <ul><li>`--detailed` Command can also be used to view file sizes and last modified dates</li><li>`--depth 2` For large containers this command can be used to limit how deep the folders are traversed</li><li>`--prefix` Command can be used to limit the results returned by searching within a prefixed path</li></ul> |

## iv. Blob commands  <a name="blobcommand"></a>

| Command   |      Description      |
|--------------|:-------------|
| `azlazy blob --help` | Displays a list of commands you can use for blobs |
| `azlazy blob --container "container name" --remove "blob to remove"` | removes a blob from a given container, note the blob name needs to contain the full path |
| `azlazy blob --container "container name" --uploadFile "c:\dinofiles\safaripics\t-rex.png" --uploadPath "safaripics"` | Uploads a blob from a given path to the container. In the example given the file `t-rex.png` will be uploaded to `safaripics\t-rex.png` if no upload path is provided the file will be uploaded to the root of the container. <ul><li>`--uploadPath` can also be optionally specified to copy the files into a subdirectory.</li></ul> |
| `azlazy blob --container "container name" --uploadDirectory "c:\dinofiles" --uploadPath "safaripics"` | Upload all the files in a given directory to a container. When uploading, the folder structure of of the directory is reflected into the container. <ul><li>`--uploadPath` can also be optionally specified to copy the files into a subdirectory.</li></ul> |

## v. Table commands <a name="tablecommand"></a>

| Command   |      Description      |
|--------------|:-------------|
| `azlazy table --help` | Displays a list of commands you can use for tables |
| `azlazy table --list` | View a list of tables in the storage account. <ul><li>`--contains` can  also be used to filter the list</li></ul>  |
| `azlazy table --sample "table name"` | Samples the data from a table, by default 10 rows will be selected. <ul><li>`--sampleCount 30` can also be used to set how many rows should be returned</li></ul> |
| `azlazy table --query "table name"` | Query's rows in a table, the filters specified below can be used in combination with each other. If no filters are specified all rows in the table will be returned.  <ul><li>`--partitionKey` used to query by the partition key</li><li>`--rowKey` used to query by the row key</li><li>`--take 10` allows you to limit how many rows are returned</li></ul> |
| `azlazy table --delete "table name"` | Removes rows from a table, to use this command one of the filters mentioned below must be used. <ul><li>`--partitionKey` delete using the partition key</li><li>`--rowKey` delete using the row key</li> |
| `azlazy table --remove "table name"` | Removes the entire table from storage |
| `azlazy addtable --name "table name"` | Creates a new table with the given table name |


More coming soon !

# 4. Contributing <a name="contributing"></a>

I haven't written any contributing guidelines yet but you can reach me here on [Faesel.com contact page](https://www.faesel.com/contact). [Development Notes](documentation/contributing.md) are also available.

# 5. Change Log <a name="changelog"></a>

For older versions check the change log [here](documentation/legacyversionhistory.md), for newer versions check out the [releases page](https://github.com/faesel/az-lazy/releases). 