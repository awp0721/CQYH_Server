using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WebApi;

namespace 游戏服务器.网络类
{
	public abstract class 游戏封包
	{
		public static byte 加密字节;

		public static Dictionary<Type, MethodInfo> 封包处理方法表;

		public static Dictionary<ushort, Type> 服务器封包类型表;

		public static Dictionary<ushort, Type> 客户端封包类型表;

		public static Dictionary<Type, ushort> 服务器封包编号表;

		public static Dictionary<Type, ushort> 客户端封包编号表;

		public static Dictionary<ushort, ushort> 服务器封包长度表;

		public static Dictionary<ushort, ushort> 客户端封包长度表;

		public static Dictionary<Type, Func<BinaryReader, 封包字段描述, object>> 封包字段读取表;

		public static Dictionary<Type, Action<BinaryWriter, 封包字段描述, object>> 封包字段写入表;

		public readonly Type 封包类型;

		private readonly ushort 封包长度;

		public abstract ushort 封包编号 { get; }

		public virtual bool 是否加密 { get; set; }

		static 游戏封包()
		{
			游戏封包.加密字节 = 129;
			游戏封包.封包处理方法表 = new Dictionary<Type, MethodInfo>();
			游戏封包.封包字段读取表 = new Dictionary<Type, Func<BinaryReader, 封包字段描述, object>>
			{
				[typeof(bool)] = delegate(BinaryReader 读取流, 封包字段描述 描述符)
				{
					读取流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					return Convert.ToBoolean(读取流.ReadByte());
				},
				[typeof(byte)] = delegate(BinaryReader 读取流, 封包字段描述 描述符)
				{
					读取流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					return 读取流.ReadByte();
				},
				[typeof(sbyte)] = delegate(BinaryReader 读取流, 封包字段描述 描述符)
				{
					读取流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					return 读取流.ReadSByte();
				},
				[typeof(byte[])] = delegate(BinaryReader 读取流, 封包字段描述 描述符)
				{
					读取流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					int num;
					num = ((描述符.长度 != 0) ? 描述符.长度 : (读取流.ReadUInt16() - 4));
					const int 最大封包字节 = 65536;
					if (num > 最大封包字节)
					{
						throw new InvalidOperationException("封包字段长度越界: " + num);
					}
					long 可用 = 读取流.BaseStream.Length - 读取流.BaseStream.Position;
					if (num > 可用)
					{
						throw new InvalidOperationException("封包字段长度超过剩余流: " + num);
					}
					return (num <= 0) ? ((object)new byte[0]) : ((object)读取流.ReadBytes(num));
				},
				[typeof(short)] = delegate(BinaryReader 读取流, 封包字段描述 描述符)
				{
					读取流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					return 读取流.ReadInt16();
				},
				[typeof(ushort)] = delegate(BinaryReader 读取流, 封包字段描述 描述符)
				{
					读取流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					return 读取流.ReadUInt16();
				},
				[typeof(int)] = delegate(BinaryReader 读取流, 封包字段描述 描述符)
				{
					读取流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					return 读取流.ReadInt32();
				},
				[typeof(uint)] = delegate(BinaryReader 读取流, 封包字段描述 描述符)
				{
					读取流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					return 读取流.ReadUInt32();
				},
				[typeof(long)] = delegate(BinaryReader 读取流, 封包字段描述 描述符)
				{
					读取流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					return 读取流.ReadInt64();
				},
				[typeof(ulong)] = delegate(BinaryReader 读取流, 封包字段描述 描述符)
				{
					读取流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					return 读取流.ReadUInt64();
				},
				[typeof(string)] = delegate(BinaryReader 读取流, 封包字段描述 描述符)
				{
					读取流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					byte[] bytes;
					bytes = 读取流.ReadBytes(描述符.长度);
					return Encoding.UTF8.GetString(bytes).Split(new char[1], StringSplitOptions.RemoveEmptyEntries)[0];
				},
				[typeof(Point)] = delegate(BinaryReader 读取流, 封包字段描述 描述符)
				{
					读取流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					Point point2;
					point2 = new Point(读取流.ReadUInt16(), 读取流.ReadUInt16());
					return 计算类.协议坐标转点阵坐标(描述符.反向 ? new Point(point2.Y, point2.X) : point2);
				}
			};
			游戏封包.封包字段写入表 = new Dictionary<Type, Action<BinaryWriter, 封包字段描述, object>>
			{
				[typeof(bool)] = delegate(BinaryWriter 写入流, 封包字段描述 描述符, object 对象)
				{
					写入流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					写入流.Write((bool)对象);
				},
				[typeof(byte)] = delegate(BinaryWriter 写入流, 封包字段描述 描述符, object 对象)
				{
					写入流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					写入流.Write((byte)对象);
				},
				[typeof(sbyte)] = delegate(BinaryWriter 写入流, 封包字段描述 描述符, object 对象)
				{
					写入流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					写入流.Write((sbyte)对象);
				},
				[typeof(byte[])] = delegate(BinaryWriter 写入流, 封包字段描述 描述符, object 对象)
				{
					写入流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					写入流.Write(对象 as byte[]);
				},
				[typeof(short)] = delegate(BinaryWriter 写入流, 封包字段描述 描述符, object 对象)
				{
					写入流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					写入流.Write((short)对象);
				},
				[typeof(ushort)] = delegate(BinaryWriter 写入流, 封包字段描述 描述符, object 对象)
				{
					写入流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					写入流.Write((ushort)对象);
				},
				[typeof(int)] = delegate(BinaryWriter 写入流, 封包字段描述 描述符, object 对象)
				{
					写入流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					写入流.Write((int)对象);
				},
				[typeof(uint)] = delegate(BinaryWriter 写入流, 封包字段描述 描述符, object 对象)
				{
					写入流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					写入流.Write((uint)对象);
				},
				[typeof(long)] = delegate(BinaryWriter bw, 封包字段描述 wfa, object obj)
				{
					bw.BaseStream.Seek(wfa.下标, SeekOrigin.Begin);
					bw.Write((long)obj);
				},
				[typeof(ulong)] = delegate(BinaryWriter bw, 封包字段描述 wfa, object obj)
				{
					bw.BaseStream.Seek(wfa.下标, SeekOrigin.Begin);
					bw.Write((ulong)obj);
				},
				[typeof(string)] = delegate(BinaryWriter 写入流, 封包字段描述 描述符, object 对象)
				{
					if (对象 is string s)
					{
						写入流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
						写入流.Write(Encoding.UTF8.GetBytes(s));
					}
				},
				[typeof(Point)] = delegate(BinaryWriter 写入流, 封包字段描述 描述符, object 对象)
				{
					Point point;
					point = 计算类.点阵坐标转协议坐标((Point)对象);
					写入流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					if (描述符.反向)
					{
						写入流.Write((ushort)point.Y);
						写入流.Write((ushort)point.X);
					}
					else
					{
						写入流.Write((ushort)point.X);
						写入流.Write((ushort)point.Y);
					}
				},
				[typeof(DateTime)] = delegate(BinaryWriter 写入流, 封包字段描述 描述符, object 对象)
				{
					写入流.BaseStream.Seek(描述符.下标, SeekOrigin.Begin);
					写入流.Write(计算类.时间转换((DateTime)对象));
				}
			};
			游戏封包.服务器封包类型表 = new Dictionary<ushort, Type>();
			游戏封包.服务器封包编号表 = new Dictionary<Type, ushort>();
			游戏封包.服务器封包长度表 = new Dictionary<ushort, ushort>();
			游戏封包.客户端封包类型表 = new Dictionary<ushort, Type>();
			游戏封包.客户端封包编号表 = new Dictionary<Type, ushort>();
			游戏封包.客户端封包长度表 = new Dictionary<ushort, ushort>();
			Type[] types;
			types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				if (!type.IsSubclassOf(typeof(游戏封包)))
				{
					continue;
				}
				封包信息描述 customAttribute;
				customAttribute = CustomAttributeExtensions.GetCustomAttribute<封包信息描述>(type);
				if (customAttribute != null)
				{
					if (customAttribute.来源 == 封包来源.客户端)
					{
						游戏封包.客户端封包类型表[customAttribute.编号] = type;
						游戏封包.客户端封包编号表[type] = customAttribute.编号;
						游戏封包.客户端封包长度表[customAttribute.编号] = customAttribute.长度;
						游戏封包.封包处理方法表[type] = typeof(客户网络).GetMethod("处理封包", new Type[1] { type });
					}
					else
					{
						游戏封包.服务器封包类型表[customAttribute.编号] = type;
						游戏封包.服务器封包编号表[type] = customAttribute.编号;
						游戏封包.服务器封包长度表[customAttribute.编号] = customAttribute.长度;
					}
				}
			}
			string text;
			text = "";
			IOrderedEnumerable<KeyValuePair<ushort, Type>> orderedEnumerable;
			orderedEnumerable = 游戏封包.服务器封包类型表.OrderBy((KeyValuePair<ushort, Type> pair) => ((KeyValuePair<ushort, Type>)pair).Key);
			IOrderedEnumerable<KeyValuePair<ushort, Type>> orderedEnumerable2;
			orderedEnumerable2 = 游戏封包.客户端封包类型表.OrderBy((KeyValuePair<ushort, Type> pair) => ((KeyValuePair<ushort, Type>)pair).Key);
			foreach (KeyValuePair<ushort, Type> item in orderedEnumerable)
			{
				text += $"{Enum.GetName(typeof(_0001_0003_0001_0001_0001_0001_0003_0003_0002_0003_0001_0003_0002_0003), item.Key)}\t{item.Key}\t{游戏封包.服务器封包长度表[item.Key]}\r\n";
			}
			string text2;
			text2 = "";
			foreach (KeyValuePair<ushort, Type> item2 in orderedEnumerable2)
			{
				text2 += $"{item2.Value.Name}\t{item2.Key}\t{游戏封包.客户端封包长度表[item2.Key]}\r\n";
			}
			File.WriteAllText("./ServerPackRule.txt", text);
			File.WriteAllText("./ClientPackRule.txt", text2);
		}

