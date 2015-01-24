public struct mVector2i
{
	public int X, Y;
	public mVector2i(int x, int y) { X = x; Y = y; }
	
	public mVector2i LessX() { return new mVector2i(X - 1, Y); }
	public mVector2i LessY() { return new mVector2i(X, Y - 1); }
	public mVector2i MoreX() { return new mVector2i(X + 1, Y); }
	public mVector2i MoreY() { return new mVector2i(X, Y + 1); }
}