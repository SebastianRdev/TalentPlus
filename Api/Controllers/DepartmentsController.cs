using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetAll()
    {
        var departments = Enum.GetValues<Departamento>()
            .Where(d => d != Departamento.Unknown)
            .Select(d => new { id = (int)d, name = d.ToString() })
            .ToList();

        return Ok(departments);
    }
}
