using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Analysis analyzes a song based on it's spectral magnitude at different moments in time.
/// Can be configured to analyze certain frequency ranges.
/// </summary>
[System.Serializable]
public class Analysis
{    
    /// <summary>
    /// The Analysis' data.
    /// </summary>
    public AnalysisData analysisData { get; private set; }

    /// <summary>
    /// The Analysis' name.
    /// </summary>
    public string name { get; private set; }

    private List<int> onsetIndices;    
    private Dictionary<int, Onset> _onsets;    
    private List<float> _magnitude;
    private List<float> _magnitudeSmooth;
    private List<float> _flux;
    private List<float> _magnitudeAvg;
    
    private int t1 = 0;
    private int t2 = 0;
    private int p1 = 0;
    private int p2 = 0;
        
    private int start;
    private int end;

    private int totalFrames;

    public Analysis(int start, int end, string name)
    {
        this.name = name;
        this.start = start;
        this.end = end;

        _magnitude = new List<float>();
        _flux = new List<float>();
        _magnitudeSmooth = new List<float>();
        _magnitudeAvg = new List<float>();
        _onsets = new Dictionary<int, Onset>(3000);

        onsetIndices = new List<int>(3000);

        analysisData = new AnalysisData(name, _magnitude, _flux, _magnitudeSmooth, _magnitudeAvg, _onsets);
    }
    
    /// <summary>
    /// Initialize the analysis for a new song.
    /// </summary>
    /// <param name='totalFrames'>
    /// Length of the new song.
    /// </param>
    public void Init(int totalFrames)
    {
        this.totalFrames = totalFrames;

        onsetIndices.Clear();

        _magnitude.Clear();
        _flux.Clear();
        _magnitudeSmooth.Clear();
        _magnitudeAvg.Clear();
        _onsets.Clear();
                
        _magnitude.Capacity = totalFrames;
        _flux.Capacity = totalFrames;
        _magnitudeSmooth.Capacity = totalFrames;
        _magnitudeAvg.Capacity = totalFrames;

        _magnitude.AddRange(new float[totalFrames]);
        _flux.AddRange(new float[totalFrames]);
        _magnitudeSmooth.AddRange(new float[totalFrames]);
        _magnitudeAvg.AddRange(new float[totalFrames]);
                
        t1 = 0;
        t2 = 0;
        p1 = 0;
        p2 = 0;

        int spectrumSize = (RhythmTool.fftWindowSize / 2);
        if (end < start || start < 0 || end < 0 || start >= spectrumSize || end > spectrumSize)
            Debug.LogError("Invalid range for analysis " + name + ". Range must be within fftWindowSize and start cannot come after end.");
    }

    /// <summary>
    /// Initialize the analysis with existing data.
    /// </summary>
    /// <param name="data"></param>
    public void Init(AnalysisData data)
    {
        onsetIndices.Clear();

        _magnitude.Clear();
        _flux.Clear();
        _magnitudeSmooth.Clear();
        _magnitudeAvg.Clear();
        _onsets.Clear();

        _magnitude.AddRange(data.magnitude);
        _flux.AddRange(data.flux);
        _magnitudeSmooth.AddRange(data.magnitudeSmooth);
        _magnitudeAvg.AddRange(data.magnitudeAvg);

        _magnitude.TrimExcess();
        _flux.TrimExcess();
        _magnitudeSmooth.TrimExcess();
        _magnitudeAvg.TrimExcess();

        foreach(KeyValuePair<int, Onset> onset in data.onsets)
        {
            _onsets.Add(onset.Key, onset.Value);
        }
    }

    /// <summary>
    /// Returns an onset. Onset will be 0 if there is none detected for this index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Onset GetOnset(int index)
    {
        Onset o;

        _onsets.TryGetValue(index, out o);

        return o;
    }
    
    /// <summary>
    /// Analyze the specified spectrum based on frequency ranges and previous spectra.
    /// </summary>
    /// <param name='spectrum'>
    /// A spectrum.
    /// </param>
    /// <param name='index'>
    /// The index of this spectrum.
    /// </param>
    public void Analyze(float[] spectrum, int index)
    {   
        _magnitude[index] = Util.Sum(spectrum, start, end);

        Smooth(index, 10, 5);
        Average(index);
               
        if (index > 1)
            _flux[index] = (_magnitude[index] - _magnitude[index - 1]);

        FindPeaks(index, 1.9f, 12);
        RankPeaks(index - 12, 50);
    }
    
    /// <summary>
    /// Draws the different results in a graph.
    /// </summary>
    /// <param name='index'>
    /// The index from where to start.
    /// </param>
    /// <param name='h'>
    /// The 
    /// </param>
    public void DrawDebugLines(int index, int h)
    {
        for (int i = 0; i < 299; i++)
        {
            if (i + 1 + index > totalFrames - 1)
                break;
            Vector3 s = new Vector3(i, _magnitude[i + index] + h * 100, 0);
            Vector3 e = new Vector3(i + 1, _magnitude[i + 1 + index] + h * 100, 0);
            Debug.DrawLine(s, e, Color.red);

            s = new Vector3(i, _magnitudeSmooth[i + index] + h * 100, 0);
            e = new Vector3(i + 1, _magnitudeSmooth[i + 1 + index] + h * 100, 0);
            Debug.DrawLine(s, e, Color.red);            

            s = new Vector3(i, _magnitudeAvg[i + index] + h * 100, 0);
            e = new Vector3(i + 1, _magnitudeAvg[i + 1 + index] + h * 100, 0);
            Debug.DrawLine(s, e, Color.black);

            s = new Vector3(i, _flux[i + index] + h * 100, 0);
            e = new Vector3(i + 1, _flux[i + 1 + index] + h * 100, 0);
            Debug.DrawLine(s, e, Color.blue);
            
            if (_onsets.ContainsKey(i + index))
            {
                Onset o = _onsets[i + index];
                                
                s = new Vector3(i, h * 100, -1);
                e = new Vector3(i, o.strength + h * 100, -1);
                Debug.DrawLine(s, e, Color.green);

                s = new Vector3(i, h * 100, 0);
                e = new Vector3(i, -o.rank + h * 100, 0);
                Debug.DrawLine(s, e, Color.white);                
            }
        }
    }

