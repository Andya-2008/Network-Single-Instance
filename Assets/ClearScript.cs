using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClearScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] GameObject gameController;
    public bool isPressed;
    private float whenPressed = 0;

    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        if (whenPressed > 0 && isPressed) {
            float elapsedTime = Time.time - whenPressed;
            if (elapsedTime >= 2) {

                whenPressed = 0;
                isPressed = false;
                gameController.GetComponent<GameControllerScript>().ClearList();
            }
        }
    }
    public void OnPointerDown(PointerEventData data)
    {
        if (whenPressed == 0)
        {
            whenPressed = Time.time;
        }
        isPressed = true;
    }
    public void OnPointerUp(PointerEventData data)
    {
        isPressed = false;
        whenPressed = 0;
    }


}
