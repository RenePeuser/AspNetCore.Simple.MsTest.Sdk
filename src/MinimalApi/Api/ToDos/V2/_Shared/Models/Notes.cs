namespace MinimalApi.Api.ToDos.V2._Shared
{
    public record Notes
    {
        public string Key { get; init; } = string.Empty;
        public string Hint { get; init; } = string.Empty;
    }
}
