using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace 游戏登录器
{
	public static class 图形验证
	{
		public static string 验证码;
		public static Bitmap 生成验证码()
		{
			Bitmap bitmap = new Bitmap(200, 60);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			using (SolidBrush 白底 = new SolidBrush(Color.White))
			using (SolidBrush 字色 = new SolidBrush(Color.Black))
			using (Font font = new Font(FontFamily.GenericSerif, 48f, FontStyle.Bold, GraphicsUnit.Pixel))
			using (Pen pen = new Pen(Color.Black, 2f))
			{
				graphics.FillRectangle(白底, 0, 0, 200, 60);
				const string text = "ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789";
				StringBuilder stringBuilder = new StringBuilder(5);
				for (int i = 0; i < 5; i++)
				{
					// 修复：原代码 random.Next(0, text.Length - 1) 抽不到末位字符；改用密码学安全随机
					int idx = RandomNumberGenerator.GetInt32(0, text.Length);
					string text2 = text.Substring(idx, 1);
					stringBuilder.Append(text2);
					int 偏移 = RandomNumberGenerator.GetInt32(0, 15);
					graphics.DrawString(text2, font, 字色, i * 38, 偏移);
				}
				验证码 = stringBuilder.ToString();
				for (int j = 0; j < 6; j++)
				{
					int x1 = RandomNumberGenerator.GetInt32(0, 200);
					int y1 = RandomNumberGenerator.GetInt32(0, 60);
					int x2 = RandomNumberGenerator.GetInt32(0, 200);
					int y2 = RandomNumberGenerator.GetInt32(0, 60);
					graphics.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
				}
			}
			return bitmap;
		}
	}
}
