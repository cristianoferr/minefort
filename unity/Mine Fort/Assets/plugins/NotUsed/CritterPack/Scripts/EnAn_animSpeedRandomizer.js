
var minSpeed:float=0.7;
var maxSpeed:float=1.5;

function Start () {
GetComponent.<Animation>().Play(GetComponent.<Animation>().clip.name);

	for (var state : AnimationState in GetComponent.<Animation>()) {
		state.speed = Random.Range(minSpeed, maxSpeed);
	}
 
//animation[animation.clip.name].speed = Random.Range(minSpeed, maxSpeed);
}