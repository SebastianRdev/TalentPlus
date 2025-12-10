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
        // Re-use preview and confirm logic for direct import
        var preview = await PreviewAsync(file);
        if (!preview.Success)
        {
            return new EmployeeExcelImportResultDto 
            { 
                Errors = new List<string> { preview.Message } 
            };
        }

        return await ConfirmAsync(preview.Data);
    }

    public async Task<ExcelPreviewResultDto> PreviewAsync(IFormFile file)
    {
        Console.WriteLine("🔵 [PreviewAsync] Starting preview...");
        
        if (file == null || file.Length == 0)
        {
            Console.WriteLine("❌ [PreviewAsync] Invalid file");
            return new ExcelPreviewResultDto { Success = false, Message = "Archivo inválido" };
        }

        Console.WriteLine($"📄 [PreviewAsync] File received: {file.FileName}, Size: {file.Length} bytes");

        var result = new ExcelPreviewResultDto { Success = true };
        var data = new ExcelPreviewData();

        Console.WriteLine("📤 [PreviewAsync] Copying file to memory stream...");
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        Console.WriteLine("✅ [PreviewAsync] File copied to memory");

        Console.WriteLine("📊 [PreviewAsync] Opening Excel package...");
        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets[0];
        var totalRows = worksheet.Dimension.Rows;
        Console.WriteLine($"📊 [PreviewAsync] Excel opened. Total rows: {totalRows}");

        // OPTIMIZATION: Load all existing employee documents once to avoid N+1 queries
        Console.WriteLine("🔍 [PreviewAsync] Loading all existing employees...");
        var allEmployees = await _employeeRepository.GetAllAsync();
        Console.WriteLine($"✅ [PreviewAsync] Loaded {allEmployees.Count} existing employees");
        
        var existingDocuments = new HashSet<string>(
            allEmployees.Select(e => e.Documento),
            StringComparer.OrdinalIgnoreCase
        );
        Console.WriteLine($"✅ [PreviewAsync] Created HashSet with {existingDocuments.Count} documents");

        Console.WriteLine($"🔄 [PreviewAsync] Starting to process {totalRows - 1} rows...");
        for (int row = 2; row <= totalRows; row++)
        {
            if (row % 10 == 0)
                Console.WriteLine($"⏳ [PreviewAsync] Processing row {row}/{totalRows}...");
            
            var rowPreview = new ExcelRowPreview { RowNumber = row };
            try
            {
                // Extract raw data for preview
                for (int col = 1; col <= 14; col++)
                {
                    var header = worksheet.Cells[1, col].Text;
                    var value = worksheet.Cells[row, col].Text;
                    rowPreview.RowData[header] = value;
                }

                // Validate by trying to parse
                var dto = ParseRow(worksheet, row);
                
                // Check if document already exists (in-memory check, no DB query)
                if (existingDocuments.Contains(dto.Documento))
                {
                    // It's an update, which is valid, but we can note it
                    // rowPreview.RowData["_Status"] = "Update";
                }

                data.ValidRows.Add(rowPreview);
                data.TotalValid++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ [PreviewAsync] Row {row} validation error: {ex.Message}");
                rowPreview.Errors.Add(ex.Message);
                data.InvalidRows.Add(rowPreview);
                data.TotalInvalid++;
            }
        }

        Console.WriteLine($"✅ [PreviewAsync] Processing complete. Valid: {data.TotalValid}, Invalid: {data.TotalInvalid}");
        result.Data = data;
        
        Console.WriteLine("🔵 [PreviewAsync] Returning result...");
        return result;
    }

    public async Task<EmployeeExcelImportResultDto> ConfirmAsync(ExcelPreviewData data)
    {
        Console.WriteLine($"🔵 [ConfirmAsync] Starting confirmation for {data.ValidRows.Count} valid rows...");
        var result = new EmployeeExcelImportResultDto();

        // OPTIMIZATION: Load all existing employees once to avoid N+1 queries
        Console.WriteLine("🔍 [ConfirmAsync] Loading all existing employees...");
        var allEmployees = await _employeeRepository.GetAllAsync();
        var existingEmployees = allEmployees.ToDictionary(e => e.Documento, StringComparer.OrdinalIgnoreCase);
        Console.WriteLine($"✅ [ConfirmAsync] Loaded {existingEmployees.Count} existing employees");

        int processedCount = 0;
        foreach (var row in data.ValidRows)
        {
            processedCount++;
            if (processedCount % 10 == 0)
                Console.WriteLine($"⏳ [ConfirmAsync] Processing row {processedCount}/{data.ValidRows.Count}...");
            
            try
            {
                // Re-construct DTO from dictionary
                var dto = MapDictionaryToDto(row.RowData);

                existingEmployees.TryGetValue(dto.Documento, out var employee);

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
                    Console.WriteLine($"✅ [ConfirmAsync] Created employee: {dto.Documento} - {dto.Nombres} {dto.Apellidos}");
                }
                else
                {
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
                    Console.WriteLine($"✅ [ConfirmAsync] Updated employee: {dto.Documento} - {dto.Nombres} {dto.Apellidos}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ConfirmAsync] Error processing row {row.RowNumber}: {ex.Message}");
                result.Errors.Add($"Fila {row.RowNumber}: {ex.Message}");
            }
        }

        Console.WriteLine($"✅ [ConfirmAsync] Confirmation complete. Created: {result.Created}, Updated: {result.Updated}, Errors: {result.Errors.Count}");
        return result;
    }

    private EmployeeExcelRowDto MapDictionaryToDto(Dictionary<string, string> data)
    {
        // Helper to safely get value
        string Get(string key, int index) 
        {
            if (data.ContainsKey(key)) return data[key];
            // Fallback to index if keys don't match exactly (simplified)
            return data.Values.ElementAtOrDefault(index) ?? "";
        }

        // Assuming standard headers order as per ParseRow
        return new EmployeeExcelRowDto
        {
            Documento = data.Values.ElementAt(0),
            Nombres = data.Values.ElementAt(1),
            Apellidos = data.Values.ElementAt(2),
            FechaNacimiento = data.Values.ElementAt(3),
            Direccion = data.Values.ElementAt(4),
            Telefono = data.Values.ElementAt(5),
            Email = data.Values.ElementAt(6),
            Cargo = data.Values.ElementAt(7),
            Salario = data.Values.ElementAt(8),
            FechaIngreso = data.Values.ElementAt(9),
            Estado = data.Values.ElementAt(10),
            NivelEducativo = data.Values.ElementAt(11),
            PerfilProfesional = data.Values.ElementAt(12),
            Departamento = data.Values.ElementAt(13)
        };
    }
    
    // Dentro de EmployeeExcelImportService
    private static string RemoveAccentsAndNormalize(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        // 1. Normaliza la cadena para descomponer caracteres con acentos en su base y el acento separado
        //    Ej: 'ó' se convierte en 'o' + acento.
        var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
        var stringBuilder = new System.Text.StringBuilder();

        // 2. Itera sobre los caracteres y omite los que son marcas no espaciadoras (los acentos)
        foreach (char c in normalizedString)
        {
            var category = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        // 3. Convierte a minúsculas y elimina espacios
        return stringBuilder.ToString().ToLower().Replace(" ", "");
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

    private static Cargo ParseCargo(string value)
    {
        // APLICA EL CAMBIO AQUÍ
        string normalizedValue = RemoveAccentsAndNormalize(value); 

        return normalizedValue switch
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
    }

    private static EstadoEmpleado ParseEstado(string value) =>
        value.ToLower() switch
        {
            "activo" => EstadoEmpleado.Activo,
            "inactivo" => EstadoEmpleado.Inactivo,
            "vacaciones" => EstadoEmpleado.Vacaciones,
            _ => throw new InvalidOperationException($"Estado inválido: {value}")
        };

    private static NivelEducativo ParseNivelEducativo(string value)
    {
        // APLICA EL CAMBIO AQUÍ
        string normalizedValue = RemoveAccentsAndNormalize(value); 

        return normalizedValue switch
        {
            "tecnico" => NivelEducativo.Tecnico,
            "tecnologo" => NivelEducativo.Tecnologo,
            "profesional" => NivelEducativo.Profesional,
            "especializacion" => NivelEducativo.Especializacion,
            "maestria" => NivelEducativo.Maestria,
            _ => throw new InvalidOperationException($"Nivel educativo inválido: {value}")
        };
    }

    private static Departamento ParseDepartamento(string value)
    {
        // APLICA EL CAMBIO AQUÍ
        string normalizedValue = RemoveAccentsAndNormalize(value); 
    
        return normalizedValue switch
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
}
