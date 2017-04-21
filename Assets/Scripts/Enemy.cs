using UnityEngine;

public class Enemy : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			other.GetComponent<PlayerChi>().Die();
		}
	}
}
