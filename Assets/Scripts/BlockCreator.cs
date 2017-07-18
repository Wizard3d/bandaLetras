using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockCreator : MonoBehaviour {

	public RhythmTool rhythmTool;

	public AudioClip audioClip;

	[SerializeField]
	private Transform spawnTransform;

	[SerializeField]
	private GameObject blockPrefab;

	[SerializeField]
	private int beatsToSpawn;

	[SerializeField]
	private AudioSource mainSample;

	[SerializeField]
	private int[] songCode; //codigo que define el orden de salida de las silabas

	[SerializeField]
	private string[] words; //aca se definen cuales silabas van a salir

	private int currentWord; 

	private GameObject[] poolBlocks = new GameObject[10];

	private int countBeat;

	private int currentBlock;

	private AnalysisData lowFrec;

	private int currentFrame;

	private float lastFrec; // la ultima frecuencia que activo la creacion de bloques

	// Use this for initialization
	void Start ()
	{
		lowFrec = rhythmTool.low;

		SetUpRhythmTool ();

		CreatPoolBlocks ();
	}



	//OnReadyToPlay is called by RhythmTool after NewSong(), when RhythmTool is ready to start playing the song.
	//When RhythmTool is ready depends on lead and on whether preCalculate is enabled.
	private void OnSongLoaded()
	{
		//Start playing the song
		rhythmTool.Play();	
		mainSample.PlayDelayed(3.5f);
	}


	// Update is called once per frame
	void Update ()
	{		
		if (CheckInSongRange()) return;

		if (CheckIsOnSet())
			SpawnBlock ();
		
		lastFrec = lowFrec.magnitude [currentFrame];
	}

	public void CheckBeat ()
	{
		countBeat++;
		if (countBeat >= beatsToSpawn) {
			countBeat = 0;
			SpawnBlock ();
		}

	}

	private void CreatPoolBlocks()
	{
		for(int i = 0; i <poolBlocks.Length; i++)
		{
			poolBlocks[i] = Instantiate(blockPrefab);
			poolBlocks[i].SetActive(false);
		}
	}

	private bool CheckIsOnSet()
	{
		currentFrame = rhythmTool.currentFrame;

		float onSet = lowFrec.GetOnset (currentFrame);


		Debug.Log("Estoy aqui");
		return (onSet > 0 && lastFrec != lowFrec.magnitude [currentFrame]);
	}

	private bool CheckInSongRange ()
	{
		return currentBlock >= rhythmTool.totalFrames;

	}

	void SetUpRhythmTool ()
	{
		rhythmTool = GetComponent<RhythmTool>();

		//Give it a song.
		rhythmTool.NewSong(audioClip);

		//Subscribe to SongLoaded event.
		rhythmTool.SongLoaded += OnSongLoaded;
	}

	void SpawnBlock ()
	{

		if (currentBlock>= poolBlocks.Length) {
			currentBlock = 0;
		} 

		poolBlocks [currentBlock].transform.position = spawnTransform.position;
		poolBlocks [currentBlock].SetActive (true);
		poolBlocks [currentBlock].GetComponent<Block> ().SetText (words [songCode [currentWord]]);
		currentBlock++;
		currentWord++;
	}
}
