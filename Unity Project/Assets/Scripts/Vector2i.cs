public struct mVector2i
{
	public int X, Y;
	public mVector2i(int x, int y) { X = x; Y = y; }
	
	public mVector2i LessX() { return new mVector2i(X - 1, Y); }
	public mVector2i LessY() { return new mVector2i(X, Y - 1); }
	public mVector2i MoreX() { return new mVector2i(X + 1, Y); }
	public mVector2i MoreY() { return new mVector2i(X, Y + 1); }


	public static bool operator==(mVector2i first, mVector2i second)
	{
		return first.X == second.X && first.Y == second.Y;
	}
	public static bool operator!=(mVector2i first, mVector2i second)
	{
		return first.X != second.X || first.Y != second.Y;
	}

	public static mVector2i operator+(mVector2i first, mVector2i second)
	{
		return new mVector2i(first.X + second.X, first.Y + second.Y);
	}
	public static mVector2i operator-(mVector2i first, mVector2i second)
	{
		return new mVector2i(first.X - second.X, first.Y - second.Y);
	}


	/// <summary>
	/// Used for pathfinding.
	/// </summary>
	public float Distance(mVector2i other)
	{
		return UnityEngine.Mathf.Sqrt(DistanceSquared(other));
	}
	/// <summary>
	/// Used for pathfinding.
	/// </summary>
	public float DistanceSquared(mVector2i other)
	{
		int xx = other.X - X,
			yy = other.Y - Y;
		return (float)((xx * xx) + (yy * yy));
	}


	public override string ToString()
	{
		return "{" + X.ToString() + ", " + Y.ToString() + "}";
	}
	public override int GetHashCode()
	{
		return (X * 73856093) ^ (Y * 19349663);
	}
}