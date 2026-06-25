using DeckPlanking.Core.Configuration;
using DeckPlanking.Core.Preview;
using DeckPlanking.Core.Patterns;
using DeckPlanking.App.Settings;

namespace DeckPlanking.App.Graphics;

public sealed class DeckPatternPreviewDrawable : IDrawable
{
    private const float LeftMargin = 36;
    private const float TopMargin = 34;
    private const float RightMargin = 12;
    private const float BottomMargin = 38;
    private const float RowGap = 3;
    private const float CenterlineGap = 12;
    private const float MinimumRulerLabelSpacing = 30;
    private const float MinimumRenderedTrenailOffset = 10;

    public IReadOnlyList<PatternPreviewRow> Rows { get; set; } = [];

    public ShiftPatternKind PatternKind { get; set; } = ShiftPatternKind.Every5;

    public int RowsPerSide { get; set; }

    public int StartPoint { get; set; }

    public double PlankLengthMillimeters { get; set; }

    public double DeckLengthMillimeters { get; set; }

    public DeckShapeKind DeckShape { get; set; } = DeckShapeKind.Rectangular;

    public double BowWidthPercentage { get; set; } = 100;

    public double SternWidthPercentage { get; set; } = 100;

    public double BowTaperLengthPercentage { get; set; } = 25;

    public double SternTaperLengthPercentage { get; set; } = 10;

    public double BowRoundnessPercentage { get; set; }

    public double SternRoundnessPercentage { get; set; }

    public double SegmentLengthMillimeters { get; set; }

    public double PlankWidthMillimeters { get; set; }

    public double KingPlankWidthMillimeters { get; set; }

    public bool UseKingPlank { get; set; }

    public bool ShowTrenails { get; set; }

    public TrenailPatternKind TrenailPatternKind { get; set; } = TrenailPatternKind.TwoPerPlankEnd;

    public DeckOrientation DeckOrientation { get; set; } = DeckOrientation.BowLeft;

    public string BowLabel { get; set; } = "Bow";

    public string SternLabel { get; set; } = "Stern";

    public PreviewSegmentInspection? SelectedSegment { get; set; }

    public double Zoom { get; set; } = 1;

    public double PanX { get; set; }

    public double PanY { get; set; }

