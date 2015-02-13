using UnityEngine;
using System.Collections;

public class SteeringAttributes : MonoBehaviour {
	
	//these are common attributes required for steering calculations 
	public float maxSpeed = 50.0f;
	public float maxForce = 50.0f;
	public float mass = 1.0f;
	public float radius = 1.0f;
	
	// These weights will be exposed in the Inspector window

	public float seekWt = 50.0f;
	public float inBoundsWt = 30.0f;
	public float avoidWt = 70.0f;
	public float avoidDist = 7.0f;
	
		
	//////////////////////////////////////////////////////MY CODE //////////////////////////////////////////////////////////
	
	//wieghted flocking variables for inspector
	public float allignWt = 60.0f;
	public float cohesionWt = 60.0f;
	public float seperationWt = 70.0f;

	public float followWt = 30.0f;
	public float fleeWt = 100.0f;
	
	//////////////////////////////////////////////////////MY CODE //////////////////////////////////////////////////////////
	


}
