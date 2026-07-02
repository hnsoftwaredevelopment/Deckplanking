using System.Net;
using System.Text;

namespace DeckPlanking.Core.Documentation;

public static class UserGuideMarkdownRenderer
{
    public static string RenderHtml(string markdown, UserGuideHtmlTheme theme)
    {
        ArgumentNullException.ThrowIfNull(markdown);
        ArgumentNullException.ThrowIfNull(theme);

        var body = new StringBuilder();
        var paragraph = new List<string>();
        var inList = false;

        foreach (var rawLine in markdown.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            var line = rawLine.Trim();
            if (line.Length == 0)
            {
                FlushParagraph(body, paragraph);
                CloseList(body, ref inList);
                continue;
            }

            if (line.StartsWith("# ", StringComparison.Ordinal))
            {
                FlushParagraph(body, paragraph);
                CloseList(body, ref inList);
                AppendHeading(body, "h1", line[2..]);
                continue;
            }

            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                FlushParagraph(body, paragraph);
                CloseList(body, ref inList);
                AppendHeading(body, "h2", line[3..]);
                continue;
            }

            if (line.StartsWith("- ", StringComparison.Ordinal))
            {
                FlushParagraph(body, paragraph);
                if (!inList)
                {
                    body.AppendLine("<ul>");
                    inList = true;
                }

                body.Append("  <li>");
                AppendInline(body, line[2..]);
                body.AppendLine("</li>");
                continue;
            }

            paragraph.Add(line);
        }

        FlushParagraph(body, paragraph);
        CloseList(body, ref inList);

        return $$"""
<!doctype html>
<html>
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <style>
    :root {
      color-scheme: light dark;
      font-family: system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
      background: {{theme.BackgroundColor}};
      color: {{theme.PrimaryTextColor}};
    }
    body {
      margin: 0;
      padding: 18px;
      background: {{theme.BackgroundColor}};
      color: {{theme.PrimaryTextColor}};
      font-size: 16px;
      line-height: 1.55;
    }
    main {
      max-width: 820px;
      margin: 0 auto;
      background: {{theme.SurfaceColor}};
      border: 1px solid {{theme.AccentColor}};
      padding: 20px;
      box-sizing: border-box;
    }
    h1 {
      margin: 0 0 18px;
      font-size: 1.65rem;
      line-height: 1.2;
    }
    h2 {
      margin: 24px 0 8px;
      font-size: 1.15rem;
      line-height: 1.25;
      color: {{theme.AccentColor}};
    }
    p {
      margin: 0 0 12px;
      color: {{theme.PrimaryTextColor}};
    }
    ul {
      margin: 0 0 12px 1.2rem;
      padding: 0;
    }
    li {
      margin: 0 0 6px;
    }
    code {
      background: color-mix(in srgb, {{theme.AccentColor}} 16%, transparent);
      color: {{theme.PrimaryTextColor}};
      padding: 0.08rem 0.28rem;
      border-radius: 4px;
      font-family: ui-monospace, "Cascadia Mono", Consolas, monospace;
      font-size: 0.92em;
    }
    .muted {
      color: {{theme.SecondaryTextColor}};
    }
  </style>
</head>
<body>
  <main>
{{body}}  </main>
</body>
</html>
""";
    }

    private static void AppendHeading(StringBuilder body, string tag, string text)
    {
        body.Append("    <");
        body.Append(tag);
        body.Append('>');
        AppendInline(body, text);
        body.Append("</");
        body.Append(tag);
        body.AppendLine(">");
    }

    private static void FlushParagraph(StringBuilder body, List<string> paragraph)
    {
        if (paragraph.Count == 0)
        {
            return;
        }

        body.Append("    <p>");
        AppendInline(body, string.Join(' ', paragraph));
        body.AppendLine("</p>");
        paragraph.Clear();
    }

    private static void CloseList(StringBuilder body, ref bool inList)
    {
        if (!inList)
        {
            return;
        }

        body.AppendLine("</ul>");
        inList = false;
    }

    private static void AppendInline(StringBuilder body, string text)
    {
        var parts = text.Split('`');
        for (var index = 0; index < parts.Length; index++)
        {
            var encoded = WebUtility.HtmlEncode(parts[index]);
            if (index % 2 == 1)
            {
                body.Append("<code>");
                body.Append(encoded);
                body.Append("</code>");
            }
            else
            {
                body.Append(encoded);
            }
        }
    }
}
