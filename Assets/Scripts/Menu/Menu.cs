using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Play()
    {
        StartCoroutine(DelayedLoad("Intro", 0.5f));
    }

    public void Credits()
    {
        StartCoroutine(DelayedLoad("Credits", 0.5f));
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ControlConfig()
    {
        StartCoroutine(DelayedLoad("ControlConfig", 0.5f));
    }

    public IEnumerator DelayedLoad(string scene, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
