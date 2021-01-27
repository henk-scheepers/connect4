using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluator
{
    public const int WIN_SCORE = 10000;

    public bool Evaluate(GameState state, out int score){
        score = 0;

        int[] scores = new int[4];

        scores[0] += EvaluateColumns(state);
        scores[1] += EvaluateRows(state);
        scores[2] += EvaluateDiagonalDown(state);
        scores[3] += EvaluateDiagonalUp(state);

        for(int i = 0; i < scores.Length; i++){
            if(Mathf.Abs(scores[i]) == WIN_SCORE){
                score = scores[i];
                return true;
            }
            score += scores[i];
        }

        return false;
    }

    //evaluate the columns of the board
    int EvaluateColumns(GameState state){
        int score = 0;

        for(int x = 0; x < state.Board.GetLength(0); x++){
            int[] sequence = new int[state.Board.GetLength(1)];

            for(int y = 0; y < state.Board.GetLength(1); y++){
                sequence[y] = state.Board[x, y];
            }

            int sequenceScore = EvaluateSequence(sequence);
            if(Mathf.Abs(sequenceScore) == WIN_SCORE){
                return sequenceScore;
            }
            score += sequenceScore;
        }
        return score;
    }

    //evaluate the rows of the board
    int EvaluateRows(GameState state){
        int score = 0;

        for(int y = 0; y < state.Board.GetLength(1); y++){
            int[] sequence = new int[state.Board.GetLength(0)];

            for(int x = 0; x < state.Board.GetLength(0); x++){
                sequence[x] = state.Board[x, y];
            }

            int sequenceScore = EvaluateSequence(sequence);
            if(Mathf.Abs(sequenceScore) == WIN_SCORE){
                return sequenceScore;
            }
            score += sequenceScore;
        }
        return score;
    }

    //evaluate the board diagonally from bottom to top
    int EvaluateDiagonalUp(GameState state){
        int score = 0;

        int width = state.Board.GetLength(0);
        int height = state.Board.GetLength(1);

        int sx = 0;
        int sy = height - 3;

        int sequenceLength = 0;

        while(sx < width - 3){
            if(sy > 0){
                sy--;
                sequenceLength = height - sy;
            }else{
                sx++;
                sequenceLength = width - sx;
            }

            int[] sequence = new int[sequenceLength];
            int sequenceIndex = 0;

            int y = sy;
            for(int x = sx; x < width && y < height; x++){
                sequence[sequenceIndex] = state.Board[x, y];
                sequenceIndex++;
                y++;
            }

            int sequenceScore = EvaluateSequence(sequence);
            if(Mathf.Abs(sequenceScore) == WIN_SCORE){
                return sequenceScore;
            }
            score += sequenceScore;
        }

        return score;
    }   

    //evaluate the board diagonally from top to bottom
    int EvaluateDiagonalDown(GameState state){
        int score = 0;

        int width = state.Board.GetLength(0);
        int height = state.Board.GetLength(1);

        int sx = 0;
        int sy = 2;

        int sequenceLength = 0;

        while(sx < 3){
            if(sy < height - 1){
                sy++;
                sequenceLength = sy + 1;
            }else{
                sx++;
                sequenceLength = width - sx;
            }

            int[] sequence = new int[sequenceLength];
            int sequenceIndex = 0;

            int y = sy;
            for(int x = sx; x < width && y >= 0; x++){
                sequence[sequenceIndex] = state.Board[x, y];
                sequenceIndex++;
                y--;
            }

            int sequenceScore = EvaluateSequence(sequence);
            if(Mathf.Abs(sequenceScore) == WIN_SCORE){
                return sequenceScore;
            }
            score += sequenceScore;
        }

        return score;
    }

    //evaluate a sequence of tiles
    int EvaluateSequence(int[] sequence){
        int score = 0;
        for(int i = 0; i <= sequence.Length - 4; i++){
            int[] quartet = new int[4];
            for(int j = 0; j < 4; j++){
                quartet[j] = sequence[i + j];
            }

            int quartetScore = EvaluateQuartet(quartet);
            if(Mathf.Abs(quartetScore) == WIN_SCORE){
                return quartetScore;
            }

            score += quartetScore;
        }
        return score;
    }

    //evaluate 4 consecutive tiles
    int EvaluateQuartet(int[] quartet){
        int p1Count = 0;
        int p2Count = 0;

        for(int i = 0; i < quartet.Length; i++){
            if(quartet[i] == 1){
                p1Count++;
            }else if(quartet[i] == 2){
                p2Count++;
            }
        }

        //quartet contains both 1 and 2 - no points
        if(p1Count > 0 && p2Count > 0){
            return 0;
        //only 1 in quartet 
        }else if(p1Count > 0){
            return p1Count == 4 ? WIN_SCORE : p1Count * p1Count;
        //only 2 in quartet
        }else if(p2Count > 0){
            return p2Count == 4 ? -WIN_SCORE : -(p2Count * p2Count);
        }
        //quartet is empty
        return 0;
    }
}
