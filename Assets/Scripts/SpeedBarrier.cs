using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBarrier : MonoBehaviour
{
    // Public Properties
    [Header("Setup Variables")]
    public SpriteRenderer sprite;
    public BoxCollider2D barrierCollider;
    public ParticleSystem breakParticles;
    [Header("General Variables")]
    [Space(10)]
    public Vector2 size = new Vector2(2, 2);
    public float breakVelocity = 30.0f;
    [Header("Debug Variables")]
    public bool debug = false;

    // Private Properties
    private AudioManager audioMng;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        // Getting the private components
        sprite = GetComponent<SpriteRenderer>();
        barrierCollider = GetComponent<BoxCollider2D>();
        breakParticles = GetComponent<ParticleSystem>();

        // Getting the Audio Manager (and complaining if it can't find one)
        audioMng = FindObjectOfType<AudioManager>();
        if (audioMng == null)
            Debug.LogError("\tNo GameObject with the [ AudioManager ] script was found in the current scene!");

        // Updating the size of the barrier
        UpdateObjectSize();
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateObjectSize()
    {
        // Setting the scale to one (basically "Disables" editing via standard Unity transform widget)
        transform.localScale = Vector3.one;

        // Updating the sprite tiling values + collider size
        sprite.size = size;
        barrierCollider.size = size;
        var breakShape = breakParticles.shape;
        breakShape.scale = size;
    }

    /// <summary>
    /// 
    /// </summary>
    private void BreakBarrier()
    {
        // Disabling the main sprite + box collider
        sprite.enabled = false;
        barrierCollider.enabled = false;

        // Playing the speed barrier break audio
        audioMng.PlayAudio("Speed Barrier Break");
        
        // Playing the break particles
        breakParticles.Play();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Getting the Player Rigidbody2D
            PlayerMover player = other.gameObject.GetComponent<PlayerMover>();
            Vector2 lastVelocity = player.GetLastVelocity();

            // IF debug is on, then print the player's velocity
            if (debug)
                Debug.Log("\tlastVelocity.magnitude:" + lastVelocity.magnitude);

            // IF the player's velocity exceeds the break velocity...
            if (lastVelocity.magnitude >= breakVelocity)
            {
                BreakBarrier();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDrawGizmos()
    {
        UpdateObjectSize();
    }
}
