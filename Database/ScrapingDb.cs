using InternetTechLab1.Models;
using LiteDB;

namespace InternetTechLab1.Database
{
    public class ScrapingDb
    {
        private readonly string _databaseName;

        public ScrapingDb(string databaseName)
        {
            _databaseName = databaseName;
        }

        public void SavePage(ScrapedPage page)
        {
            using var db = new LiteDatabase(_databaseName);
            var collection = db.GetCollection<ScrapedPage>("scraped_pages");
            collection.Insert(page);
        }

        public List<ScrapedPage> GetAllPages()
        {
            using var db = new LiteDatabase(_databaseName);
            var collection = db.GetCollection<ScrapedPage>("scraped_pages");
            return collection.Query()
                .OrderByDescending(x => x.ScrapedAt)
                .ToList();
        }

        public List<ScrapedPage> GetRecentPages(int count)
        {
            using var db = new LiteDatabase(_databaseName);
            var collection = db.GetCollection<ScrapedPage>("scraped_pages");
            return collection.Query()
                .OrderByDescending(x => x.ScrapedAt)
                .Limit(count)
                .ToList();
        }

        public int GetTotalCount()
        {
            using var db = new LiteDatabase(_databaseName);
            var collection = db.GetCollection<ScrapedPage>("scraped_pages");
            return collection.Count();
        }
    }
}