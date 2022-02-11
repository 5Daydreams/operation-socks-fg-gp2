using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneDebug : SceneFunctions
{
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private SockRegistry sockRegistry;
    [SerializeField] private SceneIndexes sceneIndexes;

    void Start()
    {
        Resume(pauseScreen);
    }
    
    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            Pause(pauseScreen);
        }

        if (keyboard.numpad1Key.wasPressedThisFrame)
        {
            LoadScene(0);
        }
        if (keyboard.numpad2Key.wasPressedThisFrame)
        {
            LoadScene(1);
        }
        if (keyboard.numpad3Key.wasPressedThisFrame)
        {
            LoadScene(2);
        }
        
        if (sockRegistry.CheckIfWon())
        {
            LoadScene(sceneIndexes.winSceneIndex);
        }
    }
    
    private void OnEnable()
    {
        EventManager.ONGameRestart += OnRestart;
    }

    private void OnDisable()
    {
        EventManager.ONGameRestart -= OnRestart;
    }

    private void OnRestart()
    {
        ReloadLevel();
    }

}
