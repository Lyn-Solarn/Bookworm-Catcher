using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelParser : MonoBehaviour
{
    public TextAsset levelFile;
    public Transform levelRoot;

    [Header("Prefabs")]
    public GameObject basePrefab;

    void Start()
    {
        LoadLevel();
    }

    // void Update()
    // {
    //     if (Keyboard.current.rKey.wasPressedThisFrame)
    //         ReloadLevel();
    // }

    void LoadLevel()
    {
        // Push lines onto a stack so we can pop bottom-up rows. This is easy to reason
        //  about, but an index-based loop over the string array is faster.
        Stack<string> levelRows = new Stack<string>();

        foreach (string line in levelFile.text.Split('\n'))
            levelRows.Push(line);

        int row = 0;
        while (levelRows.Count > 0)
        {
            string rowString = levelRows.Pop();
            char[] rowChars = rowString.ToCharArray();

            for (var columnIndex = 0; columnIndex < rowChars.Length; columnIndex++)
            {
                var currentChar = rowChars[columnIndex];

                // Dirt
                if (currentChar == 'x')
                {
                    Vector3 newPostition = new Vector3(columnIndex + 0.5f, row + 0.5f, -0.5f);
                    Transform dirtInstance = Instantiate(basePrefab, levelRoot).transform;
                    dirtInstance.position = newPostition;
                }
            }

            row++;
        }
    }
}