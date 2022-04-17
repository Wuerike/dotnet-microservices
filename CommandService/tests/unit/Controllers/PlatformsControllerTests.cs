using System;
using System.Collections.Generic;
using System.Linq;
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

public class PlatformControllerTests
{
    private readonly Mock<ICommandRepo> repoMock = new();
    private readonly Random Rand = new();
    private readonly IMapper _mapper;

    public PlatformControllerTests()
    {
        if (_mapper == null)
        {
            var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new PlatformProfile()));
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

    [Fact]
    public void GetPlatforms_WithExistingPlatforms_ReturnsAllItems()
    {
        var expectedPlatform = new[]{CreateRandomPlatform(), CreateRandomPlatform(), CreateRandomPlatform()};
        
        repoMock.Setup(repo => repo.GetAllPlatforms()).Returns(expectedPlatform);
        var controller = new PlatformController(repoMock.Object, _mapper);

        var result = controller.GetPlatforms();
        var expectedPlatformReadDto = _mapper.Map<IEnumerable<PlatformReadDto>>(expectedPlatform);
        result.Value.Should().BeEquivalentTo(expectedPlatformReadDto);
    }

    [Fact]
    public void GetPlatforms_WithNullPlatform_ReturnsNotFound()
    {       
        repoMock.Setup(repo => repo.GetAllPlatforms()).Returns(Enumerable.Empty<Platform>());
        var controller = new PlatformController(repoMock.Object, _mapper);

        var result = controller.GetPlatforms();
        result.Result.Should().BeOfType<NotFoundResult>();
    }
}