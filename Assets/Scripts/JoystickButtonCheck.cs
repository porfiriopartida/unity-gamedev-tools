using UnityEngine;

public class JoystickButtonCheck : MonoBehaviour
{
    public string joystickName = "joystick";
    public string buttonSuffix = "button";
    public string format = "{0} {1} {2}";
    public int btns = 19;
    void Start()
    {
        var joystickNames = Input.GetJoystickNames();
        foreach (var oJoystickName in joystickNames)
        {
            Debug.Log("Found joystick: " + oJoystickName);
        }
    }

    void Update()
    {
        for (var i = 0; i < btns; i++)
        {
            var btnCode = string.Format(format, joystickName, buttonSuffix, i);
            if (Input.GetKeyDown(btnCode))
            {
                Debug.Log(btnCode);
            }
        }
    }
}
