using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

public class Debug_SceneChange : MonoBehaviour
{
    [Header("Public Properties")]
    public KeyCode openCloseMenu = KeyCode.BackQuote;
    public List<KeyCode> menuInputs;
    public bool startWindowOpen = false;

    [Header("UI Properties")]
    [Space(10)]
    [SerializeField]
    protected Image panel;
    [SerializeField]
    protected Text panelText;

    [Header("Editor Settings")]
    [Space(10)]
    [SerializeField]
    [Range(0, 1)]
    float previewOpacity = 1f;

    [Header("Private Properties [NO REAL-TIME EDIT]")]
    [SerializeField]
    [Range(0, 1)]
    private float panelOpacity = 0f;
    [SerializeField]
    private int startSceneNum = 0;

    // Start()
    public void Start()
    {
        if (startWindowOpen)
            ChangeOpacity(1);
        else
            ChangeOpacity(0);
    }

    // Update()
    public void Update()
    {
        // IF the open/close key was just pressed, flip the menu visibility
        if (Input.GetKeyDown(openCloseMenu))
        {
            if (panelOpacity > 0)
                ChangeOpacity(0);
            else
                ChangeOpacity(1);
        }

        // Checking if any of the Input keys are pressed...
        for (int num = 0; panel.color.a == 1 && num < menuInputs.Count; num++)
        {
            if (Input.GetKeyDown(menuInputs[num]))
            {
                if (num == 0)
                {
                    startSceneNum = (startSceneNum + menuInputs.Count - 1) % EditorBuildSettings.scenes.Length;
                    FillInOptions();
                }
                else
                {
                    LoadScene(num + startSceneNum);
                }
            }
        }
    }

    // ChangeOpacity()
    public void ChangeOpacity(float newOpacity)
    {
        // Changing the panel color
        Color clr = panel.color;
        clr.a = newOpacity;
        panel.color = clr;

        // Changing the text color
        clr = panelText.color;
        clr.a = newOpacity;
        panelText.color = clr;

        // Changing the panel size (based on menu inputs)
        panel.rectTransform.sizeDelta = new Vector2(panel.rectTransform.sizeDelta.x, 70 + menuInputs.Count * 25);

        // Setting the panel opacity value
        panelOpacity = newOpacity;

        // IF the new opacity is greater than zero, fill in the menu options
        if (newOpacity > 0)
            FillInOptions();
    }

    // LoadScene()
    private void LoadScene(int index)
    {
        // IF the scene index is too high / low, complain and exit early
        if (index < 0 && index > EditorBuildSettings.scenes.Length - 1)
        {
            Debug.LogError("Scene [index] is too low / high!");
            return;
        }

        // Loading the indicated scene
        SceneManager.LoadSceneAsync(EditorBuildSettings.scenes[index].path);
    }

    // FillInOptions()
    private void FillInOptions()
    {
        string listOfScenes = "<b>Debug Scene Switcher:</b>\n\n";

        // Fillling out the first option ("Next" / "First Page")
        listOfScenes += "[" + menuInputs[0] + "] - ";
        if (startSceneNum + (menuInputs.Count - 1) < EditorBuildSettings.scenes.Length)
            listOfScenes += "(Next)\n";
        else
            listOfScenes += "(First Page)\n";

        // Cataloguing the scenes available in the menu
        string sceneName = "";
        for (int num = 0; num + startSceneNum < EditorBuildSettings.scenes.Length && num < (menuInputs.Count - 1); num++)
        {
            if (num > 0)
                listOfScenes += "\n";
            listOfScenes += "[" + menuInputs[num + 1] + "] - ";
            sceneName = EditorBuildSettings.scenes[startSceneNum + num].path;
            listOfScenes += sceneName.Substring(sceneName.LastIndexOf("/") + 1);
        }

        // Setting the box text
        panelText.text = listOfScenes;
    }

    // OnValidate()
    private void OnValidate()
    {
        ChangeOpacity(previewOpacity);
    }
}
