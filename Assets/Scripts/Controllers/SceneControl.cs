﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private static SceneControl instance;

    private static string masterName = "Master";
    private static string mainName = "Main";
    private static string lobbyName = "Lobby";
    private static string levelName = "LevelSelect";
    private static string endName = "EndGame";

    private static bool inLoad = false;

    void Start()
    {
        instance = this;
        instance.StartCoroutine(instance.Setup());
    }
    
    public static void ToLevelSelect()
    {
        SoundManager.PlayLevelSelectSound();
        instance.StartCoroutine(instance.LevelSelect_());
    }

    public static void ToGame(int id)
    {
        Controller.EndAllVibration();
        SoundManager.PlayStartSound();
        instance.StartCoroutine(instance.Game_(id));
    }

    public static void ToEndGame()
    {
        Controller.EndAllVibration();
        SoundManager.PlayEndSound();
        instance.StartCoroutine(instance.EndGame_());
    }

    public static void Restart()
    {
        instance.StartCoroutine(instance.Lobby_());
    }

    IEnumerator Setup()
    {
        foreach (string s in new string[] {mainName, lobbyName, levelName, endName})
        {
            if (SceneManager.GetSceneByName(s).isLoaded)
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(s));
        }

        yield return SceneManager.LoadSceneAsync(lobbyName, LoadSceneMode.Additive);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(lobbyName));
    }

    IEnumerator LevelSelect_()
    {
        StartCoroutine(ToLevelExclusive(levelName));
        while (inLoad)
            yield return null;
        CurrentlySelecting.Init(); // TODO: Make this a general call to LevelSelectUI init or something
    }

    IEnumerator Game_(int id)
    {
        StartCoroutine(ToLevelExclusive(mainName));
        while (inLoad)
            yield return null;
        LevelLoader.Create(id);
        InGameUI.Init();
    }

    IEnumerator EndGame_()
    {
        StartCoroutine(ToLevelExclusive(endName));
        while (inLoad)
            yield return null;
        EndGameUI.Init();
    }

    IEnumerator Lobby_()
    {
        Persistent.Clear();
        StartCoroutine(ToLevelExclusive(lobbyName));
        while (inLoad)
            yield return null;
    }

    IEnumerator ToLevelExclusive(string name)
    {
        inLoad = true;
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        AsyncOperation loading = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        yield return loading;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
        inLoad = false;
    }
}
