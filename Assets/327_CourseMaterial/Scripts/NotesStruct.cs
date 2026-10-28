// IMDM327 Material
using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Note
{
    static Dictionary <string,float> notes = new Dictionary<string, float>
    {
      ["C"] = 261.626f,
      ["C#"] =  277.183f,
      ["Db"] =  277.183f,
      ["D"] = 293.665f,
      ["D#"] = 311.127f,
      ["Eb"] = 311.127f,
      ["E"]  = 329.628f,
      ["F"] = 349.228f,
      ["F#"] = 369.994f,
      ["Gb"] = 369.994f,
      ["G"] = 391.995f,
      ["G#"] = 415.305f,
      ["Ab"] = 415.305f,
      ["A"] = 440f,
      ["A#"] = 466.164f,
      ["Bb"] = 466.164f,
      ["B"] = 493.883f
    };
    public string note;              
    public int octave;         
    public float frequency; 

    public Note(string name, int oct)
    {
        note = name;
        octave = oct;
        if(notes.TryGetValue(name, out frequency))
        {
            frequency = frequency * Mathf.Pow(2,octave - 4);
        } else
        {
            frequency = 0f;
        }
    }
}

