namespace MinimalApi.Api.ToDos.V2.GetAll
{
    public static class AddGetAllToDosQueryExtension
    {
        public static void AddGetAllToDosQuery(this IServiceCollection services)
        {
            services.AddSingleton<GetAllToDosQuery>();
        }
    }

    internal sealed class GetAllToDosQuery
    {
        private readonly Todo[] sampleTodos =
        {
            new(1, "Walk the dog"), new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)), new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))), new(4, "Clean the bathroom"),
            new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
        };

        internal IEnumerable<Todo> Execute()
        {
            return sampleTodos;
        }
    }
}
