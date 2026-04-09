using ARS.API.Middleware;
using ARS.Application.Services;
using ARS.Application.Validators;
using ARS.Infrastructure.Data;
using ARS.Infrastructure.Repositories;
using ARS.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Scalar.AspNetCore;
using Serilog;
using System.Security.Claims;


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "ActivityReportingSystem")
    .CreateLogger();

try
{

    Log.Information("Starting Activity Reporting System API");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();


    // Add Azure AD authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(options =>
        {
            builder.Configuration.Bind("AzureAd", options);
            options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
        },
        options =>
        {
            builder.Configuration.Bind("AzureAd", options);
        });


    builder.Services.AddAuthorization();

    // Add services
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(
                new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

    // Add .NET 9 OpenAPI
    builder.Services.AddOpenApi();

    // Add FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();


    // Configure MongoDB
    builder.Services.Configure<MongoDbSettings>(
        builder.Configuration.GetSection("MongoDbSettings"));

    // Register MongoDB context
    builder.Services.AddSingleton<MongoDbContext>();

    // Repositories
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IEntityRepository, EntityRepository>();
    builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
    builder.Services.AddScoped<ITrackingRequestRepository, TrackingRequestRepository>();
    builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
    builder.Services.AddScoped<IRequestAssignmentRepository, RequestAssignmentRepository>();
    builder.Services.AddScoped<IActivityRepository, ActivityRepository>();


    //Custom Services
    builder.Services.AddHttpContextAccessor(); 
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

    // ========================================
    // CORS (if needed for frontend)
    // ========================================
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
    });

    var app = builder.Build();

    //excepcion handling middleware
    app.UseMiddleware<ExceptionHandlingMiddleware>();


    // ========================================
    // MIDDLEWARE PIPELINE
    // ========================================

    if (app.Environment.IsDevelopment())
    {
        // Map OpenAPI endpoint
        app.MapOpenApi();

        // Use Scalar UI for API documentation
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Activity Reporting System API");
            options.WithTheme(ScalarTheme.Purple);
            options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
    }

    app.UseHttpsRedirection();

    app.UseCors("AllowAll");


    // IMPORTANT: Authentication MUST come before Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Add request logging middleware
    app.UseMiddleware<RequestLoggingMiddleware>();
    
    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
