using System.Text;

namespace Sekka.API.Middleware;

public class LocaleNormalizationMiddleware
{
    private readonly RequestDelegate _next;

    public LocaleNormalizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Normalize query parameters
        if (context.Request.QueryString.HasValue)
        {
            var normalized = NormalizeDigits(context.Request.QueryString.Value!);
            context.Request.QueryString = new QueryString(normalized);
        }

        // Normalize JSON request body
        if (context.Request.ContentType?.Contains("application/json") == true
            && context.Request.ContentLength > 0)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            if (HasEasternDigits(body))
            {
                var normalizedBody = NormalizeDigits(body);
                var bytes = Encoding.UTF8.GetBytes(normalizedBody);
                context.Request.Body = new MemoryStream(bytes);
                context.Request.ContentLength = bytes.Length;
            }
            else
            {
                context.Request.Body.Position = 0;
            }
        }

        await _next(context);
    }

    private static string NormalizeDigits(string input)
    {
        var sb = new StringBuilder(input.Length);
        foreach (var ch in input)
        {
            sb.Append(ch switch
            {
                >= '\u0660' and <= '\u0669' => (char)(ch - '\u0660' + '0'),
                >= '\u06F0' and <= '\u06F9' => (char)(ch - '\u06F0' + '0'),
                _ => ch
            });
        }
        return sb.ToString();
    }

    private static bool HasEasternDigits(string input)
    {
        foreach (var ch in input)
        {
            if ((ch >= '\u0660' && ch <= '\u0669') || (ch >= '\u06F0' && ch <= '\u06F9'))
                return true;
        }
        return false;
    }
}
