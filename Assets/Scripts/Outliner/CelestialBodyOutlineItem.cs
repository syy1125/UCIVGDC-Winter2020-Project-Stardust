using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CelestialBodyOutlineItem : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
	public MeshRenderer Renderer;
	private Vector3 _originalScale;

	public float AxialTilt;
	public float Magnification = 1;
	public float RotationRate;

	private bool _hover;

	public UnityEvent OnClick = new UnityEvent();

	private void Start()
	{
		_originalScale = Renderer.transform.localScale;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_hover = true;
		Transform rendererTransform = Renderer.transform;
		rendererTransform.Rotate(Vector3.forward, AxialTilt, Space.Self);
		rendererTransform.localScale *= Magnification;
	}

	private void Update()
	{
		if (_hover)
		{
			Renderer.transform.Rotate(Vector3.down, Time.deltaTime * RotationRate, Space.Self);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnClick.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_hover = false;
		Transform rendererTransform = Renderer.transform;
		rendererTransform.rotation = Quaternion.identity;
		rendererTransform.localScale = _originalScale;
	}
}