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
		// 初始化逻辑（如果需要）
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
		// 从鼠标位置发出射线（只检测 shapeHolderMask 层上的物体）
		RaycastHit2D hit = Physics2D.Raycast(
			Camera.main.ScreenToWorldPoint(Input.mousePosition), // 起点：鼠标点转世界坐标
			Vector2.zero,                                        // 射线方向，zero = 只检测这个点
			Mathf.Infinity,                                      // 最大距离
			shapeHolderMask                                      // 层遮罩，只检测 ShapeHolder 层
		);

		// 如果没有击中 ShapeHolder → return
		if (!hit.collider.TryGetComponent(out ShapeHolder shapeHolder))
			return;

		// 成功点到 → 记录当前 ShapeHolder
		currentShapeHolder = shapeHolder;

		// 保存鼠标点击时的屏幕位置（用于拖拽计算）
		clickedPosition = Input.mousePosition;

		// 保存 ShapeHolder 点击时的世界位置
		shapeClickedPosition = currentShapeHolder.transform.position;
	}

	private void HandleDrag()
	{
		// 鼠标当前位置相对点击时的偏移量（屏幕坐标系）
		Vector2 delta = Input.mousePosition - clickedPosition;

		// 归一化处理（把像素转成 0~1 的比例）
		delta.x /= Screen.width;
		delta.y /= Screen.height;

		// 缩放拖拽速度
		delta *= moveSpeed;

		// 目标位置 = 点击时 Shape 的位置 + 鼠标拖动偏移
		Vector2 targetPosition = (Vector2)shapeClickedPosition + delta;

		// 平滑插值（让物体平滑移动而不是瞬移）
		currentShapeHolder.transform.position =
			Vector3.Lerp(
				currentShapeHolder.transform.position,  // 当前实际位置
				targetPosition,                         // 想要到达的位置
				Time.deltaTime * 60f * 0.3f             // 插值速度（帧率独立）
			);
	}


	private void HandleMouseUp()
	{
		Debug.Log("Mouse Up");
		// TODO: 放下逻辑
	}
}
