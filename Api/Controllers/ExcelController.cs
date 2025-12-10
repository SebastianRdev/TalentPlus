using Application.DTOs.Excel;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExcelController : ControllerBase
{
    private readonly IEmployeeExcelImportService _employeeImportService;

    public ExcelController(IEmployeeExcelImportService employeeImportService)
    {
        _employeeImportService = employeeImportService;
    }

    [HttpPost("preview")]
    public async Task<IActionResult> Preview(IFormFile file, [FromQuery] string entityType)
    {
        Console.WriteLine($"🌐 [ExcelController.Preview] Request received. EntityType: {entityType}, File: {file?.FileName}");
        
        if (entityType?.ToLower() == "employee")
        {
            Console.WriteLine("🔵 [ExcelController.Preview] Calling EmployeeImportService.PreviewAsync...");
            var result = await _employeeImportService.PreviewAsync(file);
            Console.WriteLine($"✅ [ExcelController.Preview] PreviewAsync completed. Success: {result.Success}");
            
            if (!result.Success)
            {
                Console.WriteLine($"❌ [ExcelController.Preview] Preview failed: {result.Message}");
                return BadRequest(result.Message);
            }
            
            Console.WriteLine($"✅ [ExcelController.Preview] Returning OK with data. Valid: {result.Data?.TotalValid}, Invalid: {result.Data?.TotalInvalid}");
            Console.WriteLine($"📊 [ExcelController.Preview] Data size - ValidRows: {result.Data?.ValidRows?.Count}, InvalidRows: {result.Data?.InvalidRows?.Count}");
            Console.WriteLine("⏱️ [ExcelController.Preview] About to serialize and return JSON...");
            
            var response = Ok(result);
            
            Console.WriteLine("✅ [ExcelController.Preview] JSON serialized and response created");
            return response;
        }

        Console.WriteLine($"❌ [ExcelController.Preview] Unsupported entity type: {entityType}");
        return BadRequest($"Entity type '{entityType}' not supported.");
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] ExcelPreviewData data)
    {
        // For now, we assume it's always employee based on the data structure
        // In a more generic system, we might wrap this in a request object with entityType
        
        var result = await _employeeImportService.ConfirmAsync(data);
        return Ok(new { success = true, data = result });
    }
}
