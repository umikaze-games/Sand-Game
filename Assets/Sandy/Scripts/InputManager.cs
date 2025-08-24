using UnityEngine;

public class InputManager : MonoBehaviour
{
	[Header(" Settings ")]
	[SerializeField] private LayerMask shapeHolderMask;

	private ShapeHolder currentShapeHolder;
	private Vector3 clickedPosition;
	private Vector3 shapeClickedPosition;
	
	[SerializeField]
	private Vector2 moveSpeed;

	void Start()
	{
		// ��ʼ���߼��������Ҫ��
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

		// ���û�л��� ShapeHolder �� return
		if (!hit.collider.TryGetComponent(out ShapeHolder shapeHolder))
			return;

		// �ɹ��㵽 �� ��¼��ǰ ShapeHolder
		currentShapeHolder = shapeHolder;

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
		Debug.Log("Mouse Up");
		// TODO: �����߼�
	}
}
