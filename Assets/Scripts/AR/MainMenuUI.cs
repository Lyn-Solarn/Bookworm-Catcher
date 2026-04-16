using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class myMainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;


    // These are just on-click listeners that call the scene loader
    // We can add and subtract them as needed
    private void Awake() {
        playButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });

        Time.timeScale = 1f;
    }
}