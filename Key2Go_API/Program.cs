using Application.Abstraction;
using Application.Abstraction.ExternalService;
using Application.Service;
using Domain.Entity;
using Infraestructure.ExternalServices;
using Infraestructure.Persistence;
using Infraestructure.Persistence.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Key2GoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Cualquier usuario logueado con rol User, Admin o SuperAdmin
    options.AddPolicy(nameof(RoleType.User), policy => policy.RequireRole(nameof(RoleType.User), nameof(RoleType.Admin), nameof(RoleType.SuperAdmin)));

    // Admin y SuperAdmin (pero NO User)
    options.AddPolicy(nameof(RoleType.Admin), policy => policy.RequireRole(nameof(RoleType.Admin), nameof(RoleType.SuperAdmin)));

    // Solo superadmin
    options.AddPolicy(nameof(RoleType.SuperAdmin), policy => policy.RequireRole(nameof(RoleType.SuperAdmin)));
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Key2Go API",
        Version = "v1",
        Description = "API de gestión para Key2Go"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Ingrese el token JWT",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// inyecciones
#region Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
#endregion

//inyeccion solo de Interfaces, no inyecciones concretas
#region Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAccountService, AccountService>();

//EXTERNAL SERVICES
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUsdArsRateService, UsdArsRateService>();
#endregion

PollySettings DolarApi = new()
{
    RetryCount = 3,         //cuantas veces va a reintentar
    RetryAttemptInSec = 5,  //tiempo entre reintentos
    BreakInSec = 120,       //tiempo que va a estar abierto el circuito
    HandleEventsAllowed = 3  //cantidad de fallos antes de abrir el circuito
};


builder.Services
    .AddHttpClient("DolarApi", client =>
    {
        client.BaseAddress = new Uri("https://dolarapi.com/v1/");
    })
    .AddPolicyHandler(ResiliencePolicies.GetWaitAndRetryPolicy(DolarApi))
    .AddPolicyHandler(ResiliencePolicies.GetCircuitBreakerPolicy(DolarApi));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();