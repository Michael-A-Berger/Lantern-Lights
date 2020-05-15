using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffTrigger : MonoBehaviour
{
    // Public Properties
    public bool debug;

    // Private Properties
    private Rigidbody2D rigid;
    private BoxCollider2D trigger;
    private bool isHooked = false;

    // Start is called before the first frame update
    void Start()
    {
        // Getting the staff rigidbody + trigger
        rigid = GetComponent<Rigidbody2D>();
        trigger = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Lantern")
        {
            isHooked = true;
        }
        if (debug) Debug.Log("GameObject [ " + other.gameObject.name + " ] entered... ("+Time.time+")");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Lantern")
        {
            isHooked = false;
        }
        if (debug) Debug.Log("GameObject [ " + other.gameObject.name + " ] exited! ("+Time.time+")");
    }

    /// <summary>
    /// IsHooked()
    /// </summary>
    /// <returns></returns>
    public bool IsHooked()
    {
        return isHooked;
    }
}
