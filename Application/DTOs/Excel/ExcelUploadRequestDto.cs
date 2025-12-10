using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.DTOs.Excel;

public class ExcelUploadRequestDto
{
    [FromQuery]
    public IFormFile File { get; set; }
}
