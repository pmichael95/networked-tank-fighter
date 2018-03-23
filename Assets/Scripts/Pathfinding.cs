using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinding : MonoBehaviour
{
    #region ABOUT
    /**
     * Pathfinding implementation of the 3 algorithms:
     * Dijkstra (with 0 heuristic)
     * A* (with Euclidean Distance heuristic)
     * Cluster
     * 
     * => THIS SCRIPT IS ATTACHED TO THE CHARACTER! 
     **/
    #endregion

    #region VARIABLES
    // -- All nodes in our scene
    private GameObject[] nodes;

    [Tooltip("The path list of nodes to visit.")]
    public List<GameObject> pathList;
    [Tooltip("The open list for nodes.")]
    public List<GameObject> openList;
    [Tooltip("The closed list for nodes.")]
    public List<GameObject> closedList;
    [Tooltip("The goal node to reach.")]
    public GameObject goalNode;

    // Backup for the path list to clear render
    private List<GameObject> visitedPathList = new List<GameObject>();

    // Closest node to character
    private GameObject startNode;
    #endregion

    /// <summary>
    /// Initialization for variables.
    /// </summary>
    void Start()
    {
        nodes = GameObject.FindGameObjectsWithTag("Node");
    }

    /// <summary>
    /// Clears the lists and calls appropriate algorithm function.
    /// </summary>
    public void ComputePath()
    {
        // Clear the lists first
        ClearTheLists();

        // Run A*
        AStarAlgorithm();
    }

    /// <summary>
    /// A* path finding algorithm. Will execute as intended by visiting nodes.
    /// </summary>
    private void AStarAlgorithm()
    {
        // Find closest node to the character
        // By default the character will never be too far from a node.
        GameObject closestNode = GetClosestNode();
        startNode = closestNode;

        if (closestNode == null)
        {
            Debug.LogError("ERROR::UNKNOWN_CLOSEST_NODE_TO_PLAYER");
            return;
        }

        // Add this closest node to the Open List
        openList.Add(closestNode);

        while (openList.Count > 0 && openList[0] != goalNode)
        {
            // Visit the nodes
            AStarVisitNode(openList[0]);
        }

        // Finally compute the final path list
        ComputePathList();
    }

    /// <summary>
    /// Similar to the Dijkstra algorithm, we visit the node passed.
    /// This time, the heuristic is not 0, but rather the Euclidean Distance.
    /// </summary>
    /// <param name="node"></param>
    private void AStarVisitNode(GameObject node)
    {
        // Now that we're visiting this node, add it to the closed list
        closedList.Add(node);
        // As such, remove it from the open list
        openList.Remove(node);

        // Acquire the neighboring nodes
        List<GameObject> neighbors = node.GetComponent<NodeNeighbors>().GetNeighbors();

        foreach (GameObject currNeighbor in neighbors)
        {
            NodeNeighbors currentNode = currNeighbor.GetComponent<NodeNeighbors>();
            float distance = Vector3.Distance(currNeighbor.transform.position, node.transform.position);
            float costSoFar = node.GetComponent<NodeNeighbors>().costSoFar + distance;

            // Distance is the heuristic
            float heuristic = Vector3.Distance(goalNode.transform.position, currNeighbor.transform.position);
            float totalEstimateVal = costSoFar + heuristic;

            bool isInClosedList = closedList.Contains(currNeighbor);
            bool isInOpenList = openList.Contains(currNeighbor);

            if (isInClosedList && (totalEstimateVal < currentNode.totalEstimateVal))
            {
                // Update the current node's attributes
                UpdateNodeValues(currentNode, costSoFar, totalEstimateVal, heuristic, node);

                // Remove it from the closed list
                closedList.Remove(currNeighbor);
                // Add it to the open list
                openList.Add(currNeighbor);
            }
            else if (isInOpenList && (totalEstimateVal < currentNode.totalEstimateVal))
            {
                // Update the current node's attributes
                UpdateNodeValues(currentNode, costSoFar, totalEstimateVal, heuristic, node);
            }
            else if (!isInClosedList && !isInOpenList)
            {
                // Update the current node's attributes
                UpdateNodeValues(currentNode, costSoFar, totalEstimateVal, heuristic, node);

                // Add it to the open list
                openList.Add(currNeighbor);
            }
        }

        // Sort the open list
        openList.Sort((GameObject n, GameObject m) => { return n.GetComponent<NodeNeighbors>().costSoFar.CompareTo(m.GetComponent<NodeNeighbors>().costSoFar); });
    }

    /// <summary>
    /// Clears the open, closed, and path lists.
    /// </summary>
    private void ClearTheLists()
    {
        openList.Clear();
        closedList.Clear();
        pathList.Clear();
    }

    /// <summary>
    /// Updates the passed node's key essential attributes.
    /// </summary>
    /// <param name="node">The node we're updating.</param>
    /// <param name="costSoFar">New cost so far.</param>
    /// <param name="totalEstimateVal">New estimated total cost.</param>
    /// <param name="heuristic">New h(n) value.</param>
    /// <param name="previousNode">The previous node to the updated one.</param>
    private void UpdateNodeValues(NodeNeighbors node, float costSoFar, float totalEstimateVal, float heuristic, GameObject previousNode)
    {
        node.costSoFar = costSoFar;
        node.totalEstimateVal = totalEstimateVal;
        node.heuristicVal = heuristic;
        node.previousNode = previousNode;
    }

    /// <summary>
    /// Gets the node closest to the player.
    /// </summary>
    /// <returns>The closest node to the player.</returns>
    private GameObject GetClosestNode()
    {
        if (nodes == null || nodes.Length <= 0) return null;
        else
        {
            float shortestDistance = float.MaxValue;
            float dist = 0.0f;
            GameObject closestNode = null;
            foreach (GameObject _Node in nodes)
            {
                dist = Vector3.Distance(this.transform.position, _Node.transform.position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    closestNode = _Node;
                }
            }
            return closestNode;
        }
    }

    /// <summary>
    /// Computes the path list for the pathing.
    /// </summary>
    private void ComputePathList()
    {
        // Add the goal node at the start
        pathList.Add(goalNode);
        while (pathList.Count > 0 && pathList[pathList.Count - 1] != startNode)
        {
            GameObject nextNode = pathList[pathList.Count - 1].GetComponent<NodeNeighbors>().previousNode;
            // If there is no previous node
            if (!nextNode)
            {
                // Clear the list and exit
                pathList.Clear();
                Debug.LogError("ERROR::IMPOSSIBLE_PATHING_ATTEMPTED");
                return;
            }
            pathList.Add(nextNode);
        }

        // Finally, reverse the list so we get the proper order
        pathList.Reverse();
    }

    /// <summary>
    /// Computes the A* algorithm's candidate possible shortest path.
    /// </summary>
    /// <param name="start">Where we begin the search.</param>
    /// <param name="goal">The goal node to reach.</param>
    /// <returns>The possible shortest path.</returns>
    private List<GameObject> ComputeAStarWithReturn(GameObject start, GameObject goal)
    {
        // Set start and goal nodes
        startNode = start;
        goalNode = goal;

        ClearTheLists();

        // Compute the A* algorithm path
        AStarAlgorithm();

        return new List<GameObject>(pathList);
    }

    /// <summary>
    /// Public function that calls ComputeAStarWithReturn.
    /// </summary>
    /// <param name="startNode">Where we begin the search.</param>
    /// <param name="endNode">The goal node to reach.</param>
    /// <returns>A possible shortest path.</returns>
    public List<GameObject> ComputeAStar(GameObject startNode, GameObject endNode)
    {
        List<GameObject> result = ComputeAStarWithReturn(startNode, endNode);

        CleanUp();

        return result;
    }

    /// <summary>
    /// Cleans up the lists and resets goal node and start node.
    /// </summary>
    private void CleanUp()
    {
        goalNode = null;
        ClearTheLists();
        startNode = GetClosestNode();
        GetComponent<Character>().target = null;
        goalNode = startNode.GetComponent<NodeNeighbors>().GetNeighbors()[0];
        startNode.GetComponent<NodeNeighbors>().costSoFar = 0.0f;
        startNode.GetComponent<NodeNeighbors>().heuristicVal = Vector3.Distance(goalNode.transform.position, startNode.transform.position);
    }

    /// <summary>
    /// Given a path list (generated from A*), we compute total costs (Euclidean distance).
    /// </summary>
    /// <param name="path">A path list.</param>
    /// <returns>Total cost of this path.</returns>
    public float GetTotalCostOfPath(List<GameObject> path)
    {
        float total = 0.0f;

        for (var i = 0; i < path.Count - 1; i++)
        {
            total += Vector3.Distance(path[i].transform.position, path[i + 1].transform.position);
        }

        return total;
    }
}
