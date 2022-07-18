using Coletor.Models;
using Microsoft.EntityFrameworkCore;

namespace Coletor.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<Document> Document { get; set; }
        public DbSet<Collector> Collector { get; set; }
        public DbSet<FileType> FileType { get; set; }

    }
}
