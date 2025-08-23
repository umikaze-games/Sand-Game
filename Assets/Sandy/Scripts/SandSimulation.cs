using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SandSimulation : MonoBehaviour
{
	[Header(" Elements ")]
	private SpriteRenderer renderer;
	private Texture2D texture;

	[Header(" Settings ")]
	[SerializeField] private int width;
	[SerializeField] private int height;
	[SerializeField] private Color backgroundColor;

	[Header(" Data ")]
	private Cell[,] grid;
	void Start()
	{
		texture = new Texture2D(width, height);
		texture.filterMode = FilterMode.Point;

		grid = new Cell[width, height];

		for (int y = 0; y < height; y++)
			for (int x = 0; x < width; x++)
				grid[x, y].color = backgroundColor;

		grid[width / 2, height / 2] = new Cell { type = EMaterialType.Sand, color = Color.yellow };
		UpdateTexture();

		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = Sprite.Create(
			texture,
			new Rect(0, 0, width, height),
			Vector2.one / 2,
			100
		);
	}
	void Update()
	{
		HandleInput();
		SimulateSand();
		UpdateTexture();

	}

	private void HandleInput()
	{
		if (!Input.GetMouseButton(0))   // 鼠标左键“按住”检测；想要“点一下”请用 GetMouseButtonDown(0)
			return;

		// 屏幕坐标 → 世界坐标
		Vector3 worldClickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		// 世界坐标 → 网格坐标(像素格)
		Vector2Int gridCoords = WorldToGrid(worldClickedPosition);

		if (!IsInBounds(gridCoords))    // 边界检查
			return;

		// 在网格上“落一粒沙”
		Color color = Random.ColorHSV(0, 1, .8f, 1);  // 高饱和度随机色
		grid[gridCoords.x, gridCoords.y] = new Cell { type = EMaterialType.Sand, color = color };
	}

	private bool IsInBounds(Vector2Int coords)
	{
		return coords.x >= 0 && coords.x < width
			&& coords.y >= 0 && coords.y < height;
	}

	private Vector2Int WorldToGrid(Vector3 worldPos)
	{
		// 转到“物体局部空间”，自动抵消位置/旋转/缩放的影响
		Vector3 local = transform.InverseTransformPoint(worldPos);

		// 以 pivot=中心 为基准：+ width/2、height/2 把原点从中心移到左下角
		int x = Mathf.FloorToInt(local.x * 100 + width * 0.5f);
		int y = Mathf.FloorToInt(local.y * 100 + height * 0.5f);
		return new Vector2Int(x, y);
	}
	private void UpdateTexture()
	{
		for (int y = 0; y < height; y++)
			for (int x = 0; x < width; x++)
				texture.SetPixel(x, y, grid[x, y].color);

		texture.Apply();
	}


	private void SimulateSand()
	{
		for (int y = 1; y < height; y++)          // 从 y=1 开始，一直到 height-1（底→上）
		{
			for (int x = 0; x < width; x++)
			{
				if (grid[x, y].type != EMaterialType.Sand)  // 只处理沙子格子
					continue;

				TryMoveSand(x, y);
			}
		}
	}

	private void TryMoveSand(int x, int y)
	{
		// 边缘：交给专用处理，避免 x±1 越界
		if (x == 0 || x == width - 1)
		{
			HandleBorder(x, y);
			return;
		}

		// 1) 直落
		if (grid[x, y - 1].type == EMaterialType.Empty)
		{
			Swap(x, y, x, y - 1);
			return;
		}

		// 2) 斜落：左右随机，防止偏向
		int dir = UnityEngine.Random.value < 0.5f ? -1 : 1; // -1=左, +1=右
		if (grid[x + dir, y - 1].type == EMaterialType.Empty)
		{
			Swap(x, y, x + dir, y - 1);
			return;
		}
		if (grid[x - dir, y - 1].type == EMaterialType.Empty)
		{
			Swap(x, y, x - dir, y - 1);
			return;
		}

		// （可选）与水互换
		// if (grid[x, y - 1].type == EMaterialType.Water) Swap(x, y, x, y - 1);
	}

	private void HandleBorder(int x, int y)
	{
		// 先直落
		if (y > 0 && grid[x, y - 1].type == EMaterialType.Empty)
		{
			Swap(x, y, x, y - 1);
			return;
		}

		// 只能朝「场内」的那一侧斜落
		int dir = (x == 0) ? +1 : -1;
		if (y > 0 && (uint)(x + dir) < (uint)width &&
			grid[x + dir, y - 1].type == EMaterialType.Empty)
		{
			Swap(x, y, x + dir, y - 1);
		}
	}

	private void Swap(int x1, int y1, int x2, int y2)
	{
		Cell temp = grid[x1, y1];   
		grid[x1, y1] = grid[x2, y2]; 
		grid[x2, y2] = temp;
	}
}
