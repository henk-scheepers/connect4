using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMinimax : AIPlayer {

    public int MaxDepth{ get; set; } = 6;

    Evaluator evaluator = new Evaluator();

    public override GameState GetMove(GameState gameState, int player){
        return Minimax(gameState, MaxDepth, player);
    }

    GameState Minimax(GameState gameState, int depth, int player){

        bool isBoardFull = gameState.IsBoardFull;

        int evaluation;
        bool hasWinner = evaluator.Evaluate(gameState, out evaluation);

        //the game is done
        if (isBoardFull || hasWinner || depth == 0) {
            return gameState;
        }

        int bestEvaluation = player == 1 ? int.MinValue : int.MaxValue;
        GameState bestGameState = null;

        List<GameState> possibleStates = gameState.GetAllPossibleNextStates(player);

        foreach (GameState currentState in possibleStates) {

            GameState newGameState = Minimax(currentState, depth-1, player == 1 ? 2 : 1);

            int newEvaluation;
            evaluator.Evaluate(newGameState, out newEvaluation);

            if (player == 1) {
                if(newEvaluation > bestEvaluation) {
                    bestEvaluation = newEvaluation;
                    bestGameState = currentState;
                }
            }
            else {
                if (newEvaluation < bestEvaluation) {
                    bestEvaluation = newEvaluation;
                    bestGameState = currentState;
                }
            }
        }
        return bestGameState;
    }
}
