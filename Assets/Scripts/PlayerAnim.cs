using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    // Public Properties
    public bool debug;
    public bool debug_MovingRight;
    public bool debug_MovingLeft;
    
    // Private Properties
    private Animator anim;
    private const string varName_MovingRight = "MovingRight";
    private const string varName_MovingLeft = "MovingLeft";

    // Animation Enum State
    private enum AnimState
    {
        Standing,
        MoveRight,
        MoveLeft
    }

    /// <summary>
    /// Start() - Gets the Animation Controller (which should be "Player")
    /// </summary>
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// SetDebugVars()
    /// </summary>
    private void SetDebugVars()
    {
        debug_MovingRight = anim.GetBool(varName_MovingRight);
        debug_MovingLeft = anim.GetBool(varName_MovingLeft);
    }

    /// <summary>
    /// SetAnimBools()
    /// </summary>
    private void SetAnimBools(AnimState state)
    {
        anim.SetBool(varName_MovingRight, false);
        anim.SetBool(varName_MovingLeft, false);

        switch (state)
        {
            case AnimState.Standing:
                // Do nothing, variables are already set
                break;
            case AnimState.MoveRight:
                anim.SetBool(varName_MovingRight, true);
                break;
            case AnimState.MoveLeft:
                anim.SetBool(varName_MovingLeft, true);
                break;
            default:
                Debug.LogError("AnimState not defined in SetAnimBools() function!");
                break;
        }

        // IF debug is enabled, change the debug variables
        if (debug) SetDebugVars();
    }

    /// <summary>
    /// MoveRight()
    /// </summary>
    public void MoveRight()
    {
        if (!anim.GetBool(varName_MovingRight))
        {
            anim.CrossFade("Player_MoveRight", 0f);
            SetAnimBools(AnimState.MoveRight);
        }
    }

    /// <summary>
    /// MoveLeft()
    /// </summary>
    public void MoveLeft()
    {
        if (!anim.GetBool(varName_MovingLeft))
        {
            anim.CrossFade("Player_MoveLeft", 0f);
            SetAnimBools(AnimState.MoveLeft);
        }
    }

    /// <summary>
    /// StandStill()
    /// </summary>
    public void StandStill()
    {
        bool movingRight = anim.GetBool(varName_MovingRight);
        bool movingLeft = anim.GetBool(varName_MovingLeft);
        if (movingRight || movingLeft)
        {
            if (movingRight)
            {
                anim.CrossFade("Player_StandRight", 0f);
            }
            else if (movingLeft)
            {
                anim.CrossFade("Player_StandLeft", 0f);
            }
            SetAnimBools(AnimState.Standing);
        }
    }
    
    /// <summary>
    /// IsMovingRight()
    /// </summary>
    /// <returns></returns>
    public bool IsMovingRight()
    {
        return anim.GetBool(varName_MovingRight);
    }

    /// <summary>
    /// IsMovingLeft()
    /// </summary>
    /// <returns></returns>
    public bool IsMovingLeft()
    {
        return anim.GetBool(varName_MovingLeft);
    }

    /// <summary>
    /// IsStanding()
    /// </summary>
    /// <returns></returns>
    public bool IsStanding()
    {
        return !anim.GetBool(varName_MovingRight) && !anim.GetBool(varName_MovingLeft);
    }
}
