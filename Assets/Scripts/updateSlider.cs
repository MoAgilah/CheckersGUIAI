using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class updateSlider : MonoBehaviour {

    public bool soundChanged = false;
    public bool musicChanged = false;
    public bool difficultyChanged = false;

    public void Start()
    {
        //if on options page 
        if (SceneManager.GetActiveScene().name == "Options")
        {
            //assign depth value to slider for player feedback
            GameObject.Find("DifficultySlider").GetComponent<Slider>().value = DLLFunctions.GetDepth();
        }
    }
    
    //getters to allow for unity UI.Slider to alter values in different classes based on the below functions
    public void SetMusicVol(float val)
    {
        SoundManager.Instance.MusicVol = val;
    }

    public void SetSoundVol(float val)
    {
        SoundManager.Instance.SoundVol = val;
    }

    public void SetDepthVal(float val)
    {
        DLLFunctions.ChangeDepth((int)val);
    }
}
