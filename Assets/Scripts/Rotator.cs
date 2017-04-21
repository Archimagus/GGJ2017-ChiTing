using UnityEngine;

[ExecuteInEditMode]
public class Rotator : MonoBehaviour
{
	public Vector3 rotationAmmount;
	void Update()
	{
		transform.Rotate(Vector3.up, rotationAmmount.y * Time.deltaTime);
		transform.Rotate(Vector3.right, rotationAmmount.x * Time.deltaTime);
		transform.Rotate(Vector3.forward, rotationAmmount.z * Time.deltaTime);
	}
}
