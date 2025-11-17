using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordProgram.Services
{
    public interface IPasswordService
    {
        /// <summary>
        /// Asynchronously registers a new user account with the specified credentials and display name.
        /// </summary>
        /// <param name="accountName">The name of the account under which the user will be registered. Cannot be null or empty.</param>
        /// <param name="userName">The unique username for the new user. Cannot be null or empty.</param>
        /// <param name="displayName">The display name to associate with the user. Cannot be null or empty.</param>
        /// <param name="passwordPlain">The user's password in plain text. Cannot be null or empty. The password should meet any required security
        /// policies.</param>
        /// <returns>A task that represents the asynchronous operation. The result contains a value indicating whether the
        /// registration was successful and an array of error messages if the registration failed.</returns>
        Task<(bool Success, string[] Errors)> RegisterUserAsync(
            string accountName,
            string userName,
            string displayName,
            string passwordPlain);

        /// <summary>
        /// Attempts to change the password for the specified user asynchronously.
        /// </summary>
        /// <remarks>The method does not perform any password hashing; passwords must be provided in plain
        /// text. The operation may fail if the current password is incorrect or if the new password does not meet
        /// security requirements.</remarks>
        /// <param name="userId">The unique identifier of the user whose password is to be changed.</param>
        /// <param name="currentPasswordPlain">The user's current password in plain text. Cannot be null or empty.</param>
        /// <param name="newPasswordPlain">The new password to set for the user, in plain text. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The result contains a tuple with a value indicating
        /// whether the password change was successful and an array of error messages if the operation failed.</returns>
        Task<(bool Success, string[] Errors)> ChangePasswordAsync(
            int userId,
            string currentPasswordPlain,
            string newPasswordPlain);
    }
}
