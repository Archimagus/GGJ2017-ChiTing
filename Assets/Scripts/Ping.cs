using UnityEngine;

public class Ping : MonoBehaviour
{
	Light propagatedLight;

	public AnimationCurve falloffCurve;

	public float maxPingStrength = 1.0f;
	public float lightRange = 10.0f;

	[SerializeField]
	private float maxLightIntensity = 1.0f;
	[SerializeField]
	private float maxPingTime = 5.0f;
	[SerializeField]
	private float pingPropagationRate = 0.05f;
	private float lightStrength = 0.0f;
	[SerializeField]
	private float elapsedTime = 0.0f;
	[SerializeField]
	private float endTime = 0.0f;

	private float strengthOfPing = 0.0f;
	private bool conductPing = false;

	private AudioSource pingSource;
	public AudioClip pingSound;

	void Awake()
	{
		pingSource = GetComponent<AudioSource>();
	}

	void Start()
	{
		propagatedLight = GetComponent<Light>();

		propagatedLight.range = 0.0f;
		propagatedLight.intensity = 0.0f;
	}

	void Update()
	{
		if (conductPing)
		{
			ConductPing(strengthOfPing);
		}

		//Destroy(gameObject, endTime);
		Destroy(gameObject, maxPingTime);
	}

	public void SetupPing(float strength)
	{
		strengthOfPing = strength;
		conductPing = true;

		if (pingSource != null)
		{
			pingSource.volume = strength;
			pingSource.PlayOneShot(pingSound);
		}
		else
		{
			Debug.Log("Audio source not found on this game object.");
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
			propagatedLight.range += (0.1f + (1 * pingPropagationRate));
		}

	}

}
