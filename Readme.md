# NL.Serverless.AspNetCore
This repo provides code for hosting an AspNet Core App inside an Azure Function V3 HTTP Trigger without the use of TestServer.

I combined the solution of <a href="https://github.com/NicklausBrain">NicklausBrain</a> (original repo can be found <a href="https://github.com/NicklausBrain/serverless-core-api">here</a>) with the <a href="https://docs.microsoft.com/en-US/azure/azure-functions/functions-dotnet-dependency-injection">DI support for azure functions</a>. This provides a way to avoid initalizing the AspNet Core App with every incoming request.

## Prerequisites
1. .Net Core SDK 3.1.100
2. Azure Function Core Tools v3
```
npm install -g azure-function-core-tools@3
```

## Instructions
1. Install the project template for dotnet and create a new project.
```
dotnet new --install NL.Serverless.AspNetCore.Template
dotnet new serverless-aspnetcore -n Your.New.ProjectName
```
2. Happy coding your AspNet Core WebApp.
3. Run the Azure Function hosting the web app
```
func host start -build
```

## Samples
[Host the ASP.NET Boilerplate sample in an Azure Function](samples/ASP.NET%20Boilerplate)

## Credits
Most of the code is copy pasta from this repo:
https://github.com/NicklausBrain/serverless-core-api

Thanks <a href="https://github.com/NicklausBrain">NicklausBrain</a>.
