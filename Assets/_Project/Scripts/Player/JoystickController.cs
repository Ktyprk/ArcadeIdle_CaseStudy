using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour,  IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform rectTransform;

    [SerializeField] private Joystick joystick;
    [SerializeField] private RectTransform joystickBackground;

    public static float ScaleFactor { get; private set; }

    [SerializeField] private bool showJoystick = true;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        ScaleFactor = transform.parent.GetComponent<Canvas>().scaleFactor;
        joystickBackground.gameObject.SetActive(showJoystick);
    }

    public void OnPointerDown(PointerEventData pointData)
    {
        joystickBackground.gameObject.SetActive(showJoystick);
    
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            pointData.position,
            pointData.pressEventCamera,
            out Vector2 localPoint
        );

        joystickBackground.localPosition = localPoint;
    }

    public void OnPointerUp(PointerEventData pointData)
    {
        joystickBackground.gameObject.SetActive(false);

        joystick.ResetStick();
    }
    
    public void OnDrag(PointerEventData pointData)
    {
        joystick.MoveStick(pointData.position);
    }
}
