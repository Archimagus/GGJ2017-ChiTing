using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMusicManager : MonoBehaviour {

	AudioSource source;
	private bool coroutineIsRunning = false;

	void Start () {
		DontDestroyOnLoad(gameObject);
		source = GetComponent<AudioSource>();
		source.volume = 0f;
		StartCoroutine(FadeInMusic(0.9f));
	}
	

	void Update () {
		int activeScene = SceneManager.GetActiveScene().buildIndex;

		// If not single player or multiplayer scenes, fade out and then destroy
		if(activeScene != 2 && activeScene != 3) {
			if(!coroutineIsRunning) {
				StartCoroutine(FadeOutMusic());
			}
		}
	}

	IEnumerator FadeInMusic(float volume) {
		while (source.volume < volume) {
			source.volume += 0.1f * Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator FadeOutMusic() {

		coroutineIsRunning = true;

		while (source.volume > 0) {
			source.volume -= 0.5f * Time.deltaTime;
			yield return null;
		}

		yield return new WaitForSeconds(1.1f);

		DestroyImmediate(gameObject);
	}
}
