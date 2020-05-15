using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    // Public Properties
    [Header("General Variables")]
    [Tooltip("The next scene that should be loaded after the player touches the goal.")]
    public string nextSceneName = string.Empty;
    [Tooltip("How long (in seconds) that should pass before the next scene is loaded.")]
    public float waitTime = 2.0f;

    [Header("Vertical Bobble")]
    [Space(10)]
    [Tooltip("Should the goal bobble vertically?")]
    public bool verticalBobble = true;
    [Tooltip("The amplitude (in units) of how far the goal bobbles up and down.")]
    public float amplitudeY = 0.5f;
    [Tooltip("The speed (degrees / second) of the vertical bobbling.")]
    public float verticalSpeed = 180;

    [Header("Horizontal Bobble")]
    [Space(10)]
    [Tooltip("Should the goal bobble horizontally?")]
    public bool horizontalBobble = false;
    [Tooltip("The amplitude (in units) of how far the goal bobbles left and right.")]
    public float amplitudeX = 0.5f;
    [Tooltip("The speed (degrees / second) of the horizontal bobbling.")]
    public float horizontalSpeed = 180;

    // Private Properties
    private ParticleSystem goalParticles;
    private Collider2D collide;
    private Vector3 startPos;
    private bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        // Getting the private properties
        goalParticles = GetComponent<ParticleSystem>();
        collide = GetComponent<Collider2D>();
        startPos = transform.position;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeToWait"></param>
    /// <returns></returns>
    private IEnumerator LoadNextScene(float timeToWait)
    {
        // Waiting for the designated amount of seconds
        yield return new WaitForSeconds(timeToWait);

        // Loading the next scene
        if (nextSceneName != string.Empty)
            SceneManager.LoadSceneAsync(nextSceneName);
        else
            Debug.LogError("\t[ nextSceneName ] variable is not set! (Cannot load scene corresponding to an empty string.)");
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        // IF the goal has not been activated...
        if (!activated)
        {
            // Creating the new position vector
            Vector3 newPos = startPos;

            // IF there is vertical bobbling, add it to the new position vector
            if (verticalBobble)
            {
                newPos.y += Mathf.Sin(Time.time * verticalSpeed * Mathf.Deg2Rad) * amplitudeY;
            }

            // IF there is horizontal bobbling, add it to the new position vector
            if (horizontalBobble)
            {
                newPos.x += Mathf.Cos(Time.time * horizontalSpeed * Mathf.Deg2Rad) * amplitudeX;
            }

            // Setting the goal to the new position
            transform.position = newPos;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // IF the other object is the player...
        if (other.gameObject.tag == "Player")
        {
            // Telling the game that the goal was activated
            activated = true;

            // Disable the collider + Turn on the particle system
            collide.enabled = false;
            if (goalParticles.isPlaying)
                goalParticles.Stop();
            goalParticles.Play();

            // Loading the next scene
            StartCoroutine(LoadNextScene(waitTime));
        }
    }
}
