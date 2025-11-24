using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordProgram.data
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the application, providing access to user and password
    /// history entities.
    /// </summary>
    /// <remarks>Use this context to query and save instances of User and PasswordHistory. Configure this
    /// context with dependency injection and provide appropriate DbContextOptions when constructing an instance. This
    /// class is typically used with Entity Framework Core's migration and change tracking features.</remarks>
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<PasswordHistory> PasswordHistories => Set<PasswordHistory>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Configures the entity framework model for the context, including entity properties, relationships, and
        /// constraints.
        /// </summary>
        /// <remarks>This method is called by the Entity Framework runtime when the model for the context
        /// is being created. Override this method to customize the model by configuring entities, relationships, and
        /// constraints using the provided ModelBuilder instance. Call the base implementation to ensure that any
        /// configuration in the base class is applied.</remarks>
        /// <param name="modelBuilder">The builder used to construct the model for the context. Provides configuration for entity types and their
        /// relationships.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(builder =>
            {
                builder.HasKey(u => u.Id);

                builder.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(256);

                builder.Property(u => u.AccountName)
                    .IsRequired()
                    .HasMaxLength(256);

                builder.Property(u => u.DisplayName)
                    .IsRequired()
                    .HasMaxLength(512);

                builder.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(256);

                builder.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(256);

                builder.Property(u => u.PasswordHash)
                       .IsRequired();
            });


            modelBuilder.Entity<PasswordHistory>(builder =>
            {
                builder.HasKey(ph => ph.Id);

                builder.HasOne(ph => ph.User)
                          .WithMany(u => u.PasswordHistories)
                          .HasForeignKey(ph => ph.UserId)
                          .OnDelete(DeleteBehavior.Cascade);

                builder.Property(ph => ph.PasswordHash)
                    .IsRequired();
            });
        }
    }
}
