using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckerBoard : MonoBehaviour
{
    //board grid
    public Piece[,] pieces = new Piece[8, 8];

    //piece prefabs one piece and one king model
    public GameObject[] lightPiece;
    public GameObject[] darkPiece;

    //turn counter
    private int numTurns = 0;

    //accessor numturns
    public int NumTurns
    {
        get{ return numTurns; }
        set{ numTurns = value; }
    }

    //mouse location
    private Vector2 mouseOver;

    //start position and target position
    private Vector2 startDrag;
    private Vector2 endDrag;

    //board offset
    private Vector3 boardOffset = new Vector3(0.0f, 0.05f, 0.005f);

    //currently held piece
    private Piece selectedPiece;

    //pieces that have to move 
    private List<Piece> forcedPieces;

    //moves that are valid
    private List<Piece> validMove;

    //flag to say if a kill been made
    private bool hasKilled;

    //flag to say if kinged
    private bool justKinged;

    //flag to say PvP mode
    private bool playerVs;
    //flag to say PvE mode
    private bool playerVsAI;
    //flag to say EvE mode
    private bool AiVsAi;

    //is it dark player's turn
    public bool isDark = true;

    //accessor isDark
    public bool WhosTurn
    {
        get { return isDark; }
        set { isDark = value; }
    }

    //to announce the game's end result
    public TextMesh gameEndAnnoucement;

    bool GameReady = false;

    bool once = false;

    //generate grid and assign pieces
    private void GenerateBoard()
    {
        //generate board from dll checkersboard
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                GeneratePiece((char)DLLFunctions.GetPiece(x, y), x, y);
            }
        }

        //output initial board
        DebugLog.Instance.Write("The board has been generated as");
        DebugLog.Instance.OutPutBoard(isDark);
    }

    //generate pieces based on dll board
    private void GeneratePiece(char piece,int x, int y)
    {
        GameObject go;
        Piece p;

        switch (piece)
        {
            case 'o':
                //creates prefab
                go = Instantiate(darkPiece[0]) as GameObject;
                //parent's it to the board
                go.transform.SetParent(transform);
                //assigns it to the piece array
                p = go.GetComponent<Piece>();
                pieces[x, y] = p;
                //place the piece in the board
                PlacePiece(p, x, y);
                break;
            case 'O':
                //creates prefab
                go = Instantiate(darkPiece[1]) as GameObject;
                //parent's it to the board
                go.transform.SetParent(transform);
                //assigns it to the piece array
                p = go.GetComponent<Piece>();
                pieces[x, y] = p;
                //place the piece in the board
                PlacePiece(p, x, y);
                break;
            case 't':
                //creates prefab
                go = Instantiate(lightPiece[0]) as GameObject;
                //parent's it to the board
                go.transform.SetParent(transform);
                //assigns it to the piece array
                p = go.GetComponent<Piece>();
                pieces[x, y] = p;
                //place the piece in the board
                PlacePiece(p, x, y);
                break;
            case 'T':
                //creates prefab
                go = Instantiate(lightPiece[1]) as GameObject;
                //parent's it to the board
                go.transform.SetParent(transform);
                //assigns it to the piece array
                p = go.GetComponent<Piece>();
                pieces[x, y] = p;
                //place the piece in the board
                PlacePiece(p, x, y);
                break;
        }
    }

    //format array index into world position
    private void PlacePiece(Piece p, int x, int y)
    {
        Vector3 pos = new Vector3(0.35f * x, 0, 0.35f * y);

        //offset the piece based on
        p.transform.position = pos + boardOffset;
        if (GameReady && justKinged == false)
        {
            //change audio clip
            SoundManager.Instance.PlaySoundFX((int)Utils.SoundFx.PLACED);
        }
    }

    private void UpdateBoard()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                //erase current board
                Piece p = pieces[x, y];
                if (p != null)
                {
                    pieces[x, y] = null;
                    Destroy(p.gameObject);
                }

                //reinitialises the board with updated values
                GeneratePiece((char)DLLFunctions.GetPiece(x, y), x, y);
            }
        }
    }

    public void UpdateBoardFromBoard(char[,] loaded)
    {
        //clear current board
        DLLFunctions.Clear();

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                //wipe out piece
                Piece p = pieces[x, y];
                if (p != null)
                {
                    pieces[x, y] = null;
                    Destroy(p.gameObject);
                }

                //generate new piece from the board
                GeneratePiece(loaded[x, y], x, y);

                
                p = pieces[x, y];

                //tell the board about the new position of the kings and regular pieces
                if (p != null)
                {
                    if (p.isDark)
                    {
                        if (p.isKing)
                            DLLFunctions.SetP1King(Utils.Index(x, y));
                        else
                            DLLFunctions.SetP1Man(Utils.Index(x, y));
                    }

                    if (p.isLight)
                    {
                        if (p.isKing)
                            DLLFunctions.SetP2King(Utils.Index(x, y));
                        else
                            DLLFunctions.SetP2Man(Utils.Index(x, y));
                    }
                }
            }

            //test to see if there are any forced capture moves
            forcedPieces = scanForPossMove();

            //checks for number of valid moves
            validMove = scanForValidMove();

            //if there are forced capture pieces
            if (forcedPieces.Count != 0)
            {
                //then valid move's is forced move's
                validMove = forcedPieces;
            }
        }


    }

    // Use this for initialization
    public void Start()
    {
        Utils.isPaused = Utils.isGameOver = once 
        = playerVs = playerVsAI = AiVsAi = false;

        //sets the game type
        switch (Utils.type)
        {
            case (int)Utils.GameMode.PvP:
                playerVs = true;
                DLLFunctions.ResetBoard();
                break;
            case (int)Utils.GameMode.PvE:
                playerVsAI = true;
                DLLFunctions.ResetBoard();
                break;
            case (int)Utils.GameMode.EvE:
                AiVsAi = true;
                DLLFunctions.ResetBoard();
                break;
            default:
                playerVsAI = true;
                DLLFunctions.ResetBoard();
                break;
        }

        //generate new board
        GenerateBoard();

        forcedPieces = new List<Piece>();
        validMove = new List<Piece>();

        GameReady = true;

        //output new game start/dark team up first
        DebugLog.Instance.Write("A new game has began"+ System.Environment.NewLine+"It's dark team up first" + System.Environment.NewLine);

        //announce whos playing dark
        if (playerVs)
            DebugLog.Instance.Write("Human player 1");
        else if (playerVsAI)
            DebugLog.Instance.Write("Human player");
        else if (AiVsAi)
            DebugLog.Instance.Write("A.I. Player 1");
    }

    // Update is called once per frame
    void Update()
    {
        updateMouseOver();

        //if not paused or gameover
        if (!Utils.isGameOver && !Utils.isPaused)
        {
            if (playerVs)
                DoPlayerMove();
            else if (playerVsAI)
            {
                if (isDark != false)
                    DoPlayerMove();
                else
                    DoAIMove();
            }
            else if (AiVsAi)
                DoAIMove();

            GameObject.Find("First").GetComponent<TextMesh>().text = "Continue";
        }
        else if(Utils.isGameOver && !once)
        {
            once = true;
            DebugLog.Instance.Write("Game has Ended" + System.Environment.NewLine);
            //wipe out continue on game end
            GameObject.Find("First").GetComponent<TextMesh>().text = "";
            GameObject.Find("First").GetComponent<BoxCollider>().size = new Vector3(0, 0, 0);
        }

        //pause and unpause on keypress
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Utils.isPaused = !Utils.isPaused;

            //output paused state
            if (Utils.isPaused)
                DebugLog.Instance.Write("Game has been paused");
            else
                DebugLog.Instance.Write("Game has been unpaused");
        }
    }

    private void DoPlayerMove()
    {
        //pick up a piece if one isn't held
        if (selectedPiece != null)
            updatePieceDrag(selectedPiece);

        //drop that piece
        if (Input.GetMouseButtonDown(0))
            selectPiece(mouseOver.x, mouseOver.y);

        //test if move is valid
        if (Input.GetMouseButtonUp(0))
            TryMove(mouseOver.x, mouseOver.y);
    }

    private void DoAIMove()
    {
        //output old board
        DebugLog.Instance.Write("The board before search");
        DebugLog.Instance.OutPutBoard(isDark);

        //perform turn
        DLLFunctions.SearchMove(isDark);
        
        //small delay to allow the AI's move to be shown
        float delay = 2.0f;
        while (delay > 0)
        {
            delay -= Time.deltaTime;
        }
        UpdateBoard();

        //output new board
        DebugLog.Instance.Write("The board after search");
        DebugLog.Instance.OutPutBoard(isDark);

        //end turn
        numTurns++;
        isDark = !isDark;

        //output whos turn and/or team it is
        if (AiVsAi)
        {
            if (isDark)
            {
                DebugLog.Instance.Write("It's Dark team's turn");
                DebugLog.Instance.Write("A.I. Player 1");
            }
            else
            {
                DebugLog.Instance.Write("It's Light team's turn");
                DebugLog.Instance.Write("A.I. Player 2");
            }
        }

        if (playerVsAI)
        {
            if (isDark)
            {
                DebugLog.Instance.Write("It's Dark team's turn");
            }
        }

        //test to see if there are any forced capture moves
        forcedPieces = scanForPossMove();

        //checks for number of valid moves
        validMove = scanForValidMove();

        //output valid/forced moves for current player
        DebugLog.Instance.Write("has " + forcedPieces.Count + " forced moves");
        DebugLog.Instance.Write("has " + validMove.Count + " valid moves");

        //if there are forced capture pieces
        if (forcedPieces.Count != 0)
            //then valid move's is forced move's
            validMove = forcedPieces;
        
        //check victory
        CheckForVictory();
    }

    //attempt move
    private void TryMove(float xX, float xY)
    {
        int startX = -1; int startY = -1;
        int endX = -1; int endY = -1;

        //if out of bounds == -1
        if (xX == -1 || xY == -1)
        {
            //if piece is selected
            if (selectedPiece != null)
            {
                startX = Utils.PosToArrayIndex(startDrag.x);
                startY = Utils.PosToArrayIndex(startDrag.y);

                //move piece back to start position
                PlacePiece(selectedPiece, startX, startY);
            }

            //let go of piece
            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
        }
        else
        {
            //convert world position into grid index
            startX = Utils.PosToArrayIndex(startDrag.x);
            startY = Utils.PosToArrayIndex(startDrag.y);

            //convert world position into grid index
            endDrag = new Vector2(xX, xY);
            endX = Utils.PosToArrayIndex(endDrag.x);
            endY = Utils.PosToArrayIndex(endDrag.y);
        }

        //if piece is selected
        if (selectedPiece != null)
        {
            //if it hasnt moved
            if (endDrag == startDrag)
            {
                //move piece back to start position
                PlacePiece(selectedPiece, startX, startY);

                //drop piece 
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }

            //if the move is valid
            if (selectedPiece.validMove(pieces, startDrag, endDrag))
            {
                //did we kill something
                if (Mathf.Abs(Utils.PosToArrayIndex(endDrag.y) - Utils.PosToArrayIndex(startDrag.y)) == 2)
                {
                    //turn potentially captured pieces world position into grid index
                    int x = Utils.PosToArrayIndex((startDrag.x + endDrag.x) / 2);
                    int y = Utils.PosToArrayIndex((startDrag.y + endDrag.y) / 2);

                    Piece p = pieces[x, y];

                    //if a piece is at this grid coordinate
                    if (p != null)
                    {
                        //destroy piece, remove from grid, and flag kill
                        Destroy(p.gameObject);
                        pieces[x, y] = null;
                        hasKilled = true;
                    }
                }

                //were we supposed to kill anything
                if (forcedPieces.Count != 0 && !hasKilled)
                {
                    //move piece back to start position
                    PlacePiece(selectedPiece, startX, startY);

                    //drop piece
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    return;
                }

                Piece temp = pieces[endX, endY];

                //move selected piece in array
                pieces[endX, endY] = selectedPiece;

                //remove selected place from previous spot in array
                pieces[startX, startY] = temp;

                //update the pieces location
                PlacePiece(selectedPiece, endX, endY);

                //output old board
                DebugLog.Instance.Write("The board before");
                DebugLog.Instance.OutPutBoard(isDark);

                //update board with player's move
                if (hasKilled)
                {
                    DLLFunctions.UpdateBoardWithCapture(endX, endY, startX, startY);
                }
                else
                {
                    DLLFunctions.UpdateBoardWithMove(endX, endY, startX, startY);
                    DebugLog.Instance.Write(selectedPiece.name + " has moved from " + Utils.IndexToStr(startX, startY) + " to " + Utils.IndexToStr(endX, endY));
                }

                //output new board
                DebugLog.Instance.Write("The board after");
                DebugLog.Instance.OutPutBoard(isDark);

                EndTurn();
                CheckForVictory();
            }
            else
            {
                //if piece selected
                if (selectedPiece != null)
                {
                    //move back to starting position
                    PlacePiece(selectedPiece, startX, startY);
                }

                //drop piece 
                startDrag = Vector2.zero;
                selectedPiece = null;
                forcedPieces.Clear();
                validMove.Clear();
            }
        }
    }

    //check for victory
    private void CheckForVictory()
    {
        //if number of turns equals 100
        if (numTurns == 100)
        {
            Utils.isGameOver = true;

            //output game end response
            DebugLog.Instance.Write("Maximum number of turns exceeded, the game ends in a draw");
            gameEndAnnoucement.text = "It's a Draw";
        }
        else if (validMove.Count == 0)
        {
            if (isDark)
            {
                isDark = !isDark;
                //scan for if there are any forced move's

                //test to see if there are any forced capture moves
                forcedPieces = scanForPossMove();

                //checks for number of valid moves
                validMove = scanForValidMove();

                //if there are forced capture pieces
                if (forcedPieces.Count != 0)
                {
                    //then valid move's is forced move's
                    validMove = forcedPieces;
                }

                if (validMove.Count > 0)
                {
                    Utils.isGameOver = true;
                    gameEndAnnoucement.text = "Light team wins";
                    //output game end response
                    DebugLog.Instance.Write("Dark team has no valid moves, light team wins");
                }
                else
                {
                    Utils.isGameOver = true;
                    gameEndAnnoucement.text = "The game ends in a draw";
                    //output game end response
                    DebugLog.Instance.Write("Neither team has valid moves, the game ends in a draw");
                }
            }
            else
            {
                isDark = !isDark;
                //test to see if there are any forced capture moves
                forcedPieces = scanForPossMove();

                //checks for number of valid moves
                validMove = scanForValidMove();

                //if there are forced capture pieces
                if (forcedPieces.Count != 0)
                {
                    //then valid move's is forced move's
                    validMove = forcedPieces;
                }

                if (validMove.Count > 0)
                {
                    Utils.isGameOver = true;
                    gameEndAnnoucement.text = "Dark team wins";
                    //output game end response
                    DebugLog.Instance.Write("light team has no moves, Dark team wins");
                }
                else
                {
                    Utils.isGameOver = true;
                    gameEndAnnoucement.text = "The game ends in a draw";
                    //output game end response
                    DebugLog.Instance.Write("Neither team has a valid move, the game ends in a draw");
                }
            }
        }
    }

    //check for end of turn, if true switch turn
    private void EndTurn()
    {
        //world pos to array index
        int x = Utils.PosToArrayIndex(endDrag.x);
        int y = Utils.PosToArrayIndex(endDrag.y);

        //if piece is selected
        if (selectedPiece != null)
        {
            //if dark and closest row to opponent and not already a king
            if (selectedPiece.isDark && !selectedPiece.isKing && y == 7)
            {
                //destroy piece
                Destroy(selectedPiece.gameObject);

                //instantiate king piece
                GameObject go = Instantiate(darkPiece[1]) as GameObject;
                go.transform.SetParent(transform);

                // store king in destroyed piece's place
                selectedPiece = go.GetComponent<Piece>();
                pieces[x, y] = selectedPiece;

                justKinged = true;
                //change audio clip
                SoundManager.Instance.PlaySoundFX((int)Utils.SoundFx.KINGED);


                //move into same position
                PlacePiece(selectedPiece, x, y);

                DebugLog.Instance.Write(selectedPiece.name + " has just been kinged");
            }
            //if light and closest row to opponent and not already a king
            else if (selectedPiece.isLight && !selectedPiece.isKing && y == 0)
            {
                //destroy piece
                Destroy(selectedPiece.gameObject);

                //instantiate king piece
                GameObject go = Instantiate(lightPiece[1]) as GameObject;
                go.transform.SetParent(transform);

                // store king in destroyed piece's place
                selectedPiece = go.GetComponent<Piece>();
                pieces[x, y] = selectedPiece;

                justKinged = true;
                //change audio clip

                SoundManager.Instance.PlaySoundFX((int)Utils.SoundFx.KINGED);

                //move into same position
                PlacePiece(selectedPiece, x, y);

                DebugLog.Instance.Write(selectedPiece.name + " has just been kinged");
            }
        }

        //drop piece
        selectedPiece = null;
        startDrag = Vector2.zero;

        //if kill has been made and there is another kill available, and you haven't just been kinged.
        if (scanForPossMove(selectedPiece, x, y).Count != 0 && hasKilled && !justKinged)
        {
            //delay end of turn
            return;
        }

        //disable variables, increment turns made, and switch turn
        justKinged = hasKilled = false;
        isDark = !isDark;
        
        //output current player and team colour
        if (playerVs)
        {
            if (isDark)
            {
                DebugLog.Instance.Write("It is Dark teams turn");
                DebugLog.Instance.Write("Human player 1");
            }
            else
            {
                DebugLog.Instance.Write("It is Light teams turn");
                DebugLog.Instance.Write("Human player 2");
            }
        }
        else if (playerVsAI)
        {
            if (isDark)
            {
                DebugLog.Instance.Write("It is Dark teams turn");
                DebugLog.Instance.Write("Human player");
            }
            else
            {
                DebugLog.Instance.Write("It is Light teams turn");
                DebugLog.Instance.Write("A.I.'s player");
            }
            
        }

        //test to see if there are any forced capture moves
        forcedPieces = scanForPossMove();

        //checks for number of valid moves
        validMove = scanForValidMove();

        //ouput current players valid and forced moves
        DebugLog.Instance.Write("has " + forcedPieces.Count + " forced moves");
        DebugLog.Instance.Write("has " + validMove.Count + " valid moves");

        //if there are forced capture pieces
        if (forcedPieces.Count != 0)
        {
            //then valid move's is forced move's
            validMove = forcedPieces;
        }

        ++numTurns;
    }

    //check for all forced moves for current player
    private List<Piece> scanForPossMove()
    {
        //wipe forwardPieces 
        forcedPieces = new List<Piece>();

        //check grid entire grid
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                //and the grid location isn't empty and it's there turn
                if (pieces[i, j] != null && isDark == pieces[i, j].isDark)
                {
                    //check for kill moves that must be taken
                    if (pieces[i, j].IsForcedToMove(pieces, i, j))
                    {
                        //store force move into forced pieces
                        forcedPieces.Add(pieces[i, j]);
                    }
                }
            }
        }

        return forcedPieces;
    }

    private List<Piece> scanForValidMove()
    {
        //wipe forwardPieces 
        validMove = new List<Piece>();

        //check grid entire grid
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                if (pieces[i, j] != null && pieces[i, j].isDark == isDark)
                {
                    //if dark check all pieces for a valid move
                    if (pieces[i, j].isDark || pieces[i, j].isKing)
                    {
                        //left up
                        if (i - 1 >= 0 && j + 1 <= 7)
                        {
                            if (pieces[i, j].validMove(pieces, i, j, i - 1, j + 1))
                            {
                                validMove.Add(pieces[i - 1, j + 1]);
                            }
                        }

                        //right up
                        if (i + 1 <= 7 && j + 1 <= 7)
                        {
                            if (pieces[i, j].validMove(pieces, i, j, i + 1, j + 1))
                            {
                                validMove.Add(pieces[i + 1, j + 1]);
                            }
                        }
                    }

                    if(pieces[i, j].isLight || pieces[i, j].isKing)//if light check all pieces for a valid move
                    {
                        //left down
                        if (i - 1 >= 0 && j - 1 >= 0)
                        {
                            if (pieces[i, j].validMove(pieces, i, j, i - 1, j - 1))
                            {
                                validMove.Add(pieces[i - 1, j - 1]);
                            }
                        }

                        //left down
                        if (i + 1 <= 7 && j - 1 >= 0)
                        {
                            if (pieces[i, j].validMove(pieces, i, j, i + 1, j - 1))
                            {
                                validMove.Add(pieces[i + 1, j - 1]);
                            }
                        }
                    }
                }
            }
        }

        return validMove;
    }

    //check to see if multiple kills are available i.e. double jump
    private List<Piece> scanForPossMove(Piece p, int x, int y)
    {
        //wipe forced pieces
        forcedPieces = new List<Piece>();

        //if there is a forced move available
        if (pieces[x, y].IsForcedToMove(pieces, x, y))
        {
            //insert in forced pieces
            forcedPieces.Add(pieces[x, y]);
        }
        return forcedPieces;
    }

    //pick up piece, if your turn
    private void selectPiece(float x, float y)
    {
        int arrX = -1; int arrY = -1;

        //if out of bounds
        if (x == -1 || y == -1)
        {
            return;
        }
        else
        {
            //convert world position to array index
            arrX = Utils.PosToArrayIndex(x);
            arrY = Utils.PosToArrayIndex(y);
        }


        Piece p = pieces[arrX, arrY];

        //if square isn't empty and it's their turn
        if (p != null && p.isDark == isDark)
        {
            //pick up piece
            selectedPiece = p;

            //set start postion
            startDrag = mouseOver;
        }
    }

    //move piece on screen 
    private void updatePieceDrag(Piece p)
    {
        RaycastHit hit;

        //use the mouse and main camera to cast a ray cast and if succusful update selected piece position on screen
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            //using the ray cast hit point
            p.transform.position = hit.point;
        }
    }
       
    //capture mouse position and store in vector2
    private void updateMouseOver()
    {
        RaycastHit hit;

        //ray cast between main camera and mouse position
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            //if succussful remove board offset
            mouseOver.x = hit.point.x - boardOffset.x;
            mouseOver.y = hit.point.z - boardOffset.z;
        }
        else
        {
            //else out of bounds 
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }
}
