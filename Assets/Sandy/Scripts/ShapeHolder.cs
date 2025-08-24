using UnityEngine;

public class ShapeHolder : MonoBehaviour
{
	[Header(" Elements ")]
	[SerializeField] private SpriteRenderer renderer;

	public void Configure(Shape shape, Color color)
	{
		// �� shape ��ȡ��ԭʼ����
		Texture2D tex = shape.tex;

		// �½�һ����ԭʼ������ͬ��С������
		Texture2D newTex = new Texture2D(tex.width, tex.height);
		newTex.filterMode = FilterMode.Point;  // ���ط�񣨲���ֵ��

		// ������ɫ����
		Color[] colors = tex.GetPixels();

		for (int i = 0; i < colors.Length; i++)
		{
			if (colors[i].a > .1f)   // ���������ص�͸���� > 0.1������͸������
				colors[i] = color;   // �������ɴ������ɫ
		}


		newTex.SetPixels(colors);
		newTex.Apply();

		// ����һ���µ� Sprite ����ֵ�� SpriteRenderer
		renderer.sprite = Sprite.Create(
			newTex,
			new Rect(0, 0, newTex.width, newTex.height),
			Vector2.one / 2,
			100
		);
	}
}

