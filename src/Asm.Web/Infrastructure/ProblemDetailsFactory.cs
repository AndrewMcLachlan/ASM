﻿using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;

namespace Asm.Web;

public class ProblemDetailsFactory : Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory
{
    private readonly IHostEnvironment _hostEnvironment;

    private static readonly Dictionary<Type, Func<ProblemDetails>> _handers = new();

    public static IReadOnlyDictionary<Type, Func<ProblemDetails>> Handlers { get => _handers; }

    public ProblemDetailsFactory(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
    }

    public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
    {
        var errorContext = httpContext.Features.Get<IExceptionHandlerFeature>();

        if (errorContext?.Error == null)
        {
            return new ProblemDetails
            {
                Detail = detail,
                Instance = instance,
                Status = statusCode ?? 500,
                Title = title,
                Type = type,
            };
        }

        if (Handlers.ContainsKey(errorContext.Error.GetType()))
        {
            var customProblemDetails = Handlers[errorContext.Error.GetType()]();
            customProblemDetails.Status ??= 500;
            return customProblemDetails;
        }

        ProblemDetails problemDetails = new();

        switch (errorContext.Error)
        {
            case NotFoundException _:
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                problemDetails.Title = "Not found";
                problemDetails.Detail = errorContext.Error.Message;
                problemDetails.Type = "http://andrewmclachlan.com/error/notfound";
                break;
            case ExistsException _:
                httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                problemDetails.Title = "Already exists";
                problemDetails.Detail = errorContext.Error.Message;
                problemDetails.Type = "http://andrewmclachlan.com/error/exists";
                break;
            default:
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "Unexpected error occured";
                problemDetails.Detail = !_hostEnvironment.IsProduction() ? errorContext.Error.ToString() : null;
                problemDetails.Type = "http://andrewmclachlan.com/error/unknown";
                break;
        }

        problemDetails.Status = httpContext.Response.StatusCode;

        AddExtensions(httpContext, problemDetails);

        return problemDetails;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelStateDictionary, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
    {
        if (modelStateDictionary == null)
        {
            throw new ArgumentNullException(nameof(modelStateDictionary));
        }

        statusCode ??= 400;

        var problemDetails = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode,
            Type = type,
            Detail = detail,
            Instance = instance,
        };

        if (title != null)
        {
            // For validation problem details, don't overwrite the default title with null.
            problemDetails.Title = title;
        }

        AddExtensions(httpContext, problemDetails);

        return problemDetails;
    }

    public static void AddHandler<T>(Func<ProblemDetails> handler) where T : Exception
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        _handers.Add(typeof(T), handler);
    }

    private static void AddExtensions(HttpContext httpContext, ProblemDetails problemDetails)
    {
        var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
        if (traceId != null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }
    }
}