# NL.Serverless.AspNetCore
This repo provides code for hosting an AspNet Core App inside an Azure Function V2 HTTP Trigger without the use of TestServer.

I combined the solution of <a href="https://github.com/NicklausBrain">NicklausBrain</a> (original repo can be found <a href="https://github.com/NicklausBrain/serverless-core-api">here</a>) with the recently added <a href="https://docs.microsoft.com/en-US/azure/azure-functions/functions-dotnet-dependency-injection">DI support for azure functions</a>. This provides a way to avoid initalizing the AspNet Core App with every incoming request.

## Prerequisites
1. .Net Core SDK 2.2.402 (Azure Functions don´t support .Net Core 3.0 yet)
2. Latest Azure Function Core Tools
```
npm install -g azure-function-core-tools
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

## Visual Studio 2019 16.3/.Net SDK 3.0 Quirk
Currently you wont be able to build the function project in VS2019 16.3. (<a href="https://developercommunity.visualstudio.com/content/problem/748109/vs2019-163-azure-functions-tools-fails-build-due-t.html">Related bug report</a>)

The reason is that the generation of the function.json fails due to missing assemblies. I think this is caused by the .Net Core 3.0 SDK, since the dotnet.exe is called for creating the function.json.

I created a global.json for the project to force the .Net SDK Version to 2.2.402. With this  workaround you should be able to atleast build and run the function app using the core tools.
```
func host start -build
```
After you started the function app you can attach the Visual Studio Debugger to the func.exe and start debugging your code in Visual Studio.

## Credits
Most of the code is copy pasta from this repo:
https://github.com/NicklausBrain/serverless-core-api

Thanks <a href="https://github.com/NicklausBrain">NicklausBrain</a>.
