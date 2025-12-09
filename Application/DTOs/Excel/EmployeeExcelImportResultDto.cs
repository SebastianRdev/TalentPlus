using Application.Excel;
using Application.DTOs.Empleados;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Employees.Services;

public class EmployeeExcelImportService : IEmployeeExcelImportService
{
    private static readonly string[] ExpectedHeaders =
    {
        "Documento",
        "Nombres",
        "Apellidos",
        "FechaNacimiento",
        "Direccion",
        "Telefono",
        "Email",
        "Cargo",
        "Salario",
        "FechaIngreso",
        "Estado",
        "NivelEducativo",
        "PerfilProfesional",
        "Departamento"
    };

    private readonly IExcelParserService _excelParser;
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeExcelImportService(
        IExcelParserService excelParser,
        IEmployeeRepository employeeRepository)
    {
        _excelParser = excelParser;
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeExcelImportResultDto> ImportAsync(IFormFile file)
    {
        var result = new EmployeeExcelImportResultDto();

        var excel = await _excelParser.ParseAsync(file);

        ValidateHeaders(excel.Headers);

        foreach (var (row, index) in excel.Rows.Select((r, i) => (r, i + 2)))
        {
            result.TotalRows++;

            try
            {
                var document = row["Documento"];

                if (string.IsNullOrWhiteSpace(document))
                {
                    result.Errors.Add($"Fila {index}: Documento vacío.");
                    continue;
                }

                var employee = await _employeeRepository.GetByDocumentAsync(document);

                if (employee is null)
                {
                    employee = Empleado.CreateFromExcel(row);
                    await _employeeRepository.AddAsync(employee);
                    result.Created++;
                }
                else
                {
                    employee.UpdateFromExcel(row);
                    await _employeeRepository.UpdateAsync(employee);
                    result.Updated++;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Fila {index}: {ex.Message}");
            }
        }

        return result;
    }

    private static void ValidateHeaders(IReadOnlyList<string> headers)
    {
        var missing = ExpectedHeaders.Except(headers).ToList();

        if (missing.Any())
            throw new InvalidOperationException(
                $"The Excel file does not have the expected headers.: {string.Join(", ", missing)}");
    }
}
