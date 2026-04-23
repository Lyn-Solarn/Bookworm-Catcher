using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private int firstLevelSceneIndex = 4;
    [SerializeField] private int optionsSceneIndex = 2;

    //Goes to the first level game scene
   public void StartGame(){
    SceneManager.LoadSceneAsync(firstLevelSceneIndex);
   }

    //Goes to the options menu scene
   public void OpenOptions(){
    SceneManager.LoadSceneAsync(optionsSceneIndex);
   }

    //Closes the game
   public void QuitGame(){
    Application.Quit();
   }
}
