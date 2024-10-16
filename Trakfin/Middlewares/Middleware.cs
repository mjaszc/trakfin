using System.Net;

namespace Trakfin.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    class CancelledTaskBugWorkaroundMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
                // Try to suppress response content when the cancellation token has fired; ASP.NET will log to the Application event log if there's content in this case.
                if (cancellationToken.IsCancellationRequested)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                return response;

            }
            catch (Exception ex)
            {
                if (IsAspNetBugException(ex))
                    return new HttpResponseMessage(HttpStatusCode.OK);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }


        }
        private static bool IsAspNetBugException(Exception exception)
        {
            return
                (exception is TaskCanceledException || exception is OperationCanceledException)
                &&
                exception.StackTrace.Contains("System.Web.HttpApplication.ExecuteStep");
        }
    }
}
