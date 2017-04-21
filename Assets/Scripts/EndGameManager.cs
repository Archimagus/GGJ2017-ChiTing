using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{

	public GameObject backgroundMusic;
	[SerializeField]
	private Text text;

	void Awake()
	{
		Instantiate(backgroundMusic);
		text.CrossFadeAlpha(0f, 0f, false);

		StartCoroutine(TextFade());
	}


	IEnumerator TextFade()
	{
		text.CrossFadeAlpha(5, 3f, false);
		yield return new WaitForSeconds(6);
		text.CrossFadeAlpha(0f, 2.5f, false);
		yield return new WaitForSeconds(2.5f);
		SceneManager.LoadScene(6);
	}
}
