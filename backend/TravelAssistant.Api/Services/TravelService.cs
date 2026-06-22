using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TravelAssistant.Api.Data;
using TravelAssistant.Api.DTOs;
using TravelAssistant.Api.Models;

namespace TravelAssistant.Api.Services;

public class TravelService : ITravelService
{
    private readonly AppDbContext _db;
    private readonly IAIService _aiService;

    public TravelService(AppDbContext db, IAIService aiService)
    {
        _db = db;
        _aiService = aiService;
    }

    public async Task<TripPlanResponse> CreatePlanAsync(CreateTripRequest request)
    {
        var aiResult = await GenerateOrFallbackAsync(request);
        var plan = new TripPlan
        {
            Id = Guid.NewGuid(),
            Destination = request.Destination,
            Days = request.Days,
            BudgetLevel = request.BudgetLevel,
            TravelerType = request.TravelerType,
            TripSummary = aiResult.Result.TripSummary,
            BudgetBand = aiResult.Result.BudgetBand,
            Status = aiResult.Fallback ? "FALLBACK" : "SUCCESS",
            FallbackActivated = aiResult.Fallback,
            CreatedAt = DateTime.UtcNow,
            DayPlans = aiResult.Result.DayWisePlan.Select(x => new DayPlan
            {
                Id = Guid.NewGuid(),
                DayNumber = x.Day,
                Morning = x.Morning,
                Afternoon = x.Afternoon,
                Evening = x.Evening,
                Notes = x.Notes
            }).ToList()
        };

        _db.TripPlans.Add(plan);
        await _db.SaveChangesAsync();
        return Map(plan);
    }

    public async Task<TripPlanResponse?> GetPlanAsync(Guid id)
    {
        var plan = await _db.TripPlans.Include(x => x.DayPlans).FirstOrDefaultAsync(x => x.Id == id);
        return plan == null ? null : Map(plan);
    }

    public async Task<List<TripPlanResponse>> GetPlansAsync()
    {
        var plans = await _db.TripPlans.Include(x => x.DayPlans).OrderByDescending(x => x.CreatedAt).ToListAsync();
        return plans.Select(Map).ToList();
    }

    public async Task<TripPlanResponse?> RegenerateAsync(Guid id, RegenerateTripRequest request)
    {
        var plan = await _db.TripPlans.Include(x => x.DayPlans).FirstOrDefaultAsync(x => x.Id == id);
        if (plan == null) return null;

        var createRequest = new CreateTripRequest
        {
            Destination = plan.Destination,
            Days = plan.Days,
            BudgetLevel = request.BudgetLevel ?? plan.BudgetLevel,
            TravelerType = plan.TravelerType,
            Interests = request.Interests ?? new List<string> { "Sightseeing", "Food", "Culture" }
        };

        var aiResult = await GenerateOrFallbackAsync(createRequest);
        _db.DayPlans.RemoveRange(plan.DayPlans);

        plan.BudgetLevel = createRequest.BudgetLevel;
        plan.TripSummary = aiResult.Result.TripSummary;
        plan.BudgetBand = aiResult.Result.BudgetBand;
        plan.Status = aiResult.Fallback ? "FALLBACK" : "SUCCESS";
        plan.FallbackActivated = aiResult.Fallback;
        plan.UpdatedAt = DateTime.UtcNow;
        plan.DayPlans = aiResult.Result.DayWisePlan.Select(x => new DayPlan
        {
            Id = Guid.NewGuid(),
            TripPlanId = plan.Id,
            DayNumber = x.Day,
            Morning = x.Morning,
            Afternoon = x.Afternoon,
            Evening = x.Evening,
            Notes = x.Notes
        }).ToList();

        await _db.SaveChangesAsync();
        return Map(plan);
    }

    public async Task<TripPlanResponse?> UpdateDayAsync(Guid id, int day, UpdateDayRequest request)
    {
        var plan = await _db.TripPlans.Include(x => x.DayPlans).FirstOrDefaultAsync(x => x.Id == id);
        if (plan == null) return null;

        var dayPlan = plan.DayPlans.FirstOrDefault(x => x.DayNumber == day);
        if (dayPlan == null) return null;

        dayPlan.Morning = request.Morning ?? dayPlan.Morning;
        dayPlan.Afternoon = request.Afternoon ?? dayPlan.Afternoon;
        dayPlan.Evening = request.Evening ?? dayPlan.Evening;
        dayPlan.Notes = request.Notes ?? dayPlan.Notes;
        plan.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Map(plan);
    }

    private async Task<(AiItineraryResult Result, bool Fallback)> GenerateOrFallbackAsync(CreateTripRequest request)
    {
        try
        {
            var prompt = BuildPrompt(request);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var raw = await _aiService.GenerateItineraryAsync(prompt, cts.Token);
            var result = JsonSerializer.Deserialize<AiItineraryResult>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result == null || result.DayWisePlan.Count == 0) throw new Exception("Invalid AI response");
            return (result, false);
        }
        catch
        {
            return (BuildFallback(request), true);
        }
    }

    private static string BuildPrompt(CreateTripRequest request) => $$"""
Create a travel itinerary using the following details:
Destination: {{request.Destination}}
Number of Days: {{request.Days}}
Interests: {{string.Join(", ", request.Interests)}}
Budget Level: {{request.BudgetLevel}}
Traveler Type: {{request.TravelerType ?? "General"}}

Return JSON only in this exact structure:
{
  "tripSummary": "",
  "budgetBand": "",
  "dayWisePlan": [
    { "day": 1, "morning": "", "afternoon": "", "evening": "", "notes": "" }
  ]
}
""";

    private static AiItineraryResult BuildFallback(CreateTripRequest request)
    {
        return new AiItineraryResult
        {
            TripSummary = $"Default {request.Days}-day trip plan for {request.Destination}.",
            BudgetBand = request.BudgetLevel,
            DayWisePlan = Enumerable.Range(1, request.Days).Select(day => new AiDayPlan
            {
                Day = day,
                Morning = "Visit a popular local attraction.",
                Afternoon = "Try local food and explore nearby places.",
                Evening = "Relax and enjoy a safe evening activity.",
                Notes = "Fallback itinerary used because AI output was unavailable."
            }).ToList()
        };
    }

    private static TripPlanResponse Map(TripPlan plan) => new()
    {
        PlanId = plan.Id,
        Destination = plan.Destination,
        Days = plan.Days,
        TripSummary = plan.TripSummary,
        BudgetBand = plan.BudgetBand,
        Status = plan.Status,
        FallbackActivated = plan.FallbackActivated,
        DayWisePlan = plan.DayPlans.OrderBy(x => x.DayNumber).Select(x => new DayPlanResponse
        {
            Day = x.DayNumber,
            Morning = x.Morning,
            Afternoon = x.Afternoon,
            Evening = x.Evening,
            Notes = x.Notes
        }).ToList()
    };
}
