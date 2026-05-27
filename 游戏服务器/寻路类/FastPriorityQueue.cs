using System;
using System.Runtime.CompilerServices;

namespace AStar
{
	public sealed class FastPriorityQueue
	{
		private readonly Cell[] _nodes;

		public int Count { get; private set; }

		public FastPriorityQueue(int maxNodes)
		{
			this.Count = 0;
			this._nodes = new Cell[maxNodes + 1];
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			Array.Clear(this._nodes, 1, this.Count);
			this.Count = 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveInlining)]
		public bool Contains(Cell node)
		{
			return this._nodes[node.QueueIndex] == node;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveInlining)]
		public void Enqueue(Cell node, double priority)
		{
			node.F = priority;
			this.Count++;
			this._nodes[this.Count] = node;
			node.QueueIndex = this.Count;
			this.CascadeUp(node);
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveInlining)]
		public Cell Dequeue()
		{
			Cell result;
			result = this._nodes[1];
			if (this.Count == 1)
			{
				this._nodes[1] = null;
				this.Count = 0;
				return result;
			}
			Cell cell;
			cell = this._nodes[this.Count];
			this._nodes[1] = cell;
			cell.QueueIndex = 1;
			this._nodes[this.Count] = null;
			this.Count--;
			this.CascadeDown(cell);
			return result;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveInlining)]
		public void UpdatePriority(Cell node, double priority)
		{
			node.F = priority;
			this.OnNodeUpdated(node);
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveInlining)]
		public void Remove(Cell node)
		{
			if (node.QueueIndex == this.Count)
			{
				this._nodes[this.Count] = null;
				this.Count--;
				return;
			}
			Cell cell;
			cell = this._nodes[this.Count];
			this._nodes[node.QueueIndex] = cell;
			cell.QueueIndex = node.QueueIndex;
			this._nodes[this.Count] = null;
			this.Count--;
			this.OnNodeUpdated(cell);
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveInlining)]
		private void CascadeUp(Cell node)
		{
			if (node.QueueIndex <= 1)
			{
				return;
			}
			int num;
			num = node.QueueIndex >> 1;
			Cell cell;
			cell = this._nodes[num];
			if (this.HasHigherOrEqualPriority(cell, node))
			{
				return;
			}
			this._nodes[node.QueueIndex] = cell;
			cell.QueueIndex = node.QueueIndex;
			node.QueueIndex = num;
			while (num > 1)
			{
				num >>= 1;
				Cell cell2;
				cell2 = this._nodes[num];
				if (this.HasHigherOrEqualPriority(cell2, node))
				{
					break;
				}
				this._nodes[node.QueueIndex] = cell2;
				cell2.QueueIndex = node.QueueIndex;
				node.QueueIndex = num;
			}
			this._nodes[node.QueueIndex] = node;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveInlining)]
		private void CascadeDown(Cell node)
		{
			int queueIndex;
			queueIndex = node.QueueIndex;
			int num;
			num = 2 * queueIndex;
			if (num > this.Count)
			{
				return;
			}
			int num2;
			num2 = num + 1;
			Cell cell;
			cell = this._nodes[num];
			if (this.HasHigherPriority(cell, node))
			{
				if (num2 > this.Count)
				{
					node.QueueIndex = num;
					cell.QueueIndex = queueIndex;
					this._nodes[queueIndex] = cell;
					this._nodes[num] = node;
					return;
				}
				Cell cell2;
				cell2 = this._nodes[num2];
				if (this.HasHigherPriority(cell, cell2))
				{
					cell.QueueIndex = queueIndex;
					this._nodes[queueIndex] = cell;
					queueIndex = num;
				}
				else
				{
					cell2.QueueIndex = queueIndex;
					this._nodes[queueIndex] = cell2;
					queueIndex = num2;
				}
			}
			else
			{
				if (num2 > this.Count)
				{
					return;
				}
				Cell cell3;
				cell3 = this._nodes[num2];
				if (!this.HasHigherPriority(cell3, node))
				{
					return;
				}
				cell3.QueueIndex = queueIndex;
				this._nodes[queueIndex] = cell3;
				queueIndex = num2;
			}
			while (true)
			{
				num = 2 * queueIndex;
				if (num <= this.Count)
				{
					num2 = num + 1;
					cell = this._nodes[num];
					if (this.HasHigherPriority(cell, node))
					{
						if (num2 > this.Count)
						{
							node.QueueIndex = num;
							cell.QueueIndex = queueIndex;
							this._nodes[queueIndex] = cell;
							this._nodes[num] = node;
							return;
						}
						Cell cell4;
						cell4 = this._nodes[num2];
						if (this.HasHigherPriority(cell, cell4))
						{
							cell.QueueIndex = queueIndex;
							this._nodes[queueIndex] = cell;
							queueIndex = num;
						}
						else
						{
							cell4.QueueIndex = queueIndex;
							this._nodes[queueIndex] = cell4;
							queueIndex = num2;
						}
					}
					else
					{
						if (num2 > this.Count)
						{
							node.QueueIndex = queueIndex;
							this._nodes[queueIndex] = node;
							return;
						}
						Cell cell5;
						cell5 = this._nodes[num2];
						if (!this.HasHigherPriority(cell5, node))
						{
							break;
						}
						cell5.QueueIndex = queueIndex;
						this._nodes[queueIndex] = cell5;
						queueIndex = num2;
					}
					continue;
				}
				node.QueueIndex = queueIndex;
				this._nodes[queueIndex] = node;
				return;
			}
			node.QueueIndex = queueIndex;
			this._nodes[queueIndex] = node;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveInlining)]
		private bool HasHigherPriority(Cell higher, Cell lower)
		{
			return higher.F < lower.F;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveInlining)]
		private bool HasHigherOrEqualPriority(Cell higher, Cell lower)
		{
			return higher.F <= lower.F;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveInlining)]
		private void OnNodeUpdated(Cell node)
		{
			int num;
			num = node.QueueIndex >> 1;
			if (num > 0 && this.HasHigherPriority(node, this._nodes[num]))
			{
				this.CascadeUp(node);
			}
			else
			{
				this.CascadeDown(node);
			}
		}
	}
}
