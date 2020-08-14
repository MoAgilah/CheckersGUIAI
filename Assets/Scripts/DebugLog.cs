using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;

public class DebugLog : MonoBehaviour {
    //debug file name
    private string LogFile;
    //flags for processing
    public bool EchoToConsole = true;
    public bool AddTimeStamp = true;
    public bool boardLoaded = false;
    //which player's turn it is
    public bool player;

    private StreamWriter OutputStream;      //stream to debug game in general
    private StreamWriter SaveLoadStream;    //stream to save board

    static DebugLog instance = null;

    //loaded board in char array form
    public char[,] loadedBoard = new char[8, 8];

    public static DebugLog Instance
    {
        get { return instance; }
    }

    public bool GetTurn
    {
        get { return player; }
    }

    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);   //destroy duplicates
        }
        else
        {
            //if not created yet, create singleton
            //ensure object and instance stay alive between scenes
            instance = this;
            DontDestroyOnLoad(instance);
            DontDestroyOnLoad(gameObject);

            //create and time stamp log file
            LogFile = "/Resources/LogFile[" + GetDateAndTime() + "].txt";

            //create stream
            OutputStream = new StreamWriter(Application.dataPath + LogFile);
        }
    }

    string GetDateAndTime()
    {
        string date = DateTime.Now.Date.ToString("dd-MM-yyyy");
        string time = DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString();
        return date + " " + time;
    }

    //output board for log
    public void OutPutBoard(bool playersTurn)
    {
        //output board from dll 
        string row = "";
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                row += (char)DLLFunctions.GetPiece(x, y);
            }
            row += Environment.NewLine;
        }

        EchoToConsole = AddTimeStamp = false;   //turn off flags
        Write(row);    //output board to write
        EchoToConsole = AddTimeStamp = true;    //turn on flags
    }

    public void ExecuteSave(bool turn,string msg)
    {
        //save board and players turn
        SaveBoard(turn, msg);
        Write("The saved board");
        OutPutBoard(GetTurn);
    }

    public void SaveBoard(bool playersTurn,string name)
    {
        //open stream with 
        SaveLoadStream = new StreamWriter(Application.dataPath + "/Resources/"+ name+".txt");

        //save board from dll 
        string row = "";
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                row += (char)DLLFunctions.GetPiece(x, y);
            }

            if(y!=7)
            row += Environment.NewLine;
        }
       
        //send to file
        instance.Save(row);
        instance.Save(playersTurn.ToString());

        SaveLoadStream.Close();
    }

    public void ExecuteLoad(string msg)
    {
        //output old board
        Write("The board before");
        OutPutBoard(GetTurn);
        LoadBoard(msg);
        GameObject.Find("GameBoard").GetComponent<CheckerBoard>().UpdateBoardFromBoard(loadedBoard);
        //output new board
        Write("The board after");
        OutPutBoard(GetTurn);
    }

    public void LoadBoard(string name)
    {
        //extract file into string array
        string[] board = File.ReadAllLines(Application.dataPath + "/Resources/" + name + ".txt");

        //extract board from string array into char array
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                string temp = board[y];
                loadedBoard[x, y] = temp[x];
            }
        }

        //check the length of the final string to assign players turn
        if (board[8].Length == 4)
            GameObject.Find("GameBoard").GetComponent<CheckerBoard>().WhosTurn = true;
        else
            GameObject.Find("GameBoard").GetComponent<CheckerBoard>().WhosTurn = false;
    }



    public void Write(String msg)
    {
        //add a time stamp
        if (AddTimeStamp)
        {
            DateTime now = DateTime.Now;
            msg = string.Format("[{0:H:mm:ss}] {1}", now, msg);
        }

        //write line
        if (OutputStream != null)
        {
            OutputStream.WriteLine(msg);
            OutputStream.Flush();
        }

        //output to console
        if (EchoToConsole)
        {
            UnityEngine.Debug.Log(msg);
        }
    }


    public void Save(string msg)
    {
        //write line 
        if (OutputStream != null)
        {
            SaveLoadStream.WriteLine(msg);
            SaveLoadStream.Flush();
        }
    }

    void OnDestroy()
    {
        //every time a duplicate is destroyed 
        //flag to the singleton that he has changed scene
        Utils.sceneChanged[1] = true;
    }
}
