# Microservices with .Net 6 demo

This repository contains two applications, PlatformService and CommandService, which ideally should be in individual repos, but for simplicity of development I've used this single repo.

Even though expose .env files in the repository is a bad practice, I've done it anyway given the demonstrative nature of this project.

## PlatformService

Manage the platforms used in some company, is possible to `add` new platforms, `get one` of them by its id or `get all` platforms from the database.

Every time a new platform is created, the PlatformService sends a message to the RabbitMQ bus, notifying all the subscribers, being the CommandService the only subcriber we get in this cenario.

**Platform model example:**
```
{
  "id": 1,
  "name": "Dotnet",
  "publisher": "Microsoft",
  "cost": "Free"
}
```

## CommandService

Given a platform, this application manages the commands we can run on it, the platforms can't be created in this context, but received from the PlatformService, both sync or async.

By listening the RabbitMq bus the CommandService can be noticed about some new platform in the company, then it stores in its own database a representation of this platform. 

As we can eventually get some problem in the bus or in the API itself, every time the CommandService is lauched, it sends a sync GRPC request to the PlatformService to read all platforms and stores the ones it has missed.

With this API we can `get all platforms` existing in this context, `add a command` for a given platform id, `get all commands` of a specific platform or `get a specific command` of a specific platform.

**Platform model example:**
```
{
  "id": 1,
  "name": "Dotnet"
}
```

**Command model example:**
```
{
  "id": 1,
  "name": "build",
  "commandLine": "dotnet build",
  "platformId": 1
}
```

## List of used concepts

- DTOS with AutoMapper
- Awaitable methods (async/wait)
- Repository Pattern
- Entity Framework
- SQL Server
- RabbitMQ in fanout mode 
- Backgroud Service as RabbitMq listener
- gRPC client/server
- Kubernetes
- XUnit with FluentAssertions

## Contributing

This project is totally open source and contributors are welcome.