namespace NaplexAPI.Services
{
    public interface IAssignToStoreService
    {
        Task AssignAdditionalStoreToUserAsync(string userId, int storeId);
    }
}
