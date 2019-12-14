using System;

namespace ManagedSquish
{
	[Flags]
	public enum SquishFlags
	{
		Dxt1 = 0x1,
		Dxt3 = 0x2,
		Dxt5 = 0x4,
		ColourIterativeClusterFit = 0x100,
		ColourClusterFit = 0x8,
		ColourRangeFit = 0x10,
		ColourMetricPerceptual = 0x20,
		ColourMetricUniform = 0x40,
		WeightColourByAlpha = 0x80
	}
}
