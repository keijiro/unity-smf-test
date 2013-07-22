#pragma strict

var smf : TextAsset;

function Start () {
    var loader = SmfLite.Loader();
    Debug.Log(loader.Load(smf.bytes)[0]);
}