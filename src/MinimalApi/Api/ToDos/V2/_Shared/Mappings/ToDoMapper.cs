namespace MinimalApi.Api.ToDos.V2._Shared
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
    }
}
