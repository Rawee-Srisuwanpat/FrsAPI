using Microsoft.EntityFrameworkCore;

namespace field_recording_api.Utilities
{
    public interface IDbContext
    {
        DbSet<T> Set<T>() where T : class;
        int SaveChanges();
        void Dispose();
    }
}
