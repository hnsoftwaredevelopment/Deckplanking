using DeckPlanking.Core.Documentation;

namespace DeckPlanking.Core.Tests;

public class UserGuideMarkdownRendererTests
{
    [Fact]
    public void RenderHtml_ConvertsHeadingsParagraphsAndInlineCode()
    {
        const string markdown = """
            # Title

            ## Projects

            Files use `.deckplanking.json`.
            """;

        var html = UserGuideMarkdownRenderer.RenderHtml(markdown, Theme());

        Assert.Contains("<h1>Title</h1>", html);
        Assert.Contains("<h2>Projects</h2>", html);
        Assert.Contains("<p>Files use <code>.deckplanking.json</code>.</p>", html);
    }

    [Fact]
    public void RenderHtml_EscapesHtmlFromMarkdown()
    {
        const string markdown = "# <script>alert(1)</script>";

        var html = UserGuideMarkdownRenderer.RenderHtml(markdown, Theme());

        Assert.Contains("&lt;script&gt;alert(1)&lt;/script&gt;", html);
        Assert.DoesNotContain("<script>alert(1)</script>", html);
    }

    private static UserGuideHtmlTheme Theme()
    {
        return new UserGuideHtmlTheme("#ffffff", "#ffffff", "#111111", "#555555", "#336699");
    }
}
