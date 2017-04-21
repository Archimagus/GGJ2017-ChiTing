using UnityEngine;

public class ControllerManager : MonoBehaviour
{

	public bool controllerDeviceDetected = false;

	private string xboxOneString = "Controller (Xbox One For Windows)";
	private string xbox360String = "Controller (XBOX 360 For Windows)";

	void Update()
	{
		CheckForConnectedController();
	}

	private void CheckForConnectedController()
	{

		if (Input.GetJoystickNames().Length > 0)
		{
			for (int i = 0; i < Input.GetJoystickNames().Length; i++)
			{
				if (Input.GetJoystickNames()[i].ToString().Equals(xbox360String) ||
						Input.GetJoystickNames()[i].ToString().Equals(xboxOneString))
				{

					controllerDeviceDetected = true;
					break;
				}
				else
				{
					controllerDeviceDetected = false;
				}
			}
		}
		else
		{
			controllerDeviceDetected = false;
		}

	}
}
