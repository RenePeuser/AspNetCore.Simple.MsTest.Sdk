using MinimalApi.Database.DbContext;

namespace MinimalApi.Api.ToDos.V1.Create
{
    internal static class AddAddNewToDoExtension
    {
        internal static void AddAddNewToDo(this IServiceCollection services)
        {
            services.AddTodoRepository();

            services.AddSingleton<AddNewToDoCommand>();
        }
    }

    internal sealed class AddNewToDoCommand(TodoRepository todoRepository,
                                            ToDoValidator toDoValidator,
                                            ToDoMapper toDoMapper)
    {
        internal async Task<Todo> ExecuteAsync(Todo todo)
        {
            // 1. Validate incoming data
            await toDoValidator.ValidateAsync(todo).ConfigureAwait(false);

            // 2. Map data into entity
            var todoEntity = toDoMapper.ToEntity(todo);

            // 3. Add new data into db
            var addedEntity = todoRepository.Add(todoEntity);

            // 4. Dependent what we want to return
            //    We map to another model etc.
            var addedToDo = toDoMapper.ToModel(addedEntity);

            // 5. Return the updated object
            return addedToDo;
        }
    }

    internal class ToDoValidator

    {
        public Task ValidateAsync(Todo _)
        {
            return Task.CompletedTask;
        }
    }
}
