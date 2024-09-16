using EmailSendingWithExcel.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailSendingWithExcel.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmailTemplate> EmailTemplates { get; set; }
    }
}
