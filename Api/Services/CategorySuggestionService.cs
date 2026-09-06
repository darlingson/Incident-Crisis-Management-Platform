using Api.Services.Interfaces;

namespace Api.Services
{
    public class CategorySuggestionService : ICategorySuggestionService
    {
        public IReadOnlyList<int> GetSuggestedCategoryIds(string? content)
        {
            var suggestions = new List<int>();
            if (string.IsNullOrWhiteSpace(content)) return suggestions;

            var text = content.ToLowerInvariant();

            if (text.Contains("leak") || text.Contains("plumbing") || text.Contains("elevator"))
                suggestions.Add(2); // facilities

            if (text.Contains("theft") || text.Contains("intruder") || text.Contains("unauthorized"))
                suggestions.Add(3); // security

            if (text.Contains("harassment") || text.Contains("bullying") || text.Contains("payroll"))
                suggestions.Add(4); // HR

            if (text.Contains("slip") || text.Contains("fall") || text.Contains("hazard") || text.Contains("injury"))
                suggestions.Add(5); // safety

            if (text.Contains("audit") || text.Contains("policy") || text.Contains("violation"))
                suggestions.Add(6); // compliance

            if (!suggestions.Any())
                suggestions.Add(7); // other

            return suggestions.Distinct().ToList();
        }
    }
}
