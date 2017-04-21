using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
	public enum ChangeType
	{
		NextScene,
		Number,
		Name,
	}

	public ChangeType SceneChangeType = ChangeType.NextScene;
	public int SceneIndex;
	public string SceneName;

	public void ChangeScene()
	{
		switch (SceneChangeType)
		{
			case ChangeType.NextScene:
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
				break;
			case ChangeType.Number:
				SceneManager.LoadScene(SceneIndex);
				break;
			case ChangeType.Name:
				SceneManager.LoadScene(SceneName);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}
