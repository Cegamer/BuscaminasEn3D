using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class ModeSelection : MonoBehaviour
{
    private int mapSize;
    private int minesNumber;
    public InputField mapSizeIn;
    public InputField minesNumberIn;
    public int getMapSize() { return mapSize; }
    public int getMinesNumber() { return minesNumber; }
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        
    }
    public void setEasyMode()
    {
        mapSize = 10;
        minesNumber = 10;
        SceneManager.LoadScene("Buscaminas", LoadSceneMode.Single);
    }
    public void setMediumMode()
    {
        mapSize = 16;
        minesNumber = 40;
        SceneManager.LoadScene("Buscaminas", LoadSceneMode.Single);
    }
    public void setHardMode()
    {
        mapSize = 22;
        minesNumber = 99;
        SceneManager.LoadScene("Buscaminas", LoadSceneMode.Single);
    }
}
