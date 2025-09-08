using DemoWebAPI.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning(opts =>
{
    opts.AssumeDefaultVersionWhenUnspecified = true;
    opts.DefaultApiVersion = new(1, 0);
    opts.ReportApiVersions = true;
});

// Add Authorization
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy(PolicyConstants.MustBeTheOwner, policy =>
        policy.RequireClaim("employeeId")
    );
    opts.AddPolicy(PolicyConstants.MustBeTheOwner, policy =>
        policy.RequireClaim("title", "Owner")
    );
    opts.AddPolicy(PolicyConstants.MustBeAVeteranEmployee, policy =>
        policy.RequireClaim("employeeId", "E0001", "E0002")
    );
    // Require Auth By Default
    opts.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Add Authentication services
builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateAudience = false,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Issuer"),
                        ValidAudience = builder.Configuration.GetValue<string>("Authentication:Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.ASCII.GetBytes(
                                builder.Configuration.GetValue<string>("Authentication:SecretKey") 
                                ?? throw new InvalidOperationException("Secret key not found in configuration")
                            )
                        )
                    };
                });

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
