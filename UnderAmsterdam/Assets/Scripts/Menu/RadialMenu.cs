using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Fusion.XR.Host;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;
    [SerializeField] private GameObject visualRadialObject;
    [SerializeField] private float maxActiveAngle, minActiveAngle;
    public InputActionProperty input;
    public TextMeshProUGUI text;
    public GameObject selector;

    private int timer = 180;
    private int minutes;
    private int seconds;
    public int selection;
    private void Awake()
    {
        var bindings = new List<string> { "joystick" };
        input.EnableWithDefaultXRBindings(leftBindings: bindings);
    }
    // Start is called before the first frame update
    void Start()
    {
        parentObject = transform.parent.gameObject;
        visualRadialObject = transform.GetChild(0).gameObject;
        selection = 1;
        text.text = "03:00";
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        /////////////////////////// SELECTOR CODE //////////////////////////

        float xInput = input.action.ReadValue<Vector2>().x;
        float yInput = input.action.ReadValue<Vector2>().y;

        if (xInput > 0 && xInput > Mathf.Abs(yInput))
        {
            selector.transform.eulerAngles = transform.eulerAngles + new Vector3(0, 0, 90);
            selection = 2;
        }
        if (yInput > 0 && yInput > Mathf.Abs(xInput))
        {
            selector.transform.eulerAngles = transform.eulerAngles + new Vector3(0, 0, 0);
            selection = 1;
        }
        if (xInput < 0 && xInput < -1 * Mathf.Abs(yInput))
        {
            selector.transform.eulerAngles = transform.eulerAngles + new Vector3(0, 0, 270);
            selection = 4;
        }
        if (yInput < 0 && yInput < -1 * Mathf.Abs(xInput))
        {
            selector.transform.eulerAngles = transform.eulerAngles + new Vector3(0, 0, 180);
            selection = 3;
        }

        if (parentObject.transform.localEulerAngles.z < maxActiveAngle && parentObject.transform.localEulerAngles.z > minActiveAngle)
            visualRadialObject.SetActive(true);
        else
            visualRadialObject.SetActive(false);

        //////////////////////////// TIMER CODE ////////////////////////////
        timer = 180 - (int)Time.time;

        minutes = (int)(timer / 60);
        seconds = timer % 60;

        if (seconds < 10)
        {
            text.text = "0" + minutes.ToString() + ":0" + seconds.ToString();
        }
        else
        {
            text.text = "0" + minutes.ToString() + ":" + seconds.ToString();
        }

        if (timer < 0)
        {
            //TIME IS OVER
        }

    }

}
