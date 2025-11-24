using PasswordProgram.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PasswordProgram.PasswordPolicy
{
    public class PasswordValidator : IPasswordValidator
    {
        private readonly PasswordPolicyOptions _options;
        public PasswordValidator(PasswordPolicyOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }


        /// <summary>
        /// Validates a new password against the configured password policy and user-specific requirements.
        /// </summary>
        /// <remarks>The password is validated for minimum length, required numbers of uppercase,
        /// lowercase, digit, and special characters, sufficient difference from the current password, and absence of
        /// personal information such as account name or identifiers. The specific requirements are determined by the
        /// configured password policy options.</remarks>
        /// <param name="currentPasswordPlain">The current password in plain text, or <see langword="null"/> if the user does not have an existing
        /// password. Used to ensure the new password differs sufficiently from the current password.</param>
        /// <param name="newPasswordPlain">The new password in plain text to be validated. Cannot be <see langword="null"/> or empty.</param>
        /// <param name="user">The user whose password is being changed. Used to check for personal information in the new password.</param>
        /// <returns>A <see cref="ValidationResult"/> containing any validation errors found. If the password meets all
        /// requirements, the <c>Errors</c> collection will be empty.</returns>
        public ValidationResult ValidateNewPassword(
            string? currentPasswordPlain,
            string newPasswordPlain,
            User user)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(newPasswordPlain))
            {
                result.Errors.Add("Must provide a password");
                return result;
            }

            if (newPasswordPlain.Length < _options.MinLength)
            {
                result.Errors.Add($"Password must be greater than {_options.MinLength} characters");
            }

            // Password character classes for min number, lowercase, uppercase, special
            int num = 0, lower = 0, upper = 0, special = 0;
            foreach (char ch in newPasswordPlain)
            {
                if (Char.IsUpper(ch))
                {
                    upper++;
                }
                else if (Char.IsLower(ch))
                {
                    lower++;
                }
                else if (Char.IsDigit(ch))
                {
                    num++;
                }
                else special++;
            }

            // Compare classes to minimum requirements
            // num >= 2, lower >= 2, upper >= 2, special >= 2
            if (upper < _options.MinUppercase)
            {
                result.Errors.Add($"Password must contain {_options.MinUppercase} or more uppercase letters");
            }

            if (lower < _options.MinLowercase)
            {
                result.Errors.Add($"Password must contain {_options.MinLowercase} or more lowercase letters");
            }

            if (num < _options.MinDigits)
            {
                result.Errors.Add($"Password must contain {_options.MinDigits} or more numbers");
            }

            if (special < _options.MinSpecialCharacters)
            {
                result.Errors.Add($"Password must contain {_options.MinSpecialCharacters} or more special characters");
            }

            // Check if new password differs from current password by at least 4 characters
            if (!string.IsNullOrEmpty(currentPasswordPlain))
            {
                int diffCount = CountCharacterDifference(currentPasswordPlain, newPasswordPlain);
                if (diffCount < _options.MinDifferentCharacters)
                {
                    result.Errors.Add($"New password must differ from current password by at least 4 characters");
                }
            }

            // Check if new password contains personal info
            if (ContainsPersonalInfo(newPasswordPlain, user))
            {
                result.Errors.Add($"Password can't contain account name, username, \ndisplay name, or personal identifiers");
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified password contains any part of the user's personal information, such as
        /// username, account name, display name, first name, or last name.
        /// </summary>
        /// <remarks>This method checks for substrings of length three or greater from various user
        /// attributes within the password. Use this method to help enforce password policies that prevent inclusion of
        /// personal information.</remarks>
        /// <param name="newPasswordPlain">The plain text password to evaluate for the presence of personal information.</param>
        /// <param name="user">The user whose personal information will be checked against the password. Cannot be null.</param>
        /// <returns>true if the password contains any part of the user's personal information; otherwise, false.</returns>
        private static bool ContainsPersonalInfo(string newPasswordPlain, User user)
        {
            if (string.IsNullOrWhiteSpace(newPasswordPlain))
            {
                return false;
            }

            string passLower = newPasswordPlain.ToLowerInvariant();

            var personalStrings = new[]
            {
                user.Username,
                user.AccountName,
                user.DisplayName,
                user.FirstName,
                user.LastName
            };

            // include separators in the string: space, dot, dash, underscore, comma, @
            char[] separators = { ' ', '.', '-', '_', ',', '@' };

            foreach (var value in personalStrings)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                var parts = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    var tokenLower = part.ToLowerInvariant();

                    if (tokenLower.Length < 3)
                    {
                        continue;
                    }

                    if (passLower.Contains(tokenLower))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Calculates the number of character differences between two password strings, including both differing
        /// characters and differences in length.
        /// </summary>
        /// <remarks>This method compares the passwords character by character up to the length of the
        /// shorter string, then adds the difference in lengths. The result represents the minimum number of
        /// single-character edits required to make the passwords identical. Both input strings must not be null;
        /// otherwise, a NullReferenceException will occur.</remarks>
        /// <param name="currPasswordPlain">The current password in plain text to compare against the new password. Cannot be null.</param>
        /// <param name="newPasswordPlain">The new password in plain text to compare with the current password. Cannot be null.</param>
        /// <returns>The total number of character positions at which the two passwords differ, plus the difference in their
        /// lengths.</returns>
        private static int CountCharacterDifference(string currPasswordPlain, string newPasswordPlain)
        {
            int diffCount = 0;
            int len = Math.Min(currPasswordPlain.Length, newPasswordPlain.Length);
            for (int i = 0; i < len; i++)
            {
                if (currPasswordPlain[i] != newPasswordPlain[i])
                {
                    diffCount++;
                }
            }
            diffCount += Math.Abs(currPasswordPlain.Length - newPasswordPlain.Length);

            return diffCount;
        }
    }
}
