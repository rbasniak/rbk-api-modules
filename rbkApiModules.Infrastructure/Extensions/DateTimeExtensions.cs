using System;

namespace AspNetCoreApiTemplate.Utilities
{
    /// <summary>
    /// Extensões para DateTime
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converte um objeto DateTime para uma data em formato Unix
        /// </summary>
        /// <param name="date">Data</param>
        /// <returns>Data em formato Unix</returns>
        public static long ToUnixEpochDate(this DateTime date)
        {
            return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }
    }
}
