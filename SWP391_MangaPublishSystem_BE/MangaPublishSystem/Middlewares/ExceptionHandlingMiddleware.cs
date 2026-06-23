using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using DTOs.Responses;
using DTOs.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MangaPublishSystem.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var message = exception.Message; // Trả về lỗi thực tế thay vì lỗi chung chung
            List<string>? errors = null;

            switch (exception)
            {
                case BadRequestException badRequestEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = badRequestEx.Message;
                    errors = badRequestEx.Errors;
                    _logger.LogWarning("Bad Request: {Message}", message);
                    break;

                case NotFoundException notFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundEx.Message;
                    _logger.LogWarning("Not Found: {Message}", message);
                    break;

                case KeyNotFoundException keyNotFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    message = keyNotFoundEx.Message;
                    _logger.LogWarning("Key Not Found: {Message}", message);
                    break;

                case InvalidOperationException invalidOpEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = invalidOpEx.Message;
                    _logger.LogWarning("Invalid Operation: {Message}", message);
                    break;

                case UnauthorizedException unauthorizedEx:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = unauthorizedEx.Message;
                    _logger.LogWarning("Unauthorized: {Message}", message);
                    break;
                    
                case ForbiddenException forbiddenEx:
                    statusCode = HttpStatusCode.Forbidden;
                    message = forbiddenEx.Message;
                    _logger.LogWarning("Forbidden: {Message}", message);
                    break;

                default:
                    // Trả về luôn nội dung exception (kể cả lỗi SQL hay Supabase) để dễ debug
                    message = exception.InnerException != null ? $"{exception.Message} - Inner: {exception.InnerException.Message}" : exception.Message;
                    _logger.LogError(exception, "System Exception: {Message}", message);
                    break;
            }

            var response = ApiResponse<object>.Fail(message, errors);
            
            var jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };
            
            var result = JsonSerializer.Serialize(response, jsonOptions);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(result);
        }
    }
}
