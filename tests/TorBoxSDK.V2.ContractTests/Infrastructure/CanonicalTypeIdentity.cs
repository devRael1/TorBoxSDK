namespace TorBoxSDK.V2.ContractTests.Infrastructure;

internal static class CanonicalTypeIdentity
{
    internal static string Format(Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (!type.IsGenericType)
        {
            return type.FullName
                ?? throw new InvalidOperationException("The type does not have a full name.");
        }

        Type definition = type.GetGenericTypeDefinition();
        string definitionName = definition.FullName
            ?? throw new InvalidOperationException("The generic type definition does not have a full name.");
        string arguments = string.Join(",", type.GetGenericArguments().Select(Format));
        return string.Concat(definitionName, "[", arguments, "]");
    }
}
