using System;
using System.Collections.Generic;
using UnityEngine;
public class ShapeHolder : MonoBehaviour
{
	
	[Header(" Elements ")]
	[SerializeField] private SpriteRenderer renderer;

	private Shape shape;
	public Shape Shape => shape;

	private Color color;
	public Color Color => color;
	public Bounds Bounds=> renderer.bounds;
	public void Configure(Shape shape, Color color)
	{
		// Assign shape and color, then generate a recolored texture
		this.shape = shape;
		this.color = color;

		Texture2D tex = shape.tex;
		Texture2D newTex = new Texture2D(tex.width, tex.height);
		newTex.filterMode = FilterMode.Point;

		Color[] colors = tex.GetPixels();

		// Replace non-transparent pixels with chosen color
		for (int i = 0; i < colors.Length; i++)
		{
			if (colors[i].a > .1f)
				colors[i] = color;
		}


		newTex.SetPixels(colors);
		newTex.Apply();

		// Create sprite from the new texture and assign to renderer
		renderer.sprite = Sprite.Create(
			newTex,
			new Rect(0, 0, newTex.width, newTex.height),
			Vector2.one / 2,
			100
		);
	}

	// Reset scale to full size when picked up
	public void Pickup()
=> renderer.transform.localScale = Vector3.one;

	// Scale down slightly when put back
	public void PutBack()
		=> renderer.transform.localScale = Vector3.one * 0.8f;



}

