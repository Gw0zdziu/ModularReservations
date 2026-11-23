using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Infrastructure.Exceptions;

public sealed class ConcurrencyConflictException : AppException
{
    public string EntityName { get; private set; }
    public string EntityId { get; private set; }
    public ConcurrencyConflictException(string entityName, string entityId, Exception innerException) : base($"{entityName} {entityId} was modified by another operation", innerException)
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}
