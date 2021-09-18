using Myitian.ColorPlusLibLite.XMath;
using System;

namespace Myitian.ColorPlusLibLite
{
    public static class ColorSpaceConverter
	{
		public static readonly double[] RefWhiteD65 = new double[] { 0.95047, 1.00000, 1.08883 };
		// from http://www.brucelindbloom.com
		#region RGB - XYZ
		public static ColorXYZ To_XYZ(ColorRGB linearRGB)
		{
			Matrix M = RGBWorkingSpace.GetConvertMatrix(RGBWorkingSpace.sRGB_Primaries, RefWhiteD65);
			Matrix XYZ = M * new Matrix(new double[] { linearRGB.R, linearRGB.G, linearRGB.B }, true);
			ColorXYZ colorXYZ = new ColorXYZ
			{
				X = XYZ.Data[0, 0],
				Y = XYZ.Data[1, 0],
				Z = XYZ.Data[2, 0],
			};
			return colorXYZ;
		}
		#endregion RGB - XYZ
		#region XYZ - LinearRGB
		public static ColorRGB To_LinearRGB(ColorXYZ colorXYZ)
		{
			Matrix M = RGBWorkingSpace.GetConvertMatrix(RGBWorkingSpace.sRGB_Primaries, RefWhiteD65).Inverse();
			Matrix XYZ = M * new Matrix(new double[] { colorXYZ.X, colorXYZ.Y, colorXYZ.Z }, true);
			ColorRGB colorRGB = new ColorRGB
			{
				R = XYZ.Data[0, 0],
				G = XYZ.Data[1, 0],
				B = XYZ.Data[2, 0],
				IsLinear = true
			};
			return colorRGB;
		}
		#endregion XYZ - LinearRGB

		#region CIELab - XYZ
		public static ColorXYZ To_XYZ(ColorCIELab colorCIELab)
		{
			double
				Epsilon = 216d / 24389,
				Kappa = 24389d / 27,

				fy = (colorCIELab.L + 16) / 116,
				fx = colorCIELab.A / 500 + fy,
				fz = fy - colorCIELab.B / 200,

				xr,
				yr,
				zr,

				fx_Pow3 = Math.Pow(fx, 3),
				fy_Pow3 = Math.Pow(fy, 3),
				fz_Pow3 = Math.Pow(fz, 3);

			if (fx_Pow3 > Epsilon)
				xr = fx_Pow3;
			else
				xr = (116 * fx - 16) / Kappa;

			if (colorCIELab.L > Kappa * Epsilon)
				yr = fy_Pow3;
			else
				yr = colorCIELab.L / Kappa;

			if (fz_Pow3 > Epsilon)
				zr = fz_Pow3;
			else
				zr = (116 * fz - 16) / Kappa;

			ColorXYZ colorXYZ = new ColorXYZ
			{
				X = xr * RefWhiteD65[0],
				Y = yr * RefWhiteD65[1],
				Z = zr * RefWhiteD65[2],
			};

			return colorXYZ;
		}
		#endregion CIELab - XYZ
		#region XYZ - CIELab
		public static ColorCIELab To_CIELab(ColorXYZ colorXYZ)
		{
			double
				Epsilon = 216d / 24389,
				Kappa = 24389d / 27,

				xr = colorXYZ.X / RefWhiteD65[0],
				yr = colorXYZ.Y / RefWhiteD65[1],
				zr = colorXYZ.Z / RefWhiteD65[2],

				fx,
				fy,
				fz;
			double _1_3 = 1d / 3;
			if (xr > Epsilon)
				fx = Math.Pow(xr, _1_3);
			else
				fx = (Kappa * xr + 16) / 116;

			if (yr > Epsilon)
				fy = Math.Pow(yr, _1_3);
			else
				fy = (Kappa * yr + 16) / 116;

			if (zr > Epsilon)
				fz = Math.Pow(zr, _1_3);
			else
				fz = (Kappa * zr + 16) / 116;

			ColorCIELab colorCIELab = new ColorCIELab
			{
				L = 116 * fy - 16,
				A = 500 * (fx - fy),
				B = 200 * (fy - fz),
			};

			return colorCIELab;
		}
		#endregion XYZ - CIELab
	}
}
