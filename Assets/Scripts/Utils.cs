using UnityEngine;
using System.Collections;
using System;

public static class Utils
{
    //game type flag
    public static int type = -1;
    //pause flag
    public static bool isPaused = false;
    //game over flag
    public static bool isGameOver = false;
    //start flag
    public static bool start = true;
    //flag to tell the sound manager its changed scene
    public static bool[] sceneChanged = new bool[2] { false, false };
    //flag to indicate game skinning
    public static int appearence = 0;
    //flag to indicate game skinning
    public static int music = 0;

    //game mode type enum
    public enum GameMode { PvP, PvE, EvE };
    //types of sound fx enums
    public enum SoundFx { HOVEROVER, CLICK, PLACED, KINGED };
    //types of songs enums
    public enum Songs { STANDARD, FOCUS, HIPHOP };
    //types of appearence's
    public enum Appearence{ DEFAULT,ALTERNATIVE,GANGSTA };
    //type of background music
    public enum Music { CHILLED, FOCUS, HIPHOP };

    //convert from world to array index 
    public static int PosToArrayIndex(float val)
    {
        //my pieces are 0.35 by 0.35 so have to convert 
        //convert position to acquire its array index
        if (val < 0.2)
        {
            return 0;
        }
        else if (val > 0.2 && val < 0.5)
        {
            return 1;
        }
        else if (val > 0.5 && val < 0.9)
        {
            return 2;
        }
        else if (val > 0.9 && val < 1.2)
        {
            return 3;
        }
        else if (val > 1.2 && val < 1.6)
        {
            return 4;
        }
        else if (val > 1.6 && val < 1.9)
        {
            return 5;
        }
        else if (val > 1.9 && val < 2.3)
        {
            return 6;
        }
        else if (val > 2.3 && val < 2.6)
        {
            return 7;
        }
        else
        {
            return -1;
        }
    }

    //take the number of turns to return a three digit number as string
    public static string GetThreeDigitNum(int num)
    {
        //increment num turns by 1
        int numTurns = num + 1;
        string ret = numTurns.ToString();

        //if not already in three letter format
        if (ret.Length != 3)
        {
            //make it into a three letter format
            int need = 3 - ret.Length;
            string temp = "";

            //for number of missing letter
            for (int i = 0; i < need; i++)
            {
                temp += "0";
            }

            ret = temp + ret;
        }
       
        return ret;
    }

    //get index from array index
    public static int Index(int x, int y)
    {
        return x + (y * 8);
    }

    //get index in string format from array index
    public static string IndexToStr(int x, int y)
    {
        int index = x + (y * 8);
        return index.ToString();
    }
}