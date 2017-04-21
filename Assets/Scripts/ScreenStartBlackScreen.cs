using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenStartBlackScreen : MonoBehaviour {

	public Image blackscreen;

	void Start() {
		blackscreen = GetComponentInChildren<Image>();
		StartCoroutine(FadeOut());
	}

	IEnumerator FadeOut() {

		yield return new WaitForSeconds(1f);

		if(blackscreen != null) {
			blackscreen.CrossFadeAlpha(0f, 3f, false);
		}

		yield return new WaitForSeconds(3f);

		gameObject.SetActive(false);
	}
}
