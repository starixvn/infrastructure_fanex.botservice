namespace Fanex.BotService.Core.Extensions;

using System.Linq;
using System.Text.RegularExpressions;

public static class TelegramFormatExtensions
{
    private const string BoldRegex = "\\<b>$2</b>";

    public static string ToMarkdownV2(this string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        // convert skype format to telegram format
        text = Regex.Replace(text, "(\\*\\*\\*)(.+?)(\\*\\*\\*)", "<b><i>$2</i></b>"); // bold & italic

        text = Regex.Replace(text, @"(\\\*\\\*)(.+?)(\\\*\\\*)", BoldRegex); // bold
        text = Regex.Replace(text, @"(\\\*\\\*)(.+?)(\\\*\*)", BoldRegex);
        text = Regex.Replace(text, @"(\\\*\\\*)(.+?[^\\])(\*\*)", BoldRegex);
        text = Regex.Replace(text, @"(\\\*\*)(.+?)(\\\*\*)", BoldRegex);
        text = Regex.Replace(text, @"(\\\*\*)(.+?[^\\])(\*\*)", BoldRegex);
        text = Regex.Replace(text, @"(\*\*)(.+?[^\\])(\*\*)", "<b>$2</b>");

        text = Regex.Replace(text, @"(\\\*)(.+?)(\\\*)", "\\*$2\\*"); // italic
        text = Regex.Replace(text, @"(\\\*)(.+?[^\\])(\*)", "\\*$2\\*");
        text = Regex.Replace(text, @"(\*)(.+?[^\\])(\*)", "<i>$2</i>");

        text = Regex.Replace(text, @"(\\~\\~)(.+?)(\\~~)", "~\\~$2~\\~"); // strikethrough
        text = Regex.Replace(text, @"(\\~\\~)(.+?[^\\])(~~)", "\\~\\~$2\\~\\~");
        text = Regex.Replace(text, @"(\\~~)(.+?)(\\~~)", "~\\~$2~\\~");
        text = Regex.Replace(text, @"(\\~~)(.+?[^\\])(~~)", "~\\~$2\\~\\~");
        text = Regex.Replace(text, @"(~~)(.+?[^\\])(~~)", "<s>$2</s>");

        // escape
        text = EscapeSpecialSyntax(text);

        // revert
        text = Regex.Replace(text, @"(\\<b>)(.+?)(</b>)", "\\*\\*$2\\*\\*");
        text = Regex.Replace(text, @"(\\<i>)(.+?)(</i>)", "\\*$2\\*");
        text = Regex.Replace(text, "(<b>)(.+?)(</b>)", "*$2*");
        text = Regex.Replace(text, "(<i>)(.+?)(</i>)", "_$2_");
        text = Regex.Replace(text, "(<s>)(.+?)(</s>)", "~$2~");

        // escape
        text = Regex.Replace(text, @"(.*?)(>|[\\][>])", "$1\\>");

        return text;
    }

    private static string EscapeExceptHyperlink(string text, string pattern, string replacement)
    {
        var regex = new Regex(@"\[\S+]\(https?://\S+\)");
        var hyperlinks = regex.Matches(text).ToList();
        return Regex.Replace(text, pattern, delegate (Match match)
        {
            return hyperlinks.Exists(x => x.Index <= match.Index && match.Index <= x.Index + x.Length - 1)
                ? match.Value
                : replacement;
        });
    }

    private static string EscapeSpecialSyntax(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        text = EscapeExceptHyperlink(text, @"\[|\\\[", "\\[");
        text = EscapeExceptHyperlink(text, @"]|\\\]", "\\]");
        text = EscapeExceptHyperlink(text, @"\(|\\\(", "\\(");
        text = EscapeExceptHyperlink(text, @"\)|\\\)", "\\)");

        text = Regex.Replace(text, @"_|\\_", "\\_");
        text = Regex.Replace(text, @"\*|\\\*", "\\*");
        text = Regex.Replace(text, @"~|\\~", "\\~");
        text = Regex.Replace(text, @"`|\\`", "\\`");
        text = Regex.Replace(text, @"#|\\#", "\\#");
        text = Regex.Replace(text, @"\+|\\\+", "\\+");
        text = Regex.Replace(text, @"-|\\\-", "\\-");
        text = Regex.Replace(text, @"=|\\=", "\\=");
        text = Regex.Replace(text, @"\||\\\|", "\\|");
        text = Regex.Replace(text, @"\{|\\\{", "\\{");
        text = Regex.Replace(text, @"}|\\}", "\\}");
        text = Regex.Replace(text, @"\.|\\\.", "\\.");
        text = Regex.Replace(text, @"!|\\!", "\\!");

        return text;
    }
}