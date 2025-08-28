using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	[Header(" Settings ")]
	[SerializeField] private LayerMask shapeHolderMask;

	[SerializeField] private Vector2 moveSpeed;

	[SerializeField] private float dropYThreshold;
	private ShapeHolder currentShapeHolder;
	private Vector3 clickedPosition;
	private Vector3 shapeClickedPosition;

	[Header(" Actions ")]
	public static Action<ShapeHolder> shapeDropped;

	void Start()
	{

	}

	void Update()
	{
		HandleInput();
	}

	private void HandleInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			HandleMouseDown();
		}
		else if (Input.GetMouseButton(0))
		{
			HandleDrag();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			HandleMouseUp();
		}
	}

	private void HandleMouseDown()
	{

		RaycastHit2D hit = Physics2D.Raycast(
			Camera.main.ScreenToWorldPoint(Input.mousePosition), 
			Vector2.zero,                                        
			Mathf.Infinity,
			shapeHolderMask 
		);

		if (hit.collider == null)
		{
			return;
		}

		if (!hit.collider.TryGetComponent(out ShapeHolder shapeHolder))
			return;


		currentShapeHolder = shapeHolder;
		currentShapeHolder.Pickup();

		clickedPosition = Input.mousePosition;


		shapeClickedPosition = currentShapeHolder.transform.position;
	}

	// Move the selected ShapeHolder with mouse drag
	private void HandleDrag()
	{
		Vector2 delta = Input.mousePosition - clickedPosition;

		delta.x /= Screen.width;
		delta.y /= Screen.height;

		delta *= moveSpeed;

		Vector2 targetPosition = (Vector2)shapeClickedPosition + delta;

		Bounds shapeBounds=currentShapeHolder.Bounds;
		float maxX = SandSimulation.maxX - shapeBounds.extents.x;
		float maxY = SandSimulation.maxY - shapeBounds.extents.y;

		targetPosition.x = Mathf.Clamp(targetPosition.x, -maxX, maxX);
		targetPosition.y = Mathf.Clamp(targetPosition.y, targetPosition.y, maxY);

		currentShapeHolder.transform.position =
			Vector3.Lerp(
				currentShapeHolder.transform.position,  
				targetPosition, 
				Time.deltaTime * 60f * 0.3f 
			);
	}

	// Drop the shape if valid, otherwise move it back
	private void HandleMouseUp()
	{
		if (currentShapeHolder.transform.position.y < dropYThreshold||!SandSimulation.instance.CanDropShape(currentShapeHolder))
		{
			MoveShapeBack();
		}
		else
		{
			shapeDropped?.Invoke(currentShapeHolder);

			currentShapeHolder = null;
		}
	}

	// Animate the ShapeHolder back to its original position
	private void MoveShapeBack()
	{
		currentShapeHolder.PutBack();

		LeanTween.move(
			currentShapeHolder.gameObject, 
			shapeClickedPosition,  
			0.1f 
		);

		currentShapeHolder = null;
	}

}
