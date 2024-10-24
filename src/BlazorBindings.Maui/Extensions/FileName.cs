using System.Collections.Concurrent;
using System.Reflection;

namespace BlazorBindings.Maui.Extensions;

public static class EqualityHelper
{
    private static readonly ConcurrentDictionary<(Type, string), FieldInfo> _fieldsRegisty = new();

    // Added to solve temporarily the issue https://github.com/dotnet/aspnetcore/pull/53395
    public static bool IsEqual(EventCallback callback, object obj)
    {
        if (obj is not EventCallback other)
        {
            return false;
        }

        var delegate1 = GetDelegate(callback);
        var delegate2 = GetDelegate(other);

        var receiver1 = GetReceiver(callback);
        var receiver2 = GetReceiver(other);

        return ReferenceEquals(receiver1, receiver2) && Equals(delegate1, delegate2);

        static MulticastDelegate GetDelegate(EventCallback e)
        {
            var field = GetField(typeof(EventCallback), "Delegate");
            return (MulticastDelegate)field.GetValue(e);
        }

        static IHandleEvent GetReceiver(EventCallback e)
        {
            var field = GetField(typeof(EventCallback), "Receiver");
            return (IHandleEvent)field.GetValue(e);
        }
    }
    public static bool IsEqual<T>(EventCallback<T> callback, object obj)
    {
        if (obj is not EventCallback<T> other)
        {
            return false;
        }

        var delegate1 = GetDelegate(callback);
        var delegate2 = GetDelegate(other);

        var receiver1 = GetReceiver(callback);
        var receiver2 = GetReceiver(other);

        return ReferenceEquals(receiver1, receiver2) && Equals(delegate1, delegate2);


        static MulticastDelegate GetDelegate(EventCallback<T> e)
        {
            var field = GetField(e.GetType(), "Delegate");
            return (MulticastDelegate)field.GetValue(e);
        }

        static IHandleEvent GetReceiver(EventCallback<T> e)
        {
            var field = GetField(e.GetType(), "Receiver");
            return (IHandleEvent)field.GetValue(e);
        }
    }

    private static FieldInfo GetField(Type type, string name) =>
        _fieldsRegisty.GetOrAdd(
            key: (type, name),
            value: type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
}