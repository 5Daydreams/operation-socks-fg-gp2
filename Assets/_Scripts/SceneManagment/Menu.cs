using System;
using System.Collections;
using System.Collections.Generic;
using _Code.Scriptables.TrackableValue;
using UnityEngine;

public class Menu : SceneFunctions
{
    [SerializeField] private SceneIndexes sceneIndexes;

    [SerializeField] private TrackableFloat timer;
    [SerializeField] private SockRegistry sockRegistry;


    private void ResetSavedValues()
    {
        timer.Value = 0;
        sockRegistry.ResetFullInventory();
    }
    
    public void MainButton()
    {
        ResetSavedValues();
        LoadScene(sceneIndexes.mainSceneIndex);
    }
    
    public void StartButton()
    {
        LoadScene(sceneIndexes.startSceneIndex);
    }

    public void RestartButton()
    {
        ResetSavedValues();
        LoadScene(sceneIndexes.startSceneIndex);
    }

    public void PauseButton()
    {
        Pause(gameObject);
    }
    
    public void ResumeButton()
    {
        Resume(gameObject);
    }

    public void QuitButton()
    {
        ResetSavedValues();
        Quit();
    }
}
