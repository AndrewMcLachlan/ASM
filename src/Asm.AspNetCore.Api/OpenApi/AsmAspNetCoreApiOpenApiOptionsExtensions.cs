using System.Reflection;
using Asm.AspNetCore.Api;

namespace Microsoft.AspNetCore.OpenApi;

/// <summary>
/// Extensions for <see cref="OpenApiOptions"/> providing common, reusable document and schema conventions.
/// </summary>
public static class AsmAspNetCoreApiOpenApiOptionsExtensions
{
    extension(OpenApiOptions options)
    {
        /// <summary>
        /// Marks non-nullable reference-type properties as <c>required</c> in generated schemas, filling the
        /// gap where .NET's OpenAPI generator reports them as optional.
        /// </summary>
        /// <returns>The same <see cref="OpenApiOptions"/>, for chaining.</returns>
        public OpenApiOptions AddRequiredForNonNullableProperties()
        {
            options.AddSchemaTransformer(new RequiredForNonNullableSchemaTransformer());
            return options;
        }

        /// <summary>
        /// Relocates a common path prefix (e.g. <c>/api</c>) into a relative server URL and strips it from
        /// every operation path.
        /// </summary>
        /// <param name="pathPrefix">The path prefix to relocate, e.g. <c>/api</c>.</param>
        /// <returns>The same <see cref="OpenApiOptions"/>, for chaining.</returns>
        public OpenApiOptions RelocatePathPrefixToServer(string pathPrefix)
        {
            options.AddDocumentTransformer(new ServerPathPrefixDocumentTransformer(pathPrefix));
            return options;
        }

        /// <summary>
        /// Sets the document title and derives its version from the file version of an assembly
        /// (the entry assembly by default).
        /// </summary>
        /// <param name="title">The document title.</param>
        /// <param name="assembly">The assembly whose file version supplies the document version.</param>
        /// <returns>The same <see cref="OpenApiOptions"/>, for chaining.</returns>
        public OpenApiOptions AddDocumentInfo(string title, Assembly? assembly = null)
        {
            options.AddDocumentTransformer(new DocumentInfoTransformer(title, assembly));
            return options;
        }

        /// <summary>
        /// Names schemas from a type's <see cref="System.ComponentModel.DisplayNameAttribute"/> where present,
        /// falling back to the default algorithm. See <see cref="DisplayNameSchemaReferenceIds"/>.
        /// </summary>
        /// <returns>The same <see cref="OpenApiOptions"/>, for chaining.</returns>
        public OpenApiOptions UseDisplayNameSchemaReferenceIds()
        {
            options.CreateSchemaReferenceId = DisplayNameSchemaReferenceIds.Resolve;
            return options;
        }
    }
}
