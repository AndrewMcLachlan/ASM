using Microsoft.OpenApi.Models;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds standard Swagger generation to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance that this method extends.</param>
    /// <param name="title">The Open API title.</param>
    /// <returns>The <see cref="IServiceCollection"/> instance so that calls can be chained.</returns>
    public static IServiceCollection AddStandardSwaggerGen(this IServiceCollection services, string title) =>
        services.AddSwaggerGen(options =>
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

            options.SwaggerDoc("v1", new()
            {
                Title = title,
                Version = fileVersionInfo.FileVersion ?? "0.0",
            });

            options.CustomSchemaIds(type => type.FullName?
                .Replace('+', '.'));
        });

    /// <summary>
    /// Adds standard Swagger generation to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance that this method extends.</param>
    /// <param name="setInfo">A method to set custom info.</param>
    /// <returns>The <see cref="IServiceCollection"/> instance so that calls can be chained.</returns>
    public static IServiceCollection AddStandardSwaggerGen(this IServiceCollection services, Func<OpenApiInfo, OpenApiInfo> setInfo) =>
            services.AddSwaggerGen(options =>
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.Diagnostics.FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

                OpenApiInfo info = new()
                {
                    Version = fileVersionInfo.FileVersion ?? "0.0",
                };

                options.SwaggerDoc("v1", setInfo(info));

                options.CustomSchemaIds(type => type.FullName?
                    .Replace('+', '.'));
            });
}
