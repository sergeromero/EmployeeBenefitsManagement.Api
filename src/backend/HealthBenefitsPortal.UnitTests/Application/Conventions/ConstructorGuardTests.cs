using Benefits.Application;
using FluentAssertions;
using Moq;
using System.Reflection;

namespace HealthBenefitsPortal.UnitTests.Application.Conventions
{
    public sealed class ConstructorGuardTests
    {
        [Theory]
        [MemberData(nameof(ConstructorDependencies))]
        public void Applicaton_Constructors_WithDependencies_ShouldGuardAgainstNull(string typeName, string constructorSignature, string dependencyName)
        {
            var applicationAssembly = typeof(ApplicationAssemblyMarker).Assembly;

            var type = applicationAssembly.GetType(typeName);

            type.Should().NotBeNull();

            var constructor = type!
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Single(c => GetConstructorSignature(c) == constructorSignature);

            var parameters = constructor.GetParameters();

            var dependencyParameter = parameters
                .Single(p => p.Name == dependencyName);

            var arguments = parameters
                .Select(parameter =>
                    parameter == dependencyParameter
                        ? null
                        : CreateValidArgument(parameter))
                .ToArray();

            Exception? actualException = null;

            try
            {
                constructor.Invoke(arguments);
            }
            catch (TargetInvocationException ex)
            {
                actualException = ex.InnerException;
            }

            actualException.Should()
                .BeOfType<ArgumentNullException>($"the constructor for {typeName} must reject null for dependency '{dependencyName}'");

            var argumentException = (ArgumentNullException)actualException!;

            argumentException.ParamName.Should()
                .Be(dependencyName, $"the constructor for {typeName} must identify the correct dependency");
        }

        public static TheoryData<string, string, string> ConstructorDependencies
        {
            get
            {
                var data = new TheoryData<string, string, string>();

                var applicationAssembly = typeof(ApplicationAssemblyMarker).Assembly;

                var concreteTypes = applicationAssembly
                    .GetTypes()
                    .Where(type =>
                        type.IsClass &&
                        !type.IsAbstract &&
                        !type.ContainsGenericParameters &&
                        !IsRecord(type));

                foreach (var type in concreteTypes)
                {
                    var constructors = type.GetConstructors(
                        BindingFlags.Instance | BindingFlags.Public);

                    foreach (var constructor in constructors)
                    {
                        foreach (var parameter in constructor.GetParameters())
                        {
                            if (!IsDependency(parameter))
                            {
                                continue;
                            }

                            data.Add(
                                type.FullName!,
                                GetConstructorSignature(constructor),
                                parameter.Name!);
                        }
                    }
                }

                return data;
            }
        }

        private static bool IsDependency(ParameterInfo parameter)
        {
            return parameter.ParameterType.IsInterface &&
                   !IsDataCollection(parameter.ParameterType);
        }

        private static bool IsDataCollection(Type parameterType)
        {
            return parameterType.IsGenericType &&
                   parameterType.GetGenericTypeDefinition() == typeof(IReadOnlyList<>);
        }

        private static object? CreateValidArgument(ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType;

            if (parameterType.IsInterface)
            {
                return CreateMock(parameterType);
            }

            if (parameterType.IsValueType)
            {
                return Activator.CreateInstance(parameterType);
            }

            if (parameter.HasDefaultValue)
            {
                return parameter.DefaultValue;
            }

            return null;
        }

        private static object CreateMock(Type interfaceType)
        {
            var mockType = typeof(Mock<>).MakeGenericType(interfaceType);

            var mock = (Mock)Activator.CreateInstance(mockType)!;

            return mock.Object;
        }

        private static string GetConstructorSignature(ConstructorInfo constructor)
        {
            return string.Join(
                "|",
                constructor
                    .GetParameters()
                    .Select(parameter => parameter.ParameterType.FullName));
        }

        private static bool IsRecord(Type type)
        {
            return type.GetProperty(
                "EqualityContract",
                BindingFlags.Instance |
                BindingFlags.NonPublic) is not null;
        }
    }
}

