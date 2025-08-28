using System;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
	public static ShapeManager instance;

	[Header(" Elements ")]
	[SerializeField] private ShapeHolder shapeHolderPrefab;
	[SerializeField] private Transform slotsParent;


	[Header("Data")]
	[SerializeField] private Sprite[] shapeSprites;

	private Shape[] shapes;
	public Shape[] Shapes => shapes;

	[SerializeField]
	private Color[] colors;
	public Color[] Colors => colors;

	private int shapeDroppedCounter;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else { Destroy(gameObject); }

		InputManager.shapeDropped += OnShapeDropped;
	}

	private void OnDisable()
	{
		InputManager.shapeDropped -= OnShapeDropped;
	}
	void Start()
	{
		GenerateShapes();
		PopulateSlots();
	}

	// Spawn ShapeHolders into all slot positions with random shape & color
	private void PopulateSlots()
	{
		for (int i = 0; i < slotsParent.childCount; i++)
		{
			Vector3 spawnPos = slotsParent.GetChild(i).position;
			ShapeHolder holder = Instantiate(shapeHolderPrefab, spawnPos, Quaternion.identity, transform);

			Shape shape = shapes.GetRandom();
			Color color = colors.GetRandom();

			holder.Configure(shape, color);
		}
	}

	// Convert all sprites into Shape objects
	private void GenerateShapes()
	{
		shapes = new Shape[shapeSprites.Length];

		for (int i = 0; i < shapes.Length; i++)
		{

			Texture2D tex = ExtractTextureFromSprite(shapeSprites[i]);

			shapes[i] = GenerateShapeFromTexture(tex);
			shapes[i].tex = tex;
		}
	}

	// Create a readable Texture2D from a Sprite
	private Texture2D ExtractTextureFromSprite(Sprite sprite)
	{
		Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
		tex.filterMode = FilterMode.Point;

		Color[] colors = sprite.texture.GetPixels(
			(int)sprite.rect.x,
			(int)sprite.rect.y,
			(int)sprite.rect.width,
			(int)sprite.rect.height);

		tex.SetPixels(colors);
		tex.Apply();

		return tex;
	}

	// Generate Shape (grid of cells) from a texture
	private Shape GenerateShapeFromTexture(Texture2D tex)
	{
		Shape shape = new Shape(tex.width, tex.height);

		for (int y = 0; y < tex.height; y++)
		{
			for (int x = 0; x < tex.width; x++)
			{
				Color pixelColor = tex.GetPixel(x, y);

				if (pixelColor.a < .1f)
					shape.cells[x, y] = Cell.Empty;
				else
					shape.cells[x, y] = new Cell { type = EMaterialType.Sand, color = Color.white };
			}
		}

		return shape;
	}

	// Track dropped shapes and refill slots every 3 drops
	private void OnShapeDropped(ShapeHolder holder)
	{
		shapeDroppedCounter++;

		if (shapeDroppedCounter >= 3)
		{
			shapeDroppedCounter = 0;
			PopulateSlots();
		}
	}
}

