# dapr sample with quarts

## description

* grpc microservice
* grpc microservice integratin test
* call grpc microservice integration dest

## used env and library

* vscode remote container
* dapr
* dapper
* grpc
* grpc-sdk : copy code(use grpc integration test)
* moq

## setup

```
dotnet tool install -g dotnet-grpc
```

## migration

```
cd shared
dotnet ef migrations add CreateUser --startup-project ../api/
dotnet ef database update --startup-project ../api/
```

## run

* api exec

```
cd api
dapr run --app-id api --app-port 5000 --placement-host-address dapr-placement:50010 --components-path $HOME/.dapr/components --log-level info -- dotnet run
```

* http service exec

```
cd userservice
dapr run --app-id userservice --app-port 5001 --placement-host-address dapr-placement:50010 --components-path $HOME/.dapr/components --log-level info -- dotnet run
```

* grpc service exec

```
cd userservice2
dapr run --app-id userservice2 --app-port 5002 --app-protocol grpc --placement-host-address dapr-placement:50010 --components-path $HOME/.dapr/components --log-level info -- dotnet run
```

## useage

* user create

```
curl http://localhost:5000/api/v1.0/users -X POST -H "Content-Type: application/json" -d '{"name":"hoge"}' // http micro service
curl http://localhost:5000/api/v1.0/users -X POST -H "Content-Type: application/json" -d '{"name":"hoge"}' // grpc micro service
```

* user get

```
curl http://localhost:5000/api/v1.0/users/1 // http micro service
curl http://localhost:5000/api/v2.0/users/1 // grpc micro service
```

* user login

```
curl http://localhost:5000/api/v1.0/users/login -X POST -H "Content-Type: application/json" -d '{"id":1}' // http micro service
curl http://localhost:5000/api/v1.0/users/login -X POST -H "Content-Type: application/json" -d '{"id":1}' // grpc micro service
```
