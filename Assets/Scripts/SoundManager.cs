using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    static SoundManager instance = null;    //set default instance to null for initialisation

    AudioSource[] Audio;                    //array of audio sources

    public AudioClip[] bkgMusic;            //collection of background music array

    public AudioClip[] soundFX;             //collection of sound effects music array

   //flags
    public bool MusicOn = true;             
    public bool SoundOn = true;

    public float MusicVol;
    public float SoundVol;

    void Awake()
    {
        if (Instance)
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
        }
    }

    void OnDestroy()
    {
        //every time a duplicate is destroyed 
            //flag to the singleton that he has changed scene
        Utils.sceneChanged[0] = true;
    }

    //returns instance for use
    public static SoundManager Instance
    {
        get
        {
            return instance;
        }
    }

    // Use this for initialization
    void Start()
    {
        //get all audio sources attached to the sound manager object
        Audio = GetComponents<AudioSource>();

        //set default clip on creation
        Audio[0].clip = Audio[0].clip = bkgMusic[(int)Utils.Songs.STANDARD];
        //always loop background music
        Audio[0].loop = true;

        //set music/sound volume variable based off the audio source volumes
        MusicVol = Audio[0].volume;
        SoundVol = Audio[1].volume;

        
        if (SceneManager.GetActiveScene().name == "Game" || SceneManager.GetActiveScene().name == "Options")
        {
            //assign the slider values to that of music/sound volume 
                //to give player feedback
            GameObject.Find("MusicSlider").GetComponent<Slider>().value = MusicVol;
            GameObject.Find("VolSlider").GetComponent<Slider>().value = SoundVol;
        }
    }

    void Update()
    {
        //check is music is enabled 
        if (MusicOn)
        {
            //if music isn't playing
            if (!Audio[0].isPlaying)
                Audio[0].Play();    //play audio
        }
        else
        {
            //if music is playing
            if (Audio[0].isPlaying)
                Audio[0].Stop();    //stop audio
        }

        Audio[0].volume = MusicVol;
        Audio[1].volume = SoundVol;

        //if sound manager changed scenes
        if (Utils.sceneChanged[0])
        {
            //and it's a scene with the below sliders in it
            if (SceneManager.GetActiveScene().name == "Options" || SceneManager.GetActiveScene().name == "Game")
            {
                //update the value by there current value
                GameObject.Find("MusicSlider").GetComponent<Slider>().value = MusicVol;
                GameObject.Find("VolSlider").GetComponent<Slider>().value = SoundVol;
            }

            //tell sound manager scene changed functionality has been performed
            Utils.sceneChanged[0] = false;
        }
    }


    public void PlaySoundFX(int identifier)
    {
        //dont play sounds if disabled
        if (SoundOn)
        {
            //else play sounds once
            switch (identifier)
            {
                case (int)Utils.SoundFx.HOVEROVER:
                    Audio[1].PlayOneShot(soundFX[identifier], 8F);
                    break;
                case (int)Utils.SoundFx.CLICK:
                    Audio[1].PlayOneShot(soundFX[identifier], 4F);
                    break;
                case (int)Utils.SoundFx.PLACED:
                    Audio[1].PlayOneShot(soundFX[identifier], 8F);
                    break;
                case (int)Utils.SoundFx.KINGED:
                    Audio[1].PlayOneShot(soundFX[identifier], 2F);
                    break;
                default:
                    Debug.Log("Sound FX doesn't exist");
                    break;
            }
        }
    }

    //enable switch of background music
    public void ChangeBKGMusic(int identifier)
    {
        Audio[0].Stop();    //stop current track

        //assign track and output track change
        switch (identifier)
        {
            case (int)Utils.Songs.STANDARD:
                Audio[0].clip = bkgMusic[identifier];
                DebugLog.Instance.Write("The background music has been changed too " + Utils.Songs.STANDARD.ToString());
                break;
            case (int)Utils.Songs.FOCUS:
                Audio[0].clip = bkgMusic[identifier];
                DebugLog.Instance.Write("The music has been changed too " + Utils.Songs.FOCUS.ToString());
                break;
            case (int)Utils.Songs.HIPHOP:
                Audio[0].clip = bkgMusic[identifier];
                DebugLog.Instance.Write("The music has been changed too " + Utils.Songs.HIPHOP.ToString());
                break;
            default:
                Audio[0].clip = bkgMusic[identifier];
                DebugLog.Instance.Write("The background music has been changed too " + Utils.Songs.STANDARD.ToString());
                break;
        }


        Audio[0].loop = true;
        Audio[0].Play();
    }

    //public accessors to disable or enable music and sound fx
    public void SetMusicOn()
    {
        MusicOn = !MusicOn;
    }

    public void SetSoundsOn()
    {
        SoundOn = !SoundOn;
    }


}
