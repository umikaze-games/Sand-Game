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
		if (!Input.GetMouseButton(0))   // ����������ס����⣻��Ҫ����һ�¡����� GetMouseButtonDown(0)
			return;

		// ��Ļ���� �� ��������
		Vector3 worldClickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		// �������� �� ��������(���ظ�)
		Vector2Int gridCoords = WorldToGrid(worldClickedPosition);

		if (!IsInBounds(gridCoords))    // �߽���
			return;

		// �������ϡ���һ��ɳ��
		Color color = Random.ColorHSV(0, 1, .8f, 1);  // �߱��Ͷ����ɫ
		grid[gridCoords.x, gridCoords.y] = new Cell { type = EMaterialType.Sand, color = color };
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
		for (int y = 1; y < height; y++)          // �� y=1 ��ʼ��һֱ�� height-1���ס��ϣ�
		{
			for (int x = 0; x < width; x++)
			{
				if (grid[x, y].type != EMaterialType.Sand)  // ֻ����ɳ�Ӹ���
					continue;

				TryMoveSand(x, y);
			}
		}
	}

	private void TryMoveSand(int x, int y)
	{
		// ��Ե������ר�ô������� x��1 Խ��
		if (x == 0 || x == width - 1)
		{
			HandleBorder(x, y);
			return;
		}

		// 1) ֱ��
		if (grid[x, y - 1].type == EMaterialType.Empty)
		{
			Swap(x, y, x, y - 1);
			return;
		}

		// 2) б�䣺�����������ֹƫ��
		int dir = UnityEngine.Random.value < 0.5f ? -1 : 1; // -1=��, +1=��
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

		// ����ѡ����ˮ����
		// if (grid[x, y - 1].type == EMaterialType.Water) Swap(x, y, x, y - 1);
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
	}
}
