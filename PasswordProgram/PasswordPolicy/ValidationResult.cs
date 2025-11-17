using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordProgram.PasswordPolicy
{
    public class ValidationResult
    {
        // Indicates if the validation passed without errors
        public bool isValid => Errors.Count == 0;

        // List of error messages if validation failed
        public List<string> Errors { get; } = new();
    }
}
