using NetArchTest.Rules;
using NUnit.Framework.Legacy;

namespace Architecture.Tests;

public class ArhitectureTests
{
	private const string DomainNamespace = "Domain";
	private const string ApplicationNamespace = "Application";
	private const string InfrastructureNamespace = "Infrastructure";
	private const string SharedNamespace = "Shared";
	private const string WorkerServiceNamespace = "CCFCleanWSTemplate";

	[Test]
	public void Domain_Should_Not_HaveDependencyOnOtherProjects()
	{
		// Arrange
		var assembly = typeof(Domain.AssemblyReference).Assembly;

		var otherProjects = new[]
		{
			ApplicationNamespace,
			InfrastructureNamespace,
			SharedNamespace,
		};

		// Act
		var testResult = Types
			.InAssembly(assembly)
			.ShouldNot()
			.HaveDependencyOnAll(otherProjects)
			.GetResult();

		// Assert
		ClassicAssert.True(testResult.IsSuccessful);
	}

	[Test]
	public void Application_Should_Not_HaveDependencyOnOtherProjects()
	{
		// Arrange
		var assembly = typeof(Application.AssemblyReference).Assembly;

		var otherProjects = new[]
		{
			InfrastructureNamespace,
			DomainNamespace
		};

		// Act
		var testResult = Types
			.InAssembly(assembly)
			.ShouldNot()
			.HaveDependencyOnAll(otherProjects)
			.GetResult();

		// Assert
		ClassicAssert.True(testResult.IsSuccessful);
	}

	[Test]
	public void Infrastructure_Should_Not_HaveDependencyOnOtherProjects()
	{
		// Arrange
		var assembly = typeof(Infrastructure.AssemblyReference).Assembly;

		var otherProjects = new[]
		{
			WorkerServiceNamespace
		};

		// Act
		var testResult = Types
			.InAssembly(assembly)
			.ShouldNot()
			.HaveDependencyOnAll(otherProjects)
			.GetResult();

		// Assert
		ClassicAssert.True(testResult.IsSuccessful);
	}
}