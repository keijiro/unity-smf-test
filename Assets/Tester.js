#pragma strict

var smf : TextAsset;

function Start () {
    Debug.Log(SmfLite.FileLoader.Load(smf.bytes).tracks[0]);
}