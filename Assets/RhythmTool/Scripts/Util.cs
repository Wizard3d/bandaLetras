using System;
using UnityEngine;
using System.Collections;

public static class Util
{
    private static LomontFFT fft = new LomontFFT();

    private static float[] magnitude = new float[0];
    private static float[] upsampledSignal = new float[0];
    private static float[] mono = new float[0];

    /// <summary>                                                                                            
    /// Get the forward Fourier Transform. The output is complex                                         
    /// valued after the first two entries, stored in alternating real and imaginary parts.                                                                         
    /// </summary>                                                                                           
    /// <param name="samples">The complex data stored as alternating real                                       
    /// and imaginary parts</param>        
    public static void GetSpectrum(float[] samples)
    {
        fft.RealFFT(samples, true);
    }

    /// <summary>
    /// Get the magnitude of a spectrum with complex values.
    /// </summary>
    /// <param name="spectrum">A spectrum with complex values</param>
    /// <returns>A new cached float array with the spectrum's magnidude.</returns>
    public static float[] GetSpectrumMagnitude(float[] spectrum)
    {
        if (magnitude.Length != spectrum.Length / 2)
            magnitude = new float[spectrum.Length / 2];

        GetSpectrumMagnitude(spectrum, magnitude);

        return magnitude;
    }

    /// <summary>
    /// Get the magnitude of a spectrum with complex values.
    /// </summary>
    /// <param name="spectrum">A spectrum with complex values</param>
    /// <param name="spectrumMagnitude">The magnitudes. Should be half the length of spectrum.</param>
    public static void GetSpectrumMagnitude(float[] spectrum, float[] spectrumMagnitude)
    {
        if (spectrumMagnitude.Length != spectrum.Length / 2)
            throw new Exception("SpectrumMagnitude length has to be half of spectrum length.");

        for (int i = 0; i < spectrumMagnitude.Length-2; i++)
        {
            int ii = (i * 2) + 2;
            float re = spectrum[ii];
            float im = spectrum[ii + 1];
            spectrumMagnitude[i] = Mathf.Sqrt((re * re) + (im * im));
        }

        spectrumMagnitude[spectrumMagnitude.Length - 2] = spectrum[0];
        spectrumMagnitude[spectrumMagnitude.Length - 1] = spectrum[1];
    }

    /// <summary>
    /// Reduce the number of channels in a signal by getting the mean value of all channels.
    /// </summary>
    /// <param name="samples">The signal with channels interleaved.</param>
    /// <param name="channels">Number of channels</param>
    /// <returns>A new cached float array with the reduced signal.</returns>
    public static float[] GetMono(float[] samples, int channels)
    {
        if (mono.Length != samples.Length / channels)
            mono = new float[samples.Length / channels];

        GetMono(samples, mono, channels);

        return mono;
    }

    /// <summary>
    /// Reduce the number of channels in a signal by getting the mean value of all channels.
    /// </summary>
    /// <param name="samples">The signal with channels interleaved.</param>
    /// <param name="mono">An output array.</param>
    /// <param name="channels">The number of channels.</param>
    public static void GetMono(float[] samples, float[] mono, int channels = 0)
    {        
        if(channels == 0)
            channels = samples.Length / mono.Length;

        if (samples.Length % mono.Length != 0)
            throw new ArgumentException("Sample length is not a multiple of mono length.");

        if(mono.Length * channels != samples.Length)
            throw new ArgumentException("Mono length does not match samples length for " + channels + " channels");

        for (int i = 0; i < mono.Length; i++)
        {
            float mean = 0;

            for (int ii = 0; ii < channels; ii++)
                mean += samples[i * channels + ii];

            mean /= channels;

            mono[i] = mean * 1.4f;
        }
    }
            
    /// <summary>
    /// Sum a part of an array.
    /// </summary>
    /// <param name="input">The array.</param>
    /// <param name="start">The start index of the part of the array to sum.</param>
    /// <param name="end">The end index of the part of the array to sum.</param>
    /// <returns></returns>
    public static float Sum(float[] input, int start, int end)
    {
        float output = 0;

        for (int i = start; i < end; i++)
        {
            output += input[i];
        }

        return output;
    }

    /// <summary>
    /// Smooth a signal.
    /// </summary>
    /// <param name="signal">The signal.</param>
    /// <param name="windowSize">The number of samples around the sample to use when smoothing.</param>
    /// <returns></returns>
    public static void Smooth(float[] signal, int windowSize)
    {
        //Note: this is "incorrect" (smoothing happens in place, so it will use already smoothed values of preceding indices)
        //but it gives better results when used with beat tracking
        
        for (int i = 0; i < signal.Length; i++)
        {
            float average = 0;

            for (int ii = i - (windowSize / 2); ii < i + (windowSize / 2); ii++)
            {
                if (ii > 0 && ii < signal.Length)
                    average += signal[ii];
            }

            signal[i] = average / windowSize;
        }
    }
        
    /// <summary>
    /// Stretch an array by interpolating between the original values.
    /// </summary>
    /// <param name="signal">A float array.</param>
    /// <param name="multiplier">How much to stretch the array.</param>
    /// <returns>A new cached stretched array.</returns>
    public static float[] UpsampleSingnal(float[] signal, int multiplier)
    {
        if (upsampledSignal.Length != signal.Length * multiplier)
            upsampledSignal = new float[signal.Length * multiplier];

        UpsampleSingnal(signal, upsampledSignal, multiplier);

        return upsampledSignal;
    }

    public static void UpsampleSingnal(float[] signal, float[] upsampledSignal, int multiplier)
    {
        if (upsampledSignal.Length != signal.Length * multiplier)
            throw new ArgumentException("UpsampledSignal does not match signal length and multiplier");

        for (int i = 0; i < signal.Length - 1; i++)
        {
            for (int ii = 0; ii < multiplier; ii++)
            {
                float f = (float)ii / (float)multiplier;
                upsampledSignal[(i * multiplier) + ii] = Mathf.Lerp(signal[i], signal[i + 1], f);
            }
        }
    }
}
