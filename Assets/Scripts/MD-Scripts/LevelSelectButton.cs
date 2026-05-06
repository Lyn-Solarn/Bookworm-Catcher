using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private int sceneBuildIndex;

    public void LoadSelectedLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(sceneBuildIndex);
    }
}
