using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordProgram.data
{
    /// <summary>
    /// Represents a record of a user's previous password, including the hashed password and the date it was changed.
    /// </summary>
    /// <remarks>This class is typically used to enforce password history policies, such as preventing users
    /// from reusing recent passwords. Each instance associates a password hash with a user and the time the password
    /// was last changed.</remarks>
    public class PasswordHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
