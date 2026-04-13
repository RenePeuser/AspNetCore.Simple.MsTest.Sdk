using MinimalApi.Api.ToDos.V1;

namespace MinimalApi.Database
{
    public record TodoEntity
    {
        public int Id { get; init; }
        public string? Title { get; init; }
        public Notes Notes { get; init; } = new();
        public DateOnly? DueBy { get; init; }
        public bool IsComplete { get; init; }
    }
}
