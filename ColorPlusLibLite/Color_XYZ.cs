namespace Myitian.ColorPlusLibLite
{
    public class ColorXYZ : ColorBase
	{
		public double X { get { return V0; } set { V0 = value; } }
		public double Y { get { return V1; } set { V1 = value; } }
		public double Z { get { return V2; } set { V2 = value; } }

		public ColorXYZ() { }
		public ColorXYZ(in ColorXYZ colorXYZ)
		{
			X = colorXYZ.X;
			Y = colorXYZ.Y;
			Z = colorXYZ.Z;
		}

		public override ColorRGB To_LinearRGB()
		{
			return ColorSpaceConverter.To_LinearRGB(this);
		}
		public override ColorRGB To_sRGB()
		{
			return To_LinearRGB().To_sRGB();
		}
		public override ColorCIELab To_CIELab()
		{
			return ColorSpaceConverter.To_CIELab(this);
		}
		public override ColorXYZ To_XYZ()
		{
			return new ColorXYZ(this);
		}
	}
}
