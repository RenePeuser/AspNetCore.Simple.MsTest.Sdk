using System.Collections.Immutable;
using MinimalApi.Database.DbContext;

namespace MinimalApi.Api.ToDos.V1.GetAll
{
    internal static class AddGetAllToDosQueryExtension
    {
        internal static void AddGetAllToDosQuery(this IServiceCollection services)
        {
            services.AddSingleton<GetAllToDosQuery>();
        }
    }

    internal sealed class GetAllToDosQuery(TodoRepository todoRepository,
                                           ToDoMapper toDoMapper)
    {
        internal IEnumerable<Todo> Execute()
        {
            var todoEntities = todoRepository.GetAll();

            var models = toDoMapper.ToModels(todoEntities.ToImmutableList());

            return models;
        }
    }
}
