using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordProgram.Services
{
    public class PasswordService : IPasswordService
    {
        /// <summary>
        /// Attempts to change the password for the specified user asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose password is to be changed.</param>
        /// <param name="currentPasswordPlain">The user's current password in plain text. Cannot be null or empty.</param>
        /// <param name="newPasswordPlain">The new password to set for the user, in plain text. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The result contains a tuple with a value indicating
        /// whether the password change was successful, and an array of error messages if the operation failed.</returns>
        /// <exception cref="NotImplementedException">The method is not implemented.</exception>
        public Task<(bool Success, string[] Errors)> ChangePasswordAsync(int userId, string currentPasswordPlain, string newPasswordPlain)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempts to register a new user with the specified account, user name, display name, and password.
        /// </summary>
        /// <param name="accountName">The name of the account under which the user will be registered. Cannot be null or empty.</param>
        /// <param name="userName">The unique user name to assign to the new user. Cannot be null or empty.</param>
        /// <param name="displayName">The display name for the user, as it will appear to others. Cannot be null or empty.</param>
        /// <param name="passwordPlain">The plain text password for the new user. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The result contains a tuple with a value indicating
        /// whether the registration succeeded and an array of error messages if the registration failed.</returns>
        /// <exception cref="NotImplementedException">The method is not implemented.</exception>
        public Task<(bool Success, string[] Errors)> RegisterUserAsync(string accountName, string userName, string displayName, string passwordPlain)
        {
            throw new NotImplementedException();
        }
    }
}
