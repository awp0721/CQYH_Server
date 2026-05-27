using System.Drawing;

namespace AStar
{
	public struct Vector2Int
	{
		public int X;

		public int Y;

		public Vector2Int(Point p)
		{
			this.X = p.X;
			this.Y = p.Y;
		}

		public Vector2Int(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public override string ToString()
		{
			return $"[{this.X},{this.Y}]";
		}

		public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
		{
			if (lhs.X == rhs.X)
			{
				return lhs.Y == rhs.Y;
			}
			return false;
		}

		public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
		{
			if (lhs.X == rhs.X)
			{
				return lhs.Y != rhs.Y;
			}
			return true;
		}
	}
}
