var texture:Texture[];
private var rndInt:int;

function Start () {

rndInt=Random.Range(0, texture.length);

GetComponent.<Renderer>().material.mainTexture=texture[rndInt];

}

function Update () {

}