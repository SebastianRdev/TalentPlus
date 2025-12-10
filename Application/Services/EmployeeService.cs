using Application.DTOs.Empleados;
using Application.DTOs.Excel;
using Application.Interfaces;
using Application.Interfaces.Employees;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;

        public EmployeeService(IEmployeeRepository employeeRepository, IEmailService emailService)
        {
            _employeeRepository = employeeRepository;
            _emailService = emailService;
        }

        // Método para crear un nuevo empleado
        public async Task CreateAsync(EmployeeCreateDto dto)
        {
            // Usamos el método estático Create para crear el empleado
            var employee = Empleado.Create(
                dto.Documento,
                dto.Nombres,
                dto.Apellidos,
                dto.FechaNacimiento.Value,
                dto.Direccion,
                dto.Telefono,
                dto.Email,
                dto.Cargo,
                dto.Salario.Value,
                dto.FechaIngreso.Value,
                dto.Estado,
                dto.NivelEducativo,
                dto.PerfilProfesional,
                dto.Departamento
            );

            // Guardamos el nuevo empleado en el repositorio
            await _employeeRepository.AddAsync(employee);
        }

        public async Task RegisterAsync(EmployeeRegisterDto dto)
        {
            var employee = Empleado.Create(
                dto.Documento,
                dto.Nombres,
                dto.Apellidos,
                dto.FechaNacimiento,
                dto.Direccion ?? "",
                dto.Telefono ?? "",
                dto.Email,
                dto.Cargo,
                dto.Salario ?? 0,
                dto.FechaIngreso,
                dto.Estado,
                dto.NivelEducativo,
                dto.PerfilProfesional ?? "",
                dto.Departamento
            );

            await _employeeRepository.AddAsync(employee);

            // Send Welcome Email
            var subject = "Bienvenido a TalentPlus";
            var body = $@"
                <h1>Bienvenido, {dto.Nombres} {dto.Apellidos}!</h1>
                <p>Tu registro en TalentPlus ha sido exitoso.</p>
                <p>Ya puedes autenticarte en la plataforma cuando tu cuenta sea habilitada por un administrador.</p>
                <br>
                <p>Atentamente,</p>
                <p>El equipo de TalentPlus</p>
            ";

            await _emailService.SendEmailAsync(dto.Email, subject, body);
        }

        // Método para actualizar un empleado existente
        public async Task UpdateAsync(Guid id, EmployeeUpdateDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new InvalidOperationException($"Empleado con ID {id} no encontrado.");

            // Actualizamos los campos del empleado con la nueva información
            employee.Update(
                dto.Nombres,
                dto.Apellidos,
                dto.FechaNacimiento.Value,
                dto.Direccion,
                dto.Telefono,
                dto.Email,
                dto.Cargo,
                dto.Salario.Value,
                dto.FechaIngreso.Value,
                dto.Estado,
                dto.NivelEducativo,
                dto.PerfilProfesional,
                dto.Departamento
            );

            // Guardamos los cambios en el repositorio
            await _employeeRepository.UpdateAsync(employee);
        }

        // Método para obtener un empleado por su documento
        public async Task<Empleado> GetByDocumentAsync(string document)
        {
            var employee = await _employeeRepository.GetByDocumentAsync(document);
            if (employee == null)
                throw new InvalidOperationException($"Empleado con documento {document} no encontrado.");

            return employee;
        }

        // Método para obtener todos los empleados
        public async Task<IEnumerable<Empleado>> GetAllAsync()
        {
            var (employees, totalCount) = await _employeeRepository.GetPagedAsync(1, 1000); // Temporary: get all
            return employees;
        }

        public async Task<Empleado?> GetByIdAsync(Guid id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new InvalidOperationException($"Empleado con ID {id} no encontrado.");

            await _employeeRepository.DeleteAsync(employee);
        }
        
        public async Task<EmployeeExcelImportResultDto> ImportFromExcelAsync(IFormFile file)
        {
            var result = new EmployeeExcelImportResultDto();

            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo Excel inválido.");

            using var stream = new System.IO.MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            var totalRows = worksheet.Dimension.Rows;

            // Recorremos cada fila del Excel y procesamos los datos
            for (int row = 2; row <= totalRows; row++)
            {
                try
                {
                    var dto = ParseRow(worksheet, row); // Método para mapear los datos de la fila
                    var employee = await _employeeRepository.GetByDocumentAsync(dto.Documento);

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

        // Método auxiliar para leer los valores de cada celda en la fila
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

        // Método auxiliar para validar y obtener los valores de las celdas
        private static string Required(ExcelWorksheet ws, int row, int col)
        {
            var value = ws.Cells[row, col].Text?.Trim();
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"Columna {col} vacía");

            return value;
        }

        // Métodos para parsear los enums (Cargo, EstadoEmpleado, NivelEducativo, Departamento)
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
}
