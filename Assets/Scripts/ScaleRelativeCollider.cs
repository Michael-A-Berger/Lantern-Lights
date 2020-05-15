using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleRelativeCollider : MonoBehaviour
{
    // Public Properties
    [Header("General Variables")]
    public Vector2 distanceFromEdge;
    [Header("Debug Variables")]
    [Space(10)]
    public bool drawGizmo = false;
    public Color gizmoColor = Color.magenta;

    // Private Properties
    private BoxCollider2D boxCollider = null;
    private Vector2 percentages = new Vector2(1, 1);

    // Start()
    void Start()
    {
        // Getting the object's collider
        boxCollider = GetComponent<BoxCollider2D>();

        // Complaining if the object doesn't have a collider
        if (boxCollider == null)
            Debug.LogError("\tObject [" + gameObject.name + "] doesn't have a 2D box collider!");
    }

    // CalculatePercentages()
    private void CalculatePercentages()
    {
        percentages.x = 1.0f + ((distanceFromEdge.x * 2) / gameObject.transform.localScale.x);
        percentages.y = 1.0f + ((distanceFromEdge.y * 2) / gameObject.transform.localScale.y);
    }

    // Update()
    void Update()
    {
        if (boxCollider != null)
        {
            CalculatePercentages();
            boxCollider.size = percentages;
        }
    }

    // OnDrawGizmos()
    private void OnDrawGizmos()
    {
        if (drawGizmo)
        {
            CalculatePercentages();
            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * percentages);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
