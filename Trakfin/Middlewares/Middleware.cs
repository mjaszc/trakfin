using System.Net;

namespace Trakfin.Middlewares
{
    class CancelledTaskBugWorkaroundMiddleware
    {
        private readonly RequestDelegate _next;

        public CancelledTaskBugWorkaroundMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            try
            {
                await _next(context);

                if (token.IsCancellationRequested)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    return;
                }
            }
            catch (Exception ex)
            {
                if (IsAspNetBugException(ex))
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                else
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        private static bool IsAspNetBugException(Exception exception)
        {
            return
                (exception is OperationCanceledException)
                &&
                exception.StackTrace.Contains("System.Web.HttpApplication.ExecuteStep");
        }
    }
}