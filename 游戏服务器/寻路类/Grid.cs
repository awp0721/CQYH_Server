using AStar;

namespace AStarPathing
{
	public class Grid : IGridProvider
	{
		private readonly Cell[,] _cells;

		public int Width { get; set; }

		public int Height { get; set; }

		public Cell this[int x, int y] => this._cells[x, y];

		public Vector2Int Size => new Vector2Int(this.Width, this.Height);

		public Cell this[Vector2Int location] => this._cells[location.X, location.Y];

		public Grid(int width, int height, bool _Blocked = false)
		{
			this.Width = width;
			this.Height = height;
			this._cells = new Cell[width, height];
			this.Reset(_Blocked);
		}

		public void Reset(bool _Blocked = false)
		{
			for (int i = 0; i <= this._cells.GetUpperBound(0); i++)
			{
				for (int j = 0; j <= this._cells.GetUpperBound(1); j++)
				{
					Cell cell;
					cell = this._cells[i, j];
					if (cell == null)
					{
						this._cells[i, j] = new Cell(new Vector2Int(i, j), _Blocked);
						continue;
					}
					cell.G = 0.0;
					cell.H = 0.0;
					cell.F = 0.0;
					cell.Closed = false;
					cell.Parent = null;
				}
			}
		}

		public int GetNodeId(Vector2Int location)
		{
			return location.X * this.Width + location.Y;
		}
	}
}
