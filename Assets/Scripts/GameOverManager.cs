using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{

	public Text text;
	public GameObject backgroundMusic;

	void Awake()
	{
		Instantiate(backgroundMusic);

		StartCoroutine(TextFade());
	}


	IEnumerator TextFade()
	{

		yield return new WaitForSeconds(3f);

		text.CrossFadeAlpha(0f, 3, false);

		yield return new WaitForSeconds(3);

		SceneManager.LoadScene(6);
	}
}
