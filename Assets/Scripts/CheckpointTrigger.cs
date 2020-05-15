using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    // Public Properties
    public GameObject checkpointParticles = null;
    public bool activeOnAwake = false;

    // Private Properties
    private bool activated = false;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        // Basic property checks + warnings
        if (checkpointParticles == null)
            Debug.LogError("\t[ checkpointParticles ] of [ CheckpointTrigger.cs ] not set!");

        // Deleting this trigger if the checkpoint is already activated
        if (activeOnAwake)
        {
            activated = true;
            RestartParticles();
            StartCoroutine(DeleteSelf());
        }
    }

    private void RestartParticles()
    {
        checkpointParticles.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeleteSelf()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated && other.tag == "Player")
        {
            activated = true;
            RestartParticles();
            StartCoroutine(DeleteSelf());
        }
    }
}
