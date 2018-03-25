using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region ABOUT
    /**
     * Character default script.
     * Handles movements.
     */
    #endregion

    #region DEFAULT VARIABLES
    [Tooltip("The target this character will seek and arrive to.")]
    public GameObject target = null;
    [Tooltip("Flag to determine if we can move, and if not, it means we're firing.")]
    public bool canMove = true;
    private GameObject[] players;
    #endregion

    #region DECISION VARIABLES
    private float decisionTime = 1.0f;
    private float timer = 0.0f;
    private Vector3 dir = Vector3.zero;
    private List<Vector3> moves = new List<Vector3>();
    float speed = 0.1f;
    #endregion

    /// <summary>
    /// Attempts to go after the closest player tank through nodes.
    /// </summary>
    void Update()
    {
        if (canMove)
        {
            // We give the tank a finite amount of time to make a decision
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                timer = decisionTime;
                float minDistance = float.MaxValue;
                players = GameObject.FindGameObjectsWithTag("Player");

                // Find closest player tank to go after
                for (int i = 0; i < players.Length; i++)
                {
                    float distance = (players[i].transform.position - transform.position).magnitude;

                    if (distance < minDistance)
                    {
                        target = players[i];
                        minDistance = distance;
                    }
                }

                // Reset our possible moves
                moves = new List<Vector3>();

                // --- Acquire a conclusive list of ALL possible moves, given various raycasts

                if (!Physics.Raycast(transform.position, Vector3.forward, 2.0f, 1 << 9))
                {
                    moves.Add(transform.position + Vector3.forward);
                }
                if (!Physics.Raycast(transform.position, -Vector3.forward, 2.0f, 1 << 9))
                {
                    moves.Add(transform.position - Vector3.forward);
                }
                if (!Physics.Raycast(transform.position, -Vector3.right, 2.0f, 1 << 9))
                {
                    moves.Add(transform.position - Vector3.right);
                }
                if (!Physics.Raycast(transform.position, Vector3.right, 2.0f, 1 << 9))
                {
                    moves.Add(transform.position + Vector3.right);
                }

                // Check which move is best
                Vector3 best = Vector3.zero;
                float dist = float.PositiveInfinity;
                foreach (Vector3 move in moves)
                {
                    float distance = (target.transform.position - move).magnitude;
                    if (distance < dist)
                    {
                        dist = distance;
                        best = move;
                    }
                }

                // Set direction based on best move
                dir = (best - this.transform.position).normalized;
                AlignBehavior(this.transform.position + dir);
            }

            this.transform.Translate(dir * speed * Time.deltaTime, Space.World);
        }
    }

    /// <summary>
    /// Aligns 'this' enemy tank to a target orientation.
    /// </summary>
    /// <param name="targ">The target to align to.</param>
    private void AlignBehavior(Vector3 targ)
    {
        this.transform.LookAt(targ);
    }
}
