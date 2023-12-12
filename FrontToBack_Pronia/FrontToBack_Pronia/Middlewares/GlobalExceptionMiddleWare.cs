namespace FrontToBack_Pronia.Middlewares
{
    public class GlobalExceptionMiddleWare
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleWare(RequestDelegate next)
        {
           _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception e)
            {

                context.Response.Redirect($"/error/errorpage?error={e.Message}");
            }
        }
    }
}
