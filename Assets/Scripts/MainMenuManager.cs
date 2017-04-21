using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

	public GameObject canvas;
	public GameObject mainMenu;
	public GameObject instructionsMenu;

	private GameObject currentPanel;

	List<GameObject> menuList = new List<GameObject>();

	List<Button> mainMenuButtons = new List<Button>();

	[SerializeField]
	private AudioSource source;
	public AudioClip startGameClip;

	[SerializeField]
	private Light propagatedLight;

	[SerializeField]
	private AnimationCurve falloffCurve;

	[SerializeField]
	private float maxLightIntensity = 8.0f;
	[SerializeField]
	private float maxPingTime = 5.0f;
	[SerializeField]
	private float pingPropagationRate = 0.15f;
	[SerializeField]
	private float lightStrength = 0.0f;
	[SerializeField]
	private float elapsedTime = 0.0f;
	[SerializeField]
	private float endTime = 0.0f;

	private bool conductPing = false;

	public GameObject titleSprite;
	public GameObject handSprite;

	private SpriteRenderer titleRenderer;
	private SpriteRenderer handRenderer;

	public GameObject backgroundMusic;

	void Awake()
	{
		MenuMusicSpawner spawner = GameObject.FindObjectOfType<MenuMusicSpawner>();

		if (spawner == null)
		{
			Instantiate(backgroundMusic);
		}

	}

	void Start()
	{
		menuList.Add(mainMenu);
		menuList.Add(instructionsMenu);

		Button[] buttons = mainMenu.GetComponentsInChildren<Button>();
		foreach (Button button in buttons)
		{
			mainMenuButtons.Add(button);
		}

		int count = mainMenuButtons.Count;

		if (Application.isWebPlayer || Application.isMobilePlatform || Application.platform == RuntimePlatform.WebGLPlayer)
		{
			mainMenuButtons[count - 1].gameObject.SetActive(false);
		}

		//if (Application.platform == RuntimePlatform.WebGLPlayer)
		//{
		//	// Disable multiplayer interactable button in web player
		//	mainMenuButtons[1].interactable = false;
		//}

		source = GetComponent<AudioSource>();
		propagatedLight = FindObjectOfType<Light>();

		titleRenderer = titleSprite.GetComponent<SpriteRenderer>();
		handRenderer = handSprite.GetComponent<SpriteRenderer>();

		Color color;
		color = new Color(titleRenderer.color.r, titleRenderer.color.g, titleRenderer.color.b, 0);
		titleRenderer.color = color;
	}

	void Update()
	{
		if (conductPing)
		{
			ConductPing(2);
		}
	}

	public void BackToMain()
	{
		currentPanel = mainMenu;

		UpdateMenu();
	}

	public void NewGame()
	{
		source.PlayOneShot(startGameClip);
		StartCoroutine(StartNewGame());
	}

	public void Multiplayer()
	{
		source.PlayOneShot(startGameClip);
		StartCoroutine(StartMultiplayer());
	}

	public void ShowInstructions()
	{
		currentPanel = instructionsMenu;

		UpdateMenu();
	}

	public void Credits()
	{
		var NextScene = "Credits";
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

	public void Exit()
	{
		Application.Quit();
	}

	private void UpdateMenu()
	{
		for (int i = 0; i < menuList.Count; i++)
		{
			if (menuList[i].Equals(currentPanel))
			{
				menuList[i].SetActive(true);
			}
			else
			{
				menuList[i].SetActive(false);
			}
		}
	}

	private void ConductPing(float pingStrength)
	{
		endTime = pingStrength * maxPingTime;

		if (elapsedTime < endTime)
		{
			elapsedTime += Time.deltaTime;
			float curveValue = elapsedTime / endTime;
			lightStrength = falloffCurve.Evaluate(curveValue) * maxLightIntensity;
			propagatedLight.intensity = lightStrength;
			propagatedLight.range += (0.5f + (1 * pingPropagationRate));

			Color color;
			float alpha = titleRenderer.color.a;
			if (elapsedTime < (endTime / 2))
			{
				color = new Color(titleRenderer.color.r, titleRenderer.color.g, titleRenderer.color.b, alpha += 0.01f);
			}
			else
			{
				color = new Color(titleRenderer.color.r, titleRenderer.color.g, titleRenderer.color.b, alpha -= 0.01f);
			}

			Color color2 = new Color(handRenderer.color.r, handRenderer.color.g, handRenderer.color.b, lightStrength);
			titleRenderer.color = color;
			handRenderer.color = color2;
		}

	}

	IEnumerator StartNewGame()
	{
		canvas.SetActive(false);
		for (int i = 0; i < mainMenuButtons.Count; i++)
		{
			mainMenuButtons[i].gameObject.SetActive(false);
		}
		conductPing = true;
		propagatedLight.intensity = maxLightIntensity;
		handSprite.SetActive(true);

		yield return new WaitForSeconds(6);
		var NextScene = "SinglePlayer";
		var lobby = FindObjectOfType<LobbyManager>();
		if (lobby != null)
		{
			lobby.ServerChangeScene(NextScene);
		}
		else
		{

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

	IEnumerator StartMultiplayer()
	{
		canvas.SetActive(false);
		for (int i = 0; i < mainMenuButtons.Count; i++)
		{
			mainMenuButtons[i].gameObject.SetActive(false);
		}
		conductPing = true;
		propagatedLight.intensity = maxLightIntensity;
		handSprite.SetActive(true);

		yield return new WaitForSeconds(6);
		var NextScene = "Lobby";
		var lobby = FindObjectOfType<LobbyManager>();
		if (lobby != null)
		{
			lobby.ServerChangeScene(NextScene);
		}
		else
		{

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
