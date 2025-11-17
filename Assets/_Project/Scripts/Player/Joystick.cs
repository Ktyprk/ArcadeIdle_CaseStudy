using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Joystick : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private RectTransform joystickBackground;

    public Vector2 Value;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void MoveStick(Vector2 position)
    {
        if(rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        Vector2 localPos = position - (Vector2)joystickBackground.position;
        localPos *= JoystickController.ScaleFactor;
        
        Vector2 scaledValue = localPos / (rectTransform.sizeDelta * 0.5f);
        Value = Vector2.ClampMagnitude(scaledValue, 1f);

        rectTransform.localPosition = Value * (rectTransform.sizeDelta * 0.5f);
    }

    public void ResetStick()
    {
        if(rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        rectTransform.localPosition = Vector2.zero;
        Value = Vector2.zero;
    }
}
