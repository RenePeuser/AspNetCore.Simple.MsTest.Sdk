namespace MinimalApi.Api.ToDos.V1
{
    public record Notes
    {
        public string Key { get; init; } = string.Empty;
        public string Hint { get; init; } = string.Empty;
    }
}
