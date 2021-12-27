using System.Globalization;
using System.Windows.Controls;

namespace SetFirewall.Domain
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "공백을 허용하지 않습니다.")
                : ValidationResult.ValidResult;
        }
    }

    public class NotEmptyFileValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            bool isOk = true;
            string validMsg = "";
            ValidationResult validResult;

            if (string.IsNullOrWhiteSpace((value ?? "").ToString()))
            {
                validMsg = "공백을 허용하지 않습니다.";
                isOk = false;
            }
            else
            {
                if (!System.IO.File.Exists(value.ToString()))
                {
                    validMsg = "파일 경로가 올바르지 않습니다.";
                    isOk = false;
                }
            }

            return new ValidationResult(isOk, validMsg);
            // return !isOk ? new ValidationResult(false, validMsg) : ValidationResult.ValidResult;
        }
    }
}