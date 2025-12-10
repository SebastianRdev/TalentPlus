using Application.Interfaces;
using Application.DTOs.Agent;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly ISqlAgentService _agentService;

    public AgentController(ISqlAgentService agentService)
    {
        _agentService = agentService;
    }

    [HttpPost("query")]
    public async Task<IActionResult> Query([FromBody] AgentQueryDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Message is required.");

        var response = await _agentService.ProcessUserQueryAsync(request.Message);
        return Ok(new { response });
    }
}