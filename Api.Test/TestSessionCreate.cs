using Api.Entity;
using Api.Exceptions;
using Api.Model;
using Api.Repository.Session;
using Api.Services;

namespace Api.Test;

public class SessionRepositoryTest
{
    [Fact]
    public async Task TestSessionCreate()
    {
        // Arrange
        InMemorySessionRepository sessionRepository = new();
        SessionEntity sessionEntity = new()
        {
            Name = "Test Session",
            Duration = TimeSpan.FromSeconds(10),
            Tags = new HashSet<string>(new[] { "Test", "Test2" }),
        };

        // Act
        SessionEntity result = await sessionRepository.AddAsync(sessionEntity);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Id);
        Assert.NotEmpty(result.ETag);
        Assert.NotEqual(default, result.DateAdded);
        Assert.Equal(sessionEntity.Name, result.Name);
    }

    [Fact]
    public async Task DeleteSessionWithCorrectEtag()
    {
        // Arrange
        InMemorySessionRepository sessionRepository = new();
        SessionEntity sessionEntity = new()
        {
            Name = "Test Session",
            Duration = TimeSpan.FromSeconds(10),
            Tags = new HashSet<string>(new[] { "Test", "Test2" }),
        };

        // Act
        SessionEntity result = await sessionRepository.AddAsync(sessionEntity);
        await sessionRepository.DeleteAsync(result.Id, result.ETag);

        // Assert
        Assert.Null(await sessionRepository.GetAsync(result.Id));
    }

    [Fact]
    public async Task DeleteSessionWithIncorrectEtag_ThrowsEntityModifiedException()
    {
        // Arrange
        InMemorySessionRepository sessionRepository = new();
        SessionEntity sessionEntity = new()
        {
            Name = "Test Session",
            Duration = TimeSpan.FromSeconds(10),
            Tags = new HashSet<string>(new[] { "Test", "Test2" }),
        };

        // Act
        SessionEntity result = await sessionRepository.AddAsync(sessionEntity);
        await Assert.ThrowsAsync<EntityModifiedException>(() => sessionRepository.DeleteAsync(result.Id, "IncorrectEtag"));

        // Assert
        Assert.NotNull(await sessionRepository.GetAsync(result.Id));
    }

    [Fact]
    public async Task UpdateSessionWithCorrectEtag()
    {
        // Arrange
        InMemorySessionRepository sessionRepository = new();
        SessionEntity sessionEntity = new()
        {
            Name = "Test Session",
            Duration = TimeSpan.FromSeconds(10),
            Tags = new HashSet<string>(new[] { "Test", "Test2" }),
        };

        // Act
        SessionEntity result = await sessionRepository.AddAsync(sessionEntity);
        SessionEntity updateEntity = new()
        {
            Id = result.Id,
            Name = "Updated Name",
            Duration = result.Duration,
            Tags = result.Tags,
        };
        await sessionRepository.UpdateAsync(updateEntity, result.ETag);

        // Assert
        Assert.Equal("Updated Name", (await sessionRepository.GetAsync(result.Id))?.Name);
    }

    [Fact]
    public async Task UpdateSessionWithIncorrectEtag_ThrowsEntityModifiedException()
    {
        // Arrange
        InMemorySessionRepository sessionRepository = new();
        SessionEntity sessionEntity = new()
        {
            Name = "Test Session",
            Duration = TimeSpan.FromSeconds(10),
            Tags = new HashSet<string>(new[] { "Test", "Test2" }),
        };

        // Act
        SessionEntity result = await sessionRepository.AddAsync(sessionEntity);

        SessionEntity updateEntity = new()
        {
            Id = result.Id,
            Name = "Updated Name",
            Duration = result.Duration,
            Tags = result.Tags,
        };
        await Assert.ThrowsAsync<EntityModifiedException>(() => sessionRepository.UpdateAsync(updateEntity, "IncorrectEtag"));

        // Assert
        Assert.NotEqual("Updated Name", (await sessionRepository.GetAsync(result.Id))?.Name);
    }

    [Fact]
    public async Task UpdateSessionWithIncorrectId_ThrowsEntityNotFoundException()
    {
        // Arrange
        InMemorySessionRepository sessionRepository = new();
        SessionEntity sessionEntity = new()
        {
            Name = "Test Session",
            Duration = TimeSpan.FromSeconds(10),
            Tags = new HashSet<string>(new[] { "Test", "Test2" }),
        };

        // Act
        SessionEntity result = await sessionRepository.AddAsync(sessionEntity);

        SessionEntity updateEntity = new()
        {
            Id = "IncorrectId",
            Name = "Updated Name",
            Duration = result.Duration,
            Tags = result.Tags,
        };
        await Assert.ThrowsAsync<EntityNotFoundException>(() => sessionRepository.UpdateAsync(updateEntity, result.ETag));

        // Assert
        Assert.NotEqual("Updated Name", (await sessionRepository.GetAsync(result.Id))?.Name);
    }

    [Fact]
    public async Task GetSessionWithCorrectId()
    {
        // Arrange
        InMemorySessionRepository sessionRepository = new();
        SessionEntity sessionEntity = new()
        {
            Name = "Test Session",
            Duration = TimeSpan.FromSeconds(10),
            Tags = new HashSet<string>(new[] { "Test", "Test2" }),
        };

        // Act
        SessionEntity result = await sessionRepository.AddAsync(sessionEntity);

        // Assert
        Assert.NotNull(await sessionRepository.GetAsync(result.Id));
    }

    [Fact]
    public async Task GetSessionWithIncorrectId_ReturnsNull()
    {
        // Arrange
        InMemorySessionRepository sessionRepository = new();
        SessionEntity sessionEntity = new()
        {
            Name = "Test Session",
            Duration = TimeSpan.FromSeconds(10),
            Tags = new HashSet<string>(new[] { "Test", "Test2" }),
        };

        // Act
        await sessionRepository.AddAsync(sessionEntity);

        // Assert
        Assert.Null(await sessionRepository.GetAsync("IncorrectId"));
    }

    [Fact]
    public async Task GetSessionsWithNoTags()
    {
        // Arrange
        InMemorySessionRepository sessionRepository = new();
        SessionEntity sessionEntity = new()
        {
            Name = "Test Session",
            Duration = TimeSpan.FromSeconds(10),
            Tags = new HashSet<string>(new[] { "Test", "Test2" }),
        };

        Paging paging = new()
        {
            Page = 0,
            PageSize = 10,
        };

        // Act
        await sessionRepository.AddAsync(sessionEntity);
        var result = await sessionRepository.GetAllAsync(paging, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        Assert.Null(result.Next);
    }

    [Fact]
    public async Task GetSessionsWithTags()
    {
        // Arrange
        InMemorySessionRepository sessionRepository = new();
        SessionEntity sessionEntity = new()
        {
            Name = "Test Session",
            Duration = TimeSpan.FromSeconds(10),
            Tags = new HashSet<string>(new[] { "Test", "Test2" }),
        };

        Paging paging = new()
        {
            Page = 0,
            PageSize = 10,
        };

        // Act
        await sessionRepository.AddAsync(sessionEntity);
        var result = await sessionRepository.GetAllAsync(paging, null, "Test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        Assert.Null(result.Next);
    }

    [Fact]
    public async Task GetSessionsWithTagsAndName()
    {
        // Arrange
        InMemorySessionRepository sessionRepository = new();
        SessionEntity sessionEntity = new()
        {
            Name = "Test Session",
            Duration = TimeSpan.FromSeconds(10),
            Tags = new HashSet<string>(new[] { "Test", "Test2" }),
        };

        Paging paging = new()
        {
            Page = 0,
            PageSize = 1,
        };

        // Act
        await sessionRepository.AddAsync(sessionEntity);
        var result = await sessionRepository.GetAllAsync(paging, "Session", "Test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        Assert.Null(result.Next);
    }
}