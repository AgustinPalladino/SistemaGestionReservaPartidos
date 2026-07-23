using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SistemaGestion.API.Models.Reservations;

namespace SistemaGestion.API.Tests;

public class ReservationsApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ReservationsApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostReservation_WithAuthenticatedUserAndAvailableSlot_CreatesReservation()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Reservations.RemoveRange(db.Reservations);
        db.Fields.RemoveRange(db.Fields);
        db.Users.RemoveRange(db.Users);
        await db.SaveChangesAsync();

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Ana",
            LastName = "García",
            Email = "ana@test.com",
            PasswordHash = "hash",
            Role = UserRole.Client
        };

        var field = new Field
        {
            Id = 7,
            Name = "Cancha Central",
            SportType = "Fútbol",
            PricePerHour = 120m,
            IsActive = true
        };

        db.Users.Add(user);
        db.Fields.Add(field);
        await db.SaveChangesAsync();

        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var request = new CreateReservationRequest(user.Id, field.Id, new DateOnly(2026, 7, 24), new TimeOnly(18, 0), new TimeOnly(19, 0), 120m);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "valid-token");

        var response = await client.PostAsJsonAsync("/api/reservations", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<Reservation>();
        created.Should().NotBeNull();
        created!.UserId.Should().Be(user.Id);
        created.FieldId.Should().Be(field.Id);

        var persisted = await db.Reservations.SingleAsync();
        persisted.UserId.Should().Be(user.Id);
        persisted.FieldId.Should().Be(field.Id);
        persisted.Status.Should().Be(ReservationStatus.Pending);
    }

    [Fact]
    public async Task PostReservation_WithoutAuthentication_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.PostAsJsonAsync("/api/reservations", new CreateReservationRequest(Guid.NewGuid(), 1, new DateOnly(2026, 7, 24), new TimeOnly(18, 0), new TimeOnly(19, 0), 120m));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostReservation_WhenFieldIsAlreadyBooked_ReturnsConflict()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Reservations.RemoveRange(db.Reservations);
        db.Fields.RemoveRange(db.Fields);
        db.Users.RemoveRange(db.Users);
        await db.SaveChangesAsync();

        var user = new User { Id = Guid.NewGuid(), FirstName = "Ana", LastName = "García", Email = "ana@test.com", PasswordHash = "hash", Role = UserRole.Client };
        var field = new Field { Id = 8, Name = "Cancha Norte", SportType = "Fútbol", PricePerHour = 120m, IsActive = true };
        db.Users.Add(user);
        db.Fields.Add(field);
        db.Reservations.Add(new Reservation
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FieldId = field.Id,
            Date = new DateOnly(2026, 7, 24),
            StartTime = new TimeOnly(18, 0),
            EndTime = new TimeOnly(19, 0),
            Status = ReservationStatus.Pending,
            TotalAmount = 120m,
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "valid-token");

        var response = await client.PostAsJsonAsync("/api/reservations", new CreateReservationRequest(user.Id, field.Id, new DateOnly(2026, 7, 24), new TimeOnly(18, 0), new TimeOnly(19, 0), 120m));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PostReservation_WhenFieldDoesNotExist_ReturnsNotFound()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "valid-token");

        var response = await client.PostAsJsonAsync("/api/reservations", new CreateReservationRequest(Guid.NewGuid(), 999, new DateOnly(2026, 7, 24), new TimeOnly(18, 0), new TimeOnly(19, 0), 120m));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PostReservation_WhenRequestHasInvalidContract_ReturnsBadRequest()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "valid-token");

        var response = await client.PostAsync("/api/reservations", new StringContent("{ \"userId\": \"\", \"fieldId\": 0, \"date\": \"2026-07-24\", \"startTime\": \"19:00\", \"endTime\": \"18:00\" }", Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
