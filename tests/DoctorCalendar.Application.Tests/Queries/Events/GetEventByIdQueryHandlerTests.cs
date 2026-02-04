using DoctorCalendar.Application.Dtos;
using DoctorCalendar.Application.Interfaces;
using DoctorCalendar.Application.Queries.Events;
using FluentAssertions;
using Moq;
using Xunit;

public sealed class GetEventByIdQueryHandlerTests
{
    private readonly Mock<IEventReadRepository> _repo = new();
    private readonly GetEventByIdQueryHandler _handler;

    public GetEventByIdQueryHandlerTests()
    {
        _handler = new GetEventByIdQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEvent_WhenEventExists()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        var dto = new EventDto(
            eventId,
            "Consultation",
            "Initial visit",
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            "Scheduled",
            new List<AttendeeDto>(),
            1
        );

        _repo
            .Setup(r => r.GetByIdAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _handler.Handle(new GetEventByIdQuery(eventId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(eventId);
        result.Version.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenEventDoesNotExist()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        _repo
            .Setup(r => r.GetByIdAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EventDto?)null);

        // Act
        var result = await _handler.Handle(new GetEventByIdQuery(eventId), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
