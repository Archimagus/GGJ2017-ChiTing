using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{

	public Text creditText;
	public GameObject backgroundMusic;

	private string credit;

	void Awake()
	{
		EndGameMusicManager manager = FindObjectOfType<EndGameMusicManager>();

		if (manager == null)
		{
			Instantiate(backgroundMusic);
		}
	}

	void Start()
	{
		creditText.CrossFadeAlpha(0f, 0f, false);

		StartCoroutine(RollTheCredits());
	}

	void Update()
	{
		creditText.text = credit;
	}

	public void ReturnToMain()
	{
		var NextScene = "Main Menu";
		var lobby = FindObjectOfType<LobbyManager>();
		if (lobby != null)
		{
			lobby.GoBackButton();
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

	IEnumerator RollTheCredits()
	{

		// Creators of the game

		credit = "Created By";
		creditText.CrossFadeAlpha(5f, 2f, false);

		yield return new WaitForSeconds(4);

		creditText.CrossFadeAlpha(0f, 2f, false);

		yield return new WaitForSeconds(2);

		credit = "Alex Thompson: Art";
		creditText.CrossFadeAlpha(5f, 2f, false);

		yield return new WaitForSeconds(4);

		creditText.CrossFadeAlpha(0f, 2f, false);

		yield return new WaitForSeconds(2);

		credit = "Daniel Acri: Programming";
		creditText.CrossFadeAlpha(5f, 2f, false);

		yield return new WaitForSeconds(4);

		creditText.CrossFadeAlpha(0f, 2f, false);

		yield return new WaitForSeconds(2);

		credit = "Jesse Pascoe: Programming";
		creditText.CrossFadeAlpha(5f, 2f, false);

		yield return new WaitForSeconds(4);

		creditText.CrossFadeAlpha(0f, 2f, false);

		yield return new WaitForSeconds(2);

		credit = "Micah Kuch: UI, Audio, and Design";
		creditText.CrossFadeAlpha(5f, 2f, false);

		yield return new WaitForSeconds(4);

		creditText.CrossFadeAlpha(0f, 2f, false);

		yield return new WaitForSeconds(2);

		// Asset Credits

		creditText.alignment = TextAnchor.MiddleLeft;
		credit = @"Special Thanks to Kevin MacLeod

				'Dreams Become Real' Kevin MacLeod (incompetech.com) Licensed under Creative Commons: By Attribution 3.0 License http://creativecommons.org/licenses/by/3.0/

				'Ritual' Kevin MacLeod (incompetech.com)Licensed under Creative Commons: By Attribution 3.0 License http://creativecommons.org/licenses/by/3.0/

				'Ossuary 7 - Resolve' Kevin MacLeod (incompetech.com) Licensed under Creative Commons: By Attribution 3.0 License http://creativecommons.org/licenses/by/3.0/";

		creditText.CrossFadeAlpha(5f, 2f, false);

		yield return new WaitForSeconds(4);

		creditText.CrossFadeAlpha(0f, 2f, false);

		yield return new WaitForSeconds(2);

		credit = "Global Game Jam 2017: Harrisburg, PA";
		creditText.fontSize = 34;
		creditText.alignment = TextAnchor.MiddleCenter;
		creditText.CrossFadeAlpha(5f, 2f, false);

		yield return new WaitForSeconds(4);

		creditText.CrossFadeAlpha(0f, 2f, false);

		yield return new WaitForSeconds(2);

		credit = "Thank You!";
		creditText.CrossFadeAlpha(5f, 2f, false);

		yield return new WaitForSeconds(4);

		creditText.CrossFadeAlpha(0f, 2f, false);
	}
}
