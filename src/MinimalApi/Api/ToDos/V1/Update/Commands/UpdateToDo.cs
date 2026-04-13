using MinimalApi.Database.DbContext;

namespace MinimalApi.Api.ToDos.V1.Update
{
    internal static class AddUpdateExtension
    {
        internal static void AddUpdateToDo(this IServiceCollection services)
        {
            services.AddSingleton<UpdateToDo>();
        }
    }

    internal sealed class UpdateToDo(TodoRepository todoRepository,
                                     ToDoMapper toDoMapper)
    {
        internal Todo Execute(int id, Todo todo)
        {
            var entityToUpdate = toDoMapper.ToEntity(todo);

            var todoEntity = todoRepository.Update(id, entityToUpdate);

            return toDoMapper.ToModel(todoEntity);
        }
    }
}
