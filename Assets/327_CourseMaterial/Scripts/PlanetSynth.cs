using System;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSynth : MonoBehaviour
{
     [Header("FM Timbre")]
    [Range(0f, 1f)] public float amplitude = 0.12f; //volume
    [Range(0.1f, 8f)] public float modulatorRatio = 2f;
    [Range(0f, 20f)] public float modulationIndex = 1.5f;
    [Header("Envelope (seconds)")]
    [Range(0.001f, 1f)] public float attackSeconds = 0.01f;
    [Range(0.01f, 2f)] public float releaseSeconds = 0.18f;

    const double TwoPi = Math.PI * 2.0;

    public struct Voice
    {
        public bool active, held;
        public double phase, modulatorPhase, frequency;
        public float envelope, releaseStep;
    }
    struct NoteEvent
    {
        public int key;
        public bool playing;
        public float frequency;
    }
    struct Parameters
    {
        public float gain, ratio, index, attack, release;
    }

    // Main thread sends events; only the audio thread changes oscillator voices.
    readonly object eventLock = new object();
    readonly Queue<NoteEvent> events = new Queue<NoteEvent>(64);
    public readonly Voice[] voices = new Voice[10];

    Parameters parameters;
    AudioSource source;
    int sampleRate;

     void Awake()
    {
        sampleRate = AudioSettings.outputSampleRate;
        source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.Stop();
        source.clip = null;
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        UpdateParameters();
        // Without a clip, this filter is the source (same as SoundFM).
        // Notes reach the next DSP buffer without a streaming clip's read-ahead.
    }

    void OnEnable()
    {
        if (source != null) source.Play();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParameters();
    }

    void UpdateParameters()
    {
        lock (eventLock)
            parameters = new Parameters
            {
                gain = Mathf.Clamp01(amplitude), ratio = Mathf.Max(0.1f, modulatorRatio),
                index = Mathf.Max(0f, modulationIndex), attack = Mathf.Max(0.001f, attackSeconds),
                release = Mathf.Max(0.01f, releaseSeconds)
            };
    }

       void OnAudioFilterRead(float[] data, int channels)
    {
        Parameters settings;
        lock (eventLock)
        {
            settings = parameters;
            while (events.Count > 0)
            {
                NoteEvent note = events.Dequeue();
                if (note.playing)
                    voices[note.key] = new Voice { active = true, held = true, frequency = note.frequency };
                else
                {
                    voices[note.key].held = false;
                    voices[note.key].releaseStep = voices[note.key].envelope / (settings.release * sampleRate);
                }
            }
        }

        float attackStep = 1f / (settings.attack * sampleRate);
        for (int sample = 0; sample < data.Length; sample += channels)
        {
            double mix = 0.0;
            for (int i = 0; i < voices.Length; i++)
            {
                if (!voices[i].active) continue;
                Voice voice = voices[i];
                voice.envelope = voice.held ? Math.Min(1f, voice.envelope + attackStep)
                    : Math.Max(0f, voice.envelope - voice.releaseStep);
                if (!voice.held && voice.envelope <= 0f) voice.active = false;

                mix += Math.Sin(voice.phase + settings.index * Math.Sin(voice.modulatorPhase))
                    * voice.envelope * settings.gain;
                voice.phase = (voice.phase + TwoPi * voice.frequency / sampleRate) % TwoPi;
                voice.modulatorPhase = (voice.modulatorPhase + TwoPi * voice.frequency
                    * settings.ratio / sampleRate) % TwoPi;
                voices[i] = voice;
            }
            // Soft limiting keeps simultaneous notes below digital clipping.
            float output = (float)Math.Tanh(mix);
            for (int channel = 0; channel < channels; channel++)
                data[sample + channel] = output;
        }
    }

    public void Enqueue(int key, bool playing, float frequency)
    {
        lock (eventLock)
            events.Enqueue(new NoteEvent { key = key, playing = playing, frequency = frequency });
    }


}
