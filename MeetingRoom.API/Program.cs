using MeetingRoom.Application.Services;
using MeetingRoom.Infrastructure;
using MeetingRoom.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var configuredConnectionString = builder.Configuration.GetConnectionString("MeetingRoomDb");
var resolvedDataSource = configuredConnectionString;
const string dataSourcePrefix = "Data Source=";

if (!string.IsNullOrWhiteSpace(configuredConnectionString))
{
    if (configuredConnectionString.StartsWith(dataSourcePrefix, StringComparison.OrdinalIgnoreCase))
    {
        var configuredDataSource = configuredConnectionString[dataSourcePrefix.Length..];

        if (!Path.IsPathRooted(configuredDataSource))
        {
            configuredDataSource = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, configuredDataSource));
        }

        resolvedDataSource = $"{dataSourcePrefix}{configuredDataSource}";
    }
}
else
{
    var databasePath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "MeetingRoom.Infrastructure", "Persistance", "Data", "meetingroom.db"));
    Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
    resolvedDataSource = $"{dataSourcePrefix}{databasePath}";
}

builder.Services.AddDbContext<MeetingRoomContext>(options =>
    options.UseSqlite(resolvedDataSource));

builder.Services.AddInfrastructureRepositories();

builder.Services.AddTransient<IBookingService, BookingService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MeetingRoom.API", Version = "v1" });
    options.EnableAnnotations();
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var contex = scope.ServiceProvider.GetRequiredService<MeetingRoomContext>();
    DbSeeder.SeedDb(contex);
}

app.UseHttpsRedirection();

app.UseCors("DevCors");

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("v1/swagger.json", "Meeting Roop API v1");
});

app.Run();
