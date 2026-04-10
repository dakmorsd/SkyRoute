using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkyRoute.Application;
using SkyRoute.Application.Exceptions;
using SkyRoute.Infrastructure;
using SkyRoute.Infrastructure.Configuration;
using SkyRoute.Infrastructure.Persistence;
using SkyRoute.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
	?? throw new InvalidOperationException("JWT configuration is missing.");

builder.Services.AddCors(options =>
{
	options.AddPolicy("Frontend", policy =>
	{
		if (builder.Environment.IsDevelopment())
		{
			policy.SetIsOriginAllowed(static origin =>
			{
				if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
				{
					return false;
				}

				return (string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
						|| string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
					&& (string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase)
						|| string.Equals(uri.Host, "127.0.0.1", StringComparison.OrdinalIgnoreCase));
			});
		}
		else
		{
			policy.WithOrigins("http://localhost:4200");
		}

		policy.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateIssuerSigningKey = true,
			ValidateLifetime = true,
			ValidIssuer = jwtOptions.Issuer,
			ValidAudience = jwtOptions.Audience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
			ClockSkew = TimeSpan.FromSeconds(30)
		};
	});

builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var providerBaseUrl = builder.Configuration["Urls"]
    ?? builder.Configuration.GetSection("Kestrel:Endpoints:Http:Url").Value
    ?? "http://localhost:5279";
builder.Services.AddProviderClients(providerBaseUrl);

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
	errorApp.Run(async context =>
	{
		var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

		var (statusCode, payload) = exception switch
		{
			RequestValidationException validationException =>
				(StatusCodes.Status400BadRequest, (object)new ValidationProblemDetails(new Dictionary<string, string[]>(validationException.Errors))
				{
					Status = StatusCodes.Status400BadRequest,
					Title = validationException.Message
				}),
			NotFoundException notFoundException =>
				(StatusCodes.Status404NotFound, (object)new ProblemDetails
				{
					Status = StatusCodes.Status404NotFound,
					Title = notFoundException.Message
				}),
			ConflictException conflictException =>
				(StatusCodes.Status409Conflict, (object)new ProblemDetails
				{
					Status = StatusCodes.Status409Conflict,
					Title = conflictException.Message
				}),
			UnauthorizedOperationException unauthorizedException =>
				(StatusCodes.Status401Unauthorized, (object)new ProblemDetails
				{
					Status = StatusCodes.Status401Unauthorized,
					Title = unauthorizedException.Message
				}),
			_ =>
				(StatusCodes.Status500InternalServerError, (object)new ProblemDetails
				{
					Status = StatusCodes.Status500InternalServerError,
					Title = "An unexpected error occurred."
				})
		};

		context.Response.StatusCode = statusCode;
		await context.Response.WriteAsJsonAsync(payload);
	});
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<SkyRouteDbContext>();

	if (dbContext.Database.IsRelational())
	{
		await dbContext.Database.MigrateAsync();
	}
	else
	{
		await dbContext.Database.EnsureCreatedAsync();
	}

	var seeder = scope.ServiceProvider.GetRequiredService<SkyRouteDataSeeder>();
	await seeder.SeedAsync(CancellationToken.None);
}

app.Run();

public partial class Program;
