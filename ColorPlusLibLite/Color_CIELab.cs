namespace Myitian.ColorPlusLibLite
{
    public class ColorCIELab : ColorBase
	{
		public double L { get { return V0; } set { V0 = value; } }
		public double A { get { return V1; } set { V1 = value; } }
		public double B { get { return V2; } set { V2 = value; } }

		public ColorCIELab() { }
		public ColorCIELab(ColorCIELab colorCIELab)
		{
			L = colorCIELab.L;
			A = colorCIELab.A;
			B = colorCIELab.B;
		}

		public override ColorRGB To_LinearRGB()
		{
			return To_XYZ().To_LinearRGB();
		}
		public override ColorRGB To_sRGB()
		{
			return To_XYZ().To_sRGB();
		}
		public override ColorCIELab To_CIELab()
		{
			return new ColorCIELab(this);
		}
		public override ColorXYZ To_XYZ()
		{
			return ColorSpaceConverter.To_XYZ(this);
		}
	}
}
