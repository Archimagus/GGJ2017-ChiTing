using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

	public GameObject target;
	public float smoothTime = 0.3f;

	private Vector3 velocity = Vector3.zero;

	void Start() {

	}

	void FixedUpdate() {

		if (target != null) {
			Vector3 goalPos = target.transform.position;
			goalPos.y = transform.position.y;
			transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);
		} else {
			target = ObjectManager.GetInstance().player;
        }
	}
}
