using System;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
	[Header(" Elements ")]
	[SerializeField] private ShapeHolder shapeHolderPrefab;
	[SerializeField] private Transform slotsParent;


	[Header("Data")]
    [SerializeField] private Sprite[]shapeSprites;

	public static ShapeManager instance;

	private Shape[] shapes;
	public Shape[] Shapes=>shapes;

	[SerializeField]
	private Color[] colors;
	public Color[] Colors => colors;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else { Destroy(gameObject); }
	}
	void Start()
    {
		GenerateShapes();
		PopulateSlots();
	}

	
	// Update is called once per frame
	void Update()
    {
        
    }

	private void PopulateSlots()
	{
		for (int i = 0; i < slotsParent.childCount; i++)
		{
			// 在每个子节点位置生成一个 ShapeHolder，并设为同一父节点
			Vector3 spawnPos = slotsParent.GetChild(i).position;
			ShapeHolder holder = Instantiate(shapeHolderPrefab, spawnPos, Quaternion.identity, transform);

			// 随机取一个形状与颜色
			Shape shape = shapes.GetRandom();
			Color color = colors.GetRandom();

			// 配置
			holder.Configure(shape, color);
		}
	}

	private void GenerateShapes()
	{
		// 创建一个 Shape 数组，长度与 shapeSprites 一样
		shapes = new Shape[shapeSprites.Length];

		// 遍历所有的 Sprite
		for (int i = 0; i < shapes.Length; i++)
		{
			// 先把 Sprite 转换成可读的 Texture2D
			Texture2D tex = ExtractTextureFromSprite(shapeSprites[i]);

			// 再把 Texture2D 转换成 Shape（里面是 Cell 网格）
			shapes[i] = GenerateShapeFromTexture(tex);
			shapes[i].tex = tex;
		}
	}

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
	private Shape GenerateShapeFromTexture(Texture2D tex)
	{
		// 用传入的纹理宽高来生成一个 Shape 对象
		Shape shape = new Shape(tex.width, tex.height);

		// 遍历所有像素 (二维循环)
		for (int y = 0; y < tex.height; y++)
		{
			for (int x = 0; x < tex.width; x++)
			{
				// 获取当前像素的颜色
				Color pixelColor = tex.GetPixel(x, y);

				// 如果透明度 (alpha) 很低 (<0.1)，认为是“空”
				if (pixelColor.a < .1f)
					shape.cells[x, y] = Cell.Empty; // 使用你定义的静态属性 Empty
				else
					// 否则就当成一粒沙子，这里固定给白色
					shape.cells[x, y] = new Cell { type = EMaterialType.Sand, color = Color.white };
			}
		}

		return shape;
	}

}
