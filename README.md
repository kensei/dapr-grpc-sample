# dapr sample with quarts

## used env and library

* vscode remote container
* dapr
* Quartz.NET

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
dapr run --app-id api --app-port 5000 --dapr-grpc-port 50001 --placement-host-address dapr-placement:50010 --components-path ../dapr/components --log-level info -- dotnet run
```

* service exec

```
cd userservice
dapr run --app-id userservice --app-port 5001 --dapr-grpc-port 50002 --placement-host-address dapr-placement:50010 --components-path ../dapr/components --log-level info -- dotnet run
```

## useage

* user create

```
curl http://localhost:5000/api/users -X POST -H "Content-Type: application/json" -d '{"name":"hoge"}'
```

* user get

```
curl http://localhost:5000/api/users/1
```

* user login

```
curl http://localhost:5000/api/users/login -X POST -H "Content-Type: application/json" -d '{"id":1}'
```