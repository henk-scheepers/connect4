using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;

    GameBoardTile[,] gameBoardTiles;
    
    public void GenerateBoard(int width, int height){
        gameBoardTiles = new GameBoardTile[width, height];
        Vector2 start = new Vector2(-width/2f, -height/2f) + Vector2.one * 0.5f;

        for(int x = 0; x < gameBoardTiles.GetLength(0); x++){
            for(int y = 0; y < gameBoardTiles.GetLength(1); y++){
                GameBoardTile newTile = Instantiate(tilePrefab).GetComponent<GameBoardTile>();
                newTile.transform.parent = transform;
                newTile.transform.position = start + new Vector2(x, y);
                newTile.SetPosition(x, y);
                gameBoardTiles[x, y] = newTile;
            }
        }
    }

    public void UpdateBoard(GameState state){
        if(state.Board.GetLength(0) != gameBoardTiles.GetLength(0))
            return;

        if(state.Board.GetLength(1) != gameBoardTiles.GetLength(1)){
            return;
        }

        for(int x = 0; x < gameBoardTiles.GetLength(0); x++){
            for(int y = 0; y < gameBoardTiles.GetLength(1); y++){
                GameBoardTile gameBoardTile = gameBoardTiles[x, y];
                gameBoardTile.Type = (TileType) state.Board[x, y];
            }
        }
    }
}
