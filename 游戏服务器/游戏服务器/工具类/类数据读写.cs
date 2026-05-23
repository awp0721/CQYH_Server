using System;
using System.Reflection;
using 游戏服务器.地图类;

namespace 游戏服务器.工具类
{
	public class 类数据读写
	{
		public delegate object 读委托(object O);

		public delegate void 写委托(object O, object V);

		public ActFieldType ft;

		public 读委托 读;

		public 写委托 写;

		public 类数据读写(MemberInfo AM)
		{
			Type type;
			type = null;
			switch (AM.MemberType)
			{
			case MemberTypes.Property:
			{
				PropertyInfo propertyInfo;
				propertyInfo = (PropertyInfo)AM;
				this.读 = propertyInfo.GetValue;
				this.写 = propertyInfo.SetValue;
				type = propertyInfo.PropertyType;
				break;
			}
			case MemberTypes.Field:
			{
				FieldInfo fieldInfo;
				fieldInfo = (FieldInfo)AM;
				this.读 = fieldInfo.GetValue;
				this.写 = fieldInfo.SetValue;
				type = fieldInfo.FieldType;
				break;
			}
			}
			if (!(type == typeof(int)) && !(type == typeof(uint)) && !(type == typeof(byte)) && !(type == typeof(long)) && !(type == typeof(sbyte)))
			{
				if (type == typeof(string))
				{
					this.ft = ActFieldType.ftStr;
				}
				else
				{
					this.ft = ActFieldType.ftNone;
				}
			}
			else
			{
				this.ft = ActFieldType.ftInt;
			}
		}

		public bool 检测(string mod, string sv, int iv, object cv)
		{
			if (this.读 == null)
			{
				return false;
			}
			return this.ft switch
			{
				ActFieldType.ftStr => !NPCSegment.Compare(mod, (string)cv, sv), 
				ActFieldType.ftInt => !NPCSegment.Compare(mod, (int)cv, iv), 
				_ => false, 
			};
		}

		public void 赋值(object player, string mod, string sv, int iv)
		{
			if (this.写 == null)
			{
				return;
			}
			object obj;
			obj = this.读(player);
			switch (mod)
			{
			case "%":
				if (this.ft == ActFieldType.ftInt)
				{
					this.写(player, (int)obj % iv);
				}
				break;
			case "/":
				switch (this.ft)
				{
				case ActFieldType.ftStr:
					this.写(player, ((string)obj).Replace(sv, "", ignoreCase: true, null));
					break;
				case ActFieldType.ftInt:
					this.写(player, (int)obj / iv);
					break;
				}
				break;
			case "*":
				switch (this.ft)
				{
				case ActFieldType.ftStr:
					this.写(player, (string)obj + sv);
					break;
				case ActFieldType.ftInt:
					this.写(player, (int)obj * iv);
					break;
				}
				break;
			case "-":
				switch (this.ft)
				{
				case ActFieldType.ftStr:
					this.写(player, ((string)obj).Replace(sv, "", ignoreCase: true, null));
					break;
				case ActFieldType.ftInt:
					this.写(player, (int)obj - iv);
					break;
				}
				break;
			case "+":
				switch (this.ft)
				{
				case ActFieldType.ftStr:
					this.写(player, (string)obj + sv);
					break;
				case ActFieldType.ftInt:
					this.写(player, iv + (int)obj);
					break;
				}
				break;
			case "=":
				switch (this.ft)
				{
				case ActFieldType.ftStr:
					this.写(player, sv);
					break;
				case ActFieldType.ftInt:
					this.写(player, iv);
					break;
				}
				break;
			}
		}
	}
}
