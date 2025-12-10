# TalentPlus
TalentPlus is a comprehensive web application designed for managing employees, departments, and human resources operations. It features a robust backend built with .NET Core following Clean Architecture principles and a user-friendly frontend for both administration and employee interaction.

## Features
*   **Employee Management:** Create, update, delete, and search for employees. Manage categories and stock levels.
*   **Excel Import:** Bulk upload employees and Employees using Excel files with AI-powered column mapping correction.
*   **Authentication & Authorization:** Secure login and registration with role-based access control (Admin, Employee).
*   **AI-Powered Chatbot**: Intelligent chatbot assistant to help with HR queries, employee information lookup, and common administrative tasks.
*   **Dashboard:** Overview of key metrics.

## Technology Stack
*   **Backend:** .NET Core 8, Entity Framework Core, PostgreSQL (AWS RDS).
*   **Frontend (Web Admin):** ASP.NET Core MVC, Razor Views, Bootstrap/Tailwind CSS.
*   **Libraries:**
    *   `EPPlus`: For Excel file handling.
    *   `QuestPDF`: For generating PDF receipts.
    *   `Microsoft.AspNetCore.Identity`: For user authentication and management.
    *   `AI Integration`: Gemini API for intelligent chatbot functionality.
    *   `Architecture`: Clean Architecture with separation of concerns.

## Getting Started

### Prerequisites
* .NET 8 SDK
* PostgreSQL (in the cloud)
* Docker y Docker Compose (to raise the entire area with containers)

### Configuration
Before running the application, you must create a `.env` file in the project root directory. You can refer to `.env.example` for the required values.

Then edit `.env` with your database credentials and any other necessary configuration.

## Important Setup Steps

1. **Excel Import Testing:**
   To test the Excel import functionality, use the sample file located at:
   __docs/Empleaos.xlsx

**Test Accounts**
The system includes pre-configured trial accounts:

Email: admin@talentplus.com
Password: Admin123$

Employee:

Email: employee@talentplus.com
Password: 123456

### Running the Project

#### Local (sin Docker)
1. Apply the migrations to the database:
```bash
dotnet ef Migrations add Initial
dotnet ef database update
```

2. Run the application:
```bash
dotnet run --project Api
dotnet run --project WebAdmin
```

#### Con Docker
You can set up the entire environment using Docker Compose.:
```bash
docker compose up --build -d
```

Access the services at:
```
💻 WebAdmin:       http://localhost:8082
🔌 API Swagger:    http://localhost:8081/index.html
```

## API Endpoints
**Agent**

-POST /api/Agent/query - Send queries to the AI chatbot assistant

**Auth**

-POST /api/auth/login - Authenticate user and obtain access token

**Departments**

-GET /api/departments - Get all departments

**Employees**

-GET /api/employees - Get all employees (with pagination)
-POST /api/employees - Create a new employee
-GET /api/employees/{id} - Get employee by ID
-PUT /api/employees/{id} - Update an existing employee
-DELETE /api/employees/{id} - Delete an employee
-POST /api/employees/register - Register a new employee account
-POST /api/employees/import-excel - Import employees from Excel file

**Excel**

-POST /api/Excel/preview - Preview Excel file data before importing
-POST /api/Excel/confirm - Confirm and import Excel data with AI-powered column mapping


## Screenshots

### Dashboard
![Dashboard Screenshot](__Docs/screenshots/dashboard.png__)
*Overview of the admin dashboard showing key statistics.*

### Employee Management
![Employee List Screenshot](__Docs/screenshots/employee_list.png__)
*List of products with search and filter options.*

### Excel Import with AI Mapping
![Excel Import Screenshot](__Docs/screenshots/excel_import.png__)
*Bulk upload interface showing AI-suggested column mapping.*

## Architecture
The solution follows a Clean Architecture approach:
*   **Domain:** Core entities and business logic.
*   **Application:** Application services, DTOs, and interfaces.
*   **Infrastructure:** Data access, external service implementations (Excel, PDF), and repositories.
*   **Api:** RESTful API endpoints for the frontend.
*   **WebAdmin:** MVC-based administration interface.

## Contributing
Contributions are welcome! Please fork the repository and submit a pull request.

## License
This project is licensed under the MIT License.
