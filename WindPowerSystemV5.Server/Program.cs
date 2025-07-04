global using WindPowerSystemV5.Server;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WindPowerSystemV5.Server.Data;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Microsoft.AspNetCore.Identity;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data.MongoDbModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WindPowerSystemV5.Server.Data.GraphQL;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.HttpOverrides;
using WindPowerSystemV5.Server.Config;
using WindPowerSystemV5.Server.Mappings;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using WindPowerSystemV5.Server.Utils.Exceptions;
using WindPowerSystemV5.Server.Services;
using WindPowerSystemV5.Server.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.MSSqlServer(connectionString:
                ctx.Configuration.GetConnectionString("DefaultConnection"),
                restrictedToMinimumLevel: LogEventLevel.Information,
                sinkOptions: new MSSqlServerSinkOptions
                {
                    TableName = "LogEvents",
                    AutoCreateSqlTable = true
                })
    .WriteTo.Console()
    );

// Add services to the container.
builder.Services.AddHealthChecks()
    .AddCheck("ICMP_01", new ICMPHealthCheck("www.ryadel.com", 100))
    .AddCheck("ICMP_02", new ICMPHealthCheck("www.google.com", 100))
    .AddCheck("ICMP_03", new ICMPHealthCheck($"www.{Guid.NewGuid():N}.com", 100));

builder.Services.AddSingleton<IMemoryCache>(
    new MemoryCache(new MemoryCacheOptions()));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOptions<BlobStorageOptions>()
    .BindConfiguration("AzureBlobStorage");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
    options.AddPolicy(name: "AngularPolicy",
        cfg => {
            cfg.AllowAnyHeader();
            cfg.AllowAnyMethod();
            cfg.WithOrigins(builder.Configuration["AllowedCORS"]!);
        }));

builder.Services.AddSignalR();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        //.LogTo(Console.WriteLine)
        //#if DEBUG
        //    .EnableSensitiveDataLogging()
        //    .EnableDetailedErrors()
        //#endif
);

builder.Services.Configure<NewsDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
    /*Identity API endpoints, a new set of auth-related endpoints introduced
    with.NET 8 that can be used by SPAs to obtain the access tokens required to grant authentication
    and authorization rights—a feature that looks promising but still too lacking to be used in production.
    Currently, the Identity API endpoints are not fully functional and are not recommended for production use.*/
    //.AddApiEndpoints()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<ICosmosDbContext, CosmosDbContext>();
builder.Services.AddScoped<JwtHandler>();
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();
builder.Services.AddSingleton(AutomapperConfig.CreateMapper());

builder.Services.AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddFiltering()
    .AddSorting();

// Register JwtBearerMiddleware
builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecurityKey"]!))
            };
        })
    .AddBearerToken(IdentityConstants.BearerScheme);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // Minimal API
    app.MapGet("/Error", () => Results.Problem());
    app.UseHsts();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        switch (exception)
        {
            case NotFoundException notFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { error = notFoundException.Message });
                break;
            case BadRequestException badRequestException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = badRequestException.Message });
                break;
        }
    });
});

app.UseHttpsRedirection();

// Invoke the UseForwardedHeaders middleware and configure it 
// to forward the X-Forwarded-For and X-Forwarded-Proto headers.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AngularPolicy");

app.UseHealthChecks(new PathString("/api/health"), new CustomHealthCheckOptions());

// To enable Identity API endpoints
//app.MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.MapGraphQL("/api/graphql");

// Minimal API (it is necessary for FE to verify that BE is online)
app.MapMethods("/api/heartbeat", new[] { "HEAD" },
    () => Results.Ok());

app.MapHub<HealthCheckHub>("/api/health-hub");

// Minimal API
app.MapGet("/api/broadcast/update2", async (IHubContext<HealthCheckHub> hub) =>
{
    await hub.Clients.All.SendAsync("Update", "test-message from minimal API");
    return Results.Text("Update message sent from minimal API.");
});

app.MapFallbackToFile("/index.html");

app.Run();
