using DoctorCalendar.Application.Dtos;
using DoctorCalendar.Application.Interfaces;
using DoctorCalendar.Application.Queries.Events;
using FluentAssertions;
using Moq;
using Xunit;

public sealed class ListEventsQueryHandlerTests
{
    private readonly Mock<IEventReadRepository> _repo = new();
    private readonly ListEventsQueryHandler _handler;

    public ListEventsQueryHandlerTests()
    {
        _handler = new ListEventsQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEvents()
    {
        // Arrange
        var events = new List<EventDto>
        {
            new(
                Guid.NewGuid(),
                "Consultation",
                "Initial",
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                "Scheduled",
                new List<AttendeeDto>(),
                1
            )
        };

        _repo
            .Setup(r => r.ListAsync(
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(events);

        // Act
        var result = await _handler.Handle(
            new ListEventsQuery(null, null, null, null),
            CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Consultation");
    }
}
