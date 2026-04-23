using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PausedMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausedMenu;
    [SerializeField] private int mainMenuSceneIndex = 0;
    [SerializeField] private int optionsSceneIndex = 8;
    private bool hasShownMissingReferenceWarning;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pausedMenu == null)
            {
                if (!hasShownMissingReferenceWarning)
                {
                    Debug.LogWarning("PausedMenu is not assigned in the Inspector on PausedMenu.");
                    hasShownMissingReferenceWarning = true;
                }

                return;
            }

            pausedMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    //Resumes the game
    public void ResumeGame(){
        if (pausedMenu == null)
        {
            if (!hasShownMissingReferenceWarning)
            {
                Debug.LogWarning("PausedMenu is not assigned in the Inspector on PausedMenu.");
                hasShownMissingReferenceWarning = true;
            }

            return;
        }

        pausedMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    
    //Goes to the main menu scene
   public void OpenMainMenu(){
    Time.timeScale = 1f;
    SceneManager.LoadSceneAsync(mainMenuSceneIndex);
   }

    //Goes to the options menu scene
   public void OpenSettings(){
    SceneManager.LoadSceneAsync(optionsSceneIndex);
   }

    //Closes the game
   public void QuitGame(){
    Application.Quit();
   }
}
