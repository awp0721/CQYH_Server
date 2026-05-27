using System;
using System.Collections.Generic;

namespace AStar
{
	public class AStarSearch_Staic
	{
		private static double Heuristic(Cell cell, Cell goal)
		{
			int num;
			num = Math.Abs(cell.Location.X - goal.Location.X);
			int num2;
			num2 = Math.Abs(cell.Location.Y - goal.Location.Y);
			return (double)(num + num2) + (Math.Sqrt(2.0) - 2.0) * (double)Math.Min(num, num2);
		}

		public static Cell[] Find(Vector2Int start, Vector2Int goal, IGridProvider _grid)
		{
			FastPriorityQueue fastPriorityQueue;
			fastPriorityQueue = new FastPriorityQueue(_grid.Size.X * _grid.Size.Y);
			Cell node;
			node = _grid[start];
			Cell cell;
			cell = _grid[goal];
			fastPriorityQueue.Enqueue(node, 0.0);
			Vector2Int size;
			size = _grid.Size;
			Cell cell2;
			cell2 = null;
			while (fastPriorityQueue.Count > 0)
			{
				cell2 = fastPriorityQueue.Dequeue();
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
					cell3 = _grid[position];
					if (cell3.Blocked)
					{
						if (i < 4)
						{
							flag = true;
						}
					}
					else if (!(i >= 4 && flag) && !_grid[cell3.Location].Closed)
					{
						if (!fastPriorityQueue.Contains(cell3))
						{
							cell3.G = num;
							cell3.H = AStarSearch_Staic.Heuristic(cell3, cell2);
							cell3.Parent = cell2;
							fastPriorityQueue.Enqueue(cell3, cell3.G + cell3.H);
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
