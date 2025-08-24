using UnityEngine;

public class ShapeHolder : MonoBehaviour
{
	[Header(" Elements ")]
	[SerializeField] private SpriteRenderer renderer;

	public void Configure(Shape shape, Color color)
	{
		// 从 shape 中取出原始纹理
		Texture2D tex = shape.tex;

		// 新建一个和原始纹理相同大小的纹理
		Texture2D newTex = new Texture2D(tex.width, tex.height);
		newTex.filterMode = FilterMode.Point;  // 像素风格（不插值）

		// 拷贝颜色数据
		Color[] colors = tex.GetPixels();

		for (int i = 0; i < colors.Length; i++)
		{
			if (colors[i].a > .1f)   // 如果这个像素的透明度 > 0.1（忽略透明区域）
				colors[i] = color;   // 把它换成传入的颜色
		}


		newTex.SetPixels(colors);
		newTex.Apply();

		// 生成一个新的 Sprite 并赋值给 SpriteRenderer
		renderer.sprite = Sprite.Create(
			newTex,
			new Rect(0, 0, newTex.width, newTex.height),
			Vector2.one / 2,
			100
		);
	}
}

