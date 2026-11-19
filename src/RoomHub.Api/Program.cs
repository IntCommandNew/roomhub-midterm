using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Endpoints;
using RoomHub.Api.Infrastructure.Persistence;
using RoomHub.Api.Middleware;
using RoomHub.Api.Repositories;
using RoomHub.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Starter organized in layers: Endpoints -> Services -> Repositories -> Models.
builder.Services.AddSingleton<RoomRepository>();
builder.Services.AddSingleton<BookingRepository>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<BookingService>();

// Time: "now" always comes from TimeProvider (R5).
builder.Services.AddSingleton(TimeProvider.System);

// Persistence: EF Core + SQLite. The DbContext is also the Unit of Work (same scoped instance).
builder.Services.AddDbContext<RoomHubDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RoomHub") ?? "Data Source=roomhub.db"));
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RoomHubDbContext>());
builder.Services.AddScoped<IRoomRepository, RoomHub.Api.Infrastructure.Persistence.Repositories.RoomRepository>();
builder.Services.AddScoped<IBookingRepository, RoomHub.Api.Infrastructure.Persistence.Repositories.BookingRepository>();

builder.Services.AddHandlers(typeof(Program).Assembly);

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseMiddleware<ExceptionMiddleware>();

app.MapGet("/", () => "RoomHub API is running.");
app.MapRoomEndpoints();
app.MapBookingEndpoints();

app.Run();
