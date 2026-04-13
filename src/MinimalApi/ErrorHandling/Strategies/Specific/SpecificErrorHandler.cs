using Extensions.Pack;

namespace MinimalApi.ErrorHandling.Strategies.Specific
{
    public abstract class SpecificErrorHandler<TException> : SpecificErrorHandlerBase where TException : Exception
    {
        protected override bool CanHandleException(Exception exception)
        {
            return typeof(TException) == exception.GetType();
        }

        protected sealed override Task HandleBaseAsync(HttpContext context, Exception exception)
        {
            // Is checked from base class if this instance is able to handle the exception.
            return HandleAsync(context, exception.Cast<TException>());
        }

        protected abstract Task HandleAsync(HttpContext context, TException exception);
    }
}
