using AquaLink.Farmer.Application.Common;
using AquaLink.Farmer.Application.FarmCycles;
using AquaLink.Farmer.Application.Interfaces;
using AquaLink.Farmer.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
        typeof(AquaLink.Farmer.Application.FarmCycles.GetFarmCycleQuery).Assembly);
    cfg.AddBehavior(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehaviour<,>));
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateFarmCycleCommandValidator>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();