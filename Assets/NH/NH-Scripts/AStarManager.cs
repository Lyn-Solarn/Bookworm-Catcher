using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    /*
    Code adapted from this this YouTube tutorial:
    https://www.youtube.com/watch?v=UHnOW-OimLQ&t=325s
    */

    public static AStarManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        List<Node> openSet = new List<Node>();

        foreach (Node n in FindObjectsByType<Node>(FindObjectsSortMode.InstanceID)) // Check this
        {
            n.gScore = float.MaxValue;
        }

        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            int lowestF = default;

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FScore() < openSet[lowestF].FScore())
                {
                    lowestF = i;
                }
            }

            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if (currentNode == end)
            {
                List<Node> path = new List<Node>();

                path.Insert(0, end);

                while (currentNode != start)
                {
                    currentNode = currentNode.cameFrom;
                    path.Add(currentNode);
                }

                path.Reverse();
                return path;
            }

            foreach (Node connectedNode in currentNode.connections)
            {
                float heldGScore = currentNode.gScore + Vector2.Distance(currentNode.transform.position, connectedNode.transform.position);

                if (heldGScore < connectedNode.gScore)
                {
                    connectedNode.cameFrom = currentNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = Vector2.Distance(connectedNode.transform.position, end.transform.position);

                    if (!openSet.Contains(connectedNode))
                    {
                        openSet.Add(connectedNode);
                    }
                }
            }
        }

        return null;
    }
}
