using DeckPlanking.Core.Preview;

namespace DeckPlanking.App.Graphics;

public sealed class DeckPatternPreviewDrawable : IDrawable
{
    private const float LeftMargin = 36;
    private const float TopMargin = 34;
    private const float RightMargin = 12;
    private const float BottomMargin = 12;
    private const float RowGap = 3;
    private const float CenterlineGap = 12;
    private const float MinimumRulerLabelSpacing = 30;

    public IReadOnlyList<PatternPreviewRow> Rows { get; set; } = [];

    public double DeckLengthMillimeters { get; set; }

    public double SegmentLengthMillimeters { get; set; }

    public bool UseKingPlank { get; set; }

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
        var mirroredRows = MirroredPatternPreviewBuilder.Build(Rows, UseKingPlank);
        var upperRows = mirroredRows.Where(row => row.Side == PatternPreviewSide.Upper).ToArray();
        var lowerRows = mirroredRows.Where(row => row.Side == PatternPreviewSide.Lower).ToArray();
        var kingPlankRow = mirroredRows.FirstOrDefault(row => row.Side == PatternPreviewSide.KingPlank);
        var drawingHeight = Math.Max(1, dirtyRect.Height - TopMargin - BottomMargin);
        var internalRowGaps = Math.Max(0, (upperRows.Length - 1) + (lowerRows.Length - 1)) * RowGap;
        var centerGapTotal = UseKingPlank ? CenterlineGap * 2 : CenterlineGap;
        var rowHeight = Math.Max(4, (drawingHeight - centerGapTotal - internalRowGaps) / mirroredRows.Count);
        var deckRect = new RectF(LeftMargin, TopMargin, drawingWidth, drawingHeight);
        var currentY = TopMargin;

        DrawRuler(canvas, deckLength, deckRect);

        DrawRows(canvas, upperRows, deckLength, drawingWidth, rowHeight, ref currentY);

        if (kingPlankRow is not null)
        {
            var topCenterlineY = currentY + (CenterlineGap / 2);
            DrawCenterline(canvas, deckRect.Left, deckRect.Right, topCenterlineY);
            currentY += CenterlineGap;

            var kingRect = new RectF(LeftMargin, currentY, drawingWidth, rowHeight);
            DrawKingPlank(canvas, kingRect);
            DrawRowLabel(canvas, "K", currentY, rowHeight);

            currentY += rowHeight;
            var bottomCenterlineY = currentY + (CenterlineGap / 2);
            DrawCenterline(canvas, deckRect.Left, deckRect.Right, bottomCenterlineY);
            currentY += CenterlineGap;
        }
        else
        {
            var centerlineY = currentY + (CenterlineGap / 2);
            DrawCenterline(canvas, deckRect.Left, deckRect.Right, centerlineY);
            currentY += CenterlineGap;
        }

        DrawRows(canvas, lowerRows, deckLength, drawingWidth, rowHeight, ref currentY);

        canvas.RestoreState();
    }

    private static void DrawRows(
        ICanvas canvas,
        IReadOnlyList<MirroredPatternPreviewRow> mirroredRows,
        float deckLength,
        float drawingWidth,
        float rowHeight,
        ref float currentY)
    {
        for (var index = 0; index < mirroredRows.Count; index++)
        {
            var row = mirroredRows[index].SourceRow;
            var plankRect = new RectF(LeftMargin, currentY, drawingWidth, rowHeight);

            DrawRow(canvas, plankRect, row.RowNumber);
            DrawSeams(canvas, row, deckLength, plankRect);
            DrawRowLabel(canvas, row.RowNumber.ToString(), currentY, rowHeight);

            currentY += rowHeight;
            if (index < mirroredRows.Count - 1)
            {
                currentY += RowGap;
            }
        }
    }

    private void DrawRuler(ICanvas canvas, float deckLength, RectF deckRect)
    {
        if (SegmentLengthMillimeters <= 0 || deckLength <= 0)
        {
            return;
        }

        var ticks = RulerTickBuilder.Build((decimal)SegmentLengthMillimeters, (decimal)deckLength);
        var labelStride = CalculateLabelStride((float)SegmentLengthMillimeters, deckLength, deckRect.Width);
        var baselineY = TopMargin - 7;

        canvas.StrokeColor = Color.FromArgb("#8A5A25");
        canvas.StrokeSize = 1;
        canvas.DrawLine(deckRect.Left, baselineY, deckRect.Right, baselineY);

        canvas.FontColor = Color.FromArgb("#4B2E14");
        canvas.FontSize = 10;

        for (var index = 0; index < ticks.Count; index++)
        {
            var tick = ticks[index];
            var x = deckRect.Left + ((float)tick.PositionMillimeters / deckLength * deckRect.Width);

            canvas.StrokeColor = Color.FromArgb("#8A5A25");
            canvas.StrokeSize = 1;
            canvas.DrawLine(x, baselineY - 4, x, deckRect.Top + 3);

            if (index % labelStride == 0)
            {
                canvas.DrawString(
                    tick.Label,
                    x - 34,
                    2,
                    30,
                    baselineY - 1,
                    HorizontalAlignment.Right,
                    VerticalAlignment.Center);
            }
        }
    }

    private static int CalculateLabelStride(float segmentLength, float deckLength, float drawingWidth)
    {
        if (segmentLength <= 0 || deckLength <= 0)
        {
            return 1;
        }

        var pixelsPerSegment = segmentLength / deckLength * drawingWidth;
        return Math.Max(1, (int)Math.Ceiling(MinimumRulerLabelSpacing / Math.Max(1, pixelsPerSegment)));
    }

    private static void DrawBackground(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Color.FromArgb("#FFF7EC");
        canvas.FillRectangle(dirtyRect);
    }

    private static void DrawRow(ICanvas canvas, RectF rowRect, int rowNumber)
    {
        canvas.FillColor = rowNumber % 2 == 0
            ? Color.FromArgb("#DCA85F")
            : Color.FromArgb("#CF9851");
        canvas.FillRectangle(rowRect);

        canvas.StrokeColor = Color.FromArgb("#7A4C22");
        canvas.StrokeSize = 1;
        canvas.DrawRectangle(rowRect);
    }

    private static void DrawCenterline(ICanvas canvas, float left, float right, float y)
    {
        const float dashLength = 7;
        const float dashGap = 5;

        canvas.StrokeColor = Color.FromArgb("#C52222");
        canvas.StrokeSize = 1.8f;

        for (var x = left; x < right; x += dashLength + dashGap)
        {
            canvas.DrawLine(x, y, Math.Min(x + dashLength, right), y);
        }
    }

    private static void DrawKingPlank(ICanvas canvas, RectF rowRect)
    {
        canvas.FillColor = Color.FromArgb("#B97735");
        canvas.FillRectangle(rowRect);

        canvas.StrokeColor = Color.FromArgb("#7A4C22");
        canvas.StrokeSize = 1.2f;
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

    private static void DrawRowLabel(ICanvas canvas, string label, float y, float rowHeight)
    {
        canvas.FontColor = Color.FromArgb("#4B2E14");
        canvas.FontSize = 11;
        canvas.DrawString(
            label,
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
