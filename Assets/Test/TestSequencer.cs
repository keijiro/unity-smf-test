using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestSequencer : MonoBehaviour
{
    public TextAsset sourceFile;
    SmfLite.FileContainer song;
    SmfLite.Sequencer sequencer;

    void ResetAndPlay ()
    {
        audio.Play ();
        sequencer = new SmfLite.Sequencer (song.tracks [0], song.division, 131.0f);
        ApplyMessages (sequencer.Start ());
    }

    IEnumerator Start ()
    {
        song = SmfLite.FileLoader.Load (sourceFile.bytes);
        yield return new WaitForSeconds (1.0f);
        ResetAndPlay ();
    }
    
    void Update ()
    {
        if (sequencer != null && sequencer.Playing) {
            ApplyMessages (sequencer.Advance (Time.deltaTime));
        }
    }

    void ApplyMessages (List<SmfLite.Message> messages)
    {
        if (messages != null) {
            foreach (var m in messages) {
                if ((m.status & 0xf0) == 0x90) {
                    if (m.data1 == 0x24) {
                        GameObject.Find ("Kick").SendMessage ("OnNoteOn");
                    } else if (m.data1 == 0x2a) {
                        GameObject.Find ("Hat").SendMessage ("OnNoteOn");
                    } else if (m.data1 == 0x2e) {
                        GameObject.Find ("OHat").SendMessage ("OnNoteOn");
                    } else if (m.data1 == 0x26 || m.data1 == 0x27 || m.data1 == 0x28) {
                        GameObject.Find ("Snare").SendMessage ("OnNoteOn");
                    }
                }
            }
        }
    }

    void OnGUI ()
    {
        if (GUI.Button (new Rect (0, 0, 300, 50), "Reset")) {
            ResetAndPlay ();
        }
    }
}