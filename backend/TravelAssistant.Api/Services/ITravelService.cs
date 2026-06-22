using TravelAssistant.Api.DTOs;

namespace TravelAssistant.Api.Services;

public interface ITravelService
{
    Task<TripPlanResponse> CreatePlanAsync(CreateTripRequest request);
    Task<TripPlanResponse?> GetPlanAsync(Guid id);
    Task<List<TripPlanResponse>> GetPlansAsync();
    Task<TripPlanResponse?> RegenerateAsync(Guid id, RegenerateTripRequest request);
    Task<TripPlanResponse?> UpdateDayAsync(Guid id, int day, UpdateDayRequest request);
}
