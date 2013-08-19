using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestSequencer : MonoBehaviour
{
    public TextAsset sourceFile;
    SmfLite.Sequencer sequencer;

    IEnumerator Start ()
    {
        var container = SmfLite.FileLoader.Load (sourceFile.bytes);
        Debug.Log (container);
        sequencer = new SmfLite.Sequencer (container.tracks [0], container.division, 131.0f);

        yield return new WaitForSeconds (1.0f);

        ApplyMessages (sequencer.Start ());
        audio.Play ();
    }
    
    void Update ()
    {
        if (sequencer.Playing) {
            ApplyMessages (sequencer.Advance (Time.deltaTime));
        }
    }

    void ApplyMessages (List<SmfLite.Message> messages)
    {
        if (messages != null) {
            foreach (var m in messages) {
                if ((m.status & 0xf0) == 0x90) {
                    if (m.data1 == 0x24) {
                        GameObject.Find ("Kick").SendMessage("OnNoteOn");
                    } else if (m.data1 == 0x2a) {
                        GameObject.Find ("Hat").SendMessage("OnNoteOn");
                    } else if (m.data1 == 0x2e) {
                        GameObject.Find ("OHat").SendMessage("OnNoteOn");
                    } else if (m.data1 == 0x26 || m.data1 == 0x27 || m.data1 == 0x28) {
                        GameObject.Find ("Snare").SendMessage("OnNoteOn");
                    }
                }
            }
        }
    }
}