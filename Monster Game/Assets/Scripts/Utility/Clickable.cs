using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Clickable : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    [SerializeField]
    private float DoubleClickDelay = 0.25f;

    public UnityAction<PointerEventData> OnClick;
    public UnityAction<PointerEventData> OnClickDown;
    public UnityAction<PointerEventData> OnClickUp;
    public UnityAction<PointerEventData> OnDoubleClick;

    private bool _firstClick { get; set; }
    private float _lastClickTime { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClick != null)
        {
            OnClick(eventData);
        }
        CheckDoubleClick(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnClickDown != null)
        {
            OnClickDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (OnClickUp != null)
        {
            OnClickUp(eventData);
        }
    }

    public void CheckDoubleClick(PointerEventData eventData)
    {
        if (OnDoubleClick == null)
            return;

        if (_firstClick && (Time.time - _lastClickTime) > DoubleClickDelay)
        {
            _firstClick = false;
        }

        if (!_firstClick)
        {
            _firstClick = true;
            _lastClickTime = Time.time;
        }
        else
        {
            _firstClick = false;
            OnDoubleClick(eventData);
        }
    }
}
