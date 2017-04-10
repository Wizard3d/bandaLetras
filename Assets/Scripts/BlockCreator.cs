using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockCreator : MonoBehaviour {

	public RhythmTool rhythmTool;

	public AudioClip[] audioClip;

	[SerializeField]
	private InputField myInputfield;

	[SerializeField]
	private Transform spawnTransform;

	[SerializeField]
	private GameObject blockPrefab;

	[SerializeField]
	private int beatsToSpawn;

	[SerializeField]
	private Dropdown myDropdown;

	private GameObject[] poolBlocks = new GameObject[10];

	private int countBeat;

	private int compas;

	private int currentBlock;

	private AnalysisData high;



	private int currentFrame;

	// Use this for initialization
	void Start ()
	{



		CreatPoolBlocks ();
	}

	public void SetUpSong ()
	{
		int num = myDropdown.value;

		beatsToSpawn = int.Parse( myInputfield.text);
		//Get the RhythmTool Component.
		rhythmTool = GetComponent<RhythmTool>();

		//Give it a song.
		rhythmTool.NewSong(audioClip[num]);

		//Subscribe to SongLoaded event.
		rhythmTool.SongLoaded += OnSongLoaded;

		high = rhythmTool.high;


	}

	//OnReadyToPlay is called by RhythmTool after NewSong(), when RhythmTool is ready to start playing the song.
	//When RhythmTool is ready depends on lead and on whether preCalculate is enabled.
	private void OnSongLoaded()
	{
		//Start playing the song
		rhythmTool.Play ();	
	}

	public void CheckBeat ()
	{
		countBeat++;
		if (countBeat >= beatsToSpawn) {
			countBeat = 0;
			SpawnBlock ();
		}

	}

	// Update is called once per frame
	void Update ()
	{		
		/*currentFrame = rhythmTool.currentFrame;
		if (currentBlock >= rhythmTool.totalFrames)
			return;

		float onSet = high.GetOnset (currentFrame);

	

		if (onSet > 0) {
			Debug.Log (high.magnitude[currentFrame]);
			SpawnBlock ();
		}*/
	}

	private void CreatPoolBlocks()
	{
		for(int i = 0; i <poolBlocks.Length; i++)
		{
			poolBlocks[i] = Instantiate(blockPrefab);
			poolBlocks[i].SetActive(false);
		}
	}

	void SpawnBlock ()
	{

		if (currentBlock>= poolBlocks.Length) {
			currentBlock = 0;
		} 

		poolBlocks [currentBlock].transform.position = spawnTransform.position;
		poolBlocks [currentBlock].SetActive (true);
		poolBlocks [currentBlock].GetComponent<Block> ().Speed = rhythmTool.bpm / 500;
		currentBlock++;

	}
}
