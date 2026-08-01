using MeetingRoom.Application.Abstractions.Repositories;
using MeetingRoom.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingRoom.Infrastructure
{
    /// <summary>
    /// Registers infrastructure-layer services (repositories) with the DI container.
    /// </summary>
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            return services;
        }
    }
}
