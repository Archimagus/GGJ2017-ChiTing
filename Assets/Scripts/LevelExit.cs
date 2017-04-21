using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LevelExit : NetworkBehaviour
{
	public string NextScene = "EndScene";
	private int _playerCount;
	private int _playersAtExit;

	public override void OnStartClient()
	{
		base.OnStartClient();
		_playerCount++;
	}


	void Start()
	{
		_playersAtExit = 0;
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			_playersAtExit++;
			if (_playersAtExit == _playerCount)
			{
				var lobby = FindObjectOfType<LobbyManager>();
				if (lobby != null)
				{
					lobby.ServerChangeScene(NextScene);
					return;
				}
				var networkManager = FindObjectOfType<NetworkManager>();
				if (networkManager != null && networkManager.isActiveAndEnabled)
				{
					networkManager.StopServer();
					networkManager.StopClient();
					networkManager.StopHost();
					Destroy(networkManager.gameObject);
				}
				SceneManager.LoadScene(NextScene);
			}
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			_playersAtExit--;
		}
	}
}
