private var state:int=1; //0 begin, 1 move, 2 idle, 3 die, 4 idle2
private var stateChangeDone:boolean=false;

private var timeCounter:float=0;
private var behavChangeTime:float=1;

//RANDOM ROT OVER TIME VARIABLES
private var right:boolean=true;
private var timeOfChange:float;
private var rotTimeCounter:float;
private var lendulet:float;

private var rnd:float;



private var xStart:float;
private var zStart:float;


var moveSpeed:float=0.65;
var moveAnimSpeed:float=1;
var rotSpeed:float=1;
var rotTreshold:float=80;

var killTime:float=30;

var move2:boolean=false;
private var useMove2:boolean=false;

private var killTimeCounter:float=0;

function Start () {
behavChangeTime=Random.Range(1, 4);
timeOfChange=Random.Range(0, 3);

xStart=transform.position.x;
zStart=transform.position.z;
//	animation["move"].speed = moveAnimSpeed;	

var anim=this.gameObject.GetComponent(Animation);
anim["move"].speed = 0.8f;
}

function Update () {
timeCounter+=Time.deltaTime;
killTimeCounter+=Time.deltaTime;

	
if (timeCounter>behavChangeTime)
	{
	timeCounter=0;
	if (state==1&&stateChangeDone==false)
		{
		rnd=Random.Range(0, 100);

		
		
		if (rnd<10) state=2;
		if (rnd>10&&rnd<20) state=4;

if (rnd<60) 
{
useMove2=false;
behavChangeTime=Random.Range(1, 4);
} else {
useMove2=true;
behavChangeTime=0.5;
}

		stateChangeDone=true;
		}
	if (state==2&&stateChangeDone==false)
		{
		rnd=Random.Range(0, 100);
		
		if (rnd<50) state=1;
		if (rnd>75) state=4;

		stateChangeDone=true;
		
		}

	if (state==4&&stateChangeDone==false)
		{
		rnd=Random.Range(0, 100);
		
		if (rnd<50) state=1;
		if (rnd>75) state=2;

		stateChangeDone=true;
		
		}





	
	stateChangeDone=false;
	}	


if (killTime<killTimeCounter&&state!=3)
{
Destroy(gameObject, 4);
state=3;
}
 


if (state==1) ///MOVE BEGIN
	{
	//********************* RANDOM ROTATE OVER TIME PART
	rotTimeCounter+=Time.deltaTime;
	if (rotTimeCounter>timeOfChange)
		{
		right=!right;
		timeOfChange=Random.Range(0, 3);
		rotTimeCounter=0;
		}

	if (right==true)
		{
		lendulet-=10*Time.deltaTime*rotSpeed;
		}


	if (right==false)
		{
		lendulet+=10*Time.deltaTime*rotSpeed;
		}

	if (lendulet>rotTreshold)
		{
 		lendulet=rotTreshold;
		right=true;
		}
	
	if (lendulet<-rotTreshold)
		{
		lendulet=-rotTreshold;
		right=false;
		}
	transform.Rotate(0, lendulet*Time.deltaTime*rotSpeed, 0);

	// MOVE PART
	transform.Translate(Vector3(0,0,moveSpeed)*Time.deltaTime);

	// ANIM PART
	if (!GetComponent.<Animation>().IsPlaying("move")&&(move2==false))
		{
 		//GetComponent.<Animation>().CrossFade("move", 0.3); //also fuck unity 5 for changing animation stuff
		//GetComponent.<Animation>().speed = moveAnimSpeed; // no idea what im doing
		var anim2=this.gameObject.GetComponent(Animation);
		anim2["move"].speed = moveAnimSpeed;
		GetComponent.<Animation>().CrossFade("move", 0.3);		
		}

	if (move2==true)
		{
		if (!GetComponent.<Animation>().IsPlaying("move")&&useMove2==false)
			{
			//GetComponent.<Animation>().CrossFade("move", 0.3);  
			//GetComponent.<Animation>().speed = moveAnimSpeed;   
			var anim3=this.gameObject.GetComponent(Animation);
			anim3["move"].speed = moveAnimSpeed;
			GetComponent.<Animation>().CrossFade("move", 0.3);		
			}
		
		if (!GetComponent.<Animation>().IsPlaying("move2")&&useMove2==true)
			{
			//GetComponent.<Animation>().CrossFade("move2", 0.3);
			//GetComponent.<Animation>().speed = moveAnimSpeed;	
			var anim4=this.gameObject.GetComponent(Animation);
			anim4["move2"].speed = moveAnimSpeed;
			GetComponent.<Animation>().CrossFade("move2", 0.3);	
			}



		}

	}

if (state==2)
	{	
	if (!GetComponent.<Animation>().IsPlaying("idle")) GetComponent.<Animation>().CrossFade("idle", 0.3);
	}


if (state==3)
	{	
	if (!GetComponent.<Animation>().IsPlaying("die")) GetComponent.<Animation>().CrossFade("die");

	}


if (state==4)
	{	
	if (!GetComponent.<Animation>().IsPlaying("idle2")) GetComponent.<Animation>().CrossFade("idle2", 0.5);
	}



}