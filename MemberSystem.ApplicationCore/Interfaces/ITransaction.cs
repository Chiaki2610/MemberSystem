namespace MemberSystem.ApplicationCore.Interfaces
{
    public interface ITransaction : ITransActionAsync
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
