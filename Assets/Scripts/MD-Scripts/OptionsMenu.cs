using UnityEngine;
using UnityEngine.SceneManagement;


public class OptionsMenu : MonoBehaviour
{

    //Exits the options menu and goes back to the main menu
   public void BackToMainMenu(){
    SceneManager.LoadSceneAsync(0);
   }
}
