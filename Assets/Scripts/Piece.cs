using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour
{
    public bool isLight;
    public bool isDark;
    public bool isKing;

    //check for capture moves possible, which must be enforced
    public bool IsForcedToMove(Piece[,] board, int x, int y)
    {
        if (isDark || isKing)
        {
            //top left
            if (x >= 2 && y <= 5)
            {
                Piece p = board[x - 1, y + 1];

                //if kill available and not my own team
                if (p != null && p.isLight != isLight)
                {
                    //check if jump is possible 
                    if (board[x - 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }

            //top right
            if (x <= 5 && y <= 5)
            {
                Piece p = board[x + 1, y + 1];

                //if kill available and not my own team
                if (p != null && p.isDark != isDark)
                {
                    //check if jump is possible 
                    if (board[x + 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }
        }

        if (isLight || isKing)
        {
            //bottom left
            if (x >= 2 && y >= 2)
            {
                Piece p = board[x - 1, y - 1];

                //if kill available and not my own team
                if (p != null && p.isLight != isLight)
                {
                    //check if jump is possible 
                    if (board[x - 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }

            //bottom right
            if (x <= 5 && y >= 5)
            {
                Piece p = board[x + 1, y - 1];

                //if kill available and not my own team
                if (p != null && p.isLight != isLight)
                {
                    //check if jump is possible 
                    if (board[x + 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }



    public bool validMove(Piece[,] board, int x, int y, int x2, int y2)
    {
        int deltaMove = Mathf.Abs(x2 - x);
        int deltaMoveY = y2 - y;

        //if you are landing on a occupied sqaure 
        if (board[x2, y2] != null)
        {
            return false;
        }

        //if its light or king
        if (isLight || isKing)
        {
            //normal move
            if (deltaMove == 1)
            {

                if (deltaMoveY == -1)
                {
                    return true;
                }
            }
            //kill jump
            else if (deltaMove == 2)
            {
                if (deltaMoveY == -2)
                {
                    //middle
                    int x3 = x + x2 / 2;
                    int y3 = y + y2 / 2;

                    Piece p = board[x3, y3];

                    //if capture move check not my own team
                    if (p != null && p.isLight != isLight)
                    {
                        return true;
                    }

                }
            }
        }

        //if its light or king
        if (isDark || isKing)
        {
            //normal move
            if (deltaMove == 1)
            {
                if (deltaMoveY == 1)
                {
                    return true;
                }
            }
            else if (deltaMove == 2)
            {
                if (deltaMoveY == 2)
                {
                    //middle
                    int x3 = x + x2 / 2;
                    int y3 = y + y2 / 2;

                    Piece p = board[x3, y3];

                    //if capture move check not my own team
                    if (p.isLight != isLight)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    //checks the validity of proposed move
    public bool validMove(Piece[,] board, Vector2 start, Vector2 end)
    {
        //start
        int x = Utils.PosToArrayIndex(start.x);
        int y = Utils.PosToArrayIndex(start.y);

        //end
        int x2 = Utils.PosToArrayIndex(end.x);
        int y2 = Utils.PosToArrayIndex(end.y);

        //if you are landing on a occupied sqaure 
        if (board[x2, y2] != null)
        {
            return false;
        }

        int deltaMove = Mathf.Abs(x2 - x);
        int deltaMoveY = y2 - y;

        //if its light or king
        if (isLight || isKing)
        {
            //normal move
            if (deltaMove == 1)
            {

                if (deltaMoveY == -1)
                {
                    return true;
                }
            }
            //kill jump
            else if (deltaMove == 2)
            {
                if (deltaMoveY == -2)
                {
                    //middle
                    int x3 = Utils.PosToArrayIndex((start.x + end.x) / 2);
                    int y3 = Utils.PosToArrayIndex((start.y + end.y) / 2);

                    Piece p = board[x3, y3];

                    //if capture move check not my own team
                    if (p != null && p.isLight != isLight)
                    {
                        return true;
                    }

                }
            }
        }

        //if its light or king
        if (isDark || isKing)
        {
            //normal move
            if (deltaMove == 1)
            {
                if (deltaMoveY == 1)
                {
                    return true;
                }
            }
            else if (deltaMove == 2)
            {
                if (deltaMoveY == 2)
                {
                    //middle
                    int x3 = Utils.PosToArrayIndex((start.x + end.x) / 2);
                    int y3 = Utils.PosToArrayIndex((start.y + end.y) / 2);

                    Piece p = board[x3, y3];

                    //if capture move check not my own team
                    if (p.isLight != isLight)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
