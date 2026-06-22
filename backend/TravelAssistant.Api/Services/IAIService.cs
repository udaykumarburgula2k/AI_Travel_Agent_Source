namespace TravelAssistant.Api.Services;

public interface IAIService
{
    Task<string> GenerateItineraryAsync(string prompt, CancellationToken cancellationToken = default);
}
