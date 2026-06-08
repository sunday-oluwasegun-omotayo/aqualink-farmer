using AquaLink.Cooperative.Application.Interfaces;
using AquaLink.Cooperative.Infrastructure.Persistence;
using AquaLink.Farmer.API.Auth;
using AquaLink.Farmer.Application.Common;
using AquaLink.Farmer.Application.FarmCycles;
using AquaLink.Farmer.Application.Interfaces;
using AquaLink.Farmer.Infrastructure.Persistence;
using AquaLink.Prices.Application.Interfaces;
using AquaLink.Prices.Application.Prices;
using AquaLink.Prices.Infrastructure.Jobs;
using AquaLink.Prices.Infrastructure.Persistence;
using AquaLink.Prices.Infrastructure.Services;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AquaLink Farmer API", Version = "v1" });
});

builder.Services.AddDbContext<AquaLinkFarmerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IFarmerDbContext>(provider =>
    provider.GetRequiredService<AquaLinkFarmerDbContext>());

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(AquaLink.Farmer.Application.FarmCycles.CreateFarmCycleCommand).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(AquaLink.Cooperative.Application.Cooperatives.CreateCooperativeGroupCommand).Assembly);
    cfg.RegisterServicesFromAssembly(
        typeof(AquaLink.Farmer.Application.FarmCycles.GetFarmCycleQuery).Assembly);
    cfg.RegisterServicesFromAssemblyContaining<SubmitPriceCommand>();
    cfg.AddBehavior(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehaviour<,>));
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateFarmCycleCommandValidator>();

builder.Services.AddSingleton<TokenService>();

var jwtSecret = builder.Configuration["JwtSettings:Secret"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AquaLinkCooperativeDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICooperativeDbContext>(provider =>
    provider.GetRequiredService<AquaLinkCooperativeDbContext>());

builder.Services.AddDbContext<AquaLinkPricesDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPricesDbContext>(provider =>
    provider.GetRequiredService<AquaLinkPricesDbContext>());

// Termii SMS service
builder.Services.AddHttpClient<ISmsService, TermiiSmsService>();

// Hangfire
builder.Services.AddHangfire(config => config
    .UsePostgreSqlStorage(c => c
        .UseNpgsqlConnection(
            builder.Configuration
                .GetConnectionString("DefaultConnection"))));

builder.Services.AddHangfireServer();
builder.Services.AddScoped<DailyPriceAlertJob>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AquaLinkWeb", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard("/hangfire");

// Schedule the 6am daily price alert
RecurringJob.AddOrUpdate<DailyPriceAlertJob>(
    "daily-price-alert",
    job => job.ExecuteAsync(),
    "0 6 * * *"); // 6:00 AM every day

app.UseCors("AquaLinkWeb");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();