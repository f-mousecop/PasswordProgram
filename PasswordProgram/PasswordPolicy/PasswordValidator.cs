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
            throw new NotImplementedException();
        }
    }
}
