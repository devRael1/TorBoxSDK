using System.Reflection;
using TorBoxSDK.V2.ContractTests.Infrastructure;

namespace TorBoxSDK.V2.ContractTests;

public sealed class ContractSurfaceTests
{
    [Fact]
    public void ImplementedMappingsResolveToPublicMethods()
    {
        // Arrange
        IReadOnlyList<CoverageRecord> records = CoverageManifest.Load(ContractTestPaths.CoveragePath).Records;

        // Act
        Action validate = () => ValidateImplementedMappings(records);

        // Assert
        validate();
    }

    internal static void ValidateImplementedMappings(IReadOnlyList<CoverageRecord> records)
    {
        Assembly assembly = Assembly.Load("TorBoxSDK");

        foreach (CoverageRecord record in records.Where(static record => record.ImplementationState == CoverageImplementationState.Implemented))
        {
            string publicInterface = record.PublicInterface ?? throw new InvalidDataException($"{record.Identity} is missing its public interface.");
            string publicMethod = record.PublicMethod ?? throw new InvalidDataException($"{record.Identity} is missing its public method.");
            string resultType = record.ResultType ?? throw new InvalidDataException($"{record.Identity} is missing its result type.");
            Type interfaceType = ResolveType(assembly, publicInterface);
            Assert.True(interfaceType.IsInterface && interfaceType.IsPublic, $"{record.Identity} must name a public interface.");

            Type[] parameterTypes = record.ParameterTypes.Select(typeName => ResolveType(assembly, typeName)).ToArray();
            MethodInfo method = interfaceType.GetMethod(publicMethod, BindingFlags.Public | BindingFlags.Instance, null, parameterTypes, null)
                ?? throw new InvalidDataException($"{record.Identity} must resolve to the declared public method with the exact ordered parameter types.");
            Assert.Equal(resultType, method.ReturnType.AssemblyQualifiedName);
        }
    }

    private static Type ResolveType(Assembly assembly, string typeName)
    {
        Type? type = assembly.GetType(typeName, throwOnError: false) ?? Type.GetType(typeName, throwOnError: false);
        return type ?? throw new InvalidDataException($"The declared type '{typeName}' cannot be resolved.");
    }
}
