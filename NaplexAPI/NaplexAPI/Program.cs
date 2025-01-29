using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaplexAPI.Infrastructure;
using NaplexAPI.Models.Entities;
using NaplexAPI.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringDateTimeConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Naplex", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Naplex")));  // UseSqlServer is for SQL Server. Adjust for your database.

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IAssignToStoreService, AssignToStoreService>();
builder.Services.AddScoped<IChangeRoleService, ChangeRoleService>();
builder.Services.AddScoped<IRotaService, RotaService>();
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddScoped<ITargetsService, TargetsService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            // Custom logic when the token is valid
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
         // Custom logic when authentication fails
          if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
          {
              context.Response.Headers.Add("Token-Expired", "true");
          }
          return Task.CompletedTask;
        }
    };
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

// Seed roles
var roles = new[] { "Admin", "Store Manager", "Sales Advisor" };
foreach (var role in roles)
{
    if (!await roleManager.RoleExistsAsync(role))
    {
        await roleManager.CreateAsync(new IdentityRole(role));
    }
}

//app.UseHttpsRedirection();

app.UseCors(builder => builder
    .WithOrigins("https://naplex.uk") 
    .WithOrigins("http://localhost:4200") // Localhost
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    );

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public class JsonStringDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _dateFormat = "yyyy-MM-dd";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString(), _dateFormat, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_dateFormat));
    }
}