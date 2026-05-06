using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    [SerializeField] private float SceneTransitionTimer = 30f;

    // Update is called once per frame
    void Update()
    {
        SceneTransitionTimer -= Time.deltaTime;

        if (SceneTransitionTimer <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
