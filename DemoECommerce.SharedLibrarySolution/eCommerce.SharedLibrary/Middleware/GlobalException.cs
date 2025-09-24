using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.Middleware;

public class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        //Declare default variables
        string message = "sorry, internal server error occured.";
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string title = "Error";

        try
        {
            await next(context);
            
            //ceck if Response is Too many Request // 429 status code
            if(context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                title = "Warning";
                message = "Too many requests. Please try again later.";
                statusCode = (int)StatusCodes.Status429TooManyRequests;
                await ModifyHeader(context, title, message, statusCode);
            }

            // If Response is UnAuthorized // 401 status code
            if(context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                title = "Alert";
                message = "You are not authorized to access this resource.";
                await ModifyHeader(context, title, message, statusCode);
            }

            // If Response is Forbidden // 403 status code
            if(context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                title = "Out of Access";
                message = "You don't have permission to access this resource.";
                await ModifyHeader(context, title, message, statusCode);
            }
        }catch(Exception ex)
        {
            // Log Original Exception /File, Debugger,  Console
            LogException.LogExceptions(ex);

            // check if Exception is Timeout // 408 request timeout
            if (ex is TaskCanceledException || ex is TimeoutException)
            {
                title = "Out of time";
                message = "Request timeout. Please try again later.";
                statusCode = StatusCodes.Status408RequestTimeout;
            }

            // If Expection is caught.
            // If none of the exceptions match, return default message 
            await ModifyHeader(context, title, message, statusCode);
        }
    }

    private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
    {
        // display scary-free message to client
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
        {
            Detail = message,
            Status = statusCode,
            Title = title
        }), CancellationToken.None);
        return;
    }
}
