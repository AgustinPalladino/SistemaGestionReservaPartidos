using Application.Interfaces;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using SistemaGestion.API.Controllers;
using SistemaGestion.API.Models.Reservations;

namespace SistemaGestion.API.Tests;

public class ReservationsControllerUnitTests
{
    [Fact]
    public async Task Create_WhenRequestIsValid_ReturnsCreatedReservationAndPersistsIt()
    {
        var userId = Guid.NewGuid();
        var fieldId = 5;
        var request = new CreateReservationRequest(userId, fieldId, new DateOnly(2026, 7, 24), new TimeOnly(18, 0), new TimeOnly(19, 0), 120m);

        var reservations = new Mock<IReservationRepository>();
        reservations
            .Setup(r => r.ExistsActiveReservationForUserAsync(userId, fieldId, request.Date, request.StartTime, request.EndTime, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createdReservation = new Reservation
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FieldId = fieldId,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = ReservationStatus.Pending,
            TotalAmount = request.TotalAmount,
            CreatedAt = DateTime.UtcNow
        };

        reservations
            .Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdReservation);

        var fields = new Mock<IFieldRepository>();
        fields.Setup(f => f.GetByIdAsync(fieldId, It.IsAny<CancellationToken>())).ReturnsAsync(new Field { Id = fieldId, Name = "Cancha A", IsActive = true });

        var users = new Mock<IUserRepository>();
        users.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(new User { Id = userId, FirstName = "Ana", LastName = "García", Email = "ana@test.com", PasswordHash = "hash" });

        var schedule = new Mock<IReservationScheduleService>();
        schedule.Setup(s => s.HasScheduleConflictAsync(fieldId, request.Date, request.StartTime, request.EndTime, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var controller = new ReservationsController(reservations.Object, fields.Object, users.Object, schedule.Object);

        var result = await controller.Create(request, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var created = result.Result.As<CreatedAtActionResult>();
        created.ActionName.Should().Be(nameof(ReservationsController.GetById));
        created.StatusCode.Should().Be(StatusCodes.Status201Created);

        reservations.Verify(r => r.AddAsync(It.Is<Reservation>(reservation =>
            reservation.UserId == userId &&
            reservation.FieldId == fieldId &&
            reservation.Date == request.Date &&
            reservation.StartTime == request.StartTime &&
            reservation.EndTime == request.EndTime &&
            reservation.Status == ReservationStatus.Pending), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_WhenFieldDoesNotExist_ReturnsNotFound()
    {
        var request = new CreateReservationRequest(Guid.NewGuid(), 99, new DateOnly(2026, 7, 24), new TimeOnly(18, 0), new TimeOnly(19, 0), 120m);

        var reservations = new Mock<IReservationRepository>();
        var fields = new Mock<IFieldRepository>();
        fields.Setup(f => f.GetByIdAsync(request.FieldId, It.IsAny<CancellationToken>())).ReturnsAsync((Field?)null);

        var users = new Mock<IUserRepository>();
        var schedule = new Mock<IReservationScheduleService>();

        var controller = new ReservationsController(reservations.Object, fields.Object, users.Object, schedule.Object);

        var result = await controller.Create(request, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
        reservations.Verify(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var request = new CreateReservationRequest(userId, 1, new DateOnly(2026, 7, 24), new TimeOnly(18, 0), new TimeOnly(19, 0), 120m);

        var reservations = new Mock<IReservationRepository>();
        var fields = new Mock<IFieldRepository>();
        fields.Setup(f => f.GetByIdAsync(request.FieldId, It.IsAny<CancellationToken>())).ReturnsAsync(new Field { Id = request.FieldId, Name = "Cancha A", IsActive = true });

        var users = new Mock<IUserRepository>();
        users.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var schedule = new Mock<IReservationScheduleService>();

        var controller = new ReservationsController(reservations.Object, fields.Object, users.Object, schedule.Object);

        var result = await controller.Create(request, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
        reservations.Verify(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_WhenScheduleHasConflict_ReturnsConflict()
    {
        var userId = Guid.NewGuid();
        var request = new CreateReservationRequest(userId, 1, new DateOnly(2026, 7, 24), new TimeOnly(18, 0), new TimeOnly(19, 0), 120m);

        var reservations = new Mock<IReservationRepository>();
        var fields = new Mock<IFieldRepository>();
        fields.Setup(f => f.GetByIdAsync(request.FieldId, It.IsAny<CancellationToken>())).ReturnsAsync(new Field { Id = 1, Name = "Cancha A", IsActive = true });

        var users = new Mock<IUserRepository>();
        users.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(new User { Id = userId, FirstName = "Ana", LastName = "García", Email = "ana@test.com", PasswordHash = "hash" });

        var schedule = new Mock<IReservationScheduleService>();
        schedule.Setup(s => s.HasScheduleConflictAsync(request.FieldId, request.Date, request.StartTime, request.EndTime, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var controller = new ReservationsController(reservations.Object, fields.Object, users.Object, schedule.Object);

        var result = await controller.Create(request, CancellationToken.None);

        result.Result.Should().BeOfType<ConflictObjectResult>();
        reservations.Verify(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_WhenDuplicateReservationExists_ReturnsConflict()
    {
        var userId = Guid.NewGuid();
        var request = new CreateReservationRequest(userId, 1, new DateOnly(2026, 7, 24), new TimeOnly(18, 0), new TimeOnly(19, 0), 120m);

        var reservations = new Mock<IReservationRepository>();
        reservations.Setup(r => r.ExistsActiveReservationForUserAsync(userId, request.FieldId, request.Date, request.StartTime, request.EndTime, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var fields = new Mock<IFieldRepository>();
        fields.Setup(f => f.GetByIdAsync(request.FieldId, It.IsAny<CancellationToken>())).ReturnsAsync(new Field { Id = 1, Name = "Cancha A", IsActive = true });

        var users = new Mock<IUserRepository>();
        users.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(new User { Id = userId, FirstName = "Ana", LastName = "García", Email = "ana@test.com", PasswordHash = "hash" });

        var schedule = new Mock<IReservationScheduleService>();
        schedule.Setup(s => s.HasScheduleConflictAsync(request.FieldId, request.Date, request.StartTime, request.EndTime, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var controller = new ReservationsController(reservations.Object, fields.Object, users.Object, schedule.Object);

        var result = await controller.Create(request, CancellationToken.None);

        result.Result.Should().BeOfType<ConflictObjectResult>();
        reservations.Verify(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_WhenRequestHasInvalidTimeRange_ReturnsBadRequest()
    {
        var request = new CreateReservationRequest(Guid.NewGuid(), 1, new DateOnly(2026, 7, 24), new TimeOnly(19, 0), new TimeOnly(18, 0), 120m);

        var reservations = new Mock<IReservationRepository>();
        var fields = new Mock<IFieldRepository>();
        fields.Setup(f => f.GetByIdAsync(request.FieldId, It.IsAny<CancellationToken>())).ReturnsAsync(new Field { Id = 1, Name = "Cancha A", IsActive = true });

        var users = new Mock<IUserRepository>();
        users.Setup(u => u.GetByIdAsync(request.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(new User { Id = request.UserId, FirstName = "Ana", LastName = "García", Email = "ana@test.com", PasswordHash = "hash" });

        var schedule = new Mock<IReservationScheduleService>();

        var controller = new ReservationsController(reservations.Object, fields.Object, users.Object, schedule.Object);

        var result = await controller.Create(request, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        reservations.Verify(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