    private void Smooth(int index, int windowSize, int iterations)
    {
        _magnitudeSmooth[index] = _magnitude[index];

        for (int i = 1; i < iterations + 1; i++)
        {
            int iterationIndex = Mathf.Max(0, index - i * (windowSize / 2));
            Smooth(iterationIndex, windowSize);
        }
    }

    private void Smooth(int index, int windowSize)
    {
        float average = 0;
        for (int i = index - (windowSize / 2); i < index + (windowSize / 2); i++)
        {
            if (i > 0 && i < totalFrames)
                average += _magnitudeSmooth[i];
        }
        _magnitudeSmooth[index] = average / windowSize;
    }

    private void FindPeaks(int index, float thresholdMultiplier, int thresholdWindowSize)
    {
        int offset = Mathf.Max(index - (thresholdWindowSize / 2) - 1, 0);

        float threshold = Threshold(offset, thresholdMultiplier, thresholdWindowSize);

        if (_flux[offset] > threshold && _flux[offset] > _flux[offset + 1] && _flux[offset] > _flux[offset - 1])
        {
            //garbage
            Onset o = new Onset(offset, _flux[offset], 0); 
            _onsets.Add(offset, o);
            onsetIndices.Add(offset);
        }
    }

    private float Threshold(int index, float multiplier, int windowSize)
    {
        int start = Mathf.Max(0, index - windowSize/2);
        int end = Mathf.Min(_flux.Count - 1, index + windowSize/2);
        float mean = 0;
        for (int i = start; i <= end; i++)
            mean += Mathf.Abs(_flux[i]);
        mean /= (end - start);

        return Mathf.Clamp(mean * multiplier, 3, 70);
    }

    private void RankPeaks(int index, int windowSize)
    {
        int offset = Mathf.Max(0, index - windowSize);

        if (!_onsets.ContainsKey(offset))
            return;
        
        int onsetIndex = onsetIndices.IndexOf(offset);

        int rank = onsetIndices.Count - onsetIndex;

        for(int i = 5; i>0; i--)
        {            
            if(onsetIndex-i>0&&onsetIndex+i<onsetIndices.Count)
            {
                float c = _flux[offset];
                float p = _flux[onsetIndices[onsetIndex - i]];
                float n = _flux[onsetIndices[onsetIndex + i]];

                if(c>p && c>n)
                {
                    rank = 6-i;
                }

                if (onsetIndices[onsetIndex - i] < offset - windowSize / 2 && onsetIndices[onsetIndex + i] > offset + windowSize / 2)
                    rank = 6-i;
            }
        }

        _onsets[offset].rank = rank;
    }
     
    private void Average(int index)
    {
        if (index == totalFrames - 1)
        {
            t2 = index;
            p2 = index;
        }

        int offset = Mathf.Max(index - 100, 1);

        if (_magnitudeSmooth[offset] < _magnitudeSmooth[offset - 1] && _magnitudeSmooth[offset] < _magnitudeSmooth[offset + 1])
        {
            t1 = t2;
            t2 = offset;
        }

        if (_magnitudeSmooth[offset] > _magnitudeSmooth[offset - 1] && _magnitudeSmooth[offset] > _magnitudeSmooth[offset + 1])
        {
            p1 = p2;
            p2 = offset;
        }

        if (t1 != t2)
        {
            if (t2 < p2)
            {
                int nt = t2 - t1;
                int np = p2 - p1;

                float ft = (_magnitudeSmooth[t2] - _magnitudeSmooth[t1]) / nt;
                float fp = (_magnitudeSmooth[p2] - _magnitudeSmooth[p1]) / np;

                for (int i = p1; i < t2; i++)
                {
                    int ti = i - t1;
                    int pi = i - p1;

                    _magnitudeAvg[i] = (_magnitudeSmooth[t1] + (ft * ti)) + (_magnitudeSmooth[p1] + (fp * pi));
                    _magnitudeAvg[i] *= .5f;
                }

                t1 = t2;
            }            
        }
        
        if (p1 != p2)
        {
            if (p2 < t2)
            {
                int nt = t2 - t1;
                int np = p2 - p1;

                float ft = (_magnitudeSmooth[t2] - _magnitudeSmooth[t1]) / nt;
                float fp = (_magnitudeSmooth[p2] - _magnitudeSmooth[p1]) / np;

                for (int i = t1; i < p2; i++)
                {
                    int ti = i - t1;
                    int pi = i - p1;

                    _magnitudeAvg[i] = (_magnitudeSmooth[t1] + (ft * ti)) + (_magnitudeSmooth[p1] + (fp * pi));
                    _magnitudeAvg[i] *= .5f;
                }

                p1 = p2;
            }
        }
    }
}
