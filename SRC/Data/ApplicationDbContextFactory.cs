using MaChiPhuShoe.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MaChiPhuShoe.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MaChiPhuShoe;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
