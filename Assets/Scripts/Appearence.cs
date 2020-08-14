using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Appearence : MonoBehaviour {

    //containers for alternative materials(textures)
    public Material[] OutBorder;
    public Material[] InBorder;
    public Material[] Dark;
    public Material[] Light;
    public Material[] LightPiece;
    public Material[] DarkPiece;

    //containers for all bit's of the board for assinging of materials
    GameObject[] squares;
    GameObject outer;
    GameObject inner;

    void Awake()
    {
        //on awake initialise the containers for the board
        outer = GameObject.Find("OuterBorder");
        inner = GameObject.Find("InnerBorder");
        squares = GameObject.FindGameObjectsWithTag("Squares");
    }

    // Use this for initialization
    void Start () {
        //on start set the appearence based off user assigned appearence value
        outer.GetComponent<Renderer>().material = OutBorder[Utils.appearence];
        inner.GetComponent<Renderer>().material = InBorder[Utils.appearence];

        for (int i = 0; i < squares.Length; i++)
        {
            //get its name
            string name = squares[i].name;

            if (name == "light") //if a light piece
            {
                squares[i].GetComponent<Renderer>().material = Light[Utils.appearence];
            }
            else if (name == "dark")    //if a dark piece
            {
                squares[i].GetComponent<Renderer>().material = Dark[Utils.appearence];
            }
        }

        //if on the game scene
        if (SceneManager.GetActiveScene().name == "Game")
        {
            //set all pieces to the player defined appearence
            GetComponent<CheckerBoard>().lightPiece[0].GetComponent<Renderer>().material = LightPiece[Utils.appearence];
            GetComponent<CheckerBoard>().lightPiece[1].GetComponent<Renderer>().material = LightPiece[Utils.appearence];

            GetComponent<CheckerBoard>().darkPiece[0].GetComponent<Renderer>().material = DarkPiece[Utils.appearence];
            GetComponent<CheckerBoard>().darkPiece[1].GetComponent<Renderer>().material = DarkPiece[Utils.appearence];
        }
    }
}
