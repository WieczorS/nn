using System.Data;
using System.Text;
using API.Config;
using API.Controllers;
using interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using solutions;


var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtConfig>();
// Add services to the container.
//builder configuration get connection string
builder.Services.AddControllers();
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));
//connection string via dependency injection


//budowanie zmiennej polaczneiowej przez ioptions

var connStrings = builder.Configuration.GetSection("ConnectionStrings").Get<DatabaseOptions>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
// builder.Services.AddTransient<NpgsqlConnection>((sp) => new NpgsqlConnection(builder.Configuration.GetConnectionString("Main")));
builder.Services.AddTransient<NpgsqlConnection>((sp) => new NpgsqlConnection(connStrings.Main));
builder.Services.AddTransient<INewsRepository, NewsMemoryRepository>();//dodanie newsrepository do kontenera DI

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    

}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.Configuration = new OpenIdConnectConfiguration();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
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



