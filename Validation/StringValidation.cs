using System;

namespace Validation
{
    public static class StringValidation
    {
        public enum StringMode
        {
            MinLength,
            MaxLength
        }
        public static bool CheckLength(string stringToCheck, int length, StringMode mode)
        {
            bool result = false;
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length", "String length has to be a positive integer!!");
            }
            if (mode == StringMode.MaxLength)
            {
                result = stringToCheck.Length <= length;
            }
            else if (mode == StringMode.MinLength)
            {
                result = stringToCheck.Length >= length;
            }
            return result;
        }
        public static bool CheckLength(string stringToCheck, int minLength, int maxLength)
        {
            bool result = false;
            if (minLength <= 0 || maxLength <= 0)
            {
                throw new ArgumentOutOfRangeException("length", "String length has to be a positive interger!!");
            }
            if (minLength > maxLength)
            {
                int temp = minLength;
                minLength = maxLength;
                maxLength = temp;
            }
            result = stringToCheck.Length >= minLength && stringToCheck.Length <= maxLength;
            return result;
        }
        public static bool IsEmail(string emailToCheck)
        {
            if (emailToCheck.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(emailToCheck);
                return addr.Address.Equals(emailToCheck);
            }
            catch
            {
                return false;
            }
        }
    }
}
