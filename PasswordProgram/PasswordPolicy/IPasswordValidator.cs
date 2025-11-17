using PasswordProgram.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordProgram.PasswordPolicy
{
    public interface IPasswordValidator
    {
        /// <summary>
        /// Validates a proposed new password for a user against password policy requirements and, if provided, the
        /// current password.
        /// </summary>
        /// <remarks>This method does not change the user's password; it only performs validation. Callers
        /// should ensure that sensitive data such as plain text passwords are handled securely.</remarks>
        /// <param name="currentPasswordPlain">The user's current password in plain text, or null if the user does not have an existing password. Used to
        /// check for similarity or reuse, if applicable.</param>
        /// <param name="newPasswordPlain">The proposed new password in plain text to be validated against password policy rules.</param>
        /// <param name="user">The user for whom the password change is being validated. Must not be null.</param>
        /// <returns>A ValidationResult indicating whether the new password meets all policy requirements. Contains error
        /// information if validation fails.</returns>
        ValidationResult ValidateNewPassword(
            string? currentPasswordPlain,
            string newPasswordPlain,
            User user);
    }
}
