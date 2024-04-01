using StudentManagementAPI.Repositories;
using StudentManagementAPI.Repositories.IRepositories;
using StudentManagementAPI.Datas;
using StudentManagementAPI.Models.StudentMSMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StudentManagementAPI.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 4a. Serilog || ILogging DI Service
// Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
//     .WriteTo.File
//     (
//         "Logs/studentMSLogs.txt",
//         rollingInterval:RollingInterval.Day
//     ).CreateLogger();
// builder.Host.UseSerilog();



builder.Services.AddControllers(option =>
{
    // option.ReturnHttpNotAcceptable=true;
}).AddXmlDataContractSerializerFormatters();
// 1. Setup DbContext
builder.Services.AddDbContext<StudentDatabaseContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});
builder.Services.AddDbContext<CourseDatabaseContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// 2. Add Repository Patterns
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();


// 3. AutoMapper
builder.Services.AddAutoMapper(typeof(StudentMSMappings));
// 4b. ILogging DI Service
builder.Services.AddSingleton<ILogging, Logging>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// 5. Use Swagger
// app.UseSwagger();
// app.UseSwaggerUI(options =>
//     {
//         foreach (var desc in provider.ApiVersionDescriptions)
//         {
//             options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json",
//                 desc.GroupName.ToUpperInvariant());
//         }
//         options.RoutePrefix = "";
//     }
// );
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
