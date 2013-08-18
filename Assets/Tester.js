#pragma strict

var smf : TextAsset;

private var seq : SmfLite.Sequencer;

function Start () {
    var container = SmfLite.FileLoader.Load(smf.bytes);
    seq = new SmfLite.Sequencer(container.tracks[0], container.division, 120);
    Debug.Log(seq.Start());
}

function Update () {
    Debug.Log(seq.Advance(Time.deltaTime));
}