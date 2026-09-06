namespace Api.Services.Interfaces
{
    public interface ICategorySuggestionService
    {
        IReadOnlyList<int> GetSuggestedCategoryIds(string? content);
    }
}
