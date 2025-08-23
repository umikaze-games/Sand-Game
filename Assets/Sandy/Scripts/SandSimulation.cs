using UnityEngine;

public class SandSimulation : MonoBehaviour
{
	[Header(" Elements ")]
	private SpriteRenderer renderer;
	private Texture2D texture;

	[Header(" Settings ")]
	[SerializeField] private int width;
	[SerializeField] private int height;
	[SerializeField] private Color backgroundColor;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		texture = new Texture2D(width, height);
		texture.filterMode = FilterMode.Point;

		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = Sprite.Create(
			texture,
			new Rect(0, 0, width, height),
			Vector2.one / 2,
			100
		);
	}

	// Update is called once per frame
	void Update()
	{
	}
}
