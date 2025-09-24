using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.SharedLibrary.Middleware;

public class ListenToOnlyApiGateway(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Extract specific header from the request
        var singedHeader = context.Request.Headers["Api-Gateway"];

        // NULL means, the request is not from API Gateway // 503 Service Unavailable
        if (singedHeader.FirstOrDefault() is null)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("Service Unavailable.");
            return;
        }
        else
        {
            await next(context);
        }

    }
}
