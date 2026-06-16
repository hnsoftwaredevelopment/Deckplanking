using DeckPlanking.Core.Preview;

namespace DeckPlanking.App.Graphics;

public sealed class DeckPatternPreviewDrawable : IDrawable
{
    private const float LeftMargin = 36;
    private const float TopMargin = 8;
    private const float RightMargin = 12;
    private const float BottomMargin = 12;
    private const float RowGap = 3;

    public IReadOnlyList<PatternPreviewRow> Rows { get; set; } = [];

    public double DeckLengthMillimeters { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();
        canvas.Antialias = true;

        DrawBackground(canvas, dirtyRect);

        if (Rows.Count == 0)
        {
            canvas.RestoreState();
            return;
        }

        var deckLength = DeckLengthMillimeters > 0 ? (float)DeckLengthMillimeters : DetermineFallbackDeckLength();
        var drawingWidth = Math.Max(1, dirtyRect.Width - LeftMargin - RightMargin);
        var drawingHeight = Math.Max(1, dirtyRect.Height - TopMargin - BottomMargin);
        var rowHeight = Math.Max(4, (drawingHeight - ((Rows.Count - 1) * RowGap)) / Rows.Count);

        for (var index = 0; index < Rows.Count; index++)
        {
            var row = Rows[index];
            var y = TopMargin + (index * (rowHeight + RowGap));
            var plankRect = new RectF(LeftMargin, y, drawingWidth, rowHeight);

            DrawRow(canvas, plankRect, index);
            DrawSeams(canvas, row, deckLength, plankRect);
            DrawRowNumber(canvas, row.RowNumber, y, rowHeight);
        }

        canvas.RestoreState();
    }

    private static void DrawBackground(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Color.FromArgb("#FFF7EC");
        canvas.FillRectangle(dirtyRect);
    }

    private static void DrawRow(ICanvas canvas, RectF rowRect, int index)
    {
        canvas.FillColor = index % 2 == 0
            ? Color.FromArgb("#DCA85F")
            : Color.FromArgb("#CF9851");
        canvas.FillRectangle(rowRect);

        canvas.StrokeColor = Color.FromArgb("#7A4C22");
        canvas.StrokeSize = 1;
        canvas.DrawRectangle(rowRect);
    }

    private static void DrawSeams(ICanvas canvas, PatternPreviewRow row, float deckLength, RectF rowRect)
    {
        if (deckLength <= 0)
        {
            return;
        }

        canvas.StrokeColor = Color.FromArgb("#3D2410");
        canvas.StrokeSize = 1.4f;

        foreach (var position in row.SeamPositionsMillimeters)
        {
            var x = rowRect.Left + ((float)position / deckLength * rowRect.Width);
            if (x <= rowRect.Left || x >= rowRect.Right)
            {
                continue;
            }

            canvas.DrawLine(x, rowRect.Top + 1, x, rowRect.Bottom - 1);
        }
    }

    private static void DrawRowNumber(ICanvas canvas, int rowNumber, float y, float rowHeight)
    {
        canvas.FontColor = Color.FromArgb("#4B2E14");
        canvas.FontSize = 11;
        canvas.DrawString(
            rowNumber.ToString(),
            0,
            y,
            LeftMargin - 8,
            rowHeight,
            HorizontalAlignment.Right,
            VerticalAlignment.Center);
    }

    private float DetermineFallbackDeckLength()
    {
        var maxPosition = Rows
            .SelectMany(row => row.SeamPositionsMillimeters)
            .DefaultIfEmpty(1m)
            .Max();

        return Math.Max(1, (float)maxPosition);
    }
}