    public DeckPatternPreviewDrawable CreateExportSnapshot()
    {
        return new DeckPatternPreviewDrawable
        {
            Rows = Rows.ToArray(),
            PatternKind = PatternKind,
            RowsPerSide = RowsPerSide,
            StartPoint = StartPoint,
            PlankLengthMillimeters = PlankLengthMillimeters,
            DeckLengthMillimeters = DeckLengthMillimeters,
            DeckShape = DeckShape,
            BowWidthPercentage = BowWidthPercentage,
            SternWidthPercentage = SternWidthPercentage,
            BowTaperLengthPercentage = BowTaperLengthPercentage,
            SternTaperLengthPercentage = SternTaperLengthPercentage,
            BowRoundnessPercentage = BowRoundnessPercentage,
            SternRoundnessPercentage = SternRoundnessPercentage,
            SegmentLengthMillimeters = SegmentLengthMillimeters,
            PlankWidthMillimeters = PlankWidthMillimeters,
            KingPlankWidthMillimeters = KingPlankWidthMillimeters,
            UseKingPlank = UseKingPlank,
            ShowTrenails = ShowTrenails,
            TrenailPatternKind = TrenailPatternKind,
            DeckOrientation = DeckOrientation,
            BowLabel = BowLabel,
            SternLabel = SternLabel,
            SelectedSegment = null,
            Zoom = 1,
            PanX = 0,
            PanY = 0
        };
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();
        canvas.Antialias = true;

        DrawBackground(canvas, dirtyRect);

        canvas.Translate((float)PanX, (float)PanY);
        canvas.Scale((float)Zoom, (float)Zoom);

        if (Rows.Count == 0)
        {
            canvas.RestoreState();
            return;
        }

        var deckLength = DeckLengthMillimeters > 0 ? (float)DeckLengthMillimeters : DetermineFallbackDeckLength();
        var drawingWidth = Math.Max(1, dirtyRect.Width - LeftMargin - RightMargin);
        var centerlineRows = BuildCenterlineRows(deckLength);
        var upperRows = centerlineRows.Where(row => row.Side == PatternPreviewSide.Upper).ToArray();
        var lowerRows = centerlineRows.Where(row => row.Side == PatternPreviewSide.Lower).ToArray();
        var kingPlankRow = centerlineRows.FirstOrDefault(row => row.Side == PatternPreviewSide.KingPlank);
        if (centerlineRows.Count == 0)
        {
            canvas.RestoreState();
            return;
        }

        var drawingHeight = Math.Max(1, dirtyRect.Height - TopMargin - BottomMargin);
        var internalRowGaps = Math.Max(0, (upperRows.Length - 1) + (lowerRows.Length - 1)) * RowGap;
        var centerGapTotal = UseKingPlank ? CenterlineGap * 2 : CenterlineGap;
        var kingPlankVisualRatio = CalculateKingPlankVisualRatio();
        var rowUnits = upperRows.Length + lowerRows.Length + (kingPlankRow is not null ? kingPlankVisualRatio : 0);
        var rowHeight = Math.Max(4, (drawingHeight - centerGapTotal - internalRowGaps) / Math.Max(1, rowUnits));
        var kingPlankHeight = Math.Max(4, rowHeight * kingPlankVisualRatio);
        var deckRect = new RectF(LeftMargin, TopMargin, drawingWidth, drawingHeight);
        var contourPath = CreateContourPath(deckRect);
        var currentY = TopMargin;
        var renderedRows = new List<(CenterlinePatternPreviewRow Row, RectF Rect)>();

        DrawRuler(canvas, deckLength, deckRect);

        canvas.SaveState();
        canvas.ClipPath(contourPath, WindingMode.EvenOdd);

        DrawRows(canvas, upperRows, deckLength, drawingWidth, rowHeight, ref currentY, renderedRows, drawLabels: false);

        if (kingPlankRow is not null)
        {
            var topCenterlineY = currentY + (CenterlineGap / 2);
            DrawCenterline(canvas, deckRect.Left, deckRect.Right, topCenterlineY);
            currentY += CenterlineGap;

            var kingRect = new RectF(LeftMargin, currentY, drawingWidth, kingPlankHeight);
            DrawKingPlank(canvas, kingRect);
            DrawSeams(canvas, kingPlankRow.SourceRow, deckLength, kingRect);
            renderedRows.Add((kingPlankRow, kingRect));

            currentY += kingPlankHeight;
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

        DrawRows(canvas, lowerRows, deckLength, drawingWidth, rowHeight, ref currentY, renderedRows, drawLabels: false);
        DrawSelectedSegment(canvas, renderedRows, deckLength, SelectedSegment);
        if (ShowTrenails)
        {
            DrawTrenails(canvas, renderedRows, deckLength, TrenailPatternKind);
        }

        canvas.RestoreState();
        DrawContour(canvas, contourPath);
        DrawRenderedRowLabels(canvas, renderedRows);
        DrawDirectionGuide(canvas, deckRect.Left, deckRect.Right, dirtyRect.Bottom - 28, DeckOrientation, BowLabel, SternLabel);

        canvas.RestoreState();
    }

    public PreviewSegmentInspection? HitTest(PointF point, RectF bounds)
    {
        if (Rows.Count == 0 || Zoom <= 0)
        {
            return null;
        }

        var localPoint = new PointF(
            (point.X - (float)PanX) / (float)Zoom,
            (point.Y - (float)PanY) / (float)Zoom);
        var deckLength = DeckLengthMillimeters > 0 ? (float)DeckLengthMillimeters : DetermineFallbackDeckLength();
        var drawingWidth = Math.Max(1, bounds.Width - LeftMargin - RightMargin);
        var centerlineRows = BuildCenterlineRows(deckLength);
        if (centerlineRows.Count == 0)
        {
            return null;
        }

        var deckRect = new RectF(LeftMargin, TopMargin, drawingWidth, Math.Max(1, bounds.Height - TopMargin - BottomMargin));
        if (!IsPointInsideContour(localPoint, deckRect))
        {
            return null;
        }

        var upperRows = centerlineRows.Where(row => row.Side == PatternPreviewSide.Upper).ToArray();
        var lowerRows = centerlineRows.Where(row => row.Side == PatternPreviewSide.Lower).ToArray();
        var kingPlankRow = centerlineRows.FirstOrDefault(row => row.Side == PatternPreviewSide.KingPlank);
        var drawingHeight = deckRect.Height;
        var internalRowGaps = Math.Max(0, (upperRows.Length - 1) + (lowerRows.Length - 1)) * RowGap;
        var centerGapTotal = UseKingPlank ? CenterlineGap * 2 : CenterlineGap;
        var kingPlankVisualRatio = CalculateKingPlankVisualRatio();
        var rowUnits = upperRows.Length + lowerRows.Length + (kingPlankRow is not null ? kingPlankVisualRatio : 0);
        var rowHeight = Math.Max(4, (drawingHeight - centerGapTotal - internalRowGaps) / Math.Max(1, rowUnits));
        var kingPlankHeight = Math.Max(4, rowHeight * kingPlankVisualRatio);
        var currentY = TopMargin;
        var renderedRows = new List<(CenterlinePatternPreviewRow Row, RectF Rect)>();

        AddRowsToLayout(upperRows, drawingWidth, rowHeight, ref currentY, renderedRows);
        if (kingPlankRow is not null)
        {
            currentY += CenterlineGap;
            renderedRows.Add((kingPlankRow, new RectF(LeftMargin, currentY, drawingWidth, kingPlankHeight)));
            currentY += kingPlankHeight + CenterlineGap;
        }
        else
        {
            currentY += CenterlineGap;
        }

        AddRowsToLayout(lowerRows, drawingWidth, rowHeight, ref currentY, renderedRows);

        for (var rowIndex = 0; rowIndex < renderedRows.Count; rowIndex++)
        {
            var (row, rect) = renderedRows[rowIndex];
            if (!rect.Contains(localPoint))
            {
                continue;
            }

            var positionMillimeters = MapXToPosition(localPoint.X, deckLength, rect);
            var segment = PlankSegmentBuilder.Build(row.SourceRow, (decimal)deckLength)
                .FirstOrDefault(candidate =>
                    positionMillimeters >= candidate.StartMillimeters
                    && positionMillimeters <= candidate.EndMillimeters);
            if (segment is null)
            {
                return null;
            }

            return new PreviewSegmentInspection(
                rowIndex,
                row.Side,
                row.SourceRow.RowNumber,
                segment.SegmentNumber,
                segment.StartMillimeters,
                segment.EndMillimeters);
        }

        return null;
    }

    private IReadOnlyList<CenterlinePatternPreviewRow> BuildCenterlineRows(float deckLength)
    {
        if (PlankLengthMillimeters <= 0 || deckLength <= 0 || RowsPerSide <= 0)
        {
            return [];
        }

        return CenterlinePatternPreviewBuilder.Build(
            (decimal)PlankLengthMillimeters,
            (decimal)deckLength,
            PatternKind,
            RowsPerSide,
            StartPoint,
            UseKingPlank);
    }

    private void DrawRows(
        ICanvas canvas,
        IReadOnlyList<CenterlinePatternPreviewRow> centerlineRows,
        float deckLength,
        float drawingWidth,
        float rowHeight,
        ref float currentY,
        IList<(CenterlinePatternPreviewRow Row, RectF Rect)> renderedRows,
        bool drawLabels = true)
    {
        for (var index = 0; index < centerlineRows.Count; index++)
        {
            var row = centerlineRows[index].SourceRow;
            var plankRect = new RectF(LeftMargin, currentY, drawingWidth, rowHeight);

            DrawRow(canvas, plankRect, row.RowNumber);
            DrawSeams(canvas, row, deckLength, plankRect);
            if (drawLabels)
            {
                DrawRowLabel(canvas, row.Phase.ToString(), currentY, rowHeight);
            }
            renderedRows.Add((centerlineRows[index], plankRect));

            currentY += rowHeight;
            if (index < centerlineRows.Count - 1)
            {
                currentY += RowGap;
            }
        }
    }

    private void DrawTrenails(
        ICanvas canvas,
        IReadOnlyList<(CenterlinePatternPreviewRow Row, RectF Rect)> renderedRows,
        float deckLength,
        TrenailPatternKind trenailPatternKind)
    {
        if (deckLength <= 0 || renderedRows.Count == 0)
        {
            return;
        }

        var points = TrenailOverlayBuilder.Build(
            renderedRows.Select(row => row.Row).ToArray(),
            (decimal)deckLength,
            TrenailOverlayBuilder.CalculateReadableDistanceFromPlankEnd(
                (decimal)deckLength,
                (decimal)renderedRows[0].Rect.Width,
                (decimal)MinimumRenderedTrenailOffset),
            patternKind: trenailPatternKind);

        canvas.FillColor = Color.FromArgb("#2A160B");

        foreach (var point in points)
        {
            var rowRect = renderedRows[point.RowIndex].Rect;
            var x = MapPositionToX(point.PositionMillimeters, deckLength, rowRect);
            var y = point.VerticalPlacement switch
            {
                TrenailVerticalPlacement.Upper => rowRect.Top + (rowRect.Height * 0.28f),
                TrenailVerticalPlacement.Center => rowRect.Center.Y,
                TrenailVerticalPlacement.Lower => rowRect.Bottom - (rowRect.Height * 0.28f),
                _ => rowRect.Center.Y
            };
            var radius = Math.Clamp(rowRect.Height * 0.11f, 1.4f, 3.2f);

            canvas.FillCircle(x, y, radius);
        }
    }

    private void DrawSelectedSegment(
        ICanvas canvas,
        IReadOnlyList<(CenterlinePatternPreviewRow Row, RectF Rect)> renderedRows,
        float deckLength,
        PreviewSegmentInspection? selectedSegment)
    {
        if (selectedSegment is null
            || selectedSegment.RenderedRowIndex < 0
            || selectedSegment.RenderedRowIndex >= renderedRows.Count
            || deckLength <= 0)
        {
            return;
        }

        var rowRect = renderedRows[selectedSegment.RenderedRowIndex].Rect;
        var startX = MapPositionToX(selectedSegment.StartMillimeters, deckLength, rowRect);
        var endX = MapPositionToX(selectedSegment.EndMillimeters, deckLength, rowRect);
        var selectedLeft = Math.Min(startX, endX);
        var selectedRight = Math.Max(startX, endX);
        var selectedRect = new RectF(selectedLeft, rowRect.Top, Math.Max(1, selectedRight - selectedLeft), rowRect.Height);

        canvas.FillColor = Color.FromRgba(255, 243, 109, 95);
        canvas.FillRectangle(selectedRect);
        canvas.StrokeColor = Color.FromArgb("#6F4B00");
        canvas.StrokeSize = 1.6f;
        canvas.DrawRectangle(selectedRect);
    }

    private static void AddRowsToLayout(
        IReadOnlyList<CenterlinePatternPreviewRow> centerlineRows,
        float drawingWidth,
        float rowHeight,
        ref float currentY,
        IList<(CenterlinePatternPreviewRow Row, RectF Rect)> renderedRows)
    {
        for (var index = 0; index < centerlineRows.Count; index++)
        {
            renderedRows.Add((centerlineRows[index], new RectF(LeftMargin, currentY, drawingWidth, rowHeight)));

            currentY += rowHeight;
            if (index < centerlineRows.Count - 1)
            {
                currentY += RowGap;
            }
        }
    }

    private PathF CreateContourPath(RectF deckRect)
    {
        var settings = new DeckContourSettings(
            DeckShape,
            (decimal)BowWidthPercentage,
            (decimal)SternWidthPercentage,
            (decimal)BowTaperLengthPercentage,
            (decimal)SternTaperLengthPercentage,
            (decimal)BowRoundnessPercentage,
            (decimal)SternRoundnessPercentage);
        var points = DeckContourBuilder.Build(settings);
        var path = new PathF();

        for (var index = 0; index < points.Count; index++)
        {
            var point = points[index];
            var xRatio = PreviewOrientationMapper.MapRatio((float)point.XRatio, DeckOrientation);
            var x = deckRect.Left + (xRatio * deckRect.Width);
            var y = deckRect.Top + ((float)point.YRatio * deckRect.Height);

            if (index == 0)
            {
                path.MoveTo(x, y);
            }
            else
            {
                path.LineTo(x, y);
            }
        }

        path.Close();
        return path;
    }

    private bool IsPointInsideContour(PointF point, RectF deckRect)
    {
        var settings = new DeckContourSettings(
            DeckShape,
            (decimal)BowWidthPercentage,
            (decimal)SternWidthPercentage,
            (decimal)BowTaperLengthPercentage,
            (decimal)SternTaperLengthPercentage,
            (decimal)BowRoundnessPercentage,
            (decimal)SternRoundnessPercentage);
        var points = DeckContourBuilder.Build(settings)
            .Select(contourPoint => new PointF(
                deckRect.Left + (PreviewOrientationMapper.MapRatio((float)contourPoint.XRatio, DeckOrientation) * deckRect.Width),
                deckRect.Top + ((float)contourPoint.YRatio * deckRect.Height)))
            .ToArray();

        var isInside = false;
        for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
        {
            if (((points[i].Y > point.Y) != (points[j].Y > point.Y))
                && point.X < (points[j].X - points[i].X) * (point.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X)
            {
                isInside = !isInside;
            }
        }

        return isInside;
    }

    private float MapPositionToX(decimal positionMillimeters, float deckLength, RectF rect)
    {
        var sternOriginRatio = deckLength <= 0
            ? 0f
            : (float)positionMillimeters / deckLength;
        var displayRatio = PreviewOrientationMapper.MapRatio(sternOriginRatio, DeckOrientation);

        return rect.Left + (displayRatio * rect.Width);
    }

    private decimal MapXToPosition(float x, float deckLength, RectF rect)
    {
        var displayRatio = (decimal)((x - rect.Left) / rect.Width);
        var sternOriginRatio = PreviewOrientationMapper.UnmapRatio(displayRatio, DeckOrientation);

        return sternOriginRatio * (decimal)deckLength;
    }

    private static void DrawContour(ICanvas canvas, PathF contourPath)
    {
        canvas.StrokeColor = Color.FromArgb("#6F4B00");
        canvas.StrokeSize = 1.6f;
        canvas.DrawPath(contourPath);
    }

    private static void DrawRenderedRowLabels(
        ICanvas canvas,
        IReadOnlyList<(CenterlinePatternPreviewRow Row, RectF Rect)> renderedRows)
    {
        foreach (var (row, rect) in renderedRows)
        {
            var label = row.Side == PatternPreviewSide.KingPlank
                ? "K"
                : row.SourceRow.Phase.ToString();
            DrawRowLabel(canvas, label, rect.Top, rect.Height);
        }
    }

    private void DrawRuler(ICanvas canvas, float deckLength, RectF deckRect)
    {
        if (SegmentLengthMillimeters <= 0 || deckLength <= 0)
        {
            return;
        }

        var ticks = RulerTickBuilder.Build((decimal)SegmentLengthMillimeters, (decimal)deckLength)
            .Select(tick => tick with { Label = DisplayLengthFormatter.FormatRulerLabel(tick.PositionMillimeters) })
            .ToArray();
        var labelStride = CalculateLabelStride((float)SegmentLengthMillimeters, deckLength, deckRect.Width);
        var baselineY = TopMargin - 7;

        canvas.StrokeColor = Color.FromArgb("#8A5A25");
        canvas.StrokeSize = 1;
        canvas.DrawLine(deckRect.Left, baselineY, deckRect.Right, baselineY);

        canvas.FontColor = Color.FromArgb("#4B2E14");
        canvas.FontSize = 10;

        for (var index = 0; index < ticks.Length; index++)
        {
            var tick = ticks[index];
            var x = MapPositionToX(tick.PositionMillimeters, deckLength, deckRect);

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

    private static void DrawDirectionGuide(
        ICanvas canvas,
        float left,
        float right,
        float y,
        DeckOrientation orientation,
        string bowLabel,
        string sternLabel)
    {
        var center = (left + right) / 2;
        var labelWidth = 58;
        var labelHeight = 18;
        var arrowY = y + (labelHeight / 2);
        var guide = orientation == DeckOrientation.BowLeft
            ? new DirectionGuide(bowLabel, sternLabel)
            : new DirectionGuide(sternLabel, bowLabel);

        canvas.FontSize = 11;
        canvas.FontColor = Color.FromArgb("#1F1F1F");
        canvas.StrokeColor = Color.FromArgb("#1F1F1F");
        canvas.StrokeSize = 1.6f;

        DrawDirectionLabel(canvas, guide.LeftLabel, center - 126, y, labelWidth, labelHeight);
        DrawLeftArrow(canvas, center - 18, arrowY, center - 62);
        DrawRightArrow(canvas, center + 18, arrowY, center + 62);
        DrawDirectionLabel(canvas, guide.RightLabel, center + 68, y, labelWidth, labelHeight);
    }

    private static void DrawDirectionLabel(ICanvas canvas, string label, float x, float y, float width, float height)
    {
        canvas.FillColor = Color.FromArgb("#FFF36D");
        canvas.FillRectangle(x, y, width, height);
        canvas.FontColor = Color.FromArgb("#1F1F1F");
        canvas.DrawString(label, x, y, width, height, HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    private static void DrawLeftArrow(ICanvas canvas, float fromX, float y, float toX)
    {
        canvas.DrawLine(fromX, y, toX, y);
        canvas.FillColor = Color.FromArgb("#1F1F1F");
        canvas.FillPath(CreateArrowHead(toX, y, pointsLeft: true));
    }

    private static void DrawRightArrow(ICanvas canvas, float fromX, float y, float toX)
    {
        canvas.DrawLine(fromX, y, toX, y);
        canvas.FillColor = Color.FromArgb("#1F1F1F");
        canvas.FillPath(CreateArrowHead(toX, y, pointsLeft: false));
    }

    private static PathF CreateArrowHead(float x, float y, bool pointsLeft)
    {
        var path = new PathF();
        var direction = pointsLeft ? 1 : -1;
        path.MoveTo(x, y);
        path.LineTo(x + (direction * 10), y - 5);
        path.LineTo(x + (direction * 10), y + 5);
        path.Close();
        return path;
    }

    private void DrawSeams(ICanvas canvas, PatternPreviewRow row, float deckLength, RectF rowRect)
    {
        if (deckLength <= 0)
        {
            return;
        }

        canvas.StrokeColor = Color.FromArgb("#3D2410");
        canvas.StrokeSize = 1.4f;

        foreach (var position in row.SeamPositionsMillimeters)
        {
            var x = MapPositionToX(position, deckLength, rowRect);
            if (x <= rowRect.Left || x >= rowRect.Right)
            {
                continue;
            }

            canvas.DrawLine(x, rowRect.Top, x, rowRect.Bottom);
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

    private float CalculateKingPlankVisualRatio()
    {
        if (!UseKingPlank)
        {
            return 1;
        }

        try
        {
            return Math.Clamp(
                (float)KingPlankVisualRatioCalculator.Calculate(
                    (decimal)PlankWidthMillimeters,
                    (decimal)KingPlankWidthMillimeters),
                0.5f,
                4f);
        }
        catch (ArgumentOutOfRangeException)
        {
            return 1;
        }
    }
}
