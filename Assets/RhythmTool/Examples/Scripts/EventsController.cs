using UnityEngine;
using System.Collections;

public class EventsController : MonoBehaviour
{
    public Transform cube1Transform;

    public Transform cube2Transform;

    public RhythmTool rhythmTool;

    public RhythmEventProvider eventProvider;

    public AudioClip audioClip;

    // Use this for initialization
    void Start()
    {
        rhythmTool = GetComponent<RhythmTool>();
        eventProvider = GetComponent<RhythmEventProvider>();

        //subscribe to events.
        eventProvider.onSongLoaded.AddListener(OnSongLoaded);
        eventProvider.onBeat.AddListener(OnBeat);
        eventProvider.onSubBeat.AddListener(OnSubBeat);

        rhythmTool.NewSong(audioClip);
    }

    private void OnSongLoaded(string name, int totalFrames)
    {
        rhythmTool.Play();
    }

    private void OnBeat(Beat beat)
    {
        //give cube 1 a random scale every beat
        cube1Transform.localScale = Random.insideUnitSphere;
    }

    private void OnSubBeat(Beat beat, int count)
    {
        //give cube 2 a random scale every whole and half beat
        if(count == 0 || count == 2)
            cube2Transform.localScale = Random.insideUnitSphere;
    }
}
