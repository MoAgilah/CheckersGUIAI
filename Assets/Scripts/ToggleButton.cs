using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour {
    
    public Material[] mat;  //a collection of the buttons materials

    public bool isLeft;     //is left back button
    public bool isRight;    //is right back button
    public bool isMiddle;   //is middle title screen button
    public bool isSfx;      //a sound effects on and off button
    public bool isMusic;    //a music on and off button
    public bool isSave;     //save current board by default or own name
    public bool isLoad;     //load current board by default or own name

    public bool isTrigger;  //a flag to tell the toggle button it doesnt have more than one materials

    //a flag to see if hover over has occured
    public bool isOver = false;

    //page id for tracking what the left and right buttons should do
    static int pageID;

    public static string lastSaved;
    public bool changed = false;

    void Start()
    {
        //if it isnt trigger set initial material
        if (!isTrigger)
            gameObject.GetComponent<Renderer>().material = mat[0];

        //set the pageID = 0
        pageID = 0;

    }

    // Update is called once per frame
    void Update()
    {
        //if soundfx button disable and enable sound
        if (isSfx)
        {
            //if sound effects on
            if (SoundManager.Instance.SoundOn)
            {
                //set the material or font colour -> green
                if (!isTrigger)
                    gameObject.GetComponent<Renderer>().material = mat[0];
                else
                    gameObject.GetComponent<TextMesh>().color = new Color32(0x00, 0xFF, 0x00, 0xFF);
            }
            else
            {
                //set the material or font colour -> red
                if (!isTrigger)
                    gameObject.GetComponent<Renderer>().material = mat[1];
                else
                    gameObject.GetComponent<TextMesh>().color = new Color32(0x83, 0x03, 0x00, 0xFF);
            }
        }

        //if music button disable and enable sound
        if (isMusic)
        {
            //if sound effects on
            if (SoundManager.Instance.MusicOn)
            {
                //set the material or font colour -> green
                if (!isTrigger)
                    gameObject.GetComponent<Renderer>().material = mat[0];
                else
                    gameObject.GetComponent<TextMesh>().color = new Color32(0x00, 0xFF, 0x00, 0xFF);
            }
            else
            {
                //set the material or font colour -> red
                if (!isTrigger)
                    gameObject.GetComponent<Renderer>().material = mat[1];
                else
                    gameObject.GetComponent<TextMesh>().color = new Color32(0x83, 0x03, 0x00, 0xFF);
            }
        }

        //if no save has been done || change has happened
        if (lastSaved != "" || changed)
        {
            //update load text field
            if (changed)
            {
                InputField load = GameObject.Find("LoadInput").GetComponent<InputField>();
                load.text = lastSaved;
                changed = false;
            }
        }
    }

    void OnMouseEnter()
    {
        //if not hovered before 
        if (!isOver)
        {
            //play sound once 
            isOver = true;
            SoundManager.Instance.PlaySoundFX((int)Utils.SoundFx.HOVEROVER);
        }

        //if no material present dont try and change it
        if (!isTrigger)
            gameObject.GetComponent<Renderer>().material = mat[1];
    }

    void OnMouseExit()
    {
        if (isOver)
            //on exit reenable sound
            isOver = false;

        //if no material present dont try and change it
        if (!isTrigger)
            gameObject.GetComponent<Renderer>().material = mat[0];
    }

    void OnMouseUp()
    {
        SoundManager.Instance.PlaySoundFX((int)Utils.SoundFx.CLICK);
        if (isLeft)
        {
            TextMesh title = GameObject.Find("Title").GetComponent<TextMesh>();
            if (pageID == 0)
            {
                DebugLog.Instance.Write("Returned to Rules Main menu from " + title.text);
                //if first page disable all additional buttons
                GameObject.Find("BkLeft").GetComponent<Renderer>().enabled = false;
                GameObject.Find("BkRight").GetComponent<Renderer>().enabled = false;

                
                if (SceneManager.GetActiveScene().name == "Rules")
                {
                    //reenable original text
                    GameObject.Find("Menu").GetComponent<Menu>().ResetRulesText();
                }
                else
                {
                    //reenable original text
                    GameObject.Find("Menu").GetComponent<Menu>().ResetControlsText();
                }
            }
            else
            {

                DebugLog.Instance.Write("Went back a page on the " + title.text + " section of the rules");
                //if on another page decrement pageID
                --pageID;
                GameObject.Find("BkRight").GetComponent<Renderer>().enabled = true;
                if (SceneManager.GetActiveScene().name == "Rules")
                {
                    GameObject.Find("Menu").GetComponent<Menu>().ChangeRulesText(16);
                }
            }

        }
        else if (isRight)
        {
            TextMesh title = GameObject.Find("Title").GetComponent<TextMesh>();
            DebugLog.Instance.Write("Went forward a page on the " + title.text + " section of the rules");
            //increment paged ID
            ++pageID;
            //change rules text
            GameObject.Find("Menu").GetComponent<Menu>().ChangeRulesText(21);
            //disable second button
            GameObject.Find("BkRight").GetComponent<Renderer>().enabled = false;
        }
        else if (isMiddle)
        {
            DebugLog.Instance.Write(SceneManager.GetActiveScene().name + " has returned to the title screen" );
            //return to title
            SceneManager.LoadScene("MainMenu");
        }
        else if (isSfx)
        {
            DebugLog.Instance.Write("Sound is enabled == " + SoundManager.Instance.SoundOn);
            //enable/disable sound fx
            GameObject.Find("SoundMgr").GetComponent<SoundManager>().SetSoundsOn();
        }
        else if (isMusic)
        {
            DebugLog.Instance.Write("Music is enabled == " + SoundManager.Instance.MusicOn);
            //enable/disable music
            GameObject.Find("SoundMgr").GetComponent<SoundManager>().SetMusicOn();
        }
        else if (isSave)
        {
            CheckerBoard cb = GameObject.Find("GameBoard").GetComponent<CheckerBoard>();
            InputField save = GameObject.Find("SaveInput").GetComponent<InputField>();

            string num = Utils.GetThreeDigitNum(cb.NumTurns);

            //if no value
            if (save.text == "")
            {
                //create default value
                lastSaved = "BoardTurn" + num;
                DebugLog.Instance.Write("File by the name of " + lastSaved + " has been saved");
                DebugLog.Instance.ExecuteSave(cb.WhosTurn, lastSaved);
                changed = true;
            }
            else
            {
                //create self named in correct format
                lastSaved = save.text + num;
                DebugLog.Instance.Write("File by the name of " + lastSaved + " has been saved");
                DebugLog.Instance.ExecuteSave(cb.WhosTurn, lastSaved);
                changed = true;
            }
        }
        else if (isLoad)
        {
            CheckerBoard cb = GameObject.Find("GameBoard").GetComponent<CheckerBoard>();
            InputField load = GameObject.Find("LoadInput").GetComponent<InputField>();

            //if text = load saved
            if (load.text == lastSaved)
            {
                //if the file exists
                if (System.IO.File.Exists(Application.dataPath + "/Resources/" + lastSaved + ".txt"))
                {
                    //extract num of turns from name
                    cb.NumTurns = System.Convert.ToInt32(lastSaved.Substring(lastSaved.Length - 3));

                    DebugLog.Instance.Write("File by the name of " + lastSaved + " has been loaded");
                    DebugLog.Instance.ExecuteLoad(lastSaved);
                }
            }
            else
            {
                if (System.IO.File.Exists(Application.dataPath + "/Resources/" + load.text + ".txt"))
                {
                    //extract num of turns from name
                    cb.NumTurns = System.Convert.ToInt32(load.text.Substring(load.text.Length - 3));

                    DebugLog.Instance.Write("File by the name of " + load.text + " has been loaded");
                    DebugLog.Instance.ExecuteLoad(load.text);
                }
            }
                
        }
    }
}
