namespace MinimalApi.ErrorHandling.Strategies.Specific
{
    public interface ISpecificErrorHandler
    {
        Task<bool> HandleExceptionAsync(HttpContext context,
                                        Exception exception,
                                        bool lastResult);
    }
}