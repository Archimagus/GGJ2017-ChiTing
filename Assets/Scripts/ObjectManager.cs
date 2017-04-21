using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectManager : MonoBehaviour
{

	private static ObjectManager om;

	public GameObject player;
	public ControllerManager controllerManager;
	public GameObject pauseCanvas;
	public PauseManager pauseManager;

	public GameObject levelMusic;

	void Awake()
	{
		pauseCanvas = GameObject.Find("PauseCanvas");
		pauseManager = GetComponent<PauseManager>();

		Instantiate(levelMusic);
	}

	void Start()
	{
		controllerManager = GetComponent<ControllerManager>();
	}

	void OnDestroy()
	{
		player = null;
		controllerManager = null;
		pauseCanvas = null;
		pauseManager = null;
		levelMusic = null;
		om = null;
	}


	public static ObjectManager GetInstance()
	{
		if (om == null)
		{
			om = FindObjectOfType<ObjectManager>();
			if (om == null)
			{
				Debug.LogError("No Game manager in scene" + SceneManager.GetActiveScene().name);
			}
		}
		return om;
	}
}
