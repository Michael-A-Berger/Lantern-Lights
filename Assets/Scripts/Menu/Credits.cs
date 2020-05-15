using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public void back()
    {
        StartCoroutine(DelayedLoad("Menu", 0.5f));
    }

    public void resources_DawnLike()
    {
        Application.OpenURL("https://opengameart.org/content/dawnlike-16x16-universal-rogue-like-tileset-v181");
    }

    public void resources_SmallTypeWriting()
    {
        Application.OpenURL("https://www.1001freefonts.com/small-type-writing.font");
    }

    public void resources_AllAudio()
    {
        Application.OpenURL("https://docs.google.com/document/d/1XGTAHmtngpTrt33yet_OhuqyoXbZzDjPeKbCTFqWsEs/");
    }

    public IEnumerator DelayedLoad(string scene, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
