# dapr sample with quarts

## used env and library

* vscode remote container
* dapr
* Quartz.NET

## migration

```
cd shared
dotnet ef migrations add CreateUser --startup-project ../api/
dotnet ef database update --startup-project ../api/
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