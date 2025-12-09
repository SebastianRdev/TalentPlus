using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Empleados;

public class ImportExcelRequestDto
{
    public IFormFile File { get; set; } = default!;
}