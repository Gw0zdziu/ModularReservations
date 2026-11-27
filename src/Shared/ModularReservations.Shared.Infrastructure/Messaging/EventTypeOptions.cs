using System.Diagnostics.CodeAnalysis;
using ModularReservations.Shared.Interfaces.Event;

namespace ModularReservations.Shared.Infrastructure.Messaging;

public class EventTypeOptions
{
    private Dictionary<string, Type> Types { get; } = new();
    private Dictionary<Type, string> Keys { get; } = new();

    public EventTypeOptions Register<TEvent>(string valueKey) where TEvent : IEvent
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valueKey);

        if (Types.ContainsKey(valueKey))
        {
            throw new ArgumentException($"Event type with key '{valueKey}' already exists.");
        }
        if (Keys.ContainsKey(typeof(TEvent)))
        {
            throw new ArgumentException($"Event type {typeof(TEvent).Name} is already registered.");
        }
        Types.Add(valueKey, typeof(TEvent));
        Keys.Add(typeof(TEvent), valueKey);
        return this;
    }

    public bool TryGetType(string key, [MaybeNullWhen(false)]out Type type)
    {
        return Types.TryGetValue(key, out type);
    }

    public bool TryGetKey(Type type, [MaybeNullWhen(false)]out string key)
    {
        return Keys.TryGetValue(type, out key);
    }
}
