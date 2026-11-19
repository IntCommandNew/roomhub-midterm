namespace RoomHub.Api.Domain.Common;

// "No value" for operations that only succeed or fail: Result<Unit>.
public sealed class Unit
{
    public static readonly Unit Value = new();
    private Unit() { }
}
