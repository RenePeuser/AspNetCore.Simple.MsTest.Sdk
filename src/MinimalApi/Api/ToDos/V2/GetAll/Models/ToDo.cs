namespace MinimalApi.Api.ToDos.V2.GetAll
{
    public record Todo(int Id,
                       string? Title,
                       DateOnly? DueBy = null,
                       bool IsComplete = false);
}
