using InternetTechLab1.Models;
using Microsoft.EntityFrameworkCore;

namespace InternetTechLab1.Database
{
    public class GitHubReposDb : DbContext
    {
        private readonly string _databaseName;

        public DbSet<GitHubRepository> GitHubRepos { get; set; }

        public GitHubReposDb(string databaseName)
        {
            _databaseName = databaseName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_databaseName}");
        }
    }
}