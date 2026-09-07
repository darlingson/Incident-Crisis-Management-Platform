namespace Api.Services.Interfaces
{
    public interface ICurrentUserService
    {
        string? GetUserId();
        bool IsAuthenticated { get; }
    }
}
