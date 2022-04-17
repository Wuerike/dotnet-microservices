using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlatformService.Controllers;
using PlatformService.Data;
using PlatformService.DataServices.Async;
using PlatformService.DataServices.Sync.Http;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.Profiles;
using Xunit;

namespace PlatformServiceUnitTests.Controllers;

public class PlatformsControllerTests
{
    private readonly Mock<IPlatformRepo> repoMock = new();
    private readonly Mock<ICommandDataClient> commandDataClientMock = new();
    private readonly Mock<IMessageBusClient> messageBusClient = new();
    private readonly Random Rand = new();
    private readonly IMapper _mapper;

    public PlatformsControllerTests()
    {
        if (_mapper == null)
        {
            var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new PlatformsProfile()));
            _mapper = mappingConfig.CreateMapper();
        }
    }

    private Platform CreateRandomPlatform()
    {
        return new()
        {
            Id = Rand.Next(100),
            Name = Guid.NewGuid().ToString(),
            Publisher = Guid.NewGuid().ToString(),
            Cost = Rand.Next(100).ToString(),
        };
    }

    private PlatformCreateDto CreateRandomPlatformCreateDto()
    {
        return new()
        {
            Name = Guid.NewGuid().ToString(),
            Publisher = Guid.NewGuid().ToString(),
            Cost = Rand.Next(100).ToString(),
        };
    }

    [Fact]
    public void GetPlatforms_WithExistingPlatforms_ReturnsAllItems()
    {
        var expectedPlatform = new[]{CreateRandomPlatform(), CreateRandomPlatform(), CreateRandomPlatform()};
        
        repoMock.Setup(repo => repo.GetAllPlatforms()).Returns(expectedPlatform);
        var controller = new PlatformsController(repoMock.Object, _mapper, commandDataClientMock.Object, messageBusClient.Object);

        var result = controller.GetPlatforms();
        result.Value.Should().BeEquivalentTo(expectedPlatform);
    }

    [Fact]
    public void GetPlatforms_WithNullPlatform_ReturnsNotFound()
    {       
        repoMock.Setup(repo => repo.GetAllPlatforms()).Returns(Enumerable.Empty<Platform>());
        var controller = new PlatformsController(repoMock.Object, _mapper, commandDataClientMock.Object, messageBusClient.Object);

        var result = controller.GetPlatforms();
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void GetPlatformById_WithExistingPlatform_ReturnsExpectedPlatform()
    {
        var expectedPlatform = CreateRandomPlatform();

        repoMock.Setup(repo => repo.GetPlatformById(It.IsAny<int>())).Returns(expectedPlatform);
        var controller = new PlatformsController(repoMock.Object, _mapper, commandDataClientMock.Object, messageBusClient.Object);

        var result = controller.GetPlatformById(Rand.Next(100));
        result.Value.Should().BeEquivalentTo(expectedPlatform);
    }

    [Fact]
    public void GetPlatformById_WithNullPlatform_ReturnsNotFound()
    {
        repoMock.Setup(repo => repo.GetPlatformById(It.IsAny<int>())).Returns((Platform)null);
        var controller = new PlatformsController(repoMock.Object, _mapper, commandDataClientMock.Object, messageBusClient.Object);

        var result = controller.GetPlatformById(Rand.Next(100));
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreatePlatform_ReceivingPlatform_ReturnsCreatedPlatform()
    {
        var platformToCreate = CreateRandomPlatformCreateDto();

        repoMock.Setup(repo => repo.CreatePlatform(It.IsAny<Platform>()));
        commandDataClientMock.Setup(repo => repo.SendPlatformToCommand(It.IsAny<PlatformReadDto>()));
        messageBusClient.Setup(repo => repo.PublishNewPlatform(It.IsAny<PlatformPublishDto>()));
        var controller = new PlatformsController(repoMock.Object, _mapper, commandDataClientMock.Object, messageBusClient.Object);

        var result = await controller.CreatePlatform(platformToCreate);
        var createdPlatform = (result.Result as CreatedAtActionResult).Value as PlatformReadDto;

        createdPlatform.Should().BeEquivalentTo(platformToCreate);
        createdPlatform.Id.Should().BeOfType(typeof(int));
    }

    [Fact]
    public async Task CreatePlatform_ReceivingPlatform_ExternalClientsWithError_ReturnsCreatedPlatform()
    {
        var platformToCreate = CreateRandomPlatformCreateDto();

        repoMock.Setup(repo => repo.CreatePlatform(It.IsAny<Platform>()));
        commandDataClientMock.Setup(repo => repo.SendPlatformToCommand(It.IsAny<PlatformReadDto>())).Throws(new Exception());
        messageBusClient.Setup(repo => repo.PublishNewPlatform(It.IsAny<PlatformPublishDto>())).Throws(new Exception());
        var controller = new PlatformsController(repoMock.Object, _mapper, commandDataClientMock.Object, messageBusClient.Object);

        var result = await controller.CreatePlatform(platformToCreate);
        var createdPlatform = (result.Result as CreatedAtActionResult).Value as PlatformReadDto;

        createdPlatform.Should().BeEquivalentTo(platformToCreate);
        createdPlatform.Id.Should().BeOfType(typeof(int));
    }
}