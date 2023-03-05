# Overview
.Net is a ecosystem of development. unlike other traditional frameworks it contains
multiple frameworks within itself. .net is a platform for building almost anything. 
from a simple console based app to iot devices everything can de developed using blockchain

![What is .NET](./Pictures/dotnet.png "what is dotnet")

### Ecosystem
Before .NET 6 there were multiple different flavours of dotnet. For example
.NET framework was used to develop windows desktop apps, .NET Core is a newer
open-source implementation and is cross-platform and finally Xamarin is the
framework used to develop mobile apps. But with the migration from .NET 5 to 6
all these frameworks were combined into one single SDK(Software development
Kit), and BCL(Base Class Library) Thus we have a basic idea that .NET is a big
ecosystem which has capability to build anything and everywhere. Now this
consists of:

 * Languages — C#, F#, VB
 * Runtime — Common language runtime
 * CLI — Dotnet CLI
 * Libraries — Base class libraries and 3rd party available through Nuget
 
### Runtime
.NET is not just a C# based framework but a common language runtime where multiple
different programming language can compile down to. C#, F# and Visual Basics are
some of the popular languages that compile down to the CLR. The CLR is like 
JVM in java. It follows the same philosophy of "write once run anywhere". 

The
common language runtime makes it easy to design components and applications
whose objects interact across languages. Objects written in different
languages can communicate with each other, and their behaviors can be tightly
integrated. For example, you can define a class and then use a different
language to derive a class from your original class or call a method on the
original class. You can also pass an instance of a class to a method of a
class written in a different language. This cross-language integration is
possible because language compilers and tools that target the runtime use a
common type system defined by the runtime. They follow the runtime's rules for
defining new types and for creating, using, persisting, and binding to types. 

### CLI

dotnet cli is a part of the .NET SDK. It is a cross-platform toolchain for developing,
building, running and publishing .NET applications. In this project we use this cli
to create the webapp, add dependencies, run the application and others. 

### Frameworks
In this project we are going to use ASP.NET for developing the server side application.
We use it to create API and create websocket endpoints for the users, so that
they can communicate with each other in realtime. 


# Building the application

### Installing the SDK
At 1st we should install the SDK and update the path. This process is platform
dependent, the process to install the SDK is well documented
[here](https://dotnet.microsoft.com/en-us/download)

### Creating a and running the project

```bash
dotnet new webapi -f net6.0
dotnet run
```

The above commands will  create and run a project in the parent operating system
Shipping this project to other fellow developer requires the developer to
already have everything setup. This is a tedious task because the dependencies might
mismatch and multiple problem might arise. 

*Knock Knock*

*Who's there*

*Duck*

*Duck who*

*Docker* 

**Docker enters to save the day**

### Containerizing with docker
Jokes aside, Docker is indeed the only thing you will need to containerize your 
application and make it easily deployable. You can learn all about docker 
[here](https://docs.docker.com/)

#### Dockerfile
At 1st we need to create a `Dockerfile` in the root of our project directory and
add the following code in
```Dockerfile
# Building stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /source

COPY . .
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

#Serve stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT [ "dotnet", "chat-room-for-nft-owners.dll" ]

```

* Build stage pulls the sdk image from microsofts repository or locally if
  already available.
  * After that it copies all the file in the project directory to inside the
image with the `copy` command. 
  * The `dotnet restore` command restores all the dependencies as well as
project specific tools as all the file is now added to a brand new image. 
  * The `dotnet publish` compiles the application, reads through its dependencies
specified in the project file, and publishes the resulting set of files to 
app. *(more details [here](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish))*

* Serve stage is responsible for launching the webapp. Here the *Asp.NET* image 
  is downloaded 
  * With `WORKDIR` the working directory is changed to app inside the image.
  * With `COPY --from=build` files from the build image is copied to this new
  ASP.net image. 
  * `ENTRYPOINT` declares the command to run when the container of the image is
  started. In this case `dotnet chat-room-for-nft-owners.dll` launches the application

#### Building the image
Navigate to the directory containing the docker file and run the following 
command

```console  
└❯ docker build -t chatapp-image -f Dockerfile .
Sending build context to Docker daemon  15.56MB
Step 1/9 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
 ---> 434482f08252
Step 2/9 : WORKDIR /source
 ---> Using cache
 ---> 95bafcd947fc
Step 3/9 : COPY . .
 ---> Using cache
 ---> c8ddb9838c81
Step 4/9 : RUN dotnet restore
 ---> Using cache
 ---> cc271e0a3e94
Step 5/9 : RUN dotnet publish -c release -o /app --no-restore
 ---> Using cache
 ---> e8c2888bdbd9
Step 6/9 : FROM mcr.microsoft.com/dotnet/aspnet:6.0
 ---> 227922d32fa7
Step 7/9 : WORKDIR /app
 ---> Using cache
 ---> 7fad601532d8
Step 8/9 : COPY --from=build /app ./
 ---> 75ce7a99a785
Step 9/9 : ENTRYPOINT [ "dotnet", "chat-room-for-nft-owners.dll" ]
 ---> Running in f8accc7080f6
Removing intermediate container f8accc7080f6
 ---> a4c4476f1900
Successfully built a4c4476f1900
Successfully tagged chatapp-image:latest
```

#### Starting the app

```console 
└❯ docker run -it --rm -p 5000:5000 -p 5001:5001 -e ASPNETCORE_HTTP_PORT=https://+:5001 -e ASPNETCORE_URLS=http://+:5000 chatapp-image
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://[::]:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /app/
```
Okay ik ik, the above command is overwhelming so let us break it down into pieced
* `docker run -it -rm` starts the container. The *it* parameter attaches the container
to the *STDIN and STDOUT* and spawns a pseudo *tty* for interative shell. The rm 
parameter stops the container when the tty is closed.
* with `-p 5001:5001` the 5001 port in the container is connected to 5001 of the
host. 
* the `-e` parameter creates environment variables inside the container
* `ASPNETCORE_HTTP_PORT` updates the default port assigned by the framework from
443 to 5001
* `ASPNETCORE_URLS` updates the default port for http from 80 to 5000


#### Accessing the application
We can make requests with browser from http://localhost:5000/swagger. We can
also do this via CLI using a dotnet package called  `httprepl`. We can install 
this package by running the following command
```console
└❮ dotnet tool install -g Microsoft.dotnet-httprepl
```
```console
└❯ httprepl http://localhost:5000
(Disconnected)> connect http://localhost:5000
Using a base address of http://localhost:5000/
Using OpenAPI description at http://localhost:5000/swagger/v1/swagger.json
For detailed tool info, see https://aka.ms/http-repl-doc

http://localhost:5000/> ls
.                 []
WeatherForecast   [GET]

http://localhost:5000/> cd WeatherForecast
/WeatherForecast    [GET]

http://localhost:5000/WeatherForecast> get
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Sun, 05 Mar 2023 14:05:04 GMT
Server: Kestrel
Transfer-Encoding: chunked

[
  {
    "date": 3/6/2023 7:50:04PM,
    "temperatureC": 13,
    "temperatureF": 55,
    "summary": "Mild"
  },
  {
    "date": 3/7/2023 7:50:04PM,
    "temperatureC": 32,
    "temperatureF": 89,
    "summary": "Freezing"
  },
  {
    "date": 3/8/2023 7:50:04PM,
    "temperatureC": -11,
    "temperatureF": 13,
    "summary": "Freezing"
  },
```







