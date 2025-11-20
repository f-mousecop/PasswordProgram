using PasswordProgram.data;
using System;
using System.Collections.Generic;
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
                return result;
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
                return result;
            }

            if (lower < _options.MinLowercase)
            {
                result.Errors.Add($"Password must contain {_options.MinLowercase} or more lowercase letters");
                return result;
            }

            if (num < _options.MinDigits)
            {
                result.Errors.Add($"Password must contain {_options.MinDigits} or more numbers");
                return result;
            }

            if (special < _options.MinSpecialCharacters)
            {
                result.Errors.Add($"Password must contain {_options.MinSpecialCharacters} or more special characters");
                return result;
            }

            return result;
        }
    }
}
