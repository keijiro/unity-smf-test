#pragma strict

var smf : TextAsset;

function Start () {
    Debug.Log(SmfLite.Loader.Load(smf.bytes));
}
