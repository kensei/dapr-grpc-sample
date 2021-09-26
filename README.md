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
