using Microsoft.AspNetCore.Identity;
using Simbir.Application.Abstractions;
using Simbir.Application.Other;
using Simbir.Application.Services;
using Simbir.Core.Entities;
using Simbir.Infrastructure.Context;
using Simbir.Infrastructure.Implementations;
using Simbir.RestApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Extensions
builder.Services.AddModifiedSwaggerGen();
builder.Services.AddPostgresDataBaseContext(builder);
builder.Services.AddJwtAuthentication(builder);

// Services
builder.Services
    .AddScoped<ITokenService, TokenService>()
    .AddScoped<IAccountService, AccountService>()
    .AddScoped<IRentService, RentService>()
    .AddScoped<ITransportService, TransportService>();

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<long>>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddSignInManager<SignInManager<ApplicationUser>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
