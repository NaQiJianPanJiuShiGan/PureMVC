﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureMVC.Patterns;
using SUIFW;

public class Ctrl_Golds : MonoBehaviour {
    private Model_GameDataProxy Proxy;
    private bool IsStartGame;
    private void Start()
    {
        
    }
    public void StartGame()
    {
        IsStartGame = true;     
    }
    public void StopGame()
    {
        IsStartGame = false;
    }
    private void Update()
    {
        Log.Write(IsStartGame.ToString());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print(IsStartGame);
        //if (IsStartGame==true)
        //{
            if (collision.tag == "Player")
            {
                Proxy = Facade.Instance.RetrieveProxy("Model_GameDataProxy") as Model_GameDataProxy;
                Proxy.AddScore();
            }
        //}
    }
}
