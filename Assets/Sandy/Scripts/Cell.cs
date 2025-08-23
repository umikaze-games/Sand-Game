using UnityEngine;

public enum EMaterialType
{
	Empty = 0,
	Sand = 1,
	Water = 2
}

public struct Cell
{
	public EMaterialType type;
	public Color color;

	public static Cell Empty
		=> new Cell
		{
			type = EMaterialType.Empty,
			color = Color.clear
		};
}
