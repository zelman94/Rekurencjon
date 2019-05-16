using System;
using System.Collections.Generic;
using System.Text;

namespace Rekurencjon
{
    public static class DateTimeExtensions
    {
        public static bool IsOlderThan(this DateTime dateTime, int days)
        {
            if (dateTime.AddDays(days).CompareTo(DateTime.Now.Date) < 0)
                return true;
            return false;
        }
    }

    public static class StringExtensions
    {
        public static bool Contains(this string text, string value, StringComparison comparision)
        {
            if (text.Trim().IndexOf(value.Trim(), comparision) >= 0)
                return true;
            return false;
        }

        public static bool ContainsAny(this string text, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                if (text.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public static bool ContainsAll(this string text, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                if (!text.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }
            return true;
        }
    }

    public static class ExceptionExtensions
    {
        public static string FullMessage(this Exception ex)
        {
            var stringBuilder = new StringBuilder();
            while (ex != null)
            {
                stringBuilder.AppendLine($"{ex.Message}");
                ex = ex.InnerException;
            }

            return stringBuilder.ToString();
        }
    }
}
