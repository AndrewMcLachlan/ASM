# Asm.Umbraco

Core helpers for Umbraco CMS applications:

- `FixedMachineInfoFactory` — an `IMachineInfoFactory` that returns a stable machine identifier, suitable for container deployments where a random name would cause duplicate server registrations on restart.
- `ImgSetTagHelper` — a Razor tag helper that renders an `<img>` with a `srcset` built from a collection of Umbraco media items.
- `IPublishedContent` extensions — `NameAsCssClass()`, `ValueOr<T>()`, `ValueAnd()`.

## Usage

```csharp
umbracoBuilder.AddFixedMachineInfoFactory(opts => opts.MachineName = "myapp");
```

Or, bind from configuration:

```csharp
umbracoBuilder.AddFixedMachineInfoFactory(builder.Configuration.GetSection("MachineInfo"));
```

Register the tag helpers in `_ViewImports.cshtml`:

```razor
@addTagHelper *, Asm.Umbraco
```
