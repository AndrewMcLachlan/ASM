using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Mvc;

/// <summary>
/// Helper class for views.
/// </summary>
public class ViewHelper(IActionContextAccessor contextAccessor, IWebHostEnvironment environment, ICompositeViewEngine viewEngine, IUrlHelperFactory urlHelperFactory, IHttpContextFactory httpContextFactory)
{
    /// <summary>
    /// A URL helper.
    /// </summary>
    public IUrlHelper UrlHelper { get; private set; } = urlHelperFactory.GetUrlHelper(contextAccessor.ActionContext!);

    /// <summary>
    /// The action context.
    /// </summary>
    public ActionContext Context { get; private set; } = contextAccessor.ActionContext!;

    /// <summary>
    /// Gets the physical location of a view layout from the URL.
    /// </summary>
    /// <param name="controller">The controller name.</param>
    /// <param name="action">The action name.</param>
    /// <param name="area">The area name.</param>
    /// <returns>The physical path to the view for the URL, otherwise <c>null</c>.</returns>
    public string? GetPhysicalPath(string controller, string action, string? area = null)
    {
        var routeData = new RouteData();
        routeData.Values["controller"] = controller;
        routeData.Values["action"] = action;
        if (area != null)
        {
            routeData.Values["area"] = area;
        }

        FeatureCollection featureCollection = new();
        featureCollection.Set<IHttpRequestFeature>(new HttpRequestFeature());
        featureCollection.Set<IHttpResponseFeature>(new HttpResponseFeature());
        featureCollection.Set<IRoutingFeature>(new RoutingFeature() { RouteData = routeData });

        var context = httpContextFactory.Create(featureCollection);

        ActionContext ac = new(context, context.GetRouteData(), new ControllerActionDescriptor());
        ControllerContext cc = new(ac);

        IView? view = viewEngine.FindView(cc, cc.RouteData.Values["action"]!.ToString()!, true).View;

        if (view == null) return null;

        return Path.Combine(environment.ContentRootPath, view.Path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
    }

    /// <summary>
    /// Get the physical path of a view from a URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The physical path.</returns>
    public async Task<string?> GetPhysicalPath(string url)
    {
        FeatureCollection featureCollection = new();
        featureCollection.Set<IHttpRequestFeature>(new HttpRequestFeature());
        featureCollection.Set<IHttpResponseFeature>(new HttpResponseFeature());
        featureCollection.Set<IRoutingFeature>(new RoutingFeature());

        var context = httpContextFactory.Create(featureCollection);

        RouteContext routeContext = new(context);

        featureCollection.Get<IRoutingFeature>()!.RouteData = routeContext.RouteData;

        await Context.HttpContext.GetRouteData().Routers[0].RouteAsync(routeContext);

        ActionContext ac = new(context, routeContext.RouteData, new ControllerActionDescriptor());
        ControllerContext cc = new(ac);

        IView? view = viewEngine.FindView(cc, cc.RouteData.Values["action"]!.ToString()!, true).View;

        if (view == null) return null;

        return  Path.Combine(environment.ContentRootPath, view.Path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
    }
}
