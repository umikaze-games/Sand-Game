using UnityEngine;

public class Shape
{
	public Cell[,] cells;
	public int width;
	public int height;

	public Shape(int width, int height)
	{
		this.width = width;
		this.height = height;
		cells = new Cell[width, height];
	}
}
