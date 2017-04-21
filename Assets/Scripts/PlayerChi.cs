using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerChi : NetworkBehaviour
{

	Rigidbody playerRigidbody;
	ControllerManager controllerManager;
	PauseManager pauseManager;

	private Material playerMaterial;

	public GameObject pingPrefab;
	public AudioClip deathSound;

	[SerializeField]
	private float maxChiAmount = 5.0f;
	public float MaxChiAmount { get { return maxChiAmount; } }
	[SerializeField]
	[SyncVar]
	private float _chiAmount = 0.0f;
	public float ChiAmount { get { return _chiAmount; } }
	[Command]
	void CmdAddChi(float val)
	{
		_chiAmount += val;
	}
	[Command]
	void CmdSetChi(float val)
	{
		_chiAmount = val;
	}
	//[ClientRpc]
	//void RpcSetChi(float val)
	//{
	//	_chiAmount = val;
	//}
	[SerializeField]
	private float pingDelay = 1.0f;
	[SerializeField]
	public float maxPingStrength = 1.0f;

	private bool delayIsActive = false;

	[SerializeField]
	private float pingChargeRate = 0.01f;

	private bool isChargingPing = false;
	private bool conductingPing = false;
	private float strengthOfPing = 0.0f;

	private float movementVelocity = 0.0f;
	[SerializeField]
	private float chiRechargeMultiplier = 0.001f;

	[SerializeField]
	private float controllerFlick = 0.0f;

	private SpriteRenderer playerRenderer;
	private MaterialPropertyBlock playerRendererPropertyBlock;
	private Animator animator;
	private bool _dyeing;

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		if (isLocalPlayer)
			CmdSetChi(maxChiAmount);
	}


	void Start()
	{
		playerRenderer = GetComponentInChildren<SpriteRenderer>();
		if (playerRenderer == null)
			Debug.LogError("No player sprite renderer", this);
		playerRendererPropertyBlock = new MaterialPropertyBlock();
		animator = GetComponent<Animator>();
		playerRigidbody = GetComponent<Rigidbody>();
		StartCoroutine(Initialize());
	}

	void Update()
	{
		if (isLocalPlayer && !_dyeing)
		{
			if (pauseManager != null && !pauseManager.isPaused)
			{
				// Controller
				ControllerHandler();

				// Keyboard
				KeyboardHandler();
			}

			if (playerRigidbody != null)
			{
				movementVelocity = playerRigidbody.velocity.sqrMagnitude;

				if (movementVelocity > 0)
				{
					RechargeChi();
				}
			}

			if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
				CmdSetChi(maxChiAmount);
		}
		UpdateRenderer();
	}

	private void UpdateRenderer()
	{
		playerRenderer.GetPropertyBlock(playerRendererPropertyBlock);
		playerRendererPropertyBlock.SetFloat("_EmmissivePower", _chiAmount / MaxChiAmount);
		playerRenderer.SetPropertyBlock(playerRendererPropertyBlock);
		animator.SetFloat("Chi", _chiAmount / MaxChiAmount);
	}
	/**
	* This function detects the right thumbstick positive X-axis flick
	* to emit an echo ping when an xbox controller is detected.
	**/
	private void ControllerHandler()
	{
		if (controllerManager != null && controllerManager.controllerDeviceDetected)
		{
			if (!conductingPing)
			{
				controllerFlick = Input.GetAxis("Xbox_RightThumbstick_X");
			}

			if (controllerFlick > 0.1f && !delayIsActive && _chiAmount > 0)
			{
				if (strengthOfPing < _chiAmount)
				{
					conductingPing = true;
					spawn(controllerFlick);
				}
				else
				{
					conductingPing = true;
					spawn(_chiAmount);
				}
			}
		}
	}

	/**
	* This function charges an echo ping when space bar is pressed
	* and emits the ping when strength is greater than 0.15 (this is
	* to avoid meaningless quick flashes of light).
	**/
	private void KeyboardHandler()
	{
		if (Input.GetKey(KeyCode.Space) && !conductingPing)
		{
			isChargingPing = true;
		}
		else
		{
			isChargingPing = false;
		}

		if (isChargingPing)
		{
			ChargePing();
		}
		else
		{
			if (_chiAmount > 0 && strengthOfPing > 0.16f)
			{
				conductingPing = true;
				if (_chiAmount > 0 && strengthOfPing < _chiAmount)
				{
					spawn(strengthOfPing);
				}
				else
				{
					spawn(_chiAmount);
				}
			}
		}
	}

	/**
	* This function increases the strength of the ping
	* as the space key is being held.
	**/
	private void ChargePing()
	{
		if (strengthOfPing < maxPingStrength)
		{
			strengthOfPing += pingChargeRate;
		}
	}

	/**
	* This function spawns an ping at the current player location
	* and passes in the ping strength from this class to the ping class
	* on the ping prefab that was instantiated.
	**/
	[ClientRpc]
	private void RpcSpawn(float pingStrength)
	{
		Vector3 position = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);

		GameObject pingObject = Instantiate(pingPrefab, position, transform.rotation);
		Ping ping = pingObject.GetComponent<Ping>();
		ping.SetupPing(pingStrength);

		StartCoroutine(Delay());
	}
	[Command]
	private void CmdSpawn(float pingStrength)
	{
		if (isServer)
			RpcSpawn(pingStrength);
	}
	private void spawn(float pingStrength)
	{
		CmdSpawn(pingStrength);
		CmdAddChi(-pingStrength);
		delayIsActive = true;
		conductingPing = false;
		strengthOfPing = 0.15f;
	}
	/**
	* This function recharges the chi amount while the player
	* has a velocity greater than zero and a chi amount of less
	* than or equal to the max chi amount.
	**/
	private void RechargeChi()
	{
		if (_chiAmount < maxChiAmount)
		{
			CmdAddChi(movementVelocity * chiRechargeMultiplier);
		}
	}

	public void Die()
	{
		if (isLocalPlayer)
		{
			GetComponent<PlayerMovement>().Die();
			CmdDie();
		}
	}
	[Command]
	private void CmdDie()
	{
		if (isServer)
			RpcDie();
	}
	[ClientRpc]
	private void RpcDie()
	{
		_dyeing = true;
		GetComponent<Animator>().SetTrigger("Death");
		GetComponent<AudioSource>().PlayOneShot(deathSound);
	}
	public void DeathAnimFinished()
	{
		string NextScene = "GameOver";
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

	/**
	* This coroutine creates a delay which is used for controller flicking.
	* It prevents the code from emitting multiple ping instances from a single
	* flick. When delayIsActive returns to false, the player can emit another ping
	* using the left thumbstick flick.
	**/
	IEnumerator Delay()
	{
		controllerFlick = 0.0f;

		yield return new WaitForSeconds(pingDelay);

		delayIsActive = false;
	}

	/**
	* This coroutine initializes components referenced in this class but are not
	* attached to the player gameobject.
	**/
	IEnumerator Initialize()
	{
		yield return new WaitForSeconds(0.5f);
		controllerManager = ObjectManager.GetInstance().controllerManager;
		pauseManager = ObjectManager.GetInstance().pauseManager;
	}
}
