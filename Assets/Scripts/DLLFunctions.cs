using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class DLLFunctions : MonoBehaviour {

    //changes the depth or intelligence of the a.i.
    [DllImport("CheckersDLL")]
    public static extern void ChangeDepth(int val);

    //returns the current depth value from the dll
    [DllImport("CheckersDLL")]
    public static extern int GetDepth();

    //get the piece based on index 
    [DllImport("CheckersDLL")]
    public static extern int GetPiece(int x, int y);

    //reset board with the new board
    [DllImport("CheckersDLL")]
    public static extern void ResetBoard();

    //clear the board
    [DllImport("CheckersDLL")]
    public static extern void Clear();

    //search for the a.i. next move
    [DllImport("CheckersDLL")]
    public static extern void SearchMove(bool playerID);

    //pass the players move to the board for a.i. processing
    [DllImport("CheckersDLL")]
    public static extern void UpdateBoardWithMove(int xDst, int yDst, int xSrc, int ySrc);

    //pass the players capture move to the board for a.i. processing
    [DllImport("CheckersDLL")]
    public static extern void UpdateBoardWithCapture(int xDst, int yDst, int xSrc, int ySrc);

    //set p1 man at index

    [DllImport("CheckersDLL")]
    public static extern void SetP1Man(int index);


    //set p2 man at index
    [DllImport("CheckersDLL")]
    public static extern void SetP2Man(int index);

    //set p1 king at index
    [DllImport("CheckersDLL")]
    public static extern void SetP1King(int index);

    //set p2 king at index
    [DllImport("CheckersDLL")]
    public static extern void SetP2King(int index);

}
