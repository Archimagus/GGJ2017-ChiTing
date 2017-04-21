using UnityEngine;

[RequireComponent(typeof(Light))]
public class ChiLight : MonoBehaviour
{
	[SerializeField]
	private float _minLightValue;
	[SerializeField]
	private float _maxLightValue;
	private PlayerChi _chi;
	private Light _light;
	// Use this for initialization
	void Start()
	{
		_chi = GetComponentInParent<PlayerChi>();
		_light = GetComponent<Light>();
	}

	// Update is called once per frame
	void Update()
	{
		_light.intensity = Mathf.Lerp(_minLightValue, _maxLightValue, _chi.ChiAmount / _chi.MaxChiAmount);
	}
}
