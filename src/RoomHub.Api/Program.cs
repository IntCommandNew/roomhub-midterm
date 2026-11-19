using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Bookings;
using RoomHub.Api.Features.Rooms;
using RoomHub.Api.Infrastructure.Persistence;
using RoomHub.Api.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Enums appear in JSON as text ("Active" / "Cancelled").
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Time: "now" always comes from TimeProvider (R5, R11).
builder.Services.AddSingleton(TimeProvider.System);

// Persistence: EF Core + SQLite. The DbContext is also the Unit of Work (same scoped instance).
builder.Services.AddDbContext<RoomHubDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RoomHub") ?? "Data Source=roomhub.db"));
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RoomHubDbContext>());
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// CQRS: every ICommandHandler / IQueryHandler in this assembly, registered as Scoped.
builder.Services.AddHandlers(typeof(Program).Assembly);

// Unexpected exceptions (bugs, database down) become a generic 500 ProblemDetails.
// Business errors never throw: they are Result/Error values.
builder.Services.AddProblemDetails();
// Malformed JSON / unreadable body is a client error: keep its 400 instead of turning it into a 500.
builder.Services.Configure<ExceptionHandlerOptions>(options =>
    options.StatusCodeSelector = exception =>
        exception is BadHttpRequestException badRequest
            ? badRequest.StatusCode
            : StatusCodes.Status500InternalServerError);

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseExceptionHandler();

app.MapGet("/", () => "RoomHub API is running.");
app.MapRooms();
app.MapBookings();

app.Run();
