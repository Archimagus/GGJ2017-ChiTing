using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToStart : MonoBehaviour
{

    public void ReturnToStartMenu()
    {
        SceneManager.LoadScene(0);
    }

}