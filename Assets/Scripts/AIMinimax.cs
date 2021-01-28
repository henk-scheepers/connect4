using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMinimax : AIPlayer {

    public int MaxDepth{ get; set; } = 6;

    Evaluator evaluator = new Evaluator();

    public override int GetMove(GameState gameState, int player){
        return Minimax(gameState, MaxDepth, player, int.MinValue, int.MaxValue);
    }

    int Minimax(GameState gameState, int depth, int player, int alpha, int beta){

        bool isBoardFull = gameState.IsBoardFull;

        int evaluation;
        bool hasWinner = evaluator.Evaluate(gameState, out evaluation);

        //the game is done
        if (isBoardFull || hasWinner || depth == 0) {
            return gameState.LastMove;
        }

        int bestEvaluation = player == 1 ? int.MinValue : int.MaxValue;
        int bestMove = -1;

        List<GameState> nextGameStates = gameState.GetAllPossibleNextStates(player);

        foreach (GameState currentState in nextGameStates) { 

            int newMove = Minimax(currentState, depth-1, player == 1 ? 2 : 1, alpha, beta);

            GameState newGameState = gameState.Duplicate();
            newGameState.PlacePlayerDotInColumn(newMove, player);   

            int newEvaluation;
            evaluator.Evaluate(newGameState, out newEvaluation);

            if (player == 1) {
                if(newEvaluation > bestEvaluation) {
                    bestEvaluation = newEvaluation;
                    bestMove = newMove;
                }

                //alpha beta pruning for maximizing player
                alpha = Mathf.Max(alpha, bestEvaluation);
                if(alpha >= beta){
                    break;
                }
            }
            else {
                if (newEvaluation < bestEvaluation) {
                    bestEvaluation = newEvaluation;
                    bestMove = newMove;
                }

                //alpha beta pruning for minimizing player
                beta = Mathf.Min(beta, bestEvaluation);
                if(beta <= alpha){
                    break;
                }
            }
        }
        return bestMove;
    }
}
