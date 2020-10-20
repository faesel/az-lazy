# Az-Lazy
Azure CLI tool for managing everything in azure storage

# Contents

1. Managing Connections
2. Managing Queues

# Managing Connections

Use the following command to add a new connection

`addconnection --name "dinosaurStorage" --accessKey "<<azure access key>>"`

Select the connection you want to use

`connection --select "dinosaurStorage"`

You can check which connection is selected the the list command, by default you will always have **devStorage** which allows you to connect to a local azure emulator

`connection --list`

Output:

```
dinosaurStorage [*] - Added on 19/10/2020
animalStorage - Added on 19/10/2020
devStorage - Added on 19/10/2020
```

To remove a connection use the remove command,

`connection --remove "dinosaurStorage"`

# Managing queues

Once a valid connection has been added you can  mamage queues, begin by viewing a list what queues are available.

`queue --list`

# Dev Links

Console colors
https://www.c-sharpcorner.com/article/change-console-foreground-and-background-color-in-c-sharp/

Queue rest client
https://docs.microsoft.com/en-us/rest/api/storageservices/queue-service-rest-api


https://docs.microsoft.com/en-us/rest/api/storageservices/authorize-with-shared-key

Color scheme
https://www.color-hex.com/color-palette/99463
