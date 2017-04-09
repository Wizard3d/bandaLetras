using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using UnityEngine.Events;

/// <summary>
/// Type of Onset passed in an event.
/// </summary>
public enum OnsetType { Low, Mid, High, All }

/// <summary>
/// Component that provides UnityEvents. Can be used anywhere and RhythmTool will find all RhythmEventProviders and call it's events when needed.
/// </summary>
[AddComponentMenu("Audio/RhythmEventProvider")]
public class RhythmEventProvider : MonoBehaviour
{
    private static ReadOnlyCollection<RhythmEventProvider> _eventProviders;
    public static ReadOnlyCollection<RhythmEventProvider> eventProviders
    {
        get
        {
            if (_eventProviders == null)
                _eventProviders = eventProviderList.AsReadOnly();
            return _eventProviders;
        }
    }

    [Tooltip("How many frames in advance events will be called")]
    public int targetOffset;

    /// <summary>
    /// The offset for this SongDataProvider. Increase to call events in advance.
    /// Use targetOffset to not skip any events when changing offset.
    /// </summary>
    [HideInInspector]
    public int offset;

    [HideInInspector]
    public float lastBeatTime;

    /// <summary>
    /// current frame without offset applied
    /// </summary>
    public int currentFrame = 0;
    /// <summary>
    /// current interpolation between current frame and next frame
    /// </summary>
    public float interpolation = 0;

    public int totalFrames = 0;
    
    /// <summary>
    /// Occurs when a beat is detected. beat
    /// </summary>
    public BeatEvent onBeat = new BeatEvent();

    /// <summary>
    /// Occurs when a quarter beat is detected. Beat, count
    /// </summary>
    public SubBeatEvent onSubBeat = new SubBeatEvent();

    /// <summary>
    /// Occurs when an onset is detected. OnsetType, onset
    /// </summary>
    public OnsetEvent onOnset = new OnsetEvent();

    /// <summary>
    /// Occurs when a large change in the song is detected. index, change
    /// </summary>
    public ChangeEvent onChange = new ChangeEvent();

    /// <summary>
    /// Occurs every Update. Provides general timing information. index, interpolation, beatLength, beatTime
    /// </summary>
    public TimingUpdateEvent timingUpdate = new TimingUpdateEvent();

    /// <summary>
    /// index, lastFrame
    /// </summary>
    public FrameChangedEvent onFrameChanged = new FrameChangedEvent();

    /// <summary>
    /// Occurs when a new song has been loaded. name, totalFrames
    /// </summary>
    public OnNewSong onSongLoaded = new OnNewSong();

    /// <summary>
    /// Occures when the song has ended.
    /// </summary>
    public UnityEvent onSongEnded = new UnityEvent();

    private static List<RhythmEventProvider> eventProviderList = new List<RhythmEventProvider>();
    public static event Action<RhythmEventProvider> EventProviderEnabled;

    void OnEnable()
    {
        if (!eventProviderList.Contains(this))
        {
            eventProviderList.Add(this);

            if (EventProviderEnabled != null)
                EventProviderEnabled(this);
        }
    }

    void OnDisable()
    {
        if (eventProviderList.Contains(this))
            eventProviderList.Remove(this);
    }

    [System.Serializable]
    public class BeatEvent : RhythmEvent<Beat>
    {        
    }

    [System.Serializable]
    public class SubBeatEvent : RhythmEvent<Beat, int>
    {        
    }

    [System.Serializable]
    public class TimingUpdateEvent : RhythmEvent<int, float, float, float>
    {       
    }

    [System.Serializable]
    public class FrameChangedEvent : RhythmEvent<int, int>
    {        
    }

    [System.Serializable]
    public class OnsetEvent : RhythmEvent<OnsetType, Onset>
    {      
    }

    [System.Serializable]
    public class ChangeEvent : RhythmEvent<int, float>
    {       
    }

    [System.Serializable]
    public class OnNewSong : RhythmEvent<string, int>
    {       
    }

    [System.Serializable]
    public abstract class RhythmEvent<T0> : UnityEvent<T0>
    {
        private int _listenerCount = 0;
        public int listenerCount { get { return _listenerCount + GetPersistentEventCount(); } }

        new public void AddListener(UnityAction<T0> call)
        {
            _listenerCount++;
            base.AddListener(call);
        }

        new public void RemoveListener(UnityAction<T0> call)
        {
            _listenerCount--;
            base.RemoveListener(call);
        }
    }

    [System.Serializable]
    public abstract class RhythmEvent<T0, T1> : UnityEvent<T0, T1>
    {
        private int _listenerCount = 0;
        public int listenerCount { get { return _listenerCount + GetPersistentEventCount(); } }

        new public void AddListener(UnityAction<T0, T1> call)
        {
            _listenerCount++;
            base.AddListener(call);
        }

        new public void RemoveListener(UnityAction<T0, T1> call)
        {
            _listenerCount--;
            base.RemoveListener(call);
        }
    }

    [System.Serializable]
    public abstract class RhythmEvent<T0, T1, T2, T3> : UnityEvent<T0, T1, T2, T3>
    {
        private int _listenerCount = 0;
        public int listenerCount { get { return _listenerCount + GetPersistentEventCount(); } }

        new public void AddListener(UnityAction<T0, T1, T2, T3> call)
        {
            _listenerCount++;
            base.AddListener(call);
        }

        new public void RemoveListener(UnityAction<T0, T1, T2, T3> call)
        {
            _listenerCount--;
            base.RemoveListener(call);
        }
    }
}

