namespace AStar
{
	public readonly struct StepDirection
	{
		public readonly int X;

		public readonly int Y;

		public StepDirection(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
	}
}
