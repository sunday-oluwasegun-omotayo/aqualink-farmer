using AquaLink.Farmer.API.Auth;
using AquaLink.Farmer.Application.Common;
using AquaLink.Farmer.Application.FarmCycles;
using AquaLink.Farmer.Application.Interfaces;
using AquaLink.Farmer.Infrastructure.Persistence;
using AquaLink.Cooperative.Application.Interfaces;
using AquaLink.Cooperative.Infrastructure.Persistence;
using FluentValidation;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();