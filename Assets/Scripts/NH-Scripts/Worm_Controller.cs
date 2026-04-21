using System;
using System.Collections.Generic;
using UnityEngine;

public class Worm_Controller : MonoBehaviour
{
    /*
    Code adapted from this this YouTube tutorial:
    https://www.youtube.com/watch?v=UHnOW-OimLQ&t=325s
    */

    [SerializeField] private Node currentNode;
    [SerializeField] private List<Node> path = new List<Node>();

    private void Update()
    {
        CreatePath();
    }

    private void CreatePath()
    {
        if (path.Count > 0)
        {
            int x = 0;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[x].transform.position.x, path[x].transform.position.y, -2), 3 * Time.deltaTime);

            if (Vector2.Distance(transform.position, path[x].transform.position) < 0.1f)
            {
                currentNode = path[x];
                path.RemoveAt(x);
            }
        } else
        {
            Node[] nodes = FindObjectsByType<Node>(FindObjectsSortMode.InstanceID);
            while (path == null || path.Count == 0)
            {
                path = AStarManager.instance.GeneratePath(currentNode, nodes[UnityEngine.Random.Range(0, nodes.Length)]);
            }
        }
    }
}
