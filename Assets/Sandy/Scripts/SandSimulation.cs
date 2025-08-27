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

			// ���� TexturePathChecker����������ɫ�Ƿ��д��ұ�������ߵ�ͨ·
			if (TexturePathChecker.TryGetConnectedRegion(texture, color, out HashSet<Vector2Int> coords))
			{
				// �������ͨ���򣬰����������Ӧ�ĸ������
				foreach (Vector2Int coord in coords)
				{
					grid[coord.x, coord.y].type = EMaterialType.Empty;
					grid[coord.x, coord.y].color = backgroundColor;
				}

				// ˢ����ͼ
				UpdateTexture();
				RaiseClearEvent("clear");
				// �ҵ�һ��ƥ����˳������������������ɫ
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

		// �������Ļ����ת��Ϊ��������
		Vector3 worldclickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		// �ٰ���������ת��Ϊ��������
		Vector2Int gridCoords = WorldToGrid(worldclickedPosition);

		// ���ȡһ�� Shape����״������һ����ɫ
		Shape randomShape = ShapeManager.instance.Shapes.GetRandom();
		Color randomColor = ShapeManager.instance.Colors.GetRandom();

		// �������״�ŵ� grid ��
		DropShape(randomShape, randomColor, gridCoords);
	}

	private void DropShape(Shape shape, Color color, Vector2Int gridCoords)
	{
		for (int y = 0; y < shape.height; y++)
		{
			for (int x = 0; x < shape.width; x++)
			{
				// �� Shape �ڲ����� (x,y) ӳ�䵽ȫ�� grid ������
				int texX = gridCoords.x - (shape.width / 2) + x;
				int texY = gridCoords.y - (shape.height / 2) + y;

				// �߽��� �� �����յ�Ԫ��
				if (!IsInBounds(new Vector2Int(texX, texY)) || shape.cells[x, y].type == EMaterialType.Empty)
					continue;

				// ȡ�� Shape ��һ����Ԫ��
				Cell cell = shape.cells[x, y];

				// ������ɫ��ʹ�����ɫ��
				cell.color = color;

				// д�뵽ȫ�� grid
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
		// ת��������ֲ��ռ䡱���Զ�����λ��/��ת/���ŵ�Ӱ��
		Vector3 local = transform.InverseTransformPoint(worldPos);

		// �� pivot=���� Ϊ��׼��+ width/2��height/2 ��ԭ��������Ƶ����½�
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
		// ��Ȼ��������ɨ����֤ͬһ֡��ÿ��ɳ���������һ��
		for (int y = 1; y < height; y++)
		{
			// ÿһ�н������ҷ����ٵ���֡���Ŷ��������Ӿ�����
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
		// ��ֱ��
		if (y > 0 && grid[x, y - 1].type == EMaterialType.Empty)
		{
			Swap(x, y, x, y - 1);
			return;
		}

		// ֻ�ܳ������ڡ�����һ��б��
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
		// 1. ����������ת��Ϊ��������
		Vector2Int gridCoords = WorldToGrid(shapeHolder.transform.position);

		// 2. ���� DropShape������״�ŵ�������
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
					return false; // �����г�ͻ���������Ѿ��ж�����
				}
			}
		}

		return true; // ���и��Ӷ��Ϸ������Է���
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
