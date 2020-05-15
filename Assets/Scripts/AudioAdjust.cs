using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioAdjust : MonoBehaviour
{
    // Private Properties
    private AudioManager audioMng;
    private Button adjustButton;
    private float percentage = 100f;

    // Start is called before the first frame update
    void Start()
    {
        audioMng = FindObjectOfType<AudioManager>();
        adjustButton = GetComponent<Button>();
        adjustButton.onClick.AddListener(UpdateVolume);
    }

    // UpdateVolume()
    private void UpdateVolume()
    {
        percentage += 10f;
        if (percentage > 100f)
            percentage = 0f;
        UpdateButtonText();
        audioMng.ResetVolume(percentage);
    }

    // UpdateButtonText()
    private void UpdateButtonText()
    {
        Text adjustText = adjustButton.GetComponentInChildren<Text>();
        adjustText.text = "<b>Volume: " + percentage + "%</b>";
    }
}
