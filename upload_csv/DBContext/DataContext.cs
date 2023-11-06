using upload_csv.Models;
using Microsoft.EntityFrameworkCore;

namespace upload_csv
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<CSV> CSV_data { get; set; } 
        
    }
}
