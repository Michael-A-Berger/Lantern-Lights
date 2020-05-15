using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    // Public Properties
    [Header("General Variables")]
    public bool awakeOnStart = false;
    [Header("Positions")]
    [Space(10)]
    public List<MoveItem> moveItems = new List<MoveItem>();

    // Private Properties
    private AudioManager audioMng = null;
    private Rigidbody2D rigid;
    private Vector3 startPos = Vector3.zero;
    private int index = 0;
    private bool isMoving = false;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        // Getting the start position value
        startPos = transform.position;

        // Getting the rigidbody
        rigid = GetComponent<Rigidbody2D>();

        // Getting the Audio Manager (and complaining if it can't find one)
        audioMng = FindObjectOfType<AudioManager>();
        if (audioMng == null)
            Debug.LogError("\tNo GameObject with the [ AudioManager ] script was found in the current scene!");

        // IF the mover should start moving when the scene starts, set the moving variable to true
        if (awakeOnStart)
        {
            StartMoving();
            SetDestination(moveItems[0]);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void StartMoving()
    {
        isMoving = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void StopMoving()
    {
        isMoving = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetDestination(MoveItem move)
    {
        // Calculating the destination
        Vector3 destination = move.position;
        if (move.relativeMove)
            destination += startPos;

        // Calculating the rigidbody velocity
        rigid.velocity = (destination - gameObject.transform.position) / move.timeToComplete;
    }

    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        // IF the mover should be moving...
        if (isMoving)
        {
            // Calculating the destination position value
            Vector3 destination = moveItems[index].position;
            if (moveItems[index].relativeMove)
                destination += startPos;

            // Calculating the destination distance vector
            Vector3 distance = destination - gameObject.transform.position;

            // IF the game object position is the same as the move list position...
            if (transform.position == destination)
            {
                // Increment the move index
                index = (index + 1) % moveItems.Count;

                // Setting the destination to the next position
                SetDestination(moveItems[index]);

                // Playing the moving platform audio
                audioMng.PlayAudio("Moving Platform");
            }
            // ELSE... (the game object position is not at its destination...
            else
            {
                // IF the current rigidbody velocity implies that the game object will pass the destination position next frame...
                if (rigid.velocity.magnitude * Time.deltaTime > distance.magnitude)
                {
                    // Setting the velocity to be just enough to get to the platform to the destination during the next frame
                    rigid.velocity = distance * Time.deltaTime;
                }

                // IF the position is less than a thousandth from the destination position...
                if (distance.magnitude * 100 < 1)
                {
                    // Setting the game objec transform to the destination position
                    rigid.MovePosition(destination);
                    rigid.velocity = Vector2.zero;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void PrintStats(string printName)
    {
        // Calculating the destination + distance
        Vector3 destination = moveItems[index].position;
        if (moveItems[index].relativeMove)
            destination += startPos;
        Vector3 distance = destination - transform.position;

        // Constructing the stats string
        string stats = "\n\t- Transform:"
                        + "\n\t\tX:\t" + transform.position.x
                        + "\n\t\tY:\t" + transform.position.y
                        + "\n\t\tZ:\t" + transform.position.z
                        + "\n\t- Velocity:"
                        + "\n\t\tX:\t" + rigid.velocity.x
                        + "\n\t\tY:\t" + rigid.velocity.y
                        + "\n\t- index:\t\t" + index
                        + "\n\t- Destination:"
                        + "\n\t\tX:\t" + destination.x
                        + "\n\t\tY:\t" + destination.y
                        + "\n\t\tZ:\t" + destination.z
                        + "\n\t- Distance:\t" + distance.magnitude;

        // Prepending the stats string with the print name
        stats = "===== " + printName + " ===== (Time: [" + Time.time + "] )\n" + stats;

        // Appending a simple text barrier
        stats = stats + "\n\n==========";

        Debug.Log(stats);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDrawGizmos()
    {
        // IF the start position is not set (draw gizmos is called in the editor), set it
        Vector3 drawPos = startPos;
        if (drawPos == Vector3.zero)
            drawPos = gameObject.transform.position;

        // Drawing each position as a gizmo
        foreach (MoveItem item in moveItems)
        {
            if (item.drawGizmo)
            {
                Vector3 gizmoPos = item.position;
                if (item.relativeMove)
                    gizmoPos += drawPos;
                Gizmos.color = item.gizmoColor;
                Gizmos.DrawSphere(gizmoPos, 0.5f);
            }
        }
    }
}

[System.Serializable]
public class MoveItem
{
    // Publc Properties
    [Header("General Variables")]
    [Tooltip("The time (in seconds) it takes for the platform to move to the given position.")]
    public float timeToComplete = 1.0f;
    [Tooltip("Is the given position relative to the starting position of the moving platform?")]
    public bool relativeMove = true;
    [Tooltip("The position the platform will move towards.")]
    public Vector3 position;

    [Header("Debug Variables")]
    [Space(10)]
    public bool drawGizmo = false;
    public Color gizmoColor = Color.magenta;

}
