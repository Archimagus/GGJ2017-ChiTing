using UnityEngine;

public class WebPlayerToggle : MonoBehaviour
{
	public bool _enabledForWebPlayer = false;
	// Use this for initialization
	void Awake()
	{
		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			gameObject.SetActive(_enabledForWebPlayer);
		}
		else
		{
			gameObject.SetActive(!_enabledForWebPlayer);
		}
	}

}
