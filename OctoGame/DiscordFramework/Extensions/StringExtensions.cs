using System;
using System.Linq;

namespace OctoGame.DiscordFramework.Extensions
{
    public enum PluralRules
    {
        None,
        E,
        IE
    }

    public static class StringExtensions
    {
        public static string PadCenter(this string text, int totalWidth, char paddingChar = ' ')
        {
            var charactersToPad = totalWidth - text.Length;
            return charactersToPad <= 0
                ? text
                : $"{new string(paddingChar, charactersToPad / 2)}{text}{new string(paddingChar, charactersToPad / 2 + charactersToPad % 2)}";
        }

        public static bool IsJumpUrl(this string text, out (ulong GuildId, ulong ChannelId, ulong MessageId) resolved)
        {
            resolved = (0, 0, 0);
            if (string.IsNullOrWhiteSpace(text)) return false;
            var split = text.Split('/', StringSplitOptions.RemoveEmptyEntries).Where(x => ulong.TryParse(x, out _))
                .Select(ulong.Parse)
                .ToList();
            if (split.Count != 3) return false;
            resolved = (split[0], split[1], split[2]);
            return true;
        }

        public static string TrimTo(this string text, int length, bool addEllipses = false)
        {
            if (length >= text.Length)
                return text;
            return addEllipses
                ? text.Substring(0, length - 3) + "..."
                : text.Substring(0, length);
        }

        public static bool IsImageUrl(this string str)
        {
            return str.EndsWith("png", StringComparison.OrdinalIgnoreCase)
                   || str.EndsWith("jpg", StringComparison.OrdinalIgnoreCase)
                   || str.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase)
                   || str.EndsWith("gif", StringComparison.OrdinalIgnoreCase)
                   || str.EndsWith("bmp", StringComparison.OrdinalIgnoreCase);
        }
    }
}