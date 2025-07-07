using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public void RestartScene()
    {
        SanityManager.Instance.SetSanity(SanityManager.Instance.GetMaxSanity());
        Time.timeScale = 1f; // Unpause
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
