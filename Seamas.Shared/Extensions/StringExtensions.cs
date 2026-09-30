using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Wang.Seamas.Shared.Extensions;

/// <summary>
/// 字符串扩展
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 转换成下划线格式
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToSnakeCase(this string source)
    {
        if (string.IsNullOrEmpty(source))
            return source;

        var words = SplitWords(source);
        return string.Join("_", words).ToLowerInvariant();
    }


    /// <summary>
    /// 转换成首字母小写驼峰格式
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToCamelCase(this string source)
    {
        if (string.IsNullOrEmpty(source))
            return source;

        var pascal = source.ToPascalCase();
        if (string.IsNullOrEmpty(pascal))
            return pascal;

        return char.ToLowerInvariant(pascal[0]) + pascal.Substring(1);
    }
    
    /// <summary>
    /// 转换成首字母大写驼峰格式
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToPascalCase(this string source)
    {
        if (string.IsNullOrEmpty(source))
            return source;

        var words = SplitWords(source);
        var sb = new StringBuilder();
        var ti = CultureInfo.InvariantCulture.TextInfo;

        foreach (var w in words)
        {
            if (string.IsNullOrEmpty(w))
                continue;

            // normalize to lower then title-case to handle acronyms like "HTTP" -> "Http"
            var lower = w.ToLowerInvariant();
            sb.Append(ti.ToTitleCase(lower));
        }

        return sb.ToString();
    }


    private static string[] SplitWords(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return Array.Empty<string>();

        s = s.Trim();

        // Replace common separators with a single space
        s = Regex.Replace(s, @"[\s_\-\.]+", " ");

        // Insert spaces between lower->Upper transitions: "myVar" -> "my Var"
        s = Regex.Replace(s, "(?<=[a-z0-9])(?=[A-Z])", " ");

        // Insert spaces between acronym->Word transitions: "HTTPServer" -> "HTTP Server"
        s = Regex.Replace(s, "(?<=[A-Z])(?=[A-Z][a-z])", " ");

        return s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }
}