using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{

    public bool sceneChange;    //on mouse click it will change scene
    public bool textChange;     //on mouse click will change the text and button's functionality
    public bool trigger;        //will trigger set up of appearence/music change

    public bool appearence;     //a flag to tell the trigger button if he is setting up the appearence or music change
    public bool isQuit;         //a flag to tell the scene change if it is infact a quit button

    public string sceneName;    //scene name to change to if button is a sceneChange
    public int change;          //a value used to change text value  for textChange or set appearence/music if it trigger
    public GameObject parent;   //parent object that holds all text values

    private bool isOver = false;    //a flag to tell if already hovered over

    // Update is called once per frame
    void OnMouseEnter()
    {
        //if hover over hasn't happened
        if (!isOver)
        {
            //play sound 
            isOver = true;
            SoundManager.Instance.PlaySoundFX((int)Utils.SoundFx.HOVEROVER);
        }

        //update colour for player feedback
        gameObject.GetComponent<TextMesh>().color = new Color32(0x00, 0xFF, 0x00, 0xFF);	//green
    }

    void OnMouseExit()
    {
        //disable hover over to replay sound on next hover over
        if (isOver)
            isOver = false;

        gameObject.GetComponent<TextMesh>().color = new Color32(0x83, 0x03, 0x00, 0xFF);	//red
    }

    void OnMouseUp()
    {
        //play sound on click
        SoundManager.Instance.PlaySoundFX((int)Utils.SoundFx.CLICK);

        //if it is a scene change
        if (sceneChange)
        {
            //if it isnt quit
            if (!isQuit)
            {
                //if in main menu scene
                if (SceneManager.GetActiveScene().name == "MainMenu")
                {
                    //find first text element and get its buttons component
                    Button btn = GameObject.Find("first").GetComponent<Button>();

                    //if change value is set up for starting main game 
                    if (btn.change == 0)
                    {
                        //assign game mode type utils for processing in game scene
                        Utils.type = change;
                        
                        //output game type
                        switch (change)
                        {
                            case (int)Utils.GameMode.PvP:
                                DebugLog.Instance.Write("The selected game mode is " + Utils.GameMode.PvP.ToString());
                                break;
                            case (int)Utils.GameMode.PvE:
                                DebugLog.Instance.Write("The selected game mode is " + Utils.GameMode.PvE.ToString());
                                break;
                            case (int)Utils.GameMode.EvE:
                                DebugLog.Instance.Write("The selected game mode is " + Utils.GameMode.EvE.ToString());
                                break;
                            default:
                                break;
                        }
                    } 
                }

                //out put scene change
                DebugLog.Instance.Write("Scene changed to " + sceneName + " from " + SceneManager.GetActiveScene().name);
                //take scene name and load level
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                //output app quit
                DebugLog.Instance.Write("Application has been sent quit message");
                //else quit application
                Application.Quit();
            }
        }
        else if(textChange)
        {
            DebugLog.Instance.Write(SceneManager.GetActiveScene().name + " has changed it's menu orientation");

            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                //change text/functionality based on change value
                parent.GetComponent<Menu>().ChangeStartText(change);
            }
            else if (SceneManager.GetActiveScene().name == "Rules" || SceneManager.GetActiveScene().name == "Controls")
            {
                //if rules enable back button
                GameObject.Find("BkLeft").GetComponent<Renderer>().enabled = true;

                //if second page exists enable forward button
                if (change == 16)
                {
                    GameObject.Find("BkRight").GetComponent<Renderer>().enabled = true;
                }

                if (SceneManager.GetActiveScene().name == "Rules")
                {
                    //change text/functionality based on change value
                    parent.GetComponent<Menu>().ChangeRulesText(change);
                }
                else
                {
                    //change text/functionality based on change value
                    parent.GetComponent<Menu>().ChangeControlsText(change);
                }
               
            }
            else if (SceneManager.GetActiveScene().name == "Game")
            {
                //if previousily paused unpause
                Utils.isPaused = false;
            }
            else if (SceneManager.GetActiveScene().name == "Options")
			{
                //change text/functionality based on change value
                parent.GetComponent<Menu>().ChangeOptionsText(change);
			}
            else if (SceneManager.GetActiveScene().name == "Controls")
            {
                //change text/functionality based on change value
                parent.GetComponent<Menu>().ChangeControlsText(change);
            }
        }
        else if (trigger)
        {
            //if setting appearence preferences
            if (appearence)
            {
                //assing utils value of appearence to ensure all boards get updated on load
                switch (change)
                {
                    case (int)Utils.Appearence.DEFAULT:
                        Utils.appearence = (int)Utils.Appearence.DEFAULT;
                        DebugLog.Instance.Write("The appearance has been changed too " + Utils.Appearence.DEFAULT.ToString());
                        break;
                    case (int)Utils.Appearence.ALTERNATIVE:
                        Utils.appearence = (int)Utils.Appearence.ALTERNATIVE;
                        DebugLog.Instance.Write("The appearance has been changed too " + Utils.Appearence.ALTERNATIVE.ToString());
                        break;
                    case (int)Utils.Appearence.GANGSTA:
                        Utils.appearence = (int)Utils.Appearence.GANGSTA;
                        DebugLog.Instance.Write("The appearance has been changed too " + Utils.Appearence.GANGSTA.ToString());
                        break;
                    default:
                        Utils.appearence = (int)Utils.Appearence.DEFAULT;
                        DebugLog.Instance.Write("The appearance has been changed too " + Utils.Appearence.DEFAULT.ToString());
                        break;
                }
            }
            else //if setting background music
            {
                //use change to set the background music
                SoundManager.Instance.ChangeBKGMusic(change);
            }
        }
    }
}
