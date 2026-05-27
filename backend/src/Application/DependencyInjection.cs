using Application.Interfaces;
using Application.Services.Reservations;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IReservationScheduleService, ReservationScheduleService>();
        return services;
    }
}
