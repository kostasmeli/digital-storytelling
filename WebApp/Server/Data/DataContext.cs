
namespace BlazorApp.Server.Data
{
	public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
	{
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Answers)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<DialogueObject>()
				.HasMany(q=>q.QuestionSet)
				.WithOne()
				.OnDelete(DeleteBehavior.Cascade);

            //Dummy Data Upon Creation
            modelBuilder.Entity<User>().HasData(
			new User { FirstName = "Konstantinos", LastName = "Melissourgos", Email = "KonstantinosMel@gmail.com", Username = "Kwstasmel", Password = "1q2w3e4r", Role = "Admin" },
			new User { FirstName = "Konstantinos", LastName = "Melissourgos", Email = "KonstantinosMel@gmail.com", Username = "UserMeli", Password = "test123" }
				);
			modelBuilder.Entity<DialogueSession>().HasData(
			new DialogueSession { id = Guid.NewGuid(), date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), Score = 1, Username = "UserMeli", DialogueTitle = "Test" , MaxScore=10 },
			new DialogueSession { id = Guid.NewGuid(), date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), Score = 1,  Username = "UserMeli" , DialogueTitle="Test" , MaxScore=10}
				);
        }
		public DbSet<DialogueObject> DialogueObjects { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<User> Users { get; set; }
		public DbSet<DialogueSession> DialogueSessions { get; set; }
	}
}
