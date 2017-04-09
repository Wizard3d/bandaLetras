using UnityEngine;
using System.Collections;

//This is a basic script that uses RhythmTool. 
//It gives a song to Rhythmtool, starts playing it and then draws lines to show some of the results.
public class DataController : MonoBehaviour
{	
	public RhythmTool rhythmTool;

    public AudioClip audioClip;

	private AnalysisData low;
	
	void Start ()
	{		
        //Get the RhythmTool Component.
		rhythmTool=GetComponent<RhythmTool>();

        //Get the data from the low frequency analysis.
        low = rhythmTool.low;

        //Subscribe to SongLoaded event.
        rhythmTool.SongLoaded += OnSongLoaded;

        //Give it a song.
		rhythmTool.NewSong(audioClip);
	}
	
    //OnReadyToPlay is called by RhythmTool after NewSong(), when RhythmTool is ready to start playing the song.
	private void OnSongLoaded()
	{
        //Start playing the song.
        rhythmTool.Play ();	
	}	
	
	// Update is called once per frame
	void Update ()
	{
        //Don't do anything if there is no song loaded.
        if (!rhythmTool.songLoaded)
            return;

        int currentFrame = rhythmTool.currentFrame;

		for(int i = 0; i < 100; i++){

            //make sure we don't try to access a frame that's out of range
            int frameIndex = Mathf.Min(currentFrame + i, rhythmTool.totalFrames);
            
            //the horizontal position of the line.
            float x = i - rhythmTool.interpolation;

            Vector3 start = new Vector3(x, low.magnitude[frameIndex], 0);
            Vector3 end = new Vector3(x+1, low.magnitude[frameIndex+1], 0);
            Debug.DrawLine(start, end, Color.black);
            
            float onset = low.GetOnset(frameIndex);

            //If there is an onset, draw a red line
            if (onset>0)
            {
				//create two vectors for drawing the line.
				//the magnitude of the onset is used as the length of the line
				start = new Vector3(x,0,0);
				end = new Vector3(x,onset,0);

				Debug.DrawLine(start,end,Color.red);
			}

            //If there is a beat, draw a white line
			if(rhythmTool.IsBeat(frameIndex)==1)
			{
				start = new Vector3(x,0,0);
				end = new Vector3(x,10,0);
				
				Debug.DrawLine(start,end,Color.white);
			}

            if(rhythmTool.IsChange(frameIndex))
            {
                float change = rhythmTool.NextChange(frameIndex);
                start = new Vector3(x, 0, 0);
                end = new Vector3(x, Mathf.Abs(change), 0);
                Debug.DrawLine(start, end, Color.yellow);
            }
		}

		//Draw a line at 0, so we can see where the current time in the song is.
		Debug.DrawLine(Vector3.zero,Vector3.up * 100,Color.red);
	}
}
