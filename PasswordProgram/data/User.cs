using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordProgram.data
{
    /// <summary>
    /// Represents a user account with authentication and profile information.
    /// </summary>
    /// <remarks>The User class contains properties for user identification, authentication, and profile
    /// display. It includes password management features such as password history and the timestamp of the last
    /// password change. This class is typically used in authentication and authorization scenarios to manage user
    /// credentials and related metadata.</remarks>
    public class User
    {
        public int Id { get; set; }

        // We get identity / profile info used in password checks
        public string Username { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public string DisplayName { get; set; } = null!;

        // Hashed password using BCrypt
        public string PasswordHash { get; set; } = null!;

        public DateTime PasswordLastChangedAt { get; set; } = DateTime.UtcNow;

        public List<PasswordHistory> PasswordHistories { get; set; } = new();
    }
}
