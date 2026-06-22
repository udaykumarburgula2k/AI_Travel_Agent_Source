using Microsoft.AspNetCore.Mvc;
using TravelAssistant.Api.DTOs;
using TravelAssistant.Api.Services;

namespace TravelAssistant.Api.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController : ControllerBase
{
    private readonly ITravelService _travelService;

    public TripsController(ITravelService travelService)
    {
        _travelService = travelService;
    }

    [HttpPost("plans")]
    public async Task<IActionResult> CreatePlan(CreateTripRequest request)
    {
        var result = await _travelService.CreatePlanAsync(request);
        return CreatedAtAction(nameof(GetPlan), new { planId = result.PlanId }, result);
    }

    [HttpGet("plans/{planId:guid}")]
    public async Task<IActionResult> GetPlan(Guid planId)
    {
        var result = await _travelService.GetPlanAsync(planId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("plans")]
    public async Task<IActionResult> GetPlans()
    {
        return Ok(await _travelService.GetPlansAsync());
    }

    [HttpPost("plans/{planId:guid}/regenerate")]
    public async Task<IActionResult> Regenerate(Guid planId, RegenerateTripRequest request)
    {
        var result = await _travelService.RegenerateAsync(planId, request);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut("plans/{planId:guid}/days/{day:int}")]
    public async Task<IActionResult> UpdateDay(Guid planId, int day, UpdateDayRequest request)
    {
        var result = await _travelService.UpdateDayAsync(planId, day, request);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("interest-options")]
    public IActionResult GetInterestOptions()
    {
        return Ok(new
        {
            interests = new[] { "Beach", "Food", "Museums", "Shopping", "Adventure", "Culture", "Nature", "Nightlife" },
            budgetLevels = new[] { "Budget", "Medium", "Luxury" },
            travelerTypes = new[] { "Solo", "Couple", "Family", "Group" }
        });
    }
}
