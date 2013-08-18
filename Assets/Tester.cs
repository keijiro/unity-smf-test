using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tester : MonoBehaviour
{
    public TextAsset sourceFile;

    List<SmfLite.Message> messageBuffer;
    SmfLite.Sequencer sequencer;
    float scale;

    void Start ()
    {
        var container = SmfLite.FileLoader.Load (sourceFile.bytes);
        sequencer = new SmfLite.Sequencer (container.tracks[0], container.division, 120.0f);
        sequencer.Start ();
    }
    
    void Update ()
    {
        messageBuffer = sequencer.Advance (Time.deltaTime);

        if (messageBuffer != null) {
            foreach (var m in messageBuffer) {
                if ((m.status & 0xf0) == 0x90) {
                    scale = 3.0f;
                }
            }
        }

        scale = 1.0f + (scale - 1.0f) * Mathf.Exp (-10.0f * Time.deltaTime);

        transform.localScale = Vector3.one * scale;
    }
}
