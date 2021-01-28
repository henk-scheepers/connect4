using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NN;

public class NeuralNetworkTrainer : MonoBehaviour
{
    AIMinimax aiMinimax = new AIMinimax();
    AIDumb aiDumb = new AIDumb();
    GameState gameState = new GameState(7, 7);
    NeuralNetwork neuralNetwork;

    [SerializeField]
    double learnRate = 0.0085;
    [SerializeField]
    double momentum = 0.005;
    [SerializeField]
    int[] hiddenLayers = new int[]{98, 80};
    [SerializeField]
    int numGames = 100;

    int width;
    int height;

    int inputLayerSize = 0;
    int outputLayerSize = 0;

    public void Setup(int width, int height){
        this.width = width;
        this.height = height;

        inputLayerSize = width * height + 1;
        outputLayerSize = width;
        neuralNetwork = new NeuralNetwork(inputLayerSize, hiddenLayers, 7, learnRate, momentum);
    }

    public void Train(){
        StartCoroutine(TrainCoroutine());
    }

    IEnumerator TrainCoroutine(){

        yield return new WaitForEndOfFrame();
    }

    DataSet PrepareDataSet(){
        double[] values = new double[inputLayerSize];
        double[] targets = new double[outputLayerSize];

        int width = gameState.Board.GetLength(0);
        int height = gameState.Board.GetLength(1);

        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){
                values[y * width + x] = gameState.Board[x, y]; 
            }
        }

        values[values.Length-1] = gameState.LastPlayerToMove;

        int player = 0;
        if(gameState.LastPlayerToMove == 0){
            player = 1;
        }else{
            player = gameState.LastPlayerToMove == 1 ? 2 : 1;
        }
        int move = aiMinimax.GetMove(gameState, player);
        targets[move] = 1;

        return new DataSet(values, targets);
    }
}
