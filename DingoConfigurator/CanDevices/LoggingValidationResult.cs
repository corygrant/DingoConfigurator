using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CanDevices
{
    public class LoggingValidationResult : ValidationResult
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public LoggingValidationResult(ValidationResult validationResult) : base(validationResult.IsValid, validationResult.ErrorContent)
        {
            if (!validationResult.IsValid)
            {
                Logger.Error(validationResult.ErrorContent?.ToString());
            }
        }

    }
}
