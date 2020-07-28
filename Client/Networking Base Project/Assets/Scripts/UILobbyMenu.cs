using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UILobbyMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private RectTransform rectTransform;

	private Vector3 position;

	public bool open;
	public bool opening;
	public bool closing;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	IEnumerator Open()
	{
		opening = true;

		// Wait to finish closing
		if (closing) yield return null;

		TweenUtility.Instance.EaseOut_Transform_QuartX(transform, transform.transform.position.x, 0, 0.5f, 0, () => {

			opening = false;

			// We have opened the menu
			open = true;
		});
	}
	public void OnPointerEnter(PointerEventData eventData)
	{
		// If..
		// 1. We are opining
		// 2. We are open
		// We want to not do anything

		// 3. closing
		// We want to wait for the menu to close, then open it

		// If were already open, do nothing
		if (open || opening) return;

		StartCoroutine(Open());
	}

	IEnumerator Close()
	{
		closing = true;

		// Wait to open finish opining
		while (opening) yield return null;

		float width = rectTransform.rect.width;
		float xPos = width * -0.83f;

		TweenUtility.Instance.EaseOut_Transform_QuartX(transform, transform.position.x, xPos, 0.5f, 0, () => {

			closing = false;

			// We have closed the menu
			open = false;
		});

	}
	public void OnPointerExit(PointerEventData eventData)
	{
		// If..
		// 1. We are closed
		// 2. we are closing
		// We want to do nothing

		// 2. We are opening, we want to wait

		if ( closing) return;

		StartCoroutine(Close());
	}
}
