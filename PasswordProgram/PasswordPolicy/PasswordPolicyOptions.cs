using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordProgram.PasswordPolicy
{
    /// <summary>
    /// Represents a set of options for configuring password policy requirements, such as minimum length and character
    /// composition.
    /// </summary>
    /// <remarks>Use this class to specify the rules that passwords must meet in order to be considered valid.
    /// These options can be used to enforce security standards for user authentication systems.</remarks>
    public class PasswordPolicyOptions
    {
        public int MinLength { get; set; } = 14;
        public int MinUppercase { get; set; } = 2;
        public int MinLowercase { get; set; } = 2;
        public int MinDigits { get; set; } = 2;
        public int MinSpecialCharacters { get; set; } = 2;


        // New password must differ by at least this many characters from current
        public int MinDifferentCharacters { get; set; } = 4;
    }
}
