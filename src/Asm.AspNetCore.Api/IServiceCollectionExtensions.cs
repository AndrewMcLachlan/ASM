using Microsoft.OpenApi.Models;

namespace Microsoft.Extensions.DependencyInjection;
public static class IServiceCollectionExtensions
{
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

    public static IServiceCollection AddStandardSwaggerGen(this IServiceCollection services, Func<OpenApiInfo, OpenApiInfo> setInfo ) =>
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
