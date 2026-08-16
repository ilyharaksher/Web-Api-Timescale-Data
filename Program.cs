using Microsoft.EntityFrameworkCore;
using Web_Api_Timescale_Data.Data;
using Web_Api_Timescale_Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add DbContext 
builder.Services.AddDbContext<ApplicationDbContext>(options => 
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// add Services
builder.Services.AddScoped<ICsvImportService, CsvImportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Swagger
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
