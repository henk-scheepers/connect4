using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int width = 7;
    [SerializeField] int height = 7;
    [SerializeField] GameBoard gameBoard;

    GameState gameState;
    int playerTurn = 1;
    int humanPlayerNumber;

    const int PLAYER1 = 1;
    const int PLAYER2 = 2;

    Evaluator evaluator = new Evaluator();
    AIPlayer aiPlayer = new DumbAI();
    
    // Start is called before the first frame update
    void Awake()
    {
        humanPlayerNumber = Random.Range(0f, 1f) > 0.5f ? PLAYER1 : PLAYER2;

        gameState = new GameState(width, height);
        gameBoard.GenerateBoard(width, height);

        if(humanPlayerNumber != playerTurn){    
            StartCoroutine(AITurn());
        }
    }

    void OnEnable() {
        GameBoardTile.OnTileClicked += OnTileClicked;
    }

    void OnDisable() {
        GameBoardTile.OnTileClicked -= OnTileClicked;
    }

    void OnTileClicked(Vector2Int position){
        if(playerTurn != humanPlayerNumber){
            return;
        }

        //if we are making a valid move then switch player turns
        if(gameState.PlacePlayerDotInColumn(position.x, playerTurn)){
            gameBoard.UpdateBoard(gameState);
            SwitchPlayers();
            Debug.Log(evaluator.Evaluate(gameState));
        }

        StartCoroutine(AITurn());
    }

    void SwitchPlayers(){
        playerTurn = playerTurn == 1 ? PLAYER2 : PLAYER1;
    }

    IEnumerator AITurn(float delay = 1f){
        yield return new WaitForSeconds(delay);
        gameState = aiPlayer.GetMove(gameState, playerTurn);
        gameBoard.UpdateBoard(gameState);
        SwitchPlayers();
    }
}
