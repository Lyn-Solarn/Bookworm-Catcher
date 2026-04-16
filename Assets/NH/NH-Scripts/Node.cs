using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    /*
    Code adapted from this this YouTube tutorial:
    https://www.youtube.com/watch?v=UHnOW-OimLQ&t=325s
    */

    public Node cameFrom;
    public List<Node> connections;

    public float gScore; // How many moves it takes to reach node from starting node
    public float hScore; // Approximate number of moves from current node to end node

    public float FScore()
    {
        return gScore + hScore;
    }

    // Visualize connections between pathfinding nodes
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (connections.Count > 0)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                Gizmos.DrawLine(transform.position, connections[i].transform.position);
            }
        }
    }
}
