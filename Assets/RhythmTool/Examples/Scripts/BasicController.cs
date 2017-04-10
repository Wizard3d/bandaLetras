using UnityEngine;
using System.Collections;

//This is a basic script that uses RhythmTool. It gives a song to the analyzer and starts playing and analyzing it.
//See DataController and the example scene for an example on how to get the data in a game.
public class BasicController : MonoBehaviour
{

    public RhythmTool rhythmTool;

	public AudioClip audioClip;

	private int countBeat;

	private int compas;

	// Use this for initialization
	void Start ()
	{
        //Get the RhythmTool Component.
        rhythmTool = GetComponent<RhythmTool>();
        
        //Give it a song.
        rhythmTool.NewSong(audioClip);

        //Subscribe to SongLoaded event.
        rhythmTool.SongLoaded += OnSongLoaded;
    }

    //OnReadyToPlay is called by RhythmTool after NewSong(), when RhythmTool is ready to start playing the song.
    //When RhythmTool is ready depends on lead and on whether preCalculate is enabled.
    private void OnSongLoaded()
	{
		//Start playing the song
		rhythmTool.Play ();	
	}

	public void Yaju ()
	{
		countBeat++;
		if (countBeat > 8) {
			countBeat = 0;
			compas++;
			Debug.Log ("Jo"+compas);
		}

	}

	// Update is called once per frame
	void Update ()
	{		
		//Draw graphs representing the data.
		rhythmTool.DrawDebugLines ();		

	}
}
