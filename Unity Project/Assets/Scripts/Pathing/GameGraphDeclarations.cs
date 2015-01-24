using System;
using System.Collections.Generic;


public class GameNode : Node
{
	public mVector2i GridCoord;
	public GameNode(mVector2i gridCoord) { GridCoord = gridCoord; }

	public override bool IsEqualTo(Node other)
	{
		return GridCoord == ((GameNode)other).GridCoord;
	}
	public override bool IsNotEqualTo(Node other)
	{
		return GridCoord != ((GameNode)other).GridCoord;
	}
	public override int GetHashCode()
	{
		return GridCoord.GetHashCode();
	}
}

public class GameEdge : Edge<GameNode>
{
	public GameEdge(GameNode start, GameNode end) : base(start, end) { }

	public override float GetSearchCost(PathFinder<GameNode> pather)
	{
		return Start.GridCoord.Distance(End.GridCoord);
	}
	public override float GetTraversalCost(PathFinder<GameNode> pather)
	{
		float d = Start.GridCoord.DistanceSquared(End.GridCoord);

		if (pather.HasSpecificEnd)
		{
			float endD = End.GridCoord.DistanceSquared(pather.End.GridCoord);
			return d + endD;
		}
		else
		{
			return d;
		}
	}
}



public class GameGraph : Graph<GameNode>
{
	public bool CanMoveDiagonally;

	private Room Rm { get { return GameRegion.Instance.RoomObj; } }


	public GameGraph(bool canMoveDiagonally) { CanMoveDiagonally = canMoveDiagonally; }


	public void GetConnections(GameNode starting, List<Edge<GameNode>> outEdgeList)
	{
		bool lessX = (starting.GridCoord.X > 0),
			 lessY = (starting.GridCoord.Y > 0),
			 moreX = (starting.GridCoord.X < (Rm.RoomGrid.GetLength(0) - 1)),
			 moreY = (starting.GridCoord.Y < (Rm.RoomGrid.GetLength(1) - 1));

		if (lessX && Rm.RoomGrid[starting.GridCoord.X - 1, starting.GridCoord.Y] == null)
		{
			outEdgeList.Add(new GameEdge(starting, new GameNode(starting.GridCoord.LessX())));
		}
		if (lessY && Rm.RoomGrid[starting.GridCoord.X, starting.GridCoord.Y - 1] == null)
		{
			outEdgeList.Add(new GameEdge(starting, new GameNode(starting.GridCoord.LessY())));
		}
		if (moreX && Rm.RoomGrid[starting.GridCoord.X + 1, starting.GridCoord.Y] == null)
		{
			outEdgeList.Add(new GameEdge(starting, new GameNode(starting.GridCoord.MoreX())));
		}
		if (moreY && Rm.RoomGrid[starting.GridCoord.X, starting.GridCoord.Y + 1] == null)
		{
			outEdgeList.Add(new GameEdge(starting, new GameNode(starting.GridCoord.MoreY())));
		}

		if (CanMoveDiagonally)
		{
			if (lessX && lessY &&
				Rm.RoomGrid[starting.GridCoord.X - 1, starting.GridCoord.Y - 1] == null)
			{
				outEdgeList.Add(new GameEdge(starting, new GameNode(starting.GridCoord.LessX().LessY())));
			}
			if (lessX && moreY &&
				Rm.RoomGrid[starting.GridCoord.X - 1, starting.GridCoord.Y + 1] == null)
			{
				outEdgeList.Add(new GameEdge(starting, new GameNode(starting.GridCoord.LessX().MoreY())));
			}
			if (moreX && lessY &&
				Rm.RoomGrid[starting.GridCoord.X + 1, starting.GridCoord.Y - 1] == null)
			{
				outEdgeList.Add(new GameEdge(starting, new GameNode(starting.GridCoord.MoreX().LessY())));
			}
			if (moreX && moreY &&
				Rm.RoomGrid[starting.GridCoord.X + 1, starting.GridCoord.Y + 1] == null)
			{
				outEdgeList.Add(new GameEdge(starting, new GameNode(starting.GridCoord.MoreX().MoreY())));
			}
		}
	}
}