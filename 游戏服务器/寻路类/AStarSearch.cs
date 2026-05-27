using System;
using System.Collections.Generic;

namespace AStar
{
	public class AStarSearch
	{
		private readonly IGridProvider _grid;

		private readonly FastPriorityQueue _open;

		public bool SameGrid(IGridProvider grid)
		{
			return this._grid == grid;
		}

		public AStarSearch(IGridProvider grid)
		{
			this._grid = grid;
			this._open = new FastPriorityQueue(this._grid.Size.X * this._grid.Size.Y);
		}

		private double Heuristic(Cell cell, Cell goal)
		{
			int num;
			num = Math.Abs(cell.Location.X - goal.Location.X);
			int num2;
			num2 = Math.Abs(cell.Location.Y - goal.Location.Y);
			return (double)(num + num2) + (Math.Sqrt(2.0) - 2.0) * (double)Math.Min(num, num2);
		}

		public void Reset()
		{
			this._grid.Reset();
			this._open.Clear();
		}

		public Cell[] Find(Vector2Int start, Vector2Int goal)
		{
			this.Reset();
			Cell node;
			node = this._grid[start];
			Cell cell;
			cell = this._grid[goal];
			this._open.Enqueue(node, 0.0);
			Vector2Int size;
			size = this._grid.Size;
			Cell cell2;
			cell2 = null;
			while (this._open.Count > 0)
			{
				cell2 = this._open.Dequeue();
				cell2.Closed = true;
				bool flag;
				flag = false;
				double num;
				num = cell2.G + 1.0;
				if (cell.Location == cell2.Location)
				{
					break;
				}
				Vector2Int position;
				position = new Vector2Int(0, 0);
				for (int i = 0; i < PathingConstants.Directions.Length; i++)
				{
					StepDirection stepDirection;
					stepDirection = PathingConstants.Directions[i];
					position.X = cell2.Location.X + stepDirection.X;
					position.Y = cell2.Location.Y + stepDirection.Y;
					if (position.X < 0 || position.X >= size.X || position.Y < 0 || position.Y >= size.Y)
					{
						continue;
					}
					Cell cell3;
					cell3 = this._grid[position];
					if (cell3.Blocked)
					{
						if (i < 4)
						{
							flag = true;
						}
					}
					else if (!(i >= 4 && flag) && !this._grid[cell3.Location].Closed)
					{
						if (!this._open.Contains(cell3))
						{
							cell3.G = num;
							cell3.H = this.Heuristic(cell3, cell2);
							cell3.Parent = cell2;
							this._open.Enqueue(cell3, cell3.G + cell3.H);
						}
						else if (num + cell3.H < cell3.F)
						{
							cell3.G = num;
							cell3.F = cell3.G + cell3.H;
							cell3.Parent = cell2;
						}
					}
				}
			}
			Stack<Cell> stack;
			stack = new Stack<Cell>();
			while (cell2 != null)
			{
				stack.Push(cell2);
				cell2 = cell2.Parent;
			}
			return stack.ToArray();
		}
	}
}
