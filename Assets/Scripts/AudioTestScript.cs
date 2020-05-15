using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTestScript : MonoBehaviour
{
    // Private Properties
    private AudioManager audioMng = null;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        audioMng = FindObjectOfType<AudioManager>();
        if (audioMng == null)
            Debug.LogError("\tGameObject with [ AudioManager ] script not found in scene!");
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        // 1 = Toggle "Music for Manatees"
        if (Input.GetKeyDown(KeyCode.Alpha1))
            if (audioMng.IsPlaying("Music for Manatees"))
                audioMng.StopAudio("Music for Manatees");
            else
                audioMng.PlayAudio("Music for Manatees");

        // 2 = Toggle "Nowhere Land"
        if (Input.GetKeyDown(KeyCode.Alpha2))
            if (audioMng.IsPlaying("Nowhere Land"))
                audioMng.StopAudio("Nowhere Land");
            else
                audioMng.PlayAudio("Nowhere Land");

        // 3 = Toggle "Pixelland"
        if (Input.GetKeyDown(KeyCode.Alpha3))
            if (audioMng.IsPlaying("Pixelland"))
                audioMng.StopAudio("Pixelland");
            else
                audioMng.PlayAudio("Pixelland");

        // 4 = Play "Lantern Boost"
        if (Input.GetKeyDown(KeyCode.Alpha4))
            audioMng.PlayAudio("Lantern Boost");

        // 5 = Play "Menu Click"
        if (Input.GetKeyDown(KeyCode.Alpha5))
            audioMng.PlayAudio("Menu Click");

        // 6 = Play "Speed Barrier Break"
        if (Input.GetKeyDown(KeyCode.Alpha6))
            audioMng.PlayAudio("Speed Barrier Break");

        // 7 = Play "Player Death"
        if (Input.GetKeyDown(KeyCode.Alpha7))
            audioMng.PlayAudio("Player Death");

        // 8 = Play "Level Complete"
        if (Input.GetKeyDown(KeyCode.Alpha8))
            audioMng.PlayAudio("Level Complete");

        // 9 = Play "Moving Platform"
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Debug.Log("\t[Moving Platforming] sound trying to be triggered!");
            audioMng.PlayAudio("Moving Platform");
        }

        // Q = Play "Player Respawn"
        if (Input.GetKeyDown(KeyCode.Q))
            audioMng.PlayAudio("Player Respawn");

        // W = Play "Normal - Jump"
        if (Input.GetKeyDown(KeyCode.W))
            audioMng.PlayAudio("Normal - Jump");

        // E = Play "Normal - Midair Jump"
        if (Input.GetKeyDown(KeyCode.E))
            audioMng.PlayAudio("Normal - Midair Jump");

        // R = Play "Normal - Landing"
        if (Input.GetKeyDown(KeyCode.R))
            audioMng.PlayAudio("Normal - Landing");

        // T = Toggle "Normal - Running"
        if (Input.GetKeyDown(KeyCode.T))
            if (audioMng.IsPlaying("Normal - Running"))
                audioMng.StopAudio("Normal - Running");
            else
                audioMng.PlayAudio("Normal - Running");

        // Y = Play "Water - Jump"
        if (Input.GetKeyDown(KeyCode.Y))
            audioMng.PlayAudio("Water - Jump");

        // U = Play "Water - Landing"
        if (Input.GetKeyDown(KeyCode.U))
            audioMng.PlayAudio("Water - Landing");

        // I = Toggle "Water - Running"
        if (Input.GetKeyDown(KeyCode.I))
            if (audioMng.IsPlaying("Water - Running"))
                audioMng.StopAudio("Water - Running");
            else                    
                audioMng.PlayAudio("Water - Running");

        // O = Play "Water - Enter"
        if (Input.GetKeyDown(KeyCode.O))
            audioMng.PlayAudio("Water - Enter");

        // P = Play "Water - Exit"
        if (Input.GetKeyDown(KeyCode.P))
            audioMng.PlayAudio("Water - Exit");
    }
}
