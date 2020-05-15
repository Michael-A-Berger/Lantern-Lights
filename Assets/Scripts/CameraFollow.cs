using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Public Properties
    public GameObject objectToFollow;
    [Range(0, 1)]
    public float lerpPrecentage = 0.2f;

    // Private Properties
    private Camera main;

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, main.transform.position.z);
        main.transform.position = Vector3.Lerp(transform.position, newPos, lerpPrecentage);
    }
}
