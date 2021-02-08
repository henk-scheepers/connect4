using System.Collections.Generic;
using System;
using NN;

public class AINeuralNetwork : AIPlayer{

    Random random = new Random();
    NeuralNetwork neuralNetwork;

    public AINeuralNetwork(NeuralNetwork neuralNetwork){
        this.neuralNetwork = neuralNetwork;
    }

    public override int GetMove(GameState gameState, int player){
        
        DataSet dataSet = gameState.PrepareDataSet(
            neuralNetwork.inputLayer.Count, 
            neuralNetwork.outputLayer.Count
        );
        double[] result  = neuralNetwork.Compute(dataSet.Values);

        double moveValue = double.MinValue;
        int move = -1;
        
        for(int i = 0; i < result.Length; i++){
            if(result[i] > moveValue){
                moveValue = result[i];
                move = i;
            }
        }

        return move;
    }
}