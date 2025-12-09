using Application.DTOs.Empleados;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Application.DTOs.Excel;
using Application.Interfaces;
using Domain.Interfaces;
using OfficeOpenXml;

namespace Application.Services;

public class EmployeeExcelImportService : IEmployeeExcelImportService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeExcelImportService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeExcelImportResultDto> ImportAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Archivo inválido");

        var result = new EmployeeExcelImportResultDto();

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets[0];

        var totalRows = worksheet.Dimension.Rows;

        // Recorremos cada fila
        for (int row = 2; row <= totalRows; row++)
        {
            result.TotalRows++;

            try
            {
                var dto = ParseRow(worksheet, row);

                // Buscar si el empleado ya existe
                var employee = await _employeeRepository.GetByDocumentAsync(dto.Documento);

                // Si no existe, lo creamos
                if (employee == null)
                {
                    employee = Empleado.Create(
                        dto.Documento,
                        dto.Nombres,
                        dto.Apellidos,
                        DateTime.Parse(dto.FechaNacimiento),
                        dto.Direccion,
                        dto.Telefono,
                        dto.Email,
                        ParseCargo(dto.Cargo),
                        decimal.Parse(dto.Salario.Replace(",", ".")),
                        DateTime.Parse(dto.FechaIngreso),
                        ParseEstado(dto.Estado),
                        ParseNivelEducativo(dto.NivelEducativo),
                        dto.PerfilProfesional,
                        ParseDepartamento(dto.Departamento)
                    );

                    await _employeeRepository.AddAsync(employee);
                    result.Created++;
                }
                else
                {
                    // Si existe, lo actualizamos
                    employee.Update(
                        dto.Nombres,
                        dto.Apellidos,
                        DateTime.Parse(dto.FechaNacimiento),
                        dto.Direccion,
                        dto.Telefono,
                        dto.Email,
                        ParseCargo(dto.Cargo),
                        decimal.Parse(dto.Salario.Replace(",", ".")),
                        DateTime.Parse(dto.FechaIngreso),
                        ParseEstado(dto.Estado),
                        ParseNivelEducativo(dto.NivelEducativo),
                        dto.PerfilProfesional,
                        ParseDepartamento(dto.Departamento)
                    );

                    await _employeeRepository.UpdateAsync(employee);
                    result.Updated++;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Fila {row}: {ex.Message}");
            }
        }

        return result;
    }

    // ============================
    // Helpers
    // ============================

    private static EmployeeExcelRowDto ParseRow(ExcelWorksheet ws, int row)
    {
        return new EmployeeExcelRowDto
        {
            Documento = Required(ws, row, 1),
            Nombres = Required(ws, row, 2),
            Apellidos = Required(ws, row, 3),
            FechaNacimiento = Required(ws, row, 4),
            Direccion = Required(ws, row, 5),
            Telefono = Required(ws, row, 6),
            Email = Required(ws, row, 7),
            Cargo = Required(ws, row, 8),
            Salario = Required(ws, row, 9),
            FechaIngreso = Required(ws, row, 10),
            Estado = Required(ws, row, 11),
            NivelEducativo = Required(ws, row, 12),
            PerfilProfesional = Required(ws, row, 13),
            Departamento = Required(ws, row, 14)
        };
    }

    private static string Required(ExcelWorksheet ws, int row, int col)
    {
        var value = ws.Cells[row, col].Text?.Trim();
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Columna {col} vacía");

        return value;
    }

    // ============================
    // Enum Parsers
    // ============================

    private static Cargo ParseCargo(string value) =>
        value.ToLower().Replace(" ", "") switch
        {
            "ingeniero" => Cargo.Ingeniero,
            "soportetecnico" => Cargo.SoporteTecnico,
            "coordinador" => Cargo.Coordinador,
            "analista" => Cargo.Analista,
            "desarrollador" => Cargo.Desarrollador,
            "administrador" => Cargo.Administrador,
            "auxiliar" => Cargo.Auxiliar,
            _ => throw new InvalidOperationException($"Cargo inválido: {value}")
        };

    private static EstadoEmpleado ParseEstado(string value) =>
        value.ToLower() switch
        {
            "activo" => EstadoEmpleado.Activo,
            "inactivo" => EstadoEmpleado.Inactivo,
            "vacaciones" => EstadoEmpleado.Vacaciones,
            _ => throw new InvalidOperationException($"Estado inválido: {value}")
        };

    private static NivelEducativo ParseNivelEducativo(string value) =>
        value.ToLower() switch
        {
            "tecnico" => NivelEducativo.Tecnico,
            "tecnologo" => NivelEducativo.Tecnologo,
            "profesional" => NivelEducativo.Profesional,
            "especializacion" => NivelEducativo.Especializacion,
            "maestria" => NivelEducativo.Maestria,
            _ => throw new InvalidOperationException($"Nivel educativo inválido: {value}")
        };

    private static Departamento ParseDepartamento(string value) =>
        value.ToLower().Replace(" ", "") switch
        {
            "logistica" => Departamento.Logistica,
            "marketing" => Departamento.Marketing,
            "recursoshumanos" => Departamento.RecursosHumanos,
            "operaciones" => Departamento.Operaciones,
            "ventas" => Departamento.Ventas,
            "tecnologia" => Departamento.Tecnologia,
            "contabilidad" => Departamento.Contabilidad,
            _ => throw new InvalidOperationException($"Departamento inválido: {value}")
        };
}
