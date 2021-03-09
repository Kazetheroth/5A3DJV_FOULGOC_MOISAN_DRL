﻿using System;
using System.Collections.Generic;
using GridWORLDO;
using Interfaces;
using TMPro;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public enum GameType
    {
        GridWORLDO,
        TicTacTard,
        Soooookolat,
    }

    [SerializeField] private GameObject parentGeneratedScene;

    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject endGoalPrefab;
    [SerializeField] private GameObject groundCellPrefab;
    [SerializeField] public GameObject planeRightArrowPrefab;
    [SerializeField] public GameObject planeBotArrowPrefab;
    [SerializeField] public GameObject planeTopArrowPrefab;
    [SerializeField] public GameObject planeLeftArrowPrefab;

    [HideInInspector]
    public bool isHuman = true;
    [HideInInspector]
    public bool isAndroid;
    
    public static GameObject currentPlayerObject;
    private IGame game;

    public static Controller instance;

    private void Start()
    {
        instance = this;
    }

    public void StartGame(GameType gameType)
    {
        DestroyOldScene();
        
        switch (gameType)
        {
            case GameType.Soooookolat:
//                game = new SoooookolatGame();
                break;
            case GameType.TicTacTard:
//                game = new TicTacTardGame();
                break;
            case GameType.GridWORLDO:
                game = new GridWORDOGame();
                break;
        }

        game.InitGame(isHuman);
        GenerateScene();
        game.InitIntent();
    }

    public void DestroyOldScene()
    {
        int safeLoopIteration = 0;

        foreach (Transform child in parentGeneratedScene.transform)
        {
            Destroy(child.gameObject);
        }

        game = null;
    }
    
    public static void InstantiateArrowByIntent(Intent intent, int x, int y, float stateValue)
    {
        GameObject go = null;
        
        switch (intent)
        {
            case Intent.WantToGoBot:
                go = Instantiate(instance.planeBotArrowPrefab, instance.parentGeneratedScene.transform);
                break;
            case Intent.WantToGoLeft:
                go = Instantiate(instance.planeLeftArrowPrefab, instance.parentGeneratedScene.transform);
                break;
            case Intent.WantToGoRight:
                go = Instantiate(instance.planeRightArrowPrefab, instance.parentGeneratedScene.transform);
                break;
            case Intent.WantToGoTop:
                go = Instantiate(instance.planeTopArrowPrefab, instance.parentGeneratedScene.transform);
                break;
            default:
                Debug.Log(intent);
                break;
        }

        if (go)
        {
            go.transform.position = new Vector3(x, 1f, y);
            go.transform.GetChild(0).GetComponent<TextMeshPro>().text = stateValue.ToString();
        }
    }

    private void Update()
    {
        game?.UpdateGame();
    }

    private void GenerateScene()
    {
        List<List<ICell>> cells = game?.GetCells();

        if (cells != null)
        {
            foreach (List<ICell> cellsPerLine in cells)
            {
                foreach (ICell cell in cellsPerLine)
                {
                    GameObject instantiateGo;
                    Vector3 pos = new Vector3(cell.GetPosition().x, 0, cell.GetPosition().y);
    
                    switch (cell.GetCellType())
                    {
                        case CellType.Obstacle:
                            instantiateGo = Instantiate(wallPrefab, parentGeneratedScene.transform);
                            instantiateGo.transform.position = pos;
                            cell.SetCellGameObject(instantiateGo);
                            break;
                        case CellType.Player:
                            currentPlayerObject = instantiateGo = Instantiate(playerPrefab, parentGeneratedScene.transform);
                            instantiateGo.transform.position = pos;

                            instantiateGo = Instantiate(groundCellPrefab, parentGeneratedScene.transform);
                            instantiateGo.transform.position = pos;
                            cell.SetCellGameObject(instantiateGo);
                            break;
                        case CellType.EndGoal:
                            instantiateGo = Instantiate(endGoalPrefab, parentGeneratedScene.transform);
                            instantiateGo.transform.position = new Vector3(cell.GetPosition().x, 0, cell.GetPosition().y);
                            cell.SetCellGameObject(instantiateGo);
                            break;
                        case CellType.Empty:
                            instantiateGo = Instantiate(groundCellPrefab, parentGeneratedScene.transform);
                            instantiateGo.transform.position = new Vector3(cell.GetPosition().x, 0, cell.GetPosition().y);
                            cell.SetCellGameObject(instantiateGo);
                            break;
                    }
                }
            }
        }
    }
}