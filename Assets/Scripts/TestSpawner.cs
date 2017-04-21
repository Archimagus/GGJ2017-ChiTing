using UnityEngine;
using UnityEngine.Networking;

/**
* The purpose of this class is to start the host immediately upon
* loading of the scene which will then spawn the player.
**/
public class TestSpawner : MonoBehaviour
{
	void Start()
	{
		NetworkManager.singleton.StartHost();
	}
}