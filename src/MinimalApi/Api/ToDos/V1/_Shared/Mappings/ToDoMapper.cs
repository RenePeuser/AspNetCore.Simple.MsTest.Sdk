using System.Collections.Immutable;
using MinimalApi.Database;

namespace MinimalApi.Api.ToDos.V1
{
    internal class ToDoMapper
    {
        public TodoEntity ToEntity(Todo todo)
        {
            return new TodoEntity()
                   {
                       DueBy = todo.DueBy,
                       Id = todo.Id,
                       IsComplete = todo.IsComplete,
                       Notes = todo.Notes,
                       Title = todo.Title
                   };
        }

        public Todo ToModel(TodoEntity todoEntity)
        {
            return new Todo()
                   {
                       DueBy = todoEntity.DueBy,
                       Id = todoEntity.Id,
                       IsComplete = todoEntity.IsComplete,
                       Notes = todoEntity.Notes,
                       Title = todoEntity.Title
                   };
        }
        
        public IImmutableList<TodoEntity> ToEntities(IImmutableList<Todo> todos)
        {
            return todos.Select(ToEntity).ToImmutableList();
        }

        public IImmutableList<Todo> ToModels(IImmutableList<TodoEntity> todoEntities)
        {
            return todoEntities.Select(ToModel).ToImmutableList();
        }
    }
}
