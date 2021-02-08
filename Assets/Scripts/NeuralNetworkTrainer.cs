using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NN;

public class NeuralNetworkTrainer : MonoBehaviour
{ 

    AIMinimax aiMinimax = new AIMinimax();
    AIPlayer opponentAI = new AIDumb();
    GameState gameState;
    NeuralNetwork neuralNetwork;
    Evaluator evaluator = new Evaluator();

    [SerializeField]
    int width = 7;
    [SerializeField]
    int height = 7;

    [SerializeField]
    double learnRate = 0.0085;
    [SerializeField]
    double momentum = 0.005;
    [SerializeField]
    int[] hiddenLayers = new int[]{98, 80};
    [SerializeField]
    int numGames = 10000;
    [SerializeField]
    int saveInterval = 100;
    [SerializeField] GameBoard gameBoard;

    int inputLayerSize = 0;
    int outputLayerSize = 0;

    int numGamesPlayed = 0;

    int numTestGamesPlayed = 0;
    int numNNGamesWon = 0;
    int numMinimaxGamesWon = 0;

    void Start(){
        gameBoard.GenerateBoard(width, height);
        Setup(width, height);
        Train();
    }

    public void Setup(int width, int height){
        this.width = width;
        this.height = height;

        inputLayerSize = width * height + 1;
        outputLayerSize = width;
        neuralNetwork = new NeuralNetwork(inputLayerSize, hiddenLayers, 7, learnRate, momentum);

        aiMinimax.MaxDepth = 8;
    }

    public void Train(){
        numGamesPlayed = 0;
        StartCoroutine(PlayTrainingGameCoroutine());
    }

    IEnumerator PlayTrainingGameCoroutine(){
        Debug.Log("GameState played: " + numGamesPlayed);
        gameState = new GameState(width, height);

        List<double> errors = new List<double>();
        int playerTurn = 1;

        AIPlayer p1 = new AIDumb();
        AIPlayer p2 = new AIDumb();

        bool isGameEnd = false;
        //repeat this until the game is finished.
        int prevScore = 0;
        while(!isGameEnd){
            int move;

            if(playerTurn == 1){
                move = p1.GetMove(gameState, playerTurn);
            }else{
                move = p2.GetMove(gameState, playerTurn);
            }

            GameState oldGameState = gameState.Duplicate();
            gameState.PlacePlayerDotInColumn(move, playerTurn);

            int score;
            bool isWon = evaluator.Evaluate(gameState, out score);
            bool isBoardFull = gameState.IsBoardFull;

            int scoreDelta = score - prevScore;
            //a positive score change is good for a maximizing player. a negative score change is good for minimizing player.
            bool isPositiveMove = (scoreDelta > 0 && playerTurn == 1) || (scoreDelta < 0 && playerTurn == 2);

            playerTurn = playerTurn == 1 ? 2 : 1;

            isGameEnd = isWon || gameState.IsBoardFull;

            if(isGameEnd || isPositiveMove){
                DataSet dataset = oldGameState.PrepareDataSet(inputLayerSize, outputLayerSize, move);
                neuralNetwork.Train(dataset, errors);
            }

            gameBoard.UpdateBoard(gameState);

            //yield return new WaitForSeconds(0.5f);
            yield return new WaitForEndOfFrame();
        }
        numGamesPlayed++;

        if(numGamesPlayed % saveInterval == 0){
            Debug.Log("Trainging Time: " + Time.time);
            string filePath = Application.streamingAssetsPath + "/nn-connect-4-" + numGamesPlayed + "-" + System.DateTime.Now.ToFileTime() + ".txt";
            Debug.Log(filePath);
            Utils.WriteToBinaryFile<NeuralNetwork>(filePath, neuralNetwork);
            neuralNetwork = Utils.ReadFromBinaryFile<NeuralNetwork>(filePath);
            Debug.Log(neuralNetwork);
            StartCoroutine(PlayTestGameCoroutine());
        }else if(numGamesPlayed < numGames){
            StartCoroutine(PlayTrainingGameCoroutine());
        }
    }

    IEnumerator PlayTestGameCoroutine(){
        AINeuralNetwork nnPlayer = new AINeuralNetwork(neuralNetwork);
        GameState testGameState = new GameState(width, height);
        bool isTestGameEnd = false;
        numTestGamesPlayed++;

        int minimaxPlayerNumber = Random.Range(1, 3);
        int playerTurn = 1;

        int nnPlayerNumber = minimaxPlayerNumber == 1 ? 2 : 1;
        Debug.Log("Minimax (" + minimaxPlayerNumber + ") vs. NN (" + nnPlayerNumber + ")");

        while(!isTestGameEnd){
            int move;

            if(playerTurn == minimaxPlayerNumber){
                move = aiMinimax.GetMove(gameState, playerTurn);
            }else{
                move = opponentAI.GetMove(gameState, playerTurn);
            }

            gameState.PlacePlayerDotInColumn(move, playerTurn);

            int score;
            bool isWon = evaluator.Evaluate(gameState, out score);;
            bool isBoardFull = gameState.IsBoardFull;
            
            if(isBoardFull && !isWon){
                Debug.Log("DRAW");
            }
            else if(isWon){
                if((score > 0 && nnPlayerNumber == 1) || (score < 0 && nnPlayerNumber == 2)){
                    Debug.Log("NN WON");
                    numNNGamesWon++;
                }else{
                    Debug.Log("Minimax WON");
                    numMinimaxGamesWon++;
                }
                Debug.Log("NN win percentage: " +  Mathf.Round(numNNGamesWon/(float)numTestGamesPlayed * 100f) + "%");
            }

            playerTurn = playerTurn == 1 ? 2 : 1;

            isTestGameEnd = isWon || gameState.IsBoardFull;

            gameBoard.UpdateBoard(gameState);

            yield return new WaitForEndOfFrame();
        }

        if(numGamesPlayed < numGames){
            StartCoroutine(PlayTrainingGameCoroutine());
        }
    }
}
