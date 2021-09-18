using Myitian.ColorPlusLibLite.XMath;
using System;

namespace Myitian.ColorPlusLibLite
{
	public static class ColorPlus
	{
		#region ΔE
		/// <summary>
		/// CIE ΔE 2000色差公式
		/// </summary>
		/// <param name="L1"></param>
		/// <param name="a1"></param>
		/// <param name="b1"></param>
		/// <param name="L2"></param>
		/// <param name="a2"></param>
		/// <param name="b2"></param>
		/// <returns>ΔE00</returns>
		public static double Delta_E00(double L1, double a1, double b1, double L2, double a2, double b2)
        {
			return Math.Sqrt(Delta_E00_Square(L1, a1, b1, L2, a2, b2));
        }
		public static double Delta_E00_Square(double L1, double a1, double b1, double L2, double a2, double b2)
		{
			//标准条件：
			//	D65光源、
			//	照度1000lx、
			//	大于4°视场、
			//	颜色均匀、
			//	CIELAB色差在0~5之间、
			//	背景为中等明度的灰（L*=50）
			double
				kL = 1,
				kC = 1,
				kH = 1,

			//1、计算C*ab
				_25pow7 = 6103515625,													//6103515625=25^7
				Cab_Avg = (Chroma(a1, b1) + Chroma(a2, b2)) / 2,                        //两个样品彩度的算术平均值
				Cab_Avg_Pow7 = Math.Pow(Cab_Avg, 7),                                    //两彩度平均值的7次方
				G = 0.5 * (1 - Math.Pow(Cab_Avg_Pow7 / (Cab_Avg_Pow7 + _25pow7), 0.5)), //G 表示 CIELAB 颜色空间的轴的调整因子。

			//2、计算L', a', C'ab, h'ab
				L_1 = L1,               //L'=L*
				a_1 = (1 + G) * a1,     //a'=(1+G)×a*
				b_1 = b1,

				L_2 = L2,
				a_2 = (1 + G) * a2,
				b_2 = b2,

				//两样本的彩度值
				C_ab1 = Chroma(a_1, b_1),
				C_ab2 = Chroma(a_2, b_2),

				//两样本的色调角
				h_ab1 = Hue(a_1, b_1),
				h_ab2 = Hue(a_2, b_2),

			//3、计算△L', △C'ab, △H'ab
				Delta_L_ = L_1 - L_2,
				Delta_C_ab = C_ab1 - C_ab2,
				Delta_h_ab = Hue(a_1, b_1) - Hue(a_2, b_2),
				Delta_H_ab = 2 * Math.Sqrt(C_ab1 * C_ab2) * Math.Sin(Math_.Radians(Delta_h_ab) / 2),

			//4、计算SL、SC、SH、T和RT(由RC计算得来)
				L__Avg = (L_1 + L_2) / 2,
				C_ab_Avg = (C_ab1 + C_ab2) / 2,
				h_ab_Avg = (h_ab1 + h_ab2) / 2,

				SL = 1 + 0.015 * Math.Pow(L__Avg - 50, 2) / Math.Sqrt(20 + Math.Pow(L__Avg - 50, 2)),
				SC = 1 + 0.045 * C_ab_Avg,
				T = 1 - 0.17 * Math.Cos(Math_.Radians(h_ab_Avg - 30))
					  + 0.24 * Math.Cos(Math_.Radians(2 * h_ab_Avg))
					  + 0.32 * Math.Cos(Math_.Radians(3 * h_ab_Avg + 6))
					  - 0.2 * Math.Cos(Math_.Radians(4 * h_ab_Avg - 63)),
				SH = 1 + 0.015 * C_ab_Avg * T,

				C_ab_Avg_Pow7 = Math.Pow(C_ab_Avg, 7),
				RC = 2 * Math.Sqrt(C_ab_Avg_Pow7 / (C_ab_Avg_Pow7 + _25pow7)),
				DeltaTheta = 30 * Math.Exp(-Math.Pow((h_ab_Avg - 275) / 25, 2)),	//△θ以°为单位
				RT = -Math.Sin(Math_.Radians(2 * DeltaTheta)) * RC,					//旋转函数RT

			//5、计算ΔE00
				L_item = Delta_L_ / (kL * SL),
				C_item = Delta_C_ab / (kC * SC),
				H_item = Delta_H_ab / (kH * SH);

			return L_item * L_item + C_item * C_item + H_item * H_item + RT * C_item * H_item;
		}
		#endregion ΔE

		/// <summary>
		/// 彩度计算
		/// </summary>
		/// <param name="a">L*a*b*中a*</param>
		/// <param name="b">L*a*b*中b*</param>
		/// <returns></returns>
		public static double Chroma(double a, double b)
		{
			return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
		}
		/// <summary>
		/// 色调角计算
		/// </summary>
		/// <param name="a">L*a*b*中a*</param>
		/// <param name="b">L*a*b*中b*</param>
		/// <returns></returns>
		public static double Hue(double a, double b)
		{
			double h = Math_.Degrees(Math.Atan(b / a));

			if (a < 0)      // a<0&&b>0, a<0&&b<0
				return 180 + h;
			if (b > 0)      // a>0&&b>0
				return h;
			return 360 + h; // a>0&&b<0
		}
	}
}
