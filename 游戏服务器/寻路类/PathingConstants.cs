namespace AStar
{
	public static class PathingConstants
	{
		public static readonly StepDirection[] Directions = new StepDirection[8]
		{
			new StepDirection(-1, 0),
			new StepDirection(1, 0),
			new StepDirection(0, 1),
			new StepDirection(0, -1),
			new StepDirection(-1, -1),
			new StepDirection(-1, 1),
			new StepDirection(1, -1),
			new StepDirection(1, 1)
		};
	}
}
