using Asm.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Mvc.Tests.TagHelpers;

[Trait("Category", "Unit")]
public class BodyTagHelperTests
{
    private static (TagHelperContext Context, TagHelperOutput Output) CreateContextAndOutput()
    {
        var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");
        var output = new TagHelperOutput("body", new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
        return (context, output);
    }

    private static ViewContext ViewContextFor(string? area, string? controller, string? action)
    {
        var routeData = new RouteData();
        if (area is not null) routeData.Values["area"] = area;
        if (controller is not null) routeData.Values["controller"] = controller;
        if (action is not null) routeData.Values["action"] = action;
        return new ViewContext { HttpContext = new DefaultHttpContext(), RouteData = routeData };
    }

    /// <summary>
    /// Given a body tag helper with area, controller, and action route values
    /// When it is processed
    /// Then it emits a body tag whose class is those values, lowercased
    /// </summary>
    [Fact]
    public void ProcessAddsRouteValuesAsLowercaseClass()
    {
        var (context, output) = CreateContextAndOutput();
        var helper = new BodyTagHelper { ViewContext = ViewContextFor("Admin", "Users", "Index") };

        helper.Process(context, output);

        Assert.Equal("body", output.TagName);
        Assert.Equal("admin users index", output.Attributes["class"].Value);
    }

    /// <summary>
    /// Given a body tag helper with no view context
    /// When it is processed
    /// Then it still emits a body tag without throwing
    /// </summary>
    [Fact]
    public void ProcessToleratesMissingViewContext()
    {
        var (context, output) = CreateContextAndOutput();
        var helper = new BodyTagHelper { ViewContext = null };

        helper.Process(context, output);

        Assert.Equal("body", output.TagName);
        Assert.True(output.Attributes.ContainsName("class"));
    }

    /// <summary>
    /// Given a body tag helper whose output already carries a class attribute
    /// When it is processed
    /// Then the existing class is merged with the route-derived class
    /// </summary>
    [Fact]
    public void ProcessMergesExistingClassAttribute()
    {
        var (context, output) = CreateContextAndOutput();
        output.Attributes.Add(new TagHelperAttribute("class", "existing"));
        var helper = new BodyTagHelper { ViewContext = ViewContextFor("Admin", "Users", "Index") };

        helper.Process(context, output);

        var className = output.Attributes["class"].Value.ToString();
        Assert.Contains("admin users index", className);
        Assert.Contains("existing", className);
    }
}