		public 游戏封包()
		{
			this.是否加密 = true;
			this.封包类型 = base.GetType();
			if (CustomAttributeExtensions.GetCustomAttribute<封包信息描述>(this.封包类型).来源 == 封包来源.服务器)
			{
				this.封包长度 = 封包长度表.服务器[(_0001_0003_0001_0001_0001_0001_0003_0003_0002_0003_0001_0003_0002_0003)this.封包编号];
			}
			else
			{
				this.封包长度 = 封包长度表.客户端[(_0002_0004_0005_0001_0005_0001_0004_0003_0005_0001_0002_0004)this.封包编号];
			}
		}

		public byte[] 取字节()
		{
			using MemoryStream memoryStream = ((this.封包长度 == 0) ? new MemoryStream() : new MemoryStream(new byte[this.封包长度]));
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			FieldInfo[] fields;
			fields = this.封包类型.GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				封包字段描述 customAttribute;
				customAttribute = CustomAttributeExtensions.GetCustomAttribute<封包字段描述>(fieldInfo);
				if (customAttribute != null)
				{
					Type fieldType;
					fieldType = fieldInfo.FieldType;
					object value;
					value = fieldInfo.GetValue(this);
					if (游戏封包.封包字段写入表.TryGetValue(fieldType, out var value2))
					{
						value2(binaryWriter, customAttribute, value);
					}
				}
			}
			binaryWriter.Seek(0, SeekOrigin.Begin);
			binaryWriter.Write(this.封包编号);
			if (this.封包长度 == 0)
			{
				binaryWriter.Write((ushort)memoryStream.Length);
			}
			byte[] array;
			array = memoryStream.ToArray();
			if (this.是否加密)
			{
				return 游戏封包.加解密(array);
			}
			return array;
		}

		private void 填封包(byte[] 原始数据)
		{
			using MemoryStream input = new MemoryStream(原始数据);
			using BinaryReader arg = new BinaryReader(input);
			FieldInfo[] fields;
			fields = this.封包类型.GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				封包字段描述 customAttribute;
				customAttribute = CustomAttributeExtensions.GetCustomAttribute<封包字段描述>(fieldInfo);
				if (customAttribute != null)
				{
					Type fieldType;
					fieldType = fieldInfo.FieldType;
					if (游戏封包.封包字段读取表.TryGetValue(fieldType, out var value))
					{
						fieldInfo.SetValue(this, value(arg, customAttribute));
					}
				}
			}
		}

		public static 游戏封包 取封包(客户网络 网络连接, byte[] 原始数据, out byte[] 剩余数据)
		{
			剩余数据 = 原始数据;
			if (原始数据.Length < 2)
			{
				return null;
			}
			ushort num;
			num = BitConverter.ToUInt16(原始数据, 0);
			if (!游戏封包.客户端封包类型表.TryGetValue(num, out var value))
			{
				if (网络连接 != null && 网络连接.绑定角色 != null && 网络连接.绑定角色.管理员模式)
				{
					剩余数据 = new byte[0];
					网络连接?.错误消息提示(new Exception($"封包组包失败! 封包编号:{num} 原始数据:{计算类.ToHexStrFromByte(游戏封包.加解密(原始数据))}"));
					return null;
				}
				throw new Exception($"封包组包失败! 封包编号:{num} 原始数据:{计算类.ToHexStrFromByte(游戏封包.加解密(原始数据))}");
			}
			if (!游戏封包.客户端封包长度表.TryGetValue(num, out var value2))
			{
				if (网络连接 != null && 网络连接.绑定角色 != null && 网络连接.绑定角色.管理员模式)
				{
					剩余数据 = new byte[0];
					网络连接?.错误消息提示(new Exception($"获取封包长度失败! 封包编号:{num} 原始数据:{计算类.ToHexStrFromByte(游戏封包.加解密(原始数据))}"));
					return null;
				}
				throw new Exception($"获取封包Length失败! 封包编号:{num:X4}");
			}
			if (value2 == 0 && 原始数据.Length < 4)
			{
				return null;
			}
			value2 = ((value2 == 0) ? BitConverter.ToUInt16(原始数据, 2) : value2);
			if (value2 < 4)
			{
				// C07: 变长封包头部恒为 编号(2)+总长(2)=4 字节, 合法总长必 >=4.
				// 总长字段填 0/1/2/3 会让游标零推进, 触发接收回调 while(true) 无限重解析同一畸形包(预鉴权 DoS).
				throw new Exception($"封包总长非法(变长封包头部不足4字节)! 封包编号:{num} 总长:{value2}");
			}
			if (原始数据.Length < value2)
			{
				return null;
			}
			游戏封包 obj;
			obj = (游戏封包)Activator.CreateInstance(value);
			byte[] 原始数据2;
			原始数据2 = 原始数据.Take(value2).ToArray();
			if (obj.是否加密)
			{
				游戏封包.加解密(原始数据2);
			}
			obj.填封包(原始数据2);
			剩余数据 = 原始数据.Skip(value2).ToArray();
			return obj;
		}

		public static byte[] 加解密(object 原始数据)
		{
			for (int i = 4; i < ((Array)原始数据).Length; i++)
			{
				((sbyte[])原始数据)[i] = (sbyte)(byte)(((byte[])原始数据)[i] ^ 游戏封包.加密字节);
			}
			return (byte[])原始数据;
		}
	}
}
