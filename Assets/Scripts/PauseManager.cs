using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{

	public ControllerManager controllerManager;
	public GameObject pauseCanvas;

	[SerializeField]
	private bool controllerHidesCursor = false;

	private List<Button> buttons = new List<Button>();

	public bool isPaused;

	public int normalFontSize = 18;

	public int selectedFontSize = 22;
	[SerializeField]
	private int currentButton = 0;
	private bool selectingInitialControllerButton = false;

	private NetworkManager networkManager;
	private LobbyManager lobbyManager;

	void Start()
	{
		StartCoroutine(Initialize());
	}


	void Update()
	{
		// Controller
		Controller();

		// Keyboard
		Keyboard();
	}

	private void Controller()
	{
		if (controllerManager != null && controllerManager.controllerDeviceDetected)
		{
			if (Input.GetButtonDown("Xbox_StartButton") && !isPaused)
			{
				isPaused = true;
				currentButton = 0;
				pauseCanvas.SetActive(true);

				StartCoroutine(SelectInitialButton());
			}

			if (controllerHidesCursor)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}

			if (isPaused)
			{
				ControllerNavigation();
			}
		}
	}

	private void ControllerNavigation()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			if (i.Equals(currentButton))
			{
				buttons[i].GetComponent<Text>().fontSize = selectedFontSize;
			}
			else
			{
				buttons[i].GetComponent<Text>().fontSize = normalFontSize;
			}
		}

		if (Input.GetAxis("Vertical") < -0.25f && currentButton < 1)
		{
			currentButton++;
		}
		else if (Input.GetAxis("Vertical") > 0.25f && currentButton > 0)
		{
			currentButton--;
		}

		if (!selectingInitialControllerButton)
		{
			buttons[currentButton].Select();
		}

		if (Input.GetButtonDown("Xbox_AButton"))
		{
			buttons[currentButton].onClick.Invoke();
		}
	}

	private void Keyboard()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
		{
			isPaused = true;
			pauseCanvas.SetActive(true);
		}
		else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
		{
			isPaused = false;
			pauseCanvas.SetActive(false);
		}
	}

	public void Resume()
	{
		isPaused = false;
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].GetComponent<Text>().fontSize = normalFontSize;
		}

		if (lobbyManager != null && lobbyManager.isActiveAndEnabled)
		{
			LobbyTopPanel topPanel = FindObjectOfType<LobbyTopPanel>();
			topPanel.ToggleVisibility(false);
		}

		pauseCanvas.SetActive(false);
	}

	public void Quit(int quitToScene = 0)
	{
		// Player wasn't spawning when in game session, quitting and then starting another
		// game session. 
		if (lobbyManager != null && lobbyManager.isActiveAndEnabled)
		{
			//lobbyManager.GoBackButton();
		}
		else
		{
			if (networkManager != null && networkManager.isActiveAndEnabled)
			{
				networkManager.StopServer();
				networkManager.StopClient();
				networkManager.StopHost();
				Destroy(networkManager.gameObject);
			}

			SceneManager.LoadScene(quitToScene);
		}
	}

	public void MouseEnter(BaseEventData eventData)
	{
		PointerEventData pointerEventData = (PointerEventData)eventData;
		Button button = pointerEventData.pointerEnter.GetComponent<Button>();

		if (button.interactable)
		{
			Text buttonText = button.GetComponent<Text>();
			buttonText.fontSize = selectedFontSize;
		}
	}

	public void MouseExit(BaseEventData eventData)
	{
		PointerEventData pointerEventData = (PointerEventData)eventData;
		Button button = pointerEventData.pointerEnter.GetComponent<Button>();

		if (button.interactable)
		{
			Text buttonText = button.GetComponent<Text>();
			buttonText.fontSize = normalFontSize;
		}
	}

	IEnumerator Initialize()
	{
		yield return new WaitForSeconds(0.1f);

		controllerManager = ObjectManager.GetInstance().controllerManager;
		pauseCanvas = ObjectManager.GetInstance().pauseCanvas;

		Button[] btn = pauseCanvas.GetComponentsInChildren<Button>();

		foreach (Button button in btn)
		{
			buttons.Add(button);
		}

		yield return new WaitForSeconds(0.1f);

		networkManager = FindObjectOfType<NetworkManager>();
		lobbyManager = FindObjectOfType<LobbyManager>();

		// Do not show the quit button in pause menu when in multiplayer or
		// play and host sessions. Player must use the back button on the lobby
		// manager panel.
		if (lobbyManager != null && lobbyManager.isActiveAndEnabled)
		{
			buttons[1].gameObject.SetActive(false);
		}

		pauseCanvas.SetActive(false);
	}

	IEnumerator SelectInitialButton()
	{
		selectingInitialControllerButton = true;

		yield return new WaitForSeconds(0.25f);

		buttons[currentButton].Select();
		selectingInitialControllerButton = false;
	}
}
