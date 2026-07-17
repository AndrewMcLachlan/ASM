using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Asm.AspNetCore.Api;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Asm.AspNetCore.Api.Tests;

public class OpenApiSchemaConventionsTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void MarkNonNullableAsRequiredMarksOnlyNonNullableProperties()
    {
        var jsonTypeInfo = JsonSerializerOptions.Default.GetTypeInfo(typeof(Model));
        var schema = new OpenApiSchema();

        RequiredForNonNullableSchemaTransformer.MarkNonNullableAsRequired(schema, jsonTypeInfo);

        Assert.NotNull(schema.Required);
        Assert.Contains("Name", schema.Required);
        Assert.Contains("Age", schema.Required);
        Assert.DoesNotContain("Nickname", schema.Required);
        Assert.DoesNotContain("OptionalAge", schema.Required);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MarkNonNullableAsRequiredIgnoresNonObjectTypes()
    {
        var jsonTypeInfo = JsonSerializerOptions.Default.GetTypeInfo(typeof(int));
        var schema = new OpenApiSchema();

        RequiredForNonNullableSchemaTransformer.MarkNonNullableAsRequired(schema, jsonTypeInfo);

        Assert.True(schema.Required is null || schema.Required.Count == 0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DisplayNameSchemaReferenceIdUsesDisplayNameWhenPresent()
    {
        var jsonTypeInfo = JsonSerializerOptions.Default.GetTypeInfo(typeof(Attributed));

        Assert.Equal("CustomName", DisplayNameSchemaReferenceIds.Resolve(jsonTypeInfo));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DisplayNameSchemaReferenceIdFallsBackToDefault()
    {
        var jsonTypeInfo = JsonSerializerOptions.Default.GetTypeInfo(typeof(Plain));

        Assert.Equal(OpenApiOptions.CreateDefaultSchemaReferenceId(jsonTypeInfo), DisplayNameSchemaReferenceIds.Resolve(jsonTypeInfo));
    }

    /// <summary>
    /// Given a non-object type (which cannot carry a DisplayName)
    /// When its schema reference ID is resolved
    /// Then the default algorithm is used rather than inspecting for a DisplayName
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void DisplayNameSchemaReferenceIdFallsBackForNonObjectTypes()
    {
        var jsonTypeInfo = JsonSerializerOptions.Default.GetTypeInfo(typeof(int));

        Assert.Equal(OpenApiOptions.CreateDefaultSchemaReferenceId(jsonTypeInfo), DisplayNameSchemaReferenceIds.Resolve(jsonTypeInfo));
    }

#nullable enable
    private sealed class Model
    {
        public string Name { get; set; } = "";

        public string? Nickname { get; set; }

        public int Age { get; set; }

        public int? OptionalAge { get; set; }
    }
#nullable restore

    [DisplayName("CustomName")]
    private sealed class Attributed
    {
        public int Value { get; set; }
    }

    private sealed class Plain
    {
        public int Value { get; set; }
    }
}
