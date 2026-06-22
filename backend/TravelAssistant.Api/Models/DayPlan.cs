namespace TravelAssistant.Api.Models;

public class DayPlan
{
    public Guid Id { get; set; }
    public Guid TripPlanId { get; set; }
    public int DayNumber { get; set; }
    public string? Morning { get; set; }
    public string? Afternoon { get; set; }
    public string? Evening { get; set; }
    public string? Notes { get; set; }
    public TripPlan? TripPlan { get; set; }
}
