using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
	private PauseManager pauseManager;

	public float Speed = 3.0f;

	[SerializeField]
	float horizontalMovement = 0.0f;
	[SerializeField]
	float verticalMovement = 0.0f;

	Rigidbody playerRigidbody;
	private Animator animator;

	public AudioClip footstep;
	[SerializeField]
	private AudioSource source;
	[SerializeField]
	private int stepCount = 1;

	private bool _dyeing;
	[SyncVar]
	private int _mapSeed = -1;


	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		ObjectManager.GetInstance().player = gameObject;
		var pos = FindObjectsOfType<NetworkStartPosition>();
		if (isServer)
			transform.position = pos[0].transform.position;
		else
			transform.position = pos[1].transform.position;
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		FindObjectOfType<MazeGenerator3D>().GenerateMaze(_mapSeed);
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		_mapSeed = Random.Range(int.MinValue, int.MaxValue);
	}

	void Start()
	{
		playerRigidbody = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		source = GetComponent<AudioSource>();
		pauseManager = ObjectManager.GetInstance().pauseManager;
	}

	void Update()
	{

		if (isLocalPlayer)
		{
			if (pauseManager != null && !pauseManager.isPaused && !_dyeing)
			{
				horizontalMovement = Input.GetAxis("Horizontal");
				verticalMovement = Input.GetAxis("Vertical");

				if (playerRigidbody.velocity.sqrMagnitude > 0)
				{
					Vector3 moveDirection = playerRigidbody.velocity;
					moveDirection.y = 0;
					Quaternion quaternion = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection), 75);

					transform.rotation = quaternion;
				}
			}
		}
		animator.SetFloat("Speed", playerRigidbody.velocity.magnitude);
	}
	void FixedUpdate()
	{
		if (isLocalPlayer)
		{
			var velocity = new Vector3(Speed * horizontalMovement * Time.fixedDeltaTime, 0,
				Speed * verticalMovement * Time.fixedDeltaTime);
			if (pauseManager != null && !pauseManager.isPaused)
			{
				playerRigidbody.AddForce(velocity, ForceMode.VelocityChange);
			}
		}
	}

	public void PlayFootstep()
	{
		if (stepCount == 1)
		{
			stepCount = 2;
			source.pitch = 1.2f;
		}
		else
		{
			stepCount = 1;
			source.pitch = 1.0f;
		}
		source.PlayOneShot(footstep);
	}
	public void Die()
	{
		_dyeing = true;
	}
}
