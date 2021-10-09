# create grpc micro service

## create service

```
dotnet new grpc -O userservice2
```

## create proto

```
mkdir proto
dotnet new proto -o User
```

## add proto

```
dotnet-grpc add-file ../hogehoge.protc --services Server
dotnet build
```

## make grpc service

* reference parent root messages file then, add "ProtoRoot="../proto" word to csproj
* show obj/Debug
* add new service class extends generate base class