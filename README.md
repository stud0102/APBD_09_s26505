# APBD 09 - Database First REST API

## Author

**Małgorzata Antosz s26505**

## Description

The project was created as part of APBD classes. The goal was to build a REST API using Entity Framework Core in the Database First approach.

The application works with an existing SQL Server database called **ApbdLecture9DbFirstTask**. Entity classes and DbContext were generated using EF Core scaffolding, without using migrations.

The API allows managing students, courses, assignments and submissions.

## Technologies

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* Database First approach
* Swagger / OpenAPI

## Database setup

The database was created using the provided SQL script:

```sql
zadanie_1_db_first_university_tasks_setup.sql
```

The script creates:

* Students
* Courses
* Enrollments
* Assignments
* Submissions

and inserts sample data.

## Packages

The following packages were used:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Swashbuckle.AspNetCore
```

## EF Core Scaffolding

The model was generated using:

```bash
dotnet ef dbcontext scaffold "Server=localhost;Database=ApbdLecture9DbFirstTask;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer --context UniversityTasksDbContext --context-dir Data --output-dir Models --no-onconfiguring
```

## Project structure

```text
APBD_09_s26505
│
├── Controllers
├── DTOs
├── Data
├── Models
├── ModelExtensions
├── Services
├── Program.cs
├── appsettings.json
└── README.md
```

## Partial classes

Additional logic was implemented using partial classes:

* Student.FullName
* Student.HasAcademicEmail()
* Assignment.IsOverdue()

Generated files were not modified directly.

## Implemented endpoints

### Courses

```http
GET /api/courses
GET /api/courses/{idCourse}/assignments
```

### Students

```http
GET /api/students/{idStudent}/dashboard
```

### Submissions

```http
POST /api/submissions
PUT /api/submissions/{idSubmission}/grade
DELETE /api/submissions/{idSubmission}
```

## Business rules

The application validates:

* student existence and activity,
* assignment publication status,
* course enrollment,
* duplicate submissions,
* repository URL format,
* score range,
* deletion of graded submissions.

## Running the application

Build the project:

```bash
dotnet build
```

Run the application:

```bash
dotnet run
```

Swagger is available after starting the application.
