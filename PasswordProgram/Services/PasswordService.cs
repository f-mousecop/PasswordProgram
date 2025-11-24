using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using PasswordProgram.data;
using PasswordProgram.PasswordPolicy;
using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordProgram.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordValidator _passwordValidator;

        public PasswordService(AppDbContext db, IPasswordValidator passwordValidator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _passwordValidator = passwordValidator ?? throw new ArgumentNullException(nameof(passwordValidator));
        }

        public async Task<(bool Success, string[] Errors)> ChangePasswordAsync(
            int userId,
            string currentPasswordPlain,
            string newPasswordPlain)
        {
            var user = await _db.Users
                .Include(u => u.PasswordHistories)
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return (false, new[] { "User not found." });
            }

            // Verify current password matches stored hash
            if (!BCrypt.Net.BCrypt.Verify(currentPasswordPlain, user.PasswordHash))
            {
                return (false, new[] { "Current password is incorrect." });
            }

            // Validate password against policy
            var validation = _passwordValidator.ValidateNewPassword(
                currentPasswordPlain,
                newPasswordPlain,
                user);

            if (!validation.isValid)
            {
                return (false, validation.Errors.ToArray());
            }


            // Check if new password matches any of the last 24 password histories
            if (user.PasswordHistories
                    .OrderByDescending(h => h.ChangedAt)
                    .Take(24)
                    .Any(h => BCrypt.Net.BCrypt.Verify(newPasswordPlain, h.PasswordHash)))
            {
                return (false, new[] { "New password must not match any of your last 24 passwords" });
            }


            // Hash new password using BCrypt
            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPasswordPlain);


            // Update user's password
            user.PasswordHash = newPasswordHash;
            user.PasswordLastChangedAt = DateTime.UtcNow;

            // Append history
            _db.PasswordHistories.Add(new PasswordHistory
            {
                UserId = user.Id,
                PasswordHash = newPasswordHash,
                ChangedAt = user.PasswordLastChangedAt
            });

            await _db.SaveChangesAsync();

            return (true, Array.Empty<string>());
        }


        public async Task<(bool Success, string[] Errors)> RegisterUserAsync(
            string accountName,
            string userName,
            string displayName,
            string firstName,
            string lastName,
            string passwordPlain)
        {
            var user = new User
            {
                AccountName = accountName,
                Username = userName,
                DisplayName = displayName,
                FirstName = firstName,
                LastName = lastName
            };

            // Current password is null for registration
            var validation = _passwordValidator.ValidateNewPassword(
                currentPasswordPlain: null,
                newPasswordPlain: passwordPlain,
                user: user);

            if (!validation.isValid)
            {
                return (false, validation.Errors.ToArray());
            }

            // Hash password using BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(passwordPlain);

            user.PasswordHash = passwordHash;
            user.PasswordLastChangedAt = DateTime.UtcNow;

            // Save user
            _db.Users.Add(user);
            await _db.SaveChangesAsync();


            // Save initial password history
            var history = new PasswordHistory
            {
                UserId = user.Id,
                PasswordHash = passwordHash,
                ChangedAt = user.PasswordLastChangedAt
            };

            _db.PasswordHistories.Add(history);
            await _db.SaveChangesAsync();

            return (true, Array.Empty<string>());
        }
    }
}
