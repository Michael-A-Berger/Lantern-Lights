using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public GameObject TextSeries;
    public GameObject ContinueButton;
    public GameObject CurrentText;
    private int CurrentIndex;

    private void Start() {
        CurrentIndex = 0;
        string CurrentName = "Text" + CurrentIndex.ToString();
        CurrentText = TextSeries.transform.Find(CurrentName).gameObject;
        CurrentText.SetActive(true);
    }
    public void LevelOne()
    {
        StartCoroutine(DelayedLoad("berger_02", 0.5f));
    }
    public void NextLine()
    {
        CurrentIndex ++;
        if (CurrentIndex < TextSeries.transform.childCount)
        {
            CurrentText.SetActive(false);
            Debug.Log(TextSeries.transform.childCount.ToString());
            string CurrentName = "Text" + CurrentIndex.ToString();
            CurrentText = TextSeries.transform.Find(CurrentName).gameObject;
            CurrentText.SetActive(true);
        }
        else
        {
            ContinueButton.SetActive(true);
        }
    }

    public void Menu() 
    {
        StartCoroutine(DelayedLoad("Menu", 0.5f));
    }

    public IEnumerator DelayedLoad(string scene, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

}
