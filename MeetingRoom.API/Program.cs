using MeetingRoom.API.Persistance;
using MeetingRoom.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MeetingRoomContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MeetingRoomDb")));

builder.Services.AddTransient<IBookingService, BookingService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MeetingRoom.API", Version = "v1" });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var contex = scope.ServiceProvider.GetRequiredService<MeetingRoomContext>();
    DbSeeder.SeedDb(contex);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("v1/swagger.json", "Meeting Roop API v1");
});

app.Run();
