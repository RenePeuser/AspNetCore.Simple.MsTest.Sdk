using MinimalApi.Database.DbContext;

namespace MinimalApi.Api.ToDos.V1.DeleteById
{
    internal static class AddDeleteToDoByIdExtension
    {
        internal static void AddDeleteToDoById(this IServiceCollection services)
        {
            services.AddSingleton<DeleteToDoById>();
        }
    }

    internal sealed class DeleteToDoById(TodoRepository todoRepository)
    {
        internal void Execute(int id)
        {
            todoRepository.Delete(id);
        }
    }
}
