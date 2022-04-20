using System;
using System.Collections.Generic;
using AutoMapper;
using CommandService.Controllers;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using CommandService.Profiles;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CommandServiceUnitTests.Controllers;

public class CommandControllerTests
{
    private readonly Mock<ICommandRepo> repoMock = new();
    private readonly Random Rand = new();
    private readonly IMapper _mapper;

    public CommandControllerTests()
    {
        if (_mapper == null)
        {
            var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new CommandProfile()));
            _mapper = mappingConfig.CreateMapper();
        }
    }

    private Platform CreateRandomPlatform()
    {
        return new()
        {
            Id = Rand.Next(100),
            Name = Guid.NewGuid().ToString()
        };
    }

    private Command CreateRandomCommand()
    {
        return new()
        {
            Id = Rand.Next(100),
            Name = Guid.NewGuid().ToString(),
            CommandLine = Guid.NewGuid().ToString(),
            PlatformId = Rand.Next(100)
        };
    }

    private CommandCreateDto CreateRandomCommandCreateDto()
    {
        return new()
        {
            Name = Guid.NewGuid().ToString(),
            CommandLine = Guid.NewGuid().ToString(),
        };
    }

    [Fact]
    public void GetCommandsByPlatformId_WithPlatformExistsFalse_ReturnsNotFound()
    {   repoMock.Setup(repo => repo.PlatformExists(It.IsAny<int>())).Returns(false);
        var controller = new CommandController(repoMock.Object, _mapper);

        var result = controller.GetCommandsByPlatformId(Rand.Next(100));
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void GetCommandsByPlatformId_WithPlatformExistsTrue_ReturnsCommands()
    {       
        var expectedCommand = new[]{CreateRandomCommand(), CreateRandomCommand()};

        repoMock.Setup(repo => repo.PlatformExists(It.IsAny<int>())).Returns(true);
        repoMock.Setup(repo => repo.GetCommandsByPlatformId(It.IsAny<int>())).Returns(expectedCommand);
        var controller = new CommandController(repoMock.Object, _mapper);

        var expectedCommandReadDto = _mapper.Map<IEnumerable<CommandReadDto>>(expectedCommand);
        var result = controller.GetCommandsByPlatformId(Rand.Next(100));

        result.Value.Should().BeEquivalentTo(expectedCommandReadDto);
    }

    [Fact]
    public void GetCommand_WithPlatformExistsFalse_ReturnsNotFound()
    {   repoMock.Setup(repo => repo.PlatformExists(It.IsAny<int>())).Returns(false);
        var controller = new CommandController(repoMock.Object, _mapper);

        var result = controller.GetCommand(Rand.Next(100), Rand.Next(100));
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void GetCommand_WithNullCommand_ReturnsNotFound()
    {   repoMock.Setup(repo => repo.PlatformExists(It.IsAny<int>())).Returns(true);
        repoMock.Setup(repo => repo.GetCommand(It.IsAny<int>(), It.IsAny<int>())).Returns((Command)null);
        var controller = new CommandController(repoMock.Object, _mapper);

        var result = controller.GetCommand(Rand.Next(100), Rand.Next(100));
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void GetCommand_WithValidCommand_ReturnsCommands()
    {       
        var expectedCommand = CreateRandomCommand();

        repoMock.Setup(repo => repo.PlatformExists(It.IsAny<int>())).Returns(true);
        repoMock.Setup(repo => repo.GetCommand(It.IsAny<int>(), It.IsAny<int>())).Returns(expectedCommand);
        var controller = new CommandController(repoMock.Object, _mapper);

        var expectedCommandReadDto = _mapper.Map<CommandReadDto>(expectedCommand);
        var result = controller.GetCommand(Rand.Next(100), Rand.Next(100));

        result.Value.Should().BeEquivalentTo(expectedCommandReadDto);
    }






    [Fact]
    public void CreateCommand_WithPlatformExistsFalse_ReturnsNotFound()
    {   
        var commandToCreate = CreateRandomCommandCreateDto();
        
        repoMock.Setup(repo => repo.PlatformExists(It.IsAny<int>())).Returns(false);
        var controller = new CommandController(repoMock.Object, _mapper);

        var result = controller.CreateCommand(Rand.Next(100), commandToCreate);
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void CreateCommand_WithPlatformExistsTrue_ReturnsCreatedCommand()
    {       
        var commandToCreate = CreateRandomCommandCreateDto();

        repoMock.Setup(repo => repo.PlatformExists(It.IsAny<int>())).Returns(true);
        var controller = new CommandController(repoMock.Object, _mapper);

        var result = controller.CreateCommand(Rand.Next(100), commandToCreate);
        var createdCommand = (result.Result as CreatedAtActionResult).Value as CommandReadDto;
        createdCommand.Should().BeEquivalentTo(commandToCreate);
    }
}