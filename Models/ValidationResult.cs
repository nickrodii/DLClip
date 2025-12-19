using System;
using System.Collections.Generic;
using System.Text;

namespace DLClip.Models
{
    internal class ValidationResult
    {
        public ValidationSeverity Severity { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }

        public static ValidationResult Success()
        {
            return new ValidationResult { Severity = ValidationSeverity.Success };
        }

        public static ValidationResult Warning(string message, string title)
        {
            return new ValidationResult { Severity = ValidationSeverity.Warning, Message = message, Title = title };
        }

        public static ValidationResult Failure(string message, string title)
        {
            return new ValidationResult { Severity = ValidationSeverity.Failure, Message = message, Title = title };
        }

        
    }
}
