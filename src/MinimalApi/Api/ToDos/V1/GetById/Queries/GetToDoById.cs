using MinimalApi.Database.DbContext;

namespace MinimalApi.Api.ToDos.V1.GetById
{
    internal static class AddGetToDoByIdExtension
    {
        internal static void AddGetToDoById(this IServiceCollection services)
        {
            services.AddSingleton<GetToDoById>();
        }
    }

    internal sealed class GetToDoById(TodoRepository todoRepository,
                                      ToDoMapper toDoMapper)
    {
        internal Todo Execute(int id)
        {
            return toDoMapper.ToModel(todoRepository.GetById(id));
        }
    }
}
