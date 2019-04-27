public struct IntPair
{
	public int x, y;

	public IntPair(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public override string ToString()
	{
		return $"({x}, {y})";
	}
}
