/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/

// The target we are following
var target : Transform;
// The distance in the x-z plane to the target
var distance = 10.0;
// the height we want the camera to be above the target
var Normalheight : float = 20.0;
var Coverheight : float  = 11.0;
// How much we 
var heightDamping = 2.0;
var rotationDamping = 3.0;

var KillDelay : float = 5.0;

private var height : float = 5.0;
private var lastKill : float = -10.0f;
private var KillCam : boolean = false;
private var Adjust : float = 1.0f;

// Place the script in the Camera-Control group in the component menu
@script AddComponentMenu("Camera-Control/Smooth Follow")

function Start(){
	Adjust = PlayerPrefs.GetFloat ("CAdjust", 1.0f);
	Normalheight *= Adjust;
	Coverheight *= Adjust;
}

function SetSettings(H:Vector3){
	Normalheight = H.x;
	Coverheight = H.y;
}

function ClearTarget(){
	target = null;
	KillCam = false;
}

function KillCamera(killer:String){
	target = GameObject.Find(killer).transform;
	KillCam = true;
	lastKill = Time.time;
}

private var isCover : boolean = false;
private var lastCover : float = -10.0;
function OnCover(){
	isCover = true;
	lastCover = Time.time;
}

function LateUpdate () {
	// Early out if we don't have a target
	
	if(KillCam && Time.time > lastKill + KillDelay){
		target = null;
		KillCam = false;
	}
	
	if(isCover){
		height = Coverheight;
	}else{
		height = Normalheight;
	}
	
	if(Time.time > lastCover + 0.25f){
		isCover = false;
	}
	
	if (!target){
		if(GameObject.FindWithTag("localPlayer") != null){
			target = GameObject.FindWithTag("localPlayer").transform;
		}
		return;
	}
	
	// Calculate the current rotation angles
	var wantedRotationAngle = target.eulerAngles.y;
	var wantedHeight = target.position.y + height;
		
	var currentRotationAngle = transform.eulerAngles.y;
	var currentHeight = transform.position.y;
	
	// Damp the rotation around the y-axis
	currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

	// Damp the height
	currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

	// Convert the angle into a rotation
	var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	
	// Set the position of the camera on the x-z plane to:
	// distance meters behind the target
	transform.position = target.position;
	transform.position -= currentRotation * Vector3.forward * distance;

	// Set the height of the camera
	transform.position.y = currentHeight;
	
	// Always look at the target
	transform.LookAt (target);
}