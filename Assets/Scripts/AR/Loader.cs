using UnityEngine.SceneManagement;

public static class Loader {


    // List all of the in use scenes here
    public enum Scene {
        MainMenuScene,
        GameScene,
        LoadingScene, 
        ARTest
    }


    private static Scene targetScene;



    public static void Load(Scene targetScene) {
        Loader.targetScene = targetScene;

        // Can chnage this to any scene we use for loading
        SceneManager.LoadScene(Scene.ARTest.ToString());
    }

    // Later we can implement LoaderCallback()
    
    // public static void LoaderCallback() {
    //     SceneManager.LoadScene(targetScene.ToString());
    // }

}