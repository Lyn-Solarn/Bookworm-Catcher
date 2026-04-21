using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    //Goes to the first level game scene
   public void StartGame(){
    SceneManager.LoadSceneAsync(3);
   }

    //Goes to the options menu scene
   public void OpenOptions(){
    SceneManager.LoadSceneAsync(2);
   }

    //Closes the game
   public void QuitGame(){
    Application.Quit();
   }
}
