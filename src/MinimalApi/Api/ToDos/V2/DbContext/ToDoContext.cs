using Extensions.Pack;
using MinimalApi.Api.ToDos.V2._Shared;

namespace MinimalApi.Api.ToDos.V2.DbContext
{
    internal static class AddTodoEntityRepositoryExtension
    {
        internal static void AddTodoEntityRepository(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<TodoEntityRepository>();
        }
    }

    // I need a TodoEntity repository class which use internaly a simple List<TodoEntity> to store the TodoEntitys
    // - get all TodoEntity
    // - delete a TodoEntity by id
    // - get a TodoEntity by id
    // - update a TodoEntity by id
    internal sealed class TodoEntityRepository
    {
        private readonly List<TodoEntity> _TodoEntitys = new List<TodoEntity>();

        public IEnumerable<TodoEntity> GetAll()
        {
            return _TodoEntitys;
        }

        public TodoEntity Add(TodoEntity TodoEntity)
        {
            _TodoEntitys.Add(TodoEntity);
            return TodoEntity;
        }

        public TodoEntity GetById(int id)
        {
            return _TodoEntitys.First(t => t.Id == id);
        }

        public void Delete(int id)
        {
            var TodoEntity = _TodoEntitys.FirstOrDefault(t => t.Id == id);

            if (TodoEntity != null)
            {
                _TodoEntitys.Remove(TodoEntity);
            }
        }

        public TodoEntity Update(int id, TodoEntity TodoEntity)
        {
            var existingTodoEntity = _TodoEntitys.FirstOrDefault(t => t.Id == id);
            if (existingTodoEntity != null)
            {
                _TodoEntitys.Remove(existingTodoEntity);
                _TodoEntitys.Add(TodoEntity);
            }

            return TodoEntity;
        }
    }
}
