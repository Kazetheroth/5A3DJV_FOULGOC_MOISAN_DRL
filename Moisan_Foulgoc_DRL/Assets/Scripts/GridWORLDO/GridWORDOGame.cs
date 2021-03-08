﻿using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using static Controller;

namespace GridWORLDO
{
    public class GridWORDOGame : IGame
    {   
        private IPlayer player;
        private IPlayerIntent playerIntent;

        private List<List<ICell>> cells;

        private const int MAX_CELLS_PER_LINE = 4;
        private const int MAX_CELLS_PER_COLUMN = 4;

        public bool InitGame(bool isHuman)
        {
            cells = new List<List<ICell>>();

            for (int i = 0; i < MAX_CELLS_PER_LINE; ++i)
            {
                List<ICell> cellsPerLine = new List<ICell>();
                
                for (int j = 0; j < MAX_CELLS_PER_COLUMN; ++j)
                {
                    cellsPerLine.Add(new GridWorldCell(
                        new Vector3(i, 0, j),
                        Random.Range(0, 10) > 8 ? CellType.Obstacle : CellType.Empty, 
                        1));
                }

                cells.Add(cellsPerLine);
            }

            int xGoal = Random.Range(0, MAX_CELLS_PER_LINE);
            int yGoal = Random.Range(0, MAX_CELLS_PER_COLUMN);

            cells[xGoal][yGoal].SetCellType(CellType.EndGoal);
            cells[xGoal][yGoal].SetReward(100000);

            int xPlayer = 0;
            int yPlayer = 0;

            do
            {
                xPlayer = Random.Range(0, MAX_CELLS_PER_LINE);
                yPlayer = Random.Range(0, MAX_CELLS_PER_COLUMN);
            } while (xPlayer == xGoal && yPlayer == yGoal);

            cells[xPlayer][yPlayer].SetCellType(CellType.Player);
            cells[xPlayer][yPlayer].SetReward(0);

            player = new GridWoldPlayer();
            player.SetCell(cells[xPlayer][yPlayer]);

            if (isHuman)
            {
                playerIntent = new GridWorldIntent();
            }
            else
            {
                playerIntent = new GridWorldAndroidIntent();
            }

            return true;
        }

        public List<List<ICell>> GetCells()
        {
            return cells;
        }

        public bool UpdateGame()
        {
            bool isMoving = false;

            Intent intent = playerIntent.GetPlayerIntent();

            switch (intent)
            {
                case Intent.WantToGoBot:
                    isMoving = GetPlayer().WantToGoBot(GetCells());
                    break;
                case Intent.WantToGoLeft:
                    isMoving = GetPlayer().WantToGoLeft(GetCells());
                    break;
                case Intent.WantToGoTop:
                    isMoving = GetPlayer().WantToGoTop(GetCells());
                    break;
                case Intent.WantToGoRight:
                    isMoving = GetPlayer().WantToGoRight(GetCells());
                    break;
            }

            if (isMoving && GetPlayer().GetCell().GetCellType() == CellType.EndGoal)
            {
                EndGame();
            }

            currentPlayerObject.transform.position = player.GetPosition();

            return true;
        }

        public bool EndGame()
        {
            throw new System.NotImplementedException();
        }

        public IPlayer GetPlayer()
        {
            return player;
        }
    }
}