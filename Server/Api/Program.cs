using api;
using Api;
using Api.Security;
using Api.Services.Classes;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


public class Program
{ 
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "ClientCors",
                policy =>
                    policy
                        .WithOrigins(
                            "https://mindst-2-commits-client.fly.dev",
                            "http://localhost:5173"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
            );
        });

        var appOptions = services.AddAppOptions(configuration);

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContext<MyDbContext>(conf => { conf.UseNpgsql(appOptions.DBConnectionString); });

        services.AddScoped<KonciousArgon2idPasswordHasher>();
        services.AddScoped<ITokenService, JwtService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<IBalanceService, BalanceService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IHistoryService, HistoryService>();

        services.AddControllers();
        services.AddOpenApiDocument(config =>
        {
            config.Title = "Dead Pidgeons API";
            config.Version = "v0.1";
        });
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // Authentication & Authorization
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtService.ValidationParameters(
                    configuration
                );

                options.MapInboundClaims = false;
                options.TokenValidationParameters.RoleClaimType = "role";
                options.TokenValidationParameters.NameClaimType = "sub";

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated successfully");
                        return Task.CompletedTask;
                    },
                };
            });
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                // Globally require users to be authenticated
                .RequireAuthenticatedUser()
                .Build();
        });
    }

    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();
        app.UseExceptionHandler();
        app.UseRouting();

        app.UseCors("ClientCors");

// Serve Swagger/OpenAPI before auth so it doesn't get caught by the fallback policy
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.GenerateApiClientsFromOpenApi("/../../client/src/generated-ts-client.ts");

        app.Run();
    }
}
    