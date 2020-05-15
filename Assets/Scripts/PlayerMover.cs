using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    // Public Properties
    [Header("General Variables")]
    public PlayerAnim animScript;
    public GameObject wandPrefab;
    public Transform currentCheckpoint = null;
    public ParticleSystem deathParticles;
    public ParticleSystem checkpointParticles;
    public float deathRespawnTime = 2.0f;
    public float forceRespawnTime = 1.0f;
    [Header("Debug Variables")]
    [Space(10)]
    public bool debug = false;
    public GameObject debugPrefab1;
    public GameObject debugPrefab2;
    [Header("Movement Settings")]
    [Space(10)]
    public List<PlayerMovementSettings> movementVars = new List<PlayerMovementSettings>();

    // Private Properties (General)
    private AudioManager audioMng;
    private PlayerMovementSettings currentMoveVars;
    private SpriteRenderer playerSprite;
    private Rigidbody2D rigid;
    private CapsuleCollider2D capsuleCollider;
    private BoxCollider2D boxTrigger;
    private Vector2 lastVelocity;
    private Vector3 slopeAxis = Vector3.right;
    private Vector2 passiveVelocity = Vector2.zero;
    private float decelTime = 0f;
    private uint jumpCounter = 0;
    private GameObject spawnedWand;
    private StaffTrigger wandScript;
    private Vector3 wandSpawnLocation = new Vector3(1, 0, 0);
    private bool inputEnabled = true;
    private bool grounded = false;

    // Private Properties (Input)
    private float currentAxisX = 0f;
    private float currentAxisY = 0f;
    private float currentFire1 = 0f;
    private float currentFire2 = 0f;
    private float lastAxisX = 0f;
    private float lastAxisY = 0f;
    private float lastFire1 = 0f;
    private float lastFire2 = 0f;

    /// <summary>
    /// Start()
    /// </summary>
    void Start()
    {
        audioMng = FindObjectOfType<AudioManager>();
        playerSprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        boxTrigger = GetComponent<BoxCollider2D>();

        // Initialization Warnings
        if (audioMng == null)
            Debug.LogError("\tNo GameObject with the [ AudioManager ] script was found in the current scene!");
        if (currentCheckpoint == null)
            Debug.LogError("\t[ currentCheckpoint ] of [ PlayerMover.cs ] script not set!");

        // Setting the movement variables
        ResetMovementVars();
    }

    /// <summary>
    /// Resets the movement settings to their "Normal" values
    /// </summary>
    private void ResetMovementVars()
    {
        // Setting the "Currrent" properties to be the "Normal" values
        currentMoveVars = GetMovementVarsByName("Normal");

        // Setting the rigidbody gravity
        rigid.gravityScale = currentMoveVars.gravity;
    }

    /// <summary>
    /// Returns the movement settings instance with the name that EXACTLY matches the given name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public PlayerMovementSettings GetMovementVarsByName(string name)
    {
        PlayerMovementSettings result = null;
        for (int num = 0; num < movementVars.Count; num++)
        {
            if (movementVars[num].name == name)
            {
                result = movementVars[num];
                num = movementVars.Count;
            }
        }
        return result;
    }

    /// <summary>
    /// Returns the previous frame's rigidbody velocity
    /// </summary>
    /// <returns></returns>
    public Vector2 GetLastVelocity()
    {
        return lastVelocity;
    }

    /// <summary>
    /// Gets the current Unity input (assuming input is enabled)
    /// </summary>
    private void GetInput()
    {
        // IF player input is enabled...
        if (inputEnabled)
        {
            // Getting the new input values
            currentAxisX = Input.GetAxisRaw("Horizontal");
            currentAxisY = Input.GetAxisRaw("Vertical");
            currentFire1 = Input.GetAxisRaw("Fire1");
            currentFire2 = Input.GetAxisRaw("Fire2");
        }
        // ELSE... (player input is disabled)
        else
        {
            // Setting all the input values to zero
            currentAxisX = 0.0f;
            currentAxisY = 0.0f;
            currentFire1 = 0.0f;
            currentFire2 = 0.0f;
        }
    }

    /// <summary>
    /// Setting the various "last___" input variables
    /// </summary>
    private void SetOldInput()
    {
        // Setting the "Last Run Time" variables
        lastAxisX = currentAxisX;
        lastAxisY = currentAxisY;
        lastFire1 = currentFire1;
        lastFire2 = currentFire2;
        lastVelocity = rigid.velocity;
    }

    /// <summary>
    /// Toggles whether the player listens for any new input
    /// </summary>
    /// <param name="targetStatus"></param>
    public void SetInputEnabled(bool targetStatus)
    {
        inputEnabled = targetStatus;
    }

    /// <summary>
    /// Update()
    /// </summary>
    void Update()
    {
        // Getting the input values
        GetInput();

        // Retrieving the correct deceleration time
        decelTime = (grounded) ? currentMoveVars.groundedTimeToStop : currentMoveVars.airborneTimeToStop;

        // IF the passive velocity is greater than zero, decrease it from the current velocity calculations
        if (passiveVelocity != Vector2.zero)
            rigid.velocity -= passiveVelocity;

        // =============================
        // ========== RUNNING ==========
        // =============================

        // IF there is any running input...
        if (currentAxisX != 0f)
        {
            // Calculating the add velocity factor
            float addVelocityX = currentAxisX * currentMoveVars.maxRunSpeed * (Time.deltaTime / currentMoveVars.timeToMaxSpeed);

            // Creating the move vector
            Vector2 moveVector = new Vector2();

            // IF the player is grounded, follow the slope axis, otherwise do NOT do that
            if (grounded)
                moveVector = new Vector2(slopeAxis.x * addVelocityX, slopeAxis.y * addVelocityX);
            else
                moveVector = new Vector2(addVelocityX, 0f);

            // IF the velocity and input directions are different OR the new velocity does not exceed the max velocity...
            float currentDirectionX = rigid.velocity.x / Mathf.Abs(rigid.velocity.x);
            if (currentDirectionX != currentAxisX || (rigid.velocity + moveVector).magnitude < currentMoveVars.maxRunSpeed)
            {
                // Adding the move vector onto the current velocity
                rigid.velocity += moveVector;
            }
            // ELSE IF the current velocity is less than the max...
            else if (rigid.velocity.magnitude < currentMoveVars.maxRunSpeed)
            {
                // Setting the current velocity to the maximum run speed
                rigid.velocity = rigid.velocity.normalized * currentMoveVars.maxRunSpeed;
            }
        }

        // IF the player is not holding down a button (but is still moving horizontally)...
        if (currentAxisX == 0f && rigid.velocity.x != 0f)
        {
            // IF the player is grounded, decelerate along the slope axis
            if (grounded)
            {
                // Calculate the deceleration vector
                Vector2 decelVector = (Time.deltaTime / decelTime) * rigid.velocity.normalized * currentMoveVars.maxRunSpeed;

                // IF the deceleration vector is LESS than the current velocity...
                if (decelVector.magnitude < rigid.velocity.magnitude)
                    rigid.velocity -= decelVector;
                else
                    rigid.velocity = new Vector2(0f, 0f);
            }
            // ELSE... (decelerate exclusively on the X axis)
            else
            {
                // Calulate the deceleration vector X value
                float decelX = (Time.deltaTime / decelTime) * (rigid.velocity.x / Mathf.Abs(rigid.velocity.x)) * currentMoveVars.maxRunSpeed;

                // IF the deceleration X value is LESS than the current X velocity...
                if (Mathf.Abs(decelX) < Mathf.Abs(rigid.velocity.x))
                    rigid.velocity -= new Vector2(decelX, 0f);
                // ELSE... (the deceleration X value is MORE than the current X velocity)
                else
                    rigid.velocity = new Vector2(0f, rigid.velocity.y);
            }
        }

        // IF the current running sound is defined...
        if (currentMoveVars.runningSound != string.Empty)
        {
            // IF there is horizontal input AND the player is grounded AND the running sound is not playing, start playing the sound
            if (currentAxisX != 0.0f && grounded && !audioMng.IsPlaying(currentMoveVars.runningSound))
                audioMng.PlayAudio(currentMoveVars.runningSound);

            // IF (there is no horizontal input OR the player is not grouned) AND the running sound is playing...
            if ((currentAxisX == 0.0f || !grounded) && audioMng.IsPlaying(currentMoveVars.runningSound))
                audioMng.StopAudio(currentMoveVars.runningSound);
        }

        // =============================
        // ========== JUMPING ==========
        // =============================

        // 

        // IF there is jump input AND it's new input AND the player can still jump...
        if (currentAxisY > 0f && currentAxisY != lastAxisY && (jumpCounter < currentMoveVars.midairJumps + 1))
        {
            // IF the current jumping sound is defined AND the player is grounded, play the basic jump sound
            if (currentMoveVars.jumpSound != string.Empty && grounded)
                audioMng.PlayAudio(currentMoveVars.jumpSound);

            // IF the current midair jumping sound is defined AND the player is not grounded...
            if (currentMoveVars.midairJumpSound != string.Empty && !grounded)
                audioMng.PlayAudio(currentMoveVars.midairJumpSound);

            // Setting all of the proper jumping variables (velocity, grounded, and jump counter vars)
            rigid.velocity = new Vector2(rigid.velocity.x, currentMoveVars.jumpVelocity);
            grounded = false;
            jumpCounter++;
        }

        // IF there is no input jump AND the lack of input just started AND the player is airborne AND the player is still jumping up...
        if (currentAxisY == 0f && currentAxisY != lastAxisY && !grounded && rigid.velocity.y > 0f)
        {
            // Reduce the player's vertical speed by half
            rigid.velocity *= new Vector2(1f, 0.5f);
        }

        // IF the player is not pressing the down button AND the current Y velocity is faster than the designated fall speed...
        if (currentAxisY > -1f && rigid.velocity.y < currentMoveVars.maxFallSpeed)
        {
            // Calculating the vertical deceleration value
            float decelY = (Time.deltaTime / decelTime) * currentMoveVars.airborneTimeToStop + Physics2D.gravity.y * -1f;

            // IF the new Y velocity is less than the max fall speed, add the deceleration value to the current Y velocity
            if (rigid.velocity.y + decelY < currentMoveVars.maxFallSpeed)
                rigid.velocity += new Vector2(0f, decelY);
            // ELSE... (the new Y velocity is more than the max fall speed, ergo set the Y velocity to the designated fall speed)
            else
                rigid.velocity = new Vector2(rigid.velocity.x, currentMoveVars.maxFallSpeed);
        }
        // ELSE IF the player is pressing the down button AND the current Y velocity is faster than the designated fall speed...
        else if (currentAxisY == -1f && rigid.velocity.y < currentMoveVars.maxFallSpeed)
        {
            // Keeping the Y velocity the same across the Update() calls
            rigid.velocity = new Vector2(rigid.velocity.x, lastVelocity.y);
        }

        // =====================================
        // ========== WAVING THE WAND ==========
        // =====================================

        // Setting the Wand spawn location
        if (currentAxisX != 0 || currentAxisY != 0)
        {
            wandSpawnLocation = new Vector2(currentAxisX, currentAxisY);
            wandSpawnLocation.Normalize();
        }

        // IF the Wand waving input is down AND it's new...
        if (currentFire1 > 0f && currentFire1 != lastFire1)
        {
            // Spawning the Wand
            spawnedWand = Instantiate(wandPrefab, transform.position + wandSpawnLocation, Quaternion.identity);
            wandScript = spawnedWand.GetComponent<StaffTrigger>();
        }

        // IF the Wand waving input is up AND it's new...
        if (currentFire1 == 0f && currentFire1 != lastFire1)
        {
            // Destroying the Wand
            Destroy(spawnedWand, 0f);
        }

        // IF the Wand has been spawned...
        if (spawnedWand != null)
        {
            // Moving the Wand to the right place
            spawnedWand.transform.position = transform.position + wandSpawnLocation;

            // Rotating the Wand
            float rotation = Vector3.SignedAngle(Vector3.right, wandSpawnLocation, Vector3.forward);
            spawnedWand.transform.rotation = Quaternion.identity;
            spawnedWand.transform.Rotate(Vector3.forward, rotation);

            // IF the Wand has been hooked...
            if (wandScript.IsHooked())
            {
                // Add the hook speed to the player's velocity
                Vector2 modVector = wandSpawnLocation * currentMoveVars.hookSpeed * Time.deltaTime * currentMoveVars.maxRunSpeed;
                rigid.velocity += modVector;

                // IF the lantern boost audio is not playing, then start playing it
                if (!audioMng.IsPlaying("Lantern Boost"))
                    audioMng.PlayAudio("Lantern Boost");
            }
        }

        // ======================================
        // ===== RESTARTING FROM CHECKPOINT =====
        // ======================================

        // IF the player is pressing the restart button AND it's a new input...
        if (currentFire2 != 0.0f && currentFire2 != lastFire2)
        {
            // Killing the player and then respawning them
            KillPlayer();
            StartCoroutine(RespawnPlayer(forceRespawnTime));
        }

        // Adding the passive velocity
        rigid.velocity += passiveVelocity;

        // Clamping the velocities
        float clampedX = Mathf.Clamp(rigid.velocity.x, -currentMoveVars.maxHorizontalVelocity, currentMoveVars.maxHorizontalVelocity);
        float clampedY = Mathf.Clamp(rigid.velocity.y, -currentMoveVars.maxVerticalVelocity, currentMoveVars.maxVerticalVelocity);
        rigid.velocity = new Vector2(clampedX, clampedY);

        // ANIMATING
        if (currentAxisX > 0f)
            animScript.MoveRight();
        else if (currentAxisX < 0f)
            animScript.MoveLeft();
        else
            animScript.StandStill();

        // Setting the old input
        SetOldInput();
    }

    /// <summary>
    /// Turns on ground-based movement for the player (based on the passed-in closest collision point)
    /// </summary>
    private void GroundPlayer(Vector3 closestPoint)
    {
        // Telling the player it has been grounded
        grounded = true;

        // Resetting the jump counter
        jumpCounter = 0;

        // Getting the new slope axis
        slopeAxis = transform.position - closestPoint;
        slopeAxis = new Vector3(slopeAxis.y, -slopeAxis.x, 0f);
        slopeAxis.Normalize();

        // Disable gravity on the rigidbody
        rigid.gravityScale = 0f;
    }

    /// <summary>
    /// Kills the player
    /// </summary>
    private void KillPlayer()
    {
        // "Killing" the player (Hide sprite, Set gravity + velocity + passive velocity to zero, Disable input, Play death audio)
        playerSprite.enabled = false;
        rigid.gravityScale = 0f;
        rigid.velocity *= 0;
        passiveVelocity *= 0;
        inputEnabled = false;
        audioMng.PlayAudio("Player Death");

        // Turning on the particles that play on death
        if (deathParticles.isPlaying)
        {
            deathParticles.Stop();
            deathParticles.time = 0;
        }
        deathParticles.Play();
    }

    /// <summary>
    /// Respawning the player
    /// </summary>
    private IEnumerator RespawnPlayer(float waitTime)
    {
        // Waiting for the designated amount of seconds
        yield return new WaitForSeconds(waitTime);

        // "Respawning" the player (Show sprite, Enable gravity, Enable input, Play respawn audio)
        playerSprite.enabled = true;
        rigid.gravityScale = currentMoveVars.gravity;
        inputEnabled = true;
        audioMng.PlayAudio("Player Respawn");

        // Changing the current position to the checkpoint
        transform.position = currentCheckpoint.position;
    }

    /// <summary>
    /// Changes the checkpoint to the passed-in Checkpoint prefab transform
    /// </summary>
    private void ChangeCheckpoint(Transform checkpoint)
    {
        // Defining the update checkpoint boolean
        bool canUpdate = true;

        // IF the current checkpoint is defined...
        if (currentCheckpoint != null)
        {
            // IF the current checkpoint and the new checkpoint are the same, then don't update the current checkpoint
            canUpdate = (checkpoint.gameObject.name != currentCheckpoint.gameObject.name);

            // IF the checkpoint can be updated, stop the old checkpoint particles
            if (canUpdate)
            {
                ParticleSystem oldParticles = currentCheckpoint.gameObject.GetComponentInChildren<ParticleSystem>();
                oldParticles.gameObject.SetActive(false);
            }
        }

        // IF the checkpoint can be updated...
        if (canUpdate)
        {
            // Playing the checkpoint particles + Setting the checkpoint transform + Playing the checkpoint sound
            if (checkpointParticles.isPlaying)
            {
                checkpointParticles.Stop();
                checkpointParticles.time = 0;
            }
            checkpointParticles.Play();
            currentCheckpoint = checkpoint;
            audioMng.PlayAudio("Player Respawn");
        }
    }

    /// <summary>
    /// Processing the passed-in 2D box collider as a platform
    /// </summary>
    private void ProcessAsPlatform(BoxCollider2D platform, bool fromEnterFunc)
    {
        // Get the closest point to the player
        Vector3 closestPoint = platform.ClosestPoint(transform.position);

        // IF the closest point is on the ground...
        if (closestPoint.y <= transform.position.y - boxTrigger.bounds.extents.y / 2)
        {
            // Grounding the player
            GroundPlayer(closestPoint);
        }

        // IF the process was started from an "Enter" function AND debug is on, spawn the debug prefabs
        if (fromEnterFunc && debug)
        {
            if (grounded)
            {
                Instantiate(debugPrefab1, closestPoint, Quaternion.identity);
            }
            else
            {
                Instantiate(debugPrefab2, closestPoint, Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// Releasing the player from the current platform they are on
    /// </summary>
    private void ReleaseFromPlatform()
    {
        // Enable gravity
        rigid.gravityScale = currentMoveVars.gravity;
        grounded = false;
    }

    /// <summary>
    /// Activates when the BOX TRIGGER first touches another trigger
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        // IF the other collider is a Platform...
        if (other.gameObject.tag == "Platform")
        {
            // Casting to the correct collider type
            BoxCollider2D otherBox = (BoxCollider2D) other;

            // Processing the platform as a proper platform
            ProcessAsPlatform(otherBox, true);

            // Playing the landing audio clip (assuming it's defined)
            if (currentMoveVars.landingSound != string.Empty && !audioMng.IsPlaying(currentMoveVars.landingSound))
                audioMng.PlayAudio(currentMoveVars.landingSound);
        }

        // IF the other collider is Water...
        if (other.gameObject.tag == "Water")
        {
            // Cut velocity by 20%
            rigid.velocity *= 0.8f;

            // Setting the water movement variables
            currentMoveVars = GetMovementVarsByName("Water");

            // Playing the "Water Entered" audio (if it's defined)
            if (currentMoveVars.settingsAppliedSound != string.Empty)
                audioMng.PlayAudio(currentMoveVars.settingsAppliedSound);
        }

        // IF the other collider is a Checkpoint Trigger...
        if (other.gameObject.tag == "Checkpoint Trigger")
        {
            ChangeCheckpoint(other.gameObject.transform.parent);
        }

        // IF the other collider is the goal...
        if (other.gameObject.tag == "Finish")
        {
            // Disabling input
            inputEnabled = false;

            // Playing the Level Complete audio + Stopping the background music
            audioMng.PlayAudio("Level Complete");
            audioMng.StopAudio("Nowhere Land");
        }
    }

    /// <summary>
    /// Activates when the BOX TRIGGER remains in contact with another trigger
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay2D(Collider2D other)
    {
        // IF the other collider is a Platform...
        if (other.gameObject.tag == "Platform")
        {
            // Casting to the correct collider type
            BoxCollider2D otherBox = (BoxCollider2D)other;

            // Processing the platform as a proper platform
            ProcessAsPlatform(otherBox, false);
        }
    }

    /// <summary>
    /// Activates when the BOX TRIGGER leaves another trigger
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit2D(Collider2D other)
    {
        // IF the other collider is a Platform...
        if (other.gameObject.tag == "Platform")
        {
            // Releasing from the platform
            ReleaseFromPlatform();
        }

        // IF the other collider is Water...
        if (other.gameObject.tag == "Water")
        {
            // Playing the "Water Exited" audio (assuming it's defined)
            if (currentMoveVars.settingsRemovedSound != string.Empty)
                audioMng.PlayAudio(currentMoveVars.settingsRemovedSound);

            // Resetting the normal movement variables
            ResetMovementVars();

            // Letting the player double-jump once to get out of the water
            jumpCounter = 1;
        }
    }

    /// <summary>
    /// Activates when the CAPSULE COLLIDER first touches another collider
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        // IF the other collider is Lava...
        if (other.gameObject.tag == "Lava")
        {
            // Killing the player
            KillPlayer();

            // Respawning the player
            StartCoroutine(RespawnPlayer(2));
        }

        // IF the other collider is a speed barrier...
        if (other.gameObject.tag == "Speed Barrier")
        {
            // Getting the Speed Barrier script
            SpeedBarrier barrier = other.gameObject.GetComponent<SpeedBarrier>();

            // IF the previous frame's velocity is greater than the speed barrier's break velocity...
            if (lastVelocity.magnitude >= barrier.breakVelocity)
            {
                // Setting the current velocity to the previous frame's velocity
                rigid.velocity = lastVelocity;
            }
            // ELSE... (the previous velocity was smaller than the break velocity...)
            else
            {
                // Process the speed barrier like any platform
                BoxCollider2D otherBox = other.gameObject.GetComponent<BoxCollider2D>();
                ProcessAsPlatform(otherBox, true);

                // Playing the landing audio clip (assuming it's defined)
                if (currentMoveVars.landingSound != string.Empty && !audioMng.IsPlaying(currentMoveVars.landingSound))
                    audioMng.PlayAudio(currentMoveVars.landingSound);
            }
        }

        // IF the other collider is a moving platform...
        if (other.gameObject.tag == "Moving Platform")
        {
            // Getting the moving platform Rigidbody 2D + Box Collider 2D
            Rigidbody2D otherRigid = other.gameObject.GetComponent<Rigidbody2D>();
            BoxCollider2D otherBox = other.gameObject.GetComponent<BoxCollider2D>();

            // Processing the moving platform like any platform
            ProcessAsPlatform(otherBox, true);

            // Setting the player's passive velocity to the platform's velocity
            passiveVelocity = otherRigid.velocity;

            // Playing the landing audio clip (assuming it's defined)
            if (currentMoveVars.landingSound != string.Empty && !audioMng.IsPlaying(currentMoveVars.landingSound))
                audioMng.PlayAudio(currentMoveVars.landingSound);
        }
    }

    /// <summary>
    /// Activates when CAPSULE COLLIDER remains in contact with another collider
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay2D(Collision2D other)
    {
        // IF the other collider is a speed barrier...
        if (other.gameObject.tag == "Speed Barrier")
        {
            // Processing the speed barrier like any platform
            BoxCollider2D otherBox = other.gameObject.GetComponent<BoxCollider2D>();
            ProcessAsPlatform(otherBox, false);
        }

        // IF the other collider is a moving platform...
        if (other.gameObject.tag == "Moving Platform")
        {
            // Getting the moving platform Rigidbody 2D + Box Collider 2D
            Rigidbody2D otherRigid = other.gameObject.GetComponent<Rigidbody2D>();
            BoxCollider2D otherBox = other.gameObject.GetComponent<BoxCollider2D>();

            // Processing the moving platform like any platform
            ProcessAsPlatform(otherBox, true);

            // IF the player's passive velocity is NOT the same as than the platform's velocity...
            if (passiveVelocity != otherRigid.velocity)
            {
                // Updating the passive velocity
                passiveVelocity = otherRigid.velocity;
            }
        }
    }

    /// <summary>
    /// Activates when the CAPSULE COLLIDER leaves another collider
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit2D(Collision2D other)
    {
        // IF the other collider is a speed barrier...
        if (other.gameObject.tag == "Speed Barrier")
        {
            // Releasing from the speed barrier like it's a platform
            ReleaseFromPlatform();
        }

        // IF the other collider is a moving platform...
        if (other.gameObject.tag == "Moving Platform")
        {
            // Set the passive velocity to zero
            passiveVelocity *= 0;

            // Releasing from the moving platform like it's a platform
            ReleaseFromPlatform();
        }
    }
}

[System.Serializable]
public class PlayerMovementSettings
{
    // Public Properties (Default values are for "Normal" settings)
    [Header("General Variables")]
    [Tooltip("What this movement setting is called (also the name to search for in the [GetMovementVarsByName()] function)")]
    public string name = "Normal";
    [Tooltip("How fast (units / second) can the player travel horizontally? (Before the X velocity is clamped.)")]
    public float maxHorizontalVelocity = 40f;
    [Tooltip("How fast (units / second) can the player travel vertically? (Before the Y velocity is clamped.)")]
    public float maxVerticalVelocity = 40f;
    [Tooltip("What is the speed boost (velocity / second) of when the Wand is inside of a Lantern?")]
    public float hookSpeed = 20f;

    [Header("Ground Movement Variables")]
    [Space(10)]
    [Tooltip("How many units does the player move in a second?")]
    public float maxRunSpeed = 10f;
    [Tooltip("How many seconds does it take for the player to reach the max run speed?")]
    public float timeToMaxSpeed = 0.2f;
    [Tooltip("How many seconds does it take for the player, on the ground, to reach a velocity of zero? (Given no input.)")]
    public float groundedTimeToStop = 0.2f;

    [Header("Airborne Movement Variables")]
    [Space(10)]
    [Tooltip("How fast (units / second) can the player fall? (Not using speed boosts.)")]
    public float maxFallSpeed = -20f;
    [Tooltip("How many seconds does it take for the player, in the air, to reach a velocity of zero? (Given no input.)")]
    public float airborneTimeToStop = 1.0f;
    [Tooltip("What is the initial Y velocity when the player jumps?")]
    public float jumpVelocity = 15f;
    [Tooltip("How many midair jumps can the player perform?")]
    public uint midairJumps = 1;
    [Tooltip("What is the gravity scale of the Rigidbody?")]
    public float gravity = 3f;

    [Header("Audio")]
    [Space(10)]
    public string jumpSound = string.Empty;
    public string midairJumpSound = string.Empty;
    public string landingSound = string.Empty;
    public string runningSound = string.Empty;
    public string settingsAppliedSound = string.Empty;
    public string settingsRemovedSound = string.Empty;
}
