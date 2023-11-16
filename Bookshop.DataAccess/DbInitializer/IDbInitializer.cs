namespace Bookshop.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {
        Task<bool> Initialize();
    }
}
