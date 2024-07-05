using SaucyRegistrations.Generators.Infrastructure;

namespace SaucyRegistrations.Generators.Tests.Infrastructure;

public class SourceWriterTests
{
    [Fact]
    public void AppendLine_ShouldAppendLine()
    {
        // Arrange
        var sourceWriter = new SourceWriter();

        // Act
        sourceWriter.AppendLine("using Microsoft.Extensions.DependencyInjection;");

        // Assert
        sourceWriter.ToString().Should().Be("using Microsoft.Extensions.DependencyInjection;\n");
    }
}