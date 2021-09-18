using Myitian.ColorPlusLibLite.XMath;
using System;
using System.Drawing;

namespace Myitian.ColorPlusLibLite
{
	#region WorkingSpace
	public static class RGBWorkingSpace
	{
		public static readonly double[,] sRGB_Primaries = new double[,]
		{
				{0.6400,0.3300},
				{0.3000,0.6000},
				{0.1500,0.0600},
		};

		public static ColorRGB SRGB_Companding(ColorRGB linearRGB)
		{
			ColorRGB output = new ColorRGB(linearRGB)
			{
				IsLinear = false
			};
			if (linearRGB.R <= 0.0031308)
				output.R = 12.92 * linearRGB.R;
			else
				output.R = 1.055 * Math.Pow(linearRGB.R, 1 / 2.4) - 0.055;
			if (linearRGB.G <= 0.0031308)
				output.G = 12.92 * linearRGB.G;
			else
				output.G = 1.055 * Math.Pow(linearRGB.G, 1 / 2.4) - 0.055;
			if (linearRGB.B <= 0.0031308)
				output.B = 12.92 * linearRGB.B;
			else
				output.B = 1.055 * Math.Pow(linearRGB.B, 1 / 2.4) - 0.055;
			return output;
		}
		public static ColorRGB Inverse_sRGB_Companding(ColorRGB sRGB)
		{
			ColorRGB output = new ColorRGB(sRGB)
			{
				IsLinear = true
			};
			if (sRGB.R <= 0.04045)
				output.R = sRGB.R / 12.92;
			else
				output.R = Math.Pow((sRGB.R + 0.055) / 1.055, 2.4);
			if (sRGB.G <= 0.04045)
				output.G = sRGB.G / 12.92;
			else
				output.G = Math.Pow((sRGB.G + 0.055) / 1.055, 2.4);
			if (sRGB.B <= 0.04045)
				output.B = sRGB.B / 12.92;
			else
				output.B = Math.Pow((sRGB.B + 0.055) / 1.055, 2.4);
			return output;
		}

		public static Matrix GetConvertMatrix(double[,] xr_yr__xg_yg__xb_yb, double[] XW_YW_ZW)
        {
			return GetConvertMatrix(
				xr_yr__xg_yg__xb_yb[0, 0],
				xr_yr__xg_yg__xb_yb[0, 1],
				xr_yr__xg_yg__xb_yb[1, 0],
				xr_yr__xg_yg__xb_yb[1, 1],
				xr_yr__xg_yg__xb_yb[2, 0],
				xr_yr__xg_yg__xb_yb[2, 1],
				XW_YW_ZW[0],
				XW_YW_ZW[1],
				XW_YW_ZW[2]);
		}
		public static Matrix GetConvertMatrix(
		   double xr, double yr,
		   double xg, double yg,
		   double xb, double yb,
		   double XW, double YW, double ZW)
		{
			double
				Xr = xr / yr,
				Yr = 1,
				Zr = (1 - xr - yr) / yr,
				Xg = xg / yg,
				Yg = 1,
				Zg = (1 - xg - yg) / yg,
				Xb = xb / yb,
				Yb = 1,
				Zb = (1 - xb - yb) / yb;

			Matrix Matrix_A = new double[,]
			{
				{ Xr,Xg,Xb },
				{ Yr,Yg,Yb },
				{ Zr,Zg,Zb }
			};
			Matrix Matrix_B = new double[,]
			{
				{ XW },
				{ YW },
				{ ZW }
			};

			Matrix SrSgSb = Matrix.GetInverseMatrix(Matrix_A) * Matrix_B;

			double
				Sr = SrSgSb[0, 0],
				Sg = SrSgSb[1, 0],
				Sb = SrSgSb[2, 0];

			return new double[,]
			{
				{ Sr*Xr,Sg*Xg,Sb*Xb },
				{ Sr*Yr,Sg*Yg,Sb*Yb },
				{ Sr*Zr,Sg*Zg,Sb*Zb }
			};
		}
	}
	#endregion WorkingSpace

	public class ColorRGB : ColorBase
	{
		public double R { get { return V0; } set { V0 = value; } }
		public double G { get { return V1; } set { V1 = value; } }
		public double B { get { return V2; } set { V2 = value; } }

		public bool IsLinear { get; set; } = true;

		public ColorRGB() { }
		public ColorRGB(ColorRGB colorRGB)
		{
			R = colorRGB.R;
			G = colorRGB.G;
			B = colorRGB.B;
			IsLinear = colorRGB.IsLinear;
		}
		public ColorRGB(Color color)
		{
			R = color.R / 255d;
			G = color.G / 255d;
			B = color.B / 255d;
			IsLinear = false;
		}

		public override ColorRGB To_LinearRGB()
		{
			if (IsLinear)
				return new ColorRGB(this);
			else
				return RGBWorkingSpace.Inverse_sRGB_Companding(this);
		}
		public override ColorRGB To_sRGB()
		{
			if (IsLinear)
				return RGBWorkingSpace.SRGB_Companding(this);
			else
				return new ColorRGB(this);
		}
		public override ColorXYZ To_XYZ()
		{
			return ColorSpaceConverter.To_XYZ(To_LinearRGB());
		}
		public override ColorCIELab To_CIELab()
        {
			return To_XYZ().To_CIELab();
		}
	}
}