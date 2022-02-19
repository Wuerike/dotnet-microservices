using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlatformService.Controllers;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.Profiles;
using Xunit;

namespace PlatformServiceUnitTests;

public class PlatformsControllerTests
{
    private readonly Mock<IPlatformRepo> repoMock = new();
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
        var controller = new PlatformsController(repoMock.Object, _mapper);

        var result = controller.GetPlatforms();
        result.Value.Should().BeEquivalentTo(expectedPlatform);
    }

    [Fact]
    public void GetPlatforms_WithNullPlatform_ReturnsNotFound()
    {       
        repoMock.Setup(repo => repo.GetAllPlatforms()).Returns(Enumerable.Empty<Platform>());
        var controller = new PlatformsController(repoMock.Object, _mapper);

        var result = controller.GetPlatforms();
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void GetPlatformById_WithExistingPlatform_ReturnsExpectedPlatform()
    {
        var expectedPlatform = CreateRandomPlatform();

        repoMock.Setup(repo => repo.GetPlatformById(It.IsAny<int>())).Returns(expectedPlatform);
        var controller = new PlatformsController(repoMock.Object, _mapper);

        var result = controller.GetPlatformById(Rand.Next(100));
        result.Value.Should().BeEquivalentTo(expectedPlatform);
    }

    [Fact]
    public void GetPlatformById_WithNullPlatform_ReturnsNotFound()
    {
        repoMock.Setup(repo => repo.GetPlatformById(It.IsAny<int>())).Returns((Platform)null);
        var controller = new PlatformsController(repoMock.Object, _mapper);

        var result = controller.GetPlatformById(Rand.Next(100));
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void CreatePlatform_ReceivingPlatform_ReturnsCreatedPlatform()
    {
        var platformToCreate = CreateRandomPlatformCreateDto();

        var controller = new PlatformsController(repoMock.Object, _mapper);

        var result = controller.CreatePlatform(platformToCreate);
        var createdPlatform= (result.Result as CreatedAtActionResult).Value as PlatformReadDto;

        createdPlatform.Should().BeEquivalentTo(platformToCreate);
        createdPlatform.Id.Should().BeOfType(typeof(int));
    }
}