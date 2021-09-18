namespace Myitian.ColorPlusLibLite
{
    public abstract class ColorBase
	{
		public double V0 = 0;
		public double V1 = 0;
		public double V2 = 0;

		public abstract ColorRGB To_LinearRGB();
		public abstract ColorRGB To_sRGB();
		public abstract ColorCIELab To_CIELab();
		public abstract ColorXYZ To_XYZ();
	}
}
