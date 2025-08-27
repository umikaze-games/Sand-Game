using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class SandSimulation : MonoBehaviour
{
	public static SandSimulation instance;

	[Header(" Elements ")]
	private SpriteRenderer renderer;
	private Texture2D texture;

	[Header(" Settings ")]
	[SerializeField] private int width;
	[SerializeField] private int height;
	[SerializeField] private Color backgroundColor;

	[Header(" Data ")]
	private Cell[,] grid;

	public static float maxX;
	public static float maxY;

	private bool sandMoved;
	private bool searchedForMatch;

	public static Action<string> clearEvent;
	public static Action<string> dropEvent;
	private void Awake()
	{
		Application.targetFrameRate = 60;
		InputManager.shapeDropped += OnShapeDropped;

		if (instance == null) { instance = this; }
		else { Destroy(gameObject); }
	}


	private void OnDisable()
	{
		InputManager.shapeDropped -= OnShapeDropped;
	}


	void Start()
	{
		texture = new Texture2D(width, height);
		texture.filterMode = FilterMode.Point;

		grid = new Cell[width, height];

		for (int y = 0; y < height; y++)
			for (int x = 0; x < width; x++)
				grid[x, y].color = backgroundColor;

		
		UpdateTexture();

		CalculateBounds();

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
		for (int i = 0; i < 3; i++)
		{
			SimulateSand();

			if (!sandMoved && !searchedForMatch)
			{
				TryFindMatch();
			}

			UpdateTexture();
		}

	}

	private void TryFindMatch()
	{
		searchedForMatch = true;

		for (int i = 0; i < ShapeManager.instance.Colors.Length; i++)
		{
			Color color = ShapeManager.instance.Colors[i];

			// 调用 TexturePathChecker：检测这个颜色是否有从右边连到左边的通路
			if (TexturePathChecker.TryGetConnectedRegion(texture, color, out HashSet<Vector2Int> coords))
			{
				// 如果有连通区域，把所有坐标对应的格子清空
				foreach (Vector2Int coord in coords)
				{
					grid[coord.x, coord.y].type = EMaterialType.Empty;
					grid[coord.x, coord.y].color = backgroundColor;
				}

				// 刷新贴图
				UpdateTexture();
				RaiseClearEvent("clear");
				// 找到一个匹配就退出，不继续检测其他颜色
				break;
			}
		}
	}


	private void CalculateBounds()
	{
		maxX = (float)width / 200f;
		maxY= (float)height / 200f;	
	}

	private void HandleInput()
	{
		if (!Input.GetMouseButtonDown(0))
			return;

		// 把鼠标屏幕坐标转换为世界坐标
		Vector3 worldclickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		// 再把世界坐标转换为网格坐标
		Vector2Int gridCoords = WorldToGrid(worldclickedPosition);

		// 随机取一个 Shape（形状），和一个颜色
		Shape randomShape = ShapeManager.instance.Shapes.GetRandom();
		Color randomColor = ShapeManager.instance.Colors.GetRandom();

		// 把这个形状放到 grid 上
		DropShape(randomShape, randomColor, gridCoords);
	}

	private void DropShape(Shape shape, Color color, Vector2Int gridCoords)
	{
		for (int y = 0; y < shape.height; y++)
		{
			for (int x = 0; x < shape.width; x++)
			{
				// 把 Shape 内部坐标 (x,y) 映射到全局 grid 的坐标
				int texX = gridCoords.x - (shape.width / 2) + x;
				int texY = gridCoords.y - (shape.height / 2) + y;

				// 边界检查 或 跳过空单元格
				if (!IsInBounds(new Vector2Int(texX, texY)) || shape.cells[x, y].type == EMaterialType.Empty)
					continue;

				// 取出 Shape 的一个单元格
				Cell cell = shape.cells[x, y];

				// 给它上色（使用随机色）
				cell.color = color;

				// 写入到全局 grid
				grid[texX, texY] = cell;
			}
		}
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
		sandMoved = false;
		// 仍然从下往上扫，保证同一帧里每粒沙子最多下落一次
		for (int y = 1; y < height; y++)
		{
			// 每一行交替左右方向；再叠加帧数扰动，减少视觉条纹
			bool rightToLeft = ((y & 1) == (Time.frameCount & 1));

			if (rightToLeft)
			{
				for (int x = width - 1; x >= 0; x--)
				{
					if (grid[x, y].type != EMaterialType.Sand) continue;
					TryMoveSand(x, y);
				}
			}
			else
			{
				for (int x = 0; x < width; x++)
				{
					if (grid[x, y].type != EMaterialType.Sand) continue;
					TryMoveSand(x, y);
				}
			}
		}

	}


	private void TryMoveSand(int x, int y)
	{
		// Border
		if (x == 0 || x == width - 1)
		{
			HandleBorder(x, y);
			return;
		}

		// Below
		if (grid[x, y - 1].type == EMaterialType.Empty)
			Swap(x, y, x, y - 1);

		// Down Right
		else if (grid[x + 1, y - 1].type == EMaterialType.Empty)
			Swap(x, y, x + 1, y - 1);

		// Down Left
		else if (grid[x - 1, y - 1].type == EMaterialType.Empty)
			Swap(x, y, x - 1, y - 1);
		
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
		sandMoved = true;
		searchedForMatch = false;
	}

	private void OnShapeDropped(ShapeHolder shapeHolder)
	{
		// 1. 把世界坐标转换为网格坐标
		Vector2Int gridCoords = WorldToGrid(shapeHolder.transform.position);

		// 2. 调用 DropShape，把形状放到网格上
		DropShape(shapeHolder.Shape, shapeHolder.Color, gridCoords);
	

		Destroy(shapeHolder.gameObject);
	}

	public bool CanDropShape(ShapeHolder holder)
	{
		Shape shape = holder.Shape;
		Vector2Int gridCoords = WorldToGrid(holder.transform.position);

		for (int y = 0; y < shape.height; y++)
		{
			for (int x = 0; x < shape.width; x++)
			{
				int texX = gridCoords.x - (shape.width / 2) + x;
				int texY = gridCoords.y - (shape.height / 2) + y;

				if (IsInBounds(new Vector2Int(texX, texY))
					&& shape.cells[x, y].type != EMaterialType.Empty
					&& grid[texX, texY].type != EMaterialType.Empty)
				{
					return false; // 发现有冲突：格子里已经有东西了
				}
			}
		}

		return true; // 所有格子都合法，可以放置
	}

	public void RaiseClearEvent(string name)
	{ 
		clearEvent.Invoke(name);
	}

	public void RaiseDropEvent(string name)
	{
		dropEvent.Invoke(name);
	}

}
