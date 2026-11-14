using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string sceneName = "Game";

    public void LoadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}