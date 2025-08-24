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
		// �����λ�÷������ߣ�ֻ��� shapeHolderMask ���ϵ����壩
		RaycastHit2D hit = Physics2D.Raycast(
			Camera.main.ScreenToWorldPoint(Input.mousePosition), // ��㣺����ת��������
			Vector2.zero,                                        // ���߷���zero = ֻ��������
			Mathf.Infinity,                                      // ������
			shapeHolderMask                                      // �����֣�ֻ��� ShapeHolder ��
		);

		if (hit.collider == null)
		{
			return;
		}
		// ���û�л��� ShapeHolder �� return
		if (!hit.collider.TryGetComponent(out ShapeHolder shapeHolder))
			return;

		// �ɹ��㵽 �� ��¼��ǰ ShapeHolder
		currentShapeHolder = shapeHolder;
		currentShapeHolder.Pickup();
		// ���������ʱ����Ļλ�ã�������ק���㣩
		clickedPosition = Input.mousePosition;

		// ���� ShapeHolder ���ʱ������λ��
		shapeClickedPosition = currentShapeHolder.transform.position;
	}

	private void HandleDrag()
	{
		// ��굱ǰλ����Ե��ʱ��ƫ��������Ļ����ϵ��
		Vector2 delta = Input.mousePosition - clickedPosition;

		// ��һ������������ת�� 0~1 �ı�����
		delta.x /= Screen.width;
		delta.y /= Screen.height;

		// ������ק�ٶ�
		delta *= moveSpeed;

		// Ŀ��λ�� = ���ʱ Shape ��λ�� + ����϶�ƫ��
		Vector2 targetPosition = (Vector2)shapeClickedPosition + delta;

		Bounds shapeBounds=currentShapeHolder.Bounds;
		float maxX = SandSimulation.maxX - shapeBounds.extents.x;
		float maxY = SandSimulation.maxY - shapeBounds.extents.y;

		targetPosition.x = Mathf.Clamp(targetPosition.x, -maxX, maxX);
		targetPosition.y = Mathf.Clamp(targetPosition.y, targetPosition.y, maxY);
		// ƽ����ֵ��������ƽ���ƶ�������˲�ƣ�
		currentShapeHolder.transform.position =
			Vector3.Lerp(
				currentShapeHolder.transform.position,  // ��ǰʵ��λ��
				targetPosition,                         // ��Ҫ�����λ��
				Time.deltaTime * 60f * 0.3f             // ��ֵ�ٶȣ�֡�ʶ�����
			);
	}


	private void HandleMouseUp()
	{
		// �ж��ܲ��ܶ��� Shape
		// ���λ��̫�� �� �ص�ԭλ��
		// ����Ϸ� �� ���������¼�
		if (currentShapeHolder.transform.position.y < dropYThreshold||!SandSimulation.instance.CanDropShape(currentShapeHolder))
		{
			MoveShapeBack();
		}
		else
		{
			// ʹ�� ?.Invoke ��ֹ�¼�û�ж���ʱ����
			shapeDropped?.Invoke(currentShapeHolder);

			// ȡ����ǰ���ã��ͷţ�
			currentShapeHolder = null;
		}
	}

	private void MoveShapeBack()
	{
		// �� LeanTween �������� Shape �Żص��ʱ��λ��
		currentShapeHolder.PutBack();

		LeanTween.move(
			currentShapeHolder.gameObject,   // Ŀ�� GameObject
			shapeClickedPosition,            // �ص����ʱ��λ��
			0.1f                             // ����ʱ��
		);

		// �ͷŵ�ǰ����
		currentShapeHolder = null;
	}

}
