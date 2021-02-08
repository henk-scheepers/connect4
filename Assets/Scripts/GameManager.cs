using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NN;

public class GameManager : MonoBehaviour
{
    [SerializeField] int width = 7;
    [SerializeField] int height = 7;
    [SerializeField] GameBoard gameBoard;
    [SerializeField] OpponentType opponentType;

    [SerializeField] int minimaxDepth = 6;
    [SerializeField] string neuralNetworkFileName;

    GameState gameState;
    int playerTurn = 1;
    int humanPlayerNumber;

    const int PLAYER1 = 1;
    const int PLAYER2 = 2;

    Evaluator evaluator = new Evaluator();
    AIPlayer aiPlayer;

    bool gameEnded = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        humanPlayerNumber = Random.Range(0f, 1f) > 0.5f ? PLAYER1 : PLAYER2;

        gameState = new GameState(width, height);
        gameBoard.GenerateBoard(width, height);

        if(opponentType != OpponentType.HUMAN){
            InitializeAIPlayer();

            if(humanPlayerNumber != playerTurn){    
                StartCoroutine(AITurn());
            }
        }
    }

    void InitializeAIPlayer(){
        switch(opponentType){
            case OpponentType.DUMB_AI: {
                aiPlayer = new AIDumb(); break;
            }
            case OpponentType.MINIMAX_AI : {
                AIMinimax minimax = new AIMinimax(); 
                minimax.MaxDepth = minimaxDepth;
                aiPlayer = minimax; 
                break;
            }
            case OpponentType.NEURAL_NETWORK_AI : {
                NeuralNetwork nn = Utils.ReadFromBinaryFile<NeuralNetwork>(Application.streamingAssetsPath + "/" + neuralNetworkFileName);
                aiPlayer = new AINeuralNetwork(nn);
                break;
            }
        }
    }

    void OnEnable() {
        GameBoardTile.OnTileClicked += OnTileClicked;
    }

    void OnDisable() {
        GameBoardTile.OnTileClicked -= OnTileClicked;
    }

    void OnTileClicked(Vector2Int position){
        if(playerTurn != humanPlayerNumber && opponentType != OpponentType.HUMAN){
            return;
        }

        //if we are making a valid move then switch player turns
        if(gameState.PlacePlayerDotInColumn(position.x, playerTurn)){
            gameBoard.UpdateBoard(gameState);
            SwitchPlayers();
        }

        if(opponentType != OpponentType.HUMAN){
            StartCoroutine(AITurn());
        }
    }

    void SwitchPlayers(){
        int score;
        gameEnded = evaluator.Evaluate(gameState, out score);
        Debug.Log(score);
        playerTurn = playerTurn == 1 ? PLAYER2 : PLAYER1;
    }

    IEnumerator AITurn(float delay = 1f){
        yield return new WaitForSeconds(delay);
        int move = aiPlayer.GetMove(gameState, playerTurn);
        if(move != -1){
            gameState.PlacePlayerDotInColumn(move, playerTurn);
        }
        gameBoard.UpdateBoard(gameState);
        SwitchPlayers();
    }
}

public enum OpponentType{
    DUMB_AI,
    MINIMAX_AI,
    NEURAL_NETWORK_AI,
    HUMAN
}
