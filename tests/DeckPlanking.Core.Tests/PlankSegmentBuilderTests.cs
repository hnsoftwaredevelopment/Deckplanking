using DeckPlanking.Core.Preview;

namespace DeckPlanking.Core.Tests;

public sealed class PlankSegmentBuilderTests
{
    [Fact]
    public void BuildsSegmentsBetweenRowSeams()
    {
        var row = new PatternPreviewRow(
            RowNumber: 1,
            Phase: 4,
            SeamOffsetSegments: 4,
            SeamPositionsMillimeters: [112m, 252m],
            SeamPositionsText: "112, 252");

        var segments = PlankSegmentBuilder.Build(row, deckLengthMillimeters: 300m);

        Assert.Collection(
            segments,
            segment =>
            {
                Assert.Equal(1, segment.SegmentNumber);
                Assert.Equal(0m, segment.StartMillimeters);
                Assert.Equal(112m, segment.EndMillimeters);
                Assert.Equal(112m, segment.LengthMillimeters);
            },
            segment =>
            {
                Assert.Equal(2, segment.SegmentNumber);
                Assert.Equal(112m, segment.StartMillimeters);
                Assert.Equal(252m, segment.EndMillimeters);
                Assert.Equal(140m, segment.LengthMillimeters);
            },
            segment =>
            {
                Assert.Equal(3, segment.SegmentNumber);
                Assert.Equal(252m, segment.StartMillimeters);
                Assert.Equal(300m, segment.EndMillimeters);
                Assert.Equal(48m, segment.LengthMillimeters);
            });
    }

    [Fact]
    public void IgnoresSeamsOutsideDeckLength()
    {
        var row = new PatternPreviewRow(
            RowNumber: 1,
            Phase: 4,
            SeamOffsetSegments: 4,
            SeamPositionsMillimeters: [112m, 350m],
            SeamPositionsText: "112, 350");

        var segments = PlankSegmentBuilder.Build(row, deckLengthMillimeters: 300m);

        Assert.Equal(2, segments.Count);
        Assert.Equal(112m, segments[1].StartMillimeters);
        Assert.Equal(300m, segments[1].EndMillimeters);
    }
}
