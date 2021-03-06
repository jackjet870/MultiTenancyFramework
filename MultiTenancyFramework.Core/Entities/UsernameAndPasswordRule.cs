﻿namespace MultiTenancyFramework
{
    [System.ComponentModel.DataAnnotations.Schema.ComplexType]
    public class UsernameAndPasswordRule
    {
        public virtual bool AllowOnlyAlphanumericUserNames { get; set; }
        public virtual bool RequireUniqueEmail { get; set; } = true;
        public virtual bool UserLockoutEnabledByDefault { get; set; } = true;
        public virtual int MaxFailedAccessAttemptsBeforeLockout { get; set; } = 5;
        public virtual int DefaultAccountLockoutTimeSpanInMinutes { get; set; } = 5;
        public virtual int PasswordRequiredLength { get; set; } = 8;

        public virtual bool PasswordRequireNonLetterOrDigit { get; set; } = true;
        public virtual bool PasswordRequireDigit { get; set; } = true;
        public virtual bool PasswordRequireLowercase { get; set; } = true;
        public virtual bool PasswordRequireUppercase { get; set; } = true;
    }
}
