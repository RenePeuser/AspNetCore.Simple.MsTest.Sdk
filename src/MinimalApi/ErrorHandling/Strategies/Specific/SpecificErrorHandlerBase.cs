using Extensions.Pack;

namespace MinimalApi.ErrorHandling.Strategies.Specific
{
    public abstract class SpecificErrorHandlerBase : ISpecificErrorHandler
    {
        public async Task<bool> HandleExceptionAsync(HttpContext context,
                                                     Exception exception,
                                                     bool lastResult)
        {
            if (lastResult)
            {
                return true;
            }

            //  important not to safe cast because we want the specific exception type not a castable verion !!
            if (CanHandleException(exception).IsFalse())
            {
                return false;
            }

            await HandleBaseAsync(context, exception).ConfigureAwait(false);

            return true;
        }

        protected abstract bool CanHandleException(Exception exception);

        protected abstract Task HandleBaseAsync(HttpContext context,
                                                Exception exception);
    }
}