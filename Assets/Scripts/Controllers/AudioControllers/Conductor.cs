using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    public static Conductor Instance;
    
    //Song beats per minute
    //This is determined by the song you're trying to sync up to
    public float songBpm;

    //The number of seconds for each song beat
    public float secPerBeat;

    //Current song position, in seconds
    public float songPosition;

    //Current song position, in beats
    public float songPositionInBeats;

    //How many seconds have passed since the song started
    public float dspSongTime;

    public int ActiveMusic;

    //an AudioSource attached to this GameObject that will play the music.
    public AudioSource musicSource1;
    public AudioSource musicSource2;
    public AudioSource musicSource3;

    public int lastBeat;
    public bool IsBeat;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;

        //Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        //Start the music
        musicSource1.Play();

        musicSource2.volume = 0;
        musicSource2.Play();
        
        musicSource3.volume = 0;
        musicSource3.Play();

        ActiveMusic = 1;
        lastBeat = -1;
    }

    // Update is called once per frame
    void Update()
    {
        //determine how many seconds since the song started
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);

        //determine how many beats since the song started
        songPositionInBeats = songPosition / secPerBeat;

        var beat = (int) songPositionInBeats;
        IsBeat = beat != lastBeat;
        
        lastBeat = beat;
    }

    public IEnumerator ChangeMusic(int index)
    {
        while (!IsBeat)
            yield return null;

        musicSource1.volume = index == 1 ? 1 : 0;
        musicSource2.volume = index == 2 ? 1 : 0;
        musicSource3.volume = index == 3 ? 1 : 0;
    }

    public IEnumerator AllMusic()
    {
        while (!IsBeat)
            yield return null;

        musicSource1.volume = 1;
        musicSource2.volume = 1;
        musicSource3.volume = 1;
    }
}
