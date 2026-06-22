namespace TravelAssistant.Api.Models;

public class TripPlan
{
    public Guid Id { get; set; }
    public string Destination { get; set; } = string.Empty;
    public int Days { get; set; }
    public string BudgetLevel { get; set; } = string.Empty;
    public string? TravelerType { get; set; }
    public string? TripSummary { get; set; }
    public string? BudgetBand { get; set; }
    public string Status { get; set; } = "SUCCESS";
    public bool FallbackActivated { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public List<DayPlan> DayPlans { get; set; } = new();
}
