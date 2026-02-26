using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace StockManagement.Business.Constants.Middlewares
{
    public class StatusCodeHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly StatusCodePagesOptions _options;

        public StatusCodeHandlerMiddleware(RequestDelegate next, IOptions<StatusCodePagesOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode >= 400 && !context.Response.HasStarted)
            {
                switch (context.Response.StatusCode)
                {
                    case 400:
                        context.Response.Redirect($"/Home/BadRequest?code={context.Response.StatusCode}");
                        break;
                    case 404:
                        context.Response.Redirect($"/Home/NotFound?code={context.Response.StatusCode}");
                        break;
                    case 500:
                        context.Response.Redirect($"/Home/ServerError?code={context.Response.StatusCode}");
                        break;
                        //.....
                }
            }
        }
    }
}
