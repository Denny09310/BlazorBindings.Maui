using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace BlazorBindings.Maui.Extensions;

public static class EqualityHelper
{
    private static readonly ConcurrentDictionary<Key, Func<object, object>> _fieldAccessors = new();

    // Added to solve temporarily the issue https://github.com/dotnet/aspnetcore/issues/53361
    public static bool IsEqual(EventCallback callback, object obj)
    {
        if (obj is not EventCallback other)
        {
            return false;
        }

        return AreEqual(callback, other);
    }

    public static bool IsEqual<T>(EventCallback<T> callback, object obj)
    {
        if (obj is not EventCallback<T> other)
        {
            return false;
        }

        return AreEqual(callback, other);
    }

    private static bool AreEqual(object callback1, object callback2)
    {
        var delegate1 = GetDelegate(callback1);
        var delegate2 = GetDelegate(callback2);

        var receiver1 = GetReceiver(callback1);
        var receiver2 = GetReceiver(callback2);

        return ReferenceEquals(receiver1, receiver2) && Equals(delegate1, delegate2);
    }

    private static MulticastDelegate GetDelegate(object callback)
    {
        return (MulticastDelegate)GetField(callback.GetType(), "Delegate")(callback);
    }

    private static Func<object, object> GetField(Type type, string fieldName)
    {
        return _fieldAccessors.GetOrAdd(new(type, fieldName), static key =>
        {
            var fieldInfo = key.Type.GetField(key.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"Field '{key.FieldName}' not found in type '{key.Type}'.");

            // Create a lambda to access the field without reflection overhead
            var param = Expression.Parameter(typeof(object), "instance");
            var fieldAccess = Expression.Field(Expression.Convert(param, key.Type), fieldInfo);
            var lambda = Expression.Lambda<Func<object, object>>(Expression.Convert(fieldAccess, typeof(object)), param);

            return lambda.Compile();
        });
    }

    private static IHandleEvent GetReceiver(object callback)
    {
        return (IHandleEvent)GetField(callback.GetType(), "Receiver")(callback);
    }

    private sealed record Key(Type Type, string FieldName);
}