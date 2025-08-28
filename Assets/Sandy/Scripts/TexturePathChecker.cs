using System.Collections.Generic;
using UnityEngine;

public static class TexturePathChecker
{
	public static bool TryGetConnectedRegion(
		Texture2D tex,
		Color color,
		out HashSet<Vector2Int> coords,
		float tolerance = .01f)
	{
		// Perform BFS to find all connected pixels of the given color,
		// starting from the right edge of the texture.
		// Returns true if the region touches the left edge.

		int width = tex.width;
		int height = tex.height;

		bool[,] visited = new bool[width, height];
		coords = new HashSet<Vector2Int>();
		Queue<Vector2Int> queue = new Queue<Vector2Int>();


		// Initialize queue with matching pixels on the right edge
		for (int y = 0; y < height; y++)
		{
			if (IsSameColor(tex.GetPixel(width - 1, y), color, tolerance))
			{
				Vector2Int pos = new Vector2Int(width - 1, y);
				queue.Enqueue(pos);
				coords.Add(pos);
				visited[width - 1, y] = true;
			}
		}
		Vector2Int[] directions ={Vector2Int.right,Vector2Int.down,Vector2Int.left,Vector2Int.up};

		bool touchesLeft = false;

		// BFS traversal
		while (queue.Count > 0)
		{
			Vector2Int current = queue.Dequeue();

			// Check if region reaches the left edge
			if (current.x == 0)
				touchesLeft = true;

			foreach (Vector2Int direction in directions)
			{
				Vector2Int next = current + direction;

				if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
					continue;
				if (visited[next.x, next.y])
					continue;
				if (!IsSameColor(tex.GetPixel(next.x, next.y), color, tolerance))
					continue;

				queue.Enqueue(next);
				visited[next.x, next.y] = true;
				coords.Add(next);
			}
		}

		// If region does not touch left edge, fail
		if (!touchesLeft)
		{
			coords = null;
			return false;
		}
		return true;
	}

	public static bool IsSameColor(Color color1, Color color2, float tolerance)
	{
		// Compare RGB difference within tolerance (ignores alpha channel)
		return Mathf.Abs(color1.r - color2.r) < tolerance
			&& Mathf.Abs(color1.g - color2.g) < tolerance
			&& Mathf.Abs(color1.b - color2.b) < tolerance;
	}

}
