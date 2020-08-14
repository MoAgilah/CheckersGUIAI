using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    public GameObject[] text;       //collection of text to manipulated 
    public string[] msgs;           //collection of texts contained in above
    public string[] sceneNames;     //sceneNames to change if button is a sceneChange
    public float characterSize;     //the base character size for all text
    public float X, Y;              //the shared x and y coordinates to ensure positioned correctly
    public float[] Z;               //z value to allow for individual placement
    public Vector3[] colliderSize;  //the size of the collider individual to each text element
    Color col;                      //color used to alternate colour of the text

    // Use this for initialization
    void Start()
    {
        col = new Color32(0x83, 0x03, 0x00, 0xFF);	//dark red

        //set initial values for specified scene
        for (int i = 0; i < text.Length; ++i)
        {
            //local var temp for readability
            TextMesh temp = text[i].GetComponent<TextMesh>();

            //special rules for title
            if (temp.name == "Title")
            {
                if (SceneManager.GetActiveScene().name == "Rules")
                {
                    temp.characterSize = 1.5f;
                }
                else
                {
                    //increased text size
                    temp.characterSize = 1.5f;
                }
             /*
                title also has no collider so skip the setup of this    
             */
            }
            else
            {
                //set base character size
                temp.characterSize = characterSize;
                
                //initialise collider size
                text[i].GetComponent<BoxCollider>().size = colliderSize[i];
                
                //if it has a scene name to change to assign otherwise assign it an empty string
                text[i].GetComponent<Button>().sceneName = sceneNames[i];
            }

            //assign base colour
            temp.color = new Color32(0x83, 0x03, 0x00, 0xFF);	//dark red
            //assign initial text value
            temp.text = msgs[i];
            //assign initial position 
            text[i].GetComponent<Transform>().position = new Vector3(X, Y, Z[i]);
        }

        //special rules for rules page as need back and forwawrd buttons to traverse rules
        if (SceneManager.GetActiveScene().name == "Rules" || SceneManager.GetActiveScene().name == "Controls")
        {
            GameObject.Find("BkLeft").GetComponent<Renderer>().enabled = false;
            GameObject.Find("BkRight").GetComponent<Renderer>().enabled = false;
        }

        //allow pause title to be large and set alternative colour so it stands out
        if (SceneManager.GetActiveScene().name == "Game")
        {
            TextMesh title = GameObject.Find("Title").GetComponent<TextMesh>();
            //change pause title to stand out
            title.characterSize = 0.2f;

            //change pause title colour to be green as standard to stand out
            title.color = new Color32(0x00, 0xFF, 0x00, 0xFF);
        }
    }

    void Update()
    {
        //if the game isnt over
        if (Utils.isGameOver != true)
        {
            //and the scene is game
            if (SceneManager.GetActiveScene().name == "Game")
            {
                //if paused enable menu
                if (Utils.isPaused == true)
                {
                    for (int i = 0; i < text.Length; ++i)
                    {
                        //if text item not called title then set up its collider
                            //else title doesnt have a collider so don't try to set it up
                        if (text[i].name != "Title")
                        {
                            text[i].GetComponent<BoxCollider>().size = colliderSize[i];
                        }
                        text[i].GetComponent<Renderer>().enabled = true;
                    }
                }
                else
                {//disable menu
                    for (int i = 0; i < text.Length; ++i)
                    {
                        //if text item not called title then set up its collider
                        //else title doesnt have a collider so don't try to set it up
                        if (text[i].name != "Title")
                        {
                            text[i].GetComponent<BoxCollider>().size = new Vector3(0, 0, 0);
                        }
                        text[i].GetComponent<Renderer>().enabled = false;
                    }
                }
            }
        }
        else
        {
            //if game over enable end of game menu
            for (int i = 0; i < text.Length; ++i)
            {
                //if text item not called title then set up its collider
                //else title doesnt have a collider so don't try to set it up
                if (text[i].name != "Title")
                { 
                    text[i].GetComponent<BoxCollider>().size = colliderSize[i];
                }
                text[i].GetComponent<Renderer>().enabled = true;
            }
        }
    }

    //change start text based on array index of initial values
    public void ChangeStartText(int changeID)
    {
        for (int i = 0; i < 4; ++i)
        {
            TextMesh temp = text[i].GetComponent<TextMesh>();

            //set new text value
            temp.text = msgs[i + changeID];
            //set new position
            text[i].GetComponent<Transform>().position = new Vector3(X, Y, Z[i + changeID]);
            //set new scene name
            text[i].GetComponent<Button>().sceneName = sceneNames[i + changeID];

            BoxCollider BCTemp = text[i].GetComponent<BoxCollider>();
            //set new collider size
            BCTemp.size = colliderSize[i + changeID];
        }

        //switch buttons conditions based on changeID
        Button btn;
        if (changeID != 0)
        {
            //change button one from text change to scene change
            btn = text[0].GetComponent<Button>();
            btn.sceneChange = true;
            btn.textChange = false;
            //assign new change value for assigning game mode type whilst changing scenes
            btn.change = 0;

            //change button four from quit to text change
            btn = text[3].GetComponent<Button>();
            btn.sceneChange = btn.isQuit = false;
            btn.textChange = true;

            //disable unneeded text fields
            for (int i = 4; i < text.Length; i++)
            {
                text[i].GetComponent<Renderer>().enabled = false;
                text[i].GetComponent<BoxCollider>().size = new Vector3(0, 0, 0);
            }
        }
        else
        {
            //change button one back to a text change
            btn = text[0].GetComponent<Button>();
            btn.sceneChange = false;
            btn.textChange = true;
            //reassign value of change for text change
            btn.change = 5;

            //change button four back to quit button
            btn = text[3].GetComponent<Button>();
            btn.sceneChange = btn.isQuit = true;
            btn.textChange = false;

            //disable unneeded text fields
            for (int i = 4; i < text.Length; i++)
            {
                text[i].GetComponent<Renderer>().enabled = true;
                text[i].GetComponent<BoxCollider>().size = colliderSize[i + changeID];
            }
        }
    }

    public void ChangeRulesText(int changeID)
    {
        //set the four lines of new text/ disabling mouse over
        for (int i = 0; i < 4; i++)
        {
            //replace the string literal new line into an actual new line at run time
            text[i].GetComponent<TextMesh>().text = msgs[i + changeID].Replace("\\n", System.Environment.NewLine);
            //assign new position
            text[i].GetComponent<Transform>().position = new Vector3(X, Y, Z[i + changeID]);
            //disable all colliders
            text[i].GetComponent<BoxCollider>().size = new Vector3(0, 0, 0);
        }

        //disable remaining text fields
        for (int i = 4; i < text.Length - 1; i++)
        {
            text[i].GetComponent<Renderer>().enabled = false;
        }

        //move title in place and change text
        Transform pos = text[5].GetComponent<Transform>();
        pos.position = new Vector3(-145f, pos.position.y, 93f);
        text[5].GetComponent<TextMesh>().text = msgs[changeID + 4];
    }

    public void ChangeControlsText(int changeID)
    {
        //set the four lines of new text/ disabling mouse over
        for (int i = 0; i < 3; i++)
        {
            //replace the string literal new line into an actual new line at run time
            text[i].GetComponent<TextMesh>().text = msgs[i + changeID].Replace("\\n", System.Environment.NewLine);
            //assign new position
            text[i].GetComponent<Transform>().position = new Vector3(X, Y, Z[i + changeID]);
            //disable all colliders
            text[i].GetComponent<BoxCollider>().size = new Vector3(0, 0, 0);
        }

        //move title in place and change text
        Transform pos = text[3].GetComponent<Transform>();
        pos.position = new Vector3(-145f, pos.position.y, 93f);
        text[3].GetComponent<TextMesh>().text = msgs[3+changeID];
    }

    public void ResetControlsText()
    {
        //reverts rules text to main menu 
        for (int i = 0; i < text.Length; ++i)
        {
            TextMesh temp = text[i].GetComponent<TextMesh>();

            //perform start function initialisation 
            if (temp.name == "Title")
            {
                temp.characterSize = 1.2f;
            }
            else
            {
                temp.characterSize = characterSize;
                text[i].GetComponent<Button>().sceneName = sceneNames[i];
                //reenable colliders
                text[i].GetComponent<BoxCollider>().size = colliderSize[i];
            }

            temp.text = msgs[i];
            text[i].GetComponent<Transform>().position = new Vector3(X, Y, Z[i]);
        }
    }

    public void ResetRulesText()
    {
        //reverts rules text to main menu 
        for (int i = 0; i < text.Length; ++i)
        {
            TextMesh temp = text[i].GetComponent<TextMesh>();

            //perform start function initialisation 
            if (temp.name == "Title")
            {
                temp.characterSize = 1.2f;
            }
            else
            {
                temp.characterSize = characterSize;
                text[i].GetComponent<Button>().sceneName = sceneNames[i];
                //reenable colliders
                text[i].GetComponent<BoxCollider>().size = colliderSize[i];
                //if any text items were disabled reenable them
                text[i].GetComponent<Renderer>().enabled = true;
            }

            temp.text = msgs[i];
            text[i].GetComponent<Transform>().position = new Vector3(X, Y, Z[i]);
        }
    }

    public void ChangeOptionsText(int changeID)
    {
        for (int i = 0; i < text.Length; ++i)
        {
            TextMesh temp = text[i].GetComponent<TextMesh>();

            //increase title size so it stands out
            if (temp.name == "Title")
            {
                temp.characterSize = 1.5f;
            }
            else
            {
                temp.characterSize = characterSize;

                text[i].GetComponent<BoxCollider>().size = colliderSize[i + changeID];
                temp.characterSize = characterSize;
            }

            temp.color = col;
            temp.text = msgs[i + changeID];
            text[i].GetComponent<Transform>().position = new Vector3(X, Y, Z[i + changeID]);
        }

        
        //grab canvas children ready for use
        Transform canvasTransform = GameObject.Find("Canvas").GetComponent<Transform>();
        //acquire the number of children ready for use
        int childCount = canvasTransform.childCount;

        //if not the first set of text
        if (changeID != 0)
        {
            //disable sliders whilst on another page of text
            for (int i = 0; i < childCount; ++i)
            {
                canvasTransform.GetChild(i).gameObject.SetActive(false);
            }

            //change the first button from a text change to a trigger
            Button Btn = text[0].GetComponent<Button>();
            Btn.trigger = true;
            Btn.textChange = false;
            //assign id of 0 to ensure appearence or music is set correctly
            Btn.change = 0;
            
            //disable ToggleButton and its triggering of music on/off
            text[1].GetComponent<ToggleButton>().enabled = false;
            text[1].GetComponent<ToggleButton>().isMusic = false;
            //turn button into trigger for assignment of appearence/music preferences
            text[1].GetComponent<Button>().trigger = true;

            //disable ToggleButton and its triggering of sound fx on/off
            text[2].GetComponent<ToggleButton>().enabled = false;
            text[2].GetComponent<ToggleButton>().isSfx = false;
            //turn button into trigger for assignment of appearence/music preferences
            text[2].GetComponent<Button>().trigger = true;

            //if it is the first set of text change
            if (changeID == 6)
            {
                //make it so buttons do appearence set up
                Btn.appearence = true;
                text[1].GetComponent<Button>().appearence = true;
                text[2].GetComponent<Button>().appearence = true;

            }//else leave it as default to do music set up

            //set up fourth button to go back to previous text
            text[3].GetComponent<Button>().change = 0;
        }
        else
        {
            //upon returning to first set of text reenable sliders for use
            for (int i = 0; i < childCount; ++i)
            {
                canvasTransform.GetChild(i).gameObject.SetActive(true);
            }

            //change the first button back from a trigger to a text change
            Button Btn = text[0].GetComponent<Button>();
            Btn.appearence = false;
            Btn.trigger = false;
            Btn.textChange = true;
            Btn.change = 6;

            //reenable ToggleButton and its triggering of music on/off
                //whilst disabling the trigger and appearence values of the button
            text[1].GetComponent<ToggleButton>().enabled = true;
            text[1].GetComponent<ToggleButton>().isMusic = true;
            text[1].GetComponent<Button>().trigger = false;
            text[1].GetComponent<Button>().appearence = false;

            //reenable ToggleButton and its triggering of music on/off
            //whilst disabling the trigger and appearence values of the button
            text[2].GetComponent<ToggleButton>().enabled = true;
            text[2].GetComponent<ToggleButton>().isSfx = true;
            text[1].GetComponent<Button>().trigger = false;
            text[2].GetComponent<Button>().appearence = false;

            //reassign the value of the fourth button to allow for music change
            text[3].GetComponent<Button>().change = 12;
        }
    }
}

