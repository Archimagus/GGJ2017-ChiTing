using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusicSpawner : MonoBehaviour {

	AudioSource source;
	private bool coroutineIsRunning = false;
	private bool newGame = false;

	void Awake() {
		DontDestroyOnLoad(gameObject);
		source = GetComponent<AudioSource>();
		source.volume = 0;
		StartCoroutine(FadeInMusic(0.2f));
	}

	void Update () {
		int activeScene = SceneManager.GetActiveScene().buildIndex;

		if(activeScene == 0) {
			newGame = true;
		}

		if(newGame) {
			if (activeScene != 0 && activeScene != 1) {
				if (!coroutineIsRunning) {
					StartCoroutine(FadeOutMusic());
				}
			}
		} else {
			if (activeScene != 0 && activeScene != 1 && activeScene != 2) {
				if (!coroutineIsRunning) {
					StartCoroutine(FadeOutMusic());
				}
			}
		}
	}

	IEnumerator FadeInMusic(float volume) {
		while (source.volume < volume) {
			source.volume += 0.05f * Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator FadeOutMusic() {

		coroutineIsRunning = true;

		while(source.volume > 0) {
			source.volume -= 0.15f * Time.deltaTime;
			yield return null;
		}

		yield return new WaitForSeconds(1.1f);

		DestroyImmediate(gameObject);
	}
}
