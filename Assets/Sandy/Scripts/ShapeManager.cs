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
			// ��ÿ���ӽڵ�λ������һ�� ShapeHolder������Ϊͬһ���ڵ�
			Vector3 spawnPos = slotsParent.GetChild(i).position;
			ShapeHolder holder = Instantiate(shapeHolderPrefab, spawnPos, Quaternion.identity, transform);

			// ���ȡһ����״����ɫ
			Shape shape = shapes.GetRandom();
			Color color = colors.GetRandom();

			// ����
			holder.Configure(shape, color);
		}
	}

	private void GenerateShapes()
	{
		// ����һ�� Shape ���飬������ shapeSprites һ��
		shapes = new Shape[shapeSprites.Length];

		// �������е� Sprite
		for (int i = 0; i < shapes.Length; i++)
		{
			// �Ȱ� Sprite ת���ɿɶ��� Texture2D
			Texture2D tex = ExtractTextureFromSprite(shapeSprites[i]);

			// �ٰ� Texture2D ת���� Shape�������� Cell ����
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
		// �ô����������������һ�� Shape ����
		Shape shape = new Shape(tex.width, tex.height);

		// ������������ (��άѭ��)
		for (int y = 0; y < tex.height; y++)
		{
			for (int x = 0; x < tex.width; x++)
			{
				// ��ȡ��ǰ���ص���ɫ
				Color pixelColor = tex.GetPixel(x, y);

				// ���͸���� (alpha) �ܵ� (<0.1)����Ϊ�ǡ��ա�
				if (pixelColor.a < .1f)
					shape.cells[x, y] = Cell.Empty; // ʹ���㶨��ľ�̬���� Empty
				else
					// ����͵���һ��ɳ�ӣ�����̶�����ɫ
					shape.cells[x, y] = new Cell { type = EMaterialType.Sand, color = Color.white };
			}
		}

		return shape;
	}

}
