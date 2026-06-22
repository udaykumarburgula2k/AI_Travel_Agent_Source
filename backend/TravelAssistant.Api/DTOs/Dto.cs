using System.ComponentModel.DataAnnotations;

namespace TravelAssistant.Api.DTOs;

public class CreateTripRequest
{
    [Required, MaxLength(150)] public string Destination { get; set; } = string.Empty;
    [Range(1, 15)] public int Days { get; set; }
    [Required] public List<string> Interests { get; set; } = new();
    [Required, MaxLength(50)] public string BudgetLevel { get; set; } = string.Empty;
    [MaxLength(50)] public string? TravelerType { get; set; }
}

public class RegenerateTripRequest
{
    public List<string>? Interests { get; set; }
    public string? BudgetLevel { get; set; }
}

public class UpdateDayRequest
{
    public string? Morning { get; set; }
    public string? Afternoon { get; set; }
    public string? Evening { get; set; }
    public string? Notes { get; set; }
}

public class TripPlanResponse
{
    public Guid PlanId { get; set; }
    public string Destination { get; set; } = string.Empty;
    public int Days { get; set; }
    public string? TripSummary { get; set; }
    public string? BudgetBand { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool FallbackActivated { get; set; }
    public List<DayPlanResponse> DayWisePlan { get; set; } = new();
}

public class DayPlanResponse
{
    public int Day { get; set; }
    public string? Morning { get; set; }
    public string? Afternoon { get; set; }
    public string? Evening { get; set; }
    public string? Notes { get; set; }
}

public class AiItineraryResult
{
    public string TripSummary { get; set; } = string.Empty;
    public string BudgetBand { get; set; } = string.Empty;
    public List<AiDayPlan> DayWisePlan { get; set; } = new();
}

public class AiDayPlan
{
    public int Day { get; set; }
    public string Morning { get; set; } = string.Empty;
    public string Afternoon { get; set; } = string.Empty;
    public string Evening { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
