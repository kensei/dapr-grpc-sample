# create grpc micro service

## create grpc micro service project 

### create service

```
dotnet new grpc -O userservice2
```

### create proto

```
mkdir proto
dotnet new proto -o User
```

### add proto

```
dotnet-grpc add-file ../hogehoge.protc --services Both
dotnet build
```

### make grpc service

* reference parent root messages file then, add "ProtoRoot="../proto" word to csproj
* show obj/Debug
* add new service class extends generate base class

## create grpc test project 

```
dotnet new unittest -O userservice2.Tests
```

```
dotnet-grpc add-file ../proto/users/Users.proto --services Client
dotnet-grpc add-file ../proto/messages/User.proto
```