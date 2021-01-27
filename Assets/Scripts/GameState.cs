using System.Collections.Generic;

public class GameState
{
    int[,] board;
    int player;
    int visits;
    float score;

    public int[,] Board{
        get{ return board; }
    }

    public bool IsBoardFull{
        get{
            //if there are no empty cells in the top row, the board is full
            int y = board.GetLength(1)-1;
            for(int x = 0; x < board.GetLength(0); x++){
                if(board[x, y] == 0){
                    return false;
                }
            }
            return true;
        }
    }

    public GameState(int width, int height){
        board = new int[width, height];
        Reset();
    }

    public GameState(int[,] board){
        this.board = board;
    }

    public void Reset(){
        for(int x = 0; x < board.GetLength(0); x++){
            for(int y = 0; y < board.GetLength(1); y++){
                board[x, y] = 0;
            }
        }
    }

    public GameState Duplicate(){
        return new GameState(board.Clone() as int[,]);
    }

    public List<GameState> GetAllPossibleNextStates(int player){
        List<GameState> possibleStates = new List<GameState>();

        for(int i = 0; i < board.GetLength(0); i++){
            GameState copy = this.Duplicate();
            if(copy.PlacePlayerDotInColumn(i, player)){
                possibleStates.Add(copy);
            }
        }

        return possibleStates;
    }

    //returns a bool specifying whether the attempted move was valid
    public bool PlacePlayerDotInColumn(int column, int player){
        if(player < 1 || player > 2){
            return false;
        }

        if(column < 0 || column >= board.GetLength(0)){
            return false;
        }

        for(int y = 0; y < board.GetLength(1); y++){
            if(board[column, y] == 0){
                board[column, y] = player;
                return true;
            }
        }

        return false;
    }
}
