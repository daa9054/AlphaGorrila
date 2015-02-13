// The Steer component has a collection of functions
// that return forces for steering 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

public class Steer : MonoBehaviour
{
	Vector3 dv = Vector3.zero; 	// desired velocity, used in calculations
	SteeringAttributes attr; 	// attr holds several variables needed for steering calculations
	public PathAttributes  path;
	CharacterController characterController;
	int curWP;
	GameObject tar = new GameObject();
	
	void Start ()
	{
		GameObject main = GameObject.Find ("MainGO");
		attr = main.GetComponent<SteeringAttributes> ();
		path = main.GetComponent<PathAttributes>();
		characterController = gameObject.GetComponent<CharacterController> ();	
		curWP = -1;
	}
	
	
	//-------- functions that return steering forces -------------//
	public Vector3 Seek (Vector3 targetPos)
	{
		//find dv, desired velocity
		dv = targetPos - transform.position;		
		dv = dv.normalized * attr.maxSpeed; //scale by maxSpeed
		dv -= characterController.velocity;
		dv.y = 0;								// only steer in the x/z plane
		return dv;
	}

	public Vector3 Arrival (Vector3 targetPos)
	{
		Vector3 toTarget = targetPos - gameObject.transform.position;
		float dist = Vector3.Distance(targetPos, gameObject.transform.position);
		if(dist > 0)
		{
			float deceleration = 0.3f;
			float speed = dist / deceleration;
			speed = Mathf.Min(speed, 50);
			dv = toTarget * speed / dist;
			dv -= characterController.velocity;
			return dv;
		}
		return Vector3.zero;
	}

	public Vector3 Flee (Vector3 targetPos)
	{
		//find dv, desired velocity
		float dist = Vector3.Distance(targetPos, gameObject.transform.position);

		float angle = Vector3.Angle(gameObject.transform.position, targetPos);
		float sign = Mathf.Sign(Vector3.Dot(gameObject.transform.forward,
		                                    Vector3.Cross(gameObject.transform.position, targetPos)));
		
		// angle in [-179,180]
		float signed_angle = angle * sign;
		
		// angle in [0,360]
		float angle360 =  ((signed_angle) + 360) % 360;

		dv = transform.position - targetPos;		
		dv = dv.normalized * attr.maxSpeed; 	//scale by maxSpeed
		dv -= characterController.velocity;
		dv.y = 0;								// only steer in the x/z plane


		if(dist < 15 && (angle360 < 60 || angle360 > 300))
		{
			Debug.Log("Angle: " + angle360);
			return dv;
		}
		else
		{
			return Vector3.zero;
		}
	}
	
	// Alignment will be called from CalcSteeringForce by a Flocker
	// CalcSteeringForce will get the average direction from the GameManager
	// which is a component of MainGO. MainGO will update this every frame
	public Vector3 Alignment (Vector3 direction)
	{
		direction = direction.normalized; //* attr.maxSpeed;
		direction -= characterController.velocity;
		return direction;
	
	}

	// Cohesion will be called from CalcSteeringForce by a Flocker
	// CalcSteeringForce will get the centroid from the GameManager
	// which is a component of MainGO. MainGO will update this every frame
	public Vector3 Cohesion (Vector3 centroid)
	{
		dv = Arrival (centroid);
		return dv;
	}
	
	
	// Separation will be called from CalcSteeringForce by a Flocker
	// CalcSteeringForce will get the array of flockers from the GameManager
	// which is a component of MainGO.
	public Vector3 Seperation (List<GameObject> flock)
	{
		//set the safe range for neighbors
		float safeDist = 3.0f;
		//create list of distances of unsafe neighbors
		List<float> distances = new List<float>();
		//create list of flee vectors of unsafe neighbors;
		List<Vector3> awayVectors = new List<Vector3>();
		
		//Vector3 steer = Vector3.zero;
		
		//loop through arraylist of flock
		foreach(GameObject cur in flock)
		{
			//cast the index to Flock object
			GameObject curFlocker = cur;
			//calculate distance from current neighbor
			float dist = Vector3.Distance(gameObject.transform.position, curFlocker.transform.position);
			
			//if current distance is less than an acceptable range
			if(dist > 0 && dist < safeDist)
			{
				//add distance value to ArrayList
				distances.Add(dist);
				//create temporary Vector3 to flee from
				Vector3 temp = transform.position- curFlocker.transform.position;
				//add temp vector to ArrayList
				awayVectors.Add(temp);
			}
			
		}
		
		//reset desired Velocity for correct vector addition
		dv = Vector3.zero;
		
		//loop through the values that are unsafe
		for(int i = 0; i < awayVectors.Count; i++)
		{
			//normalize flee vectors and multiply by inverse proportional distance
			Vector3 away = awayVectors[i];
			float dist = distances[i];
			away = away.normalized * (1/dist);
			//add normalized and weighted vector to desired vector
			dv += away;
		}	
		
		//normalize the sum and multiply by max speed
		dv = dv.normalized * attr.maxSpeed; 
		//substract desired from current velocity
		dv -= characterController.velocity;
		//return desired vector
		dv.y = 0;	
		return dv;
		
	}

	public GameObject FindClosestPrey(List<GameObject> herd)
	{
		float minDist = 1000000.0f;
		Vector3 target = Vector3.zero;
		Vector3 curPoint;
		for(int i = 0; i < herd.Count; i++)
		{
			curPoint = herd[i].transform.position;
			float dist = Vector3.Distance(curPoint, gameObject.transform.position);
			if(dist < minDist)
			{
				minDist = dist;
				target = curPoint;
				tar = herd[i];
			}
		}
		return tar;
	}

	
	// tether type containment - not very good!
	public Vector3 StayInBounds (float radius, Vector3 center)
	{
		if (Vector3.Distance (transform.position, center) > radius)
			return Seek (center);
		else
			return Vector3.zero;
	}
	

	public Vector3 AvoidObstacle (GameObject obst, float safeDistance)
	{ 
		dv = Vector3.zero;
		float obRadius = obst.GetComponent<ObstacleScript> ().Radius;

		//vector from vehicle to center of obstacle
		Vector3 vecToCenter = obst.transform.position - transform.position;
		//eliminate y component so we have a 2D vector in the x, z plane
		vecToCenter.y = 0;
		float dist = vecToCenter.magnitude;

		// if too far to worry about, out of here
		if (dist > safeDistance + obRadius + attr.radius)
			return Vector3.zero;
		
		//if behind us, out of here
		if (Vector3.Dot (vecToCenter, transform.forward) < 0)
			return Vector3.zero;

		float rightDotVTC = Vector3.Dot (vecToCenter, transform.right);
		
		//if we can pass safely, out of here
		if (Mathf.Abs (rightDotVTC) > attr.radius + obRadius)
			return Vector3.zero;
				
		//obstacle on right so we steer to left
		if (rightDotVTC > 0)
			dv += transform.right * -attr.maxSpeed * safeDistance / dist;
		else
		//obstacle on left so we steer to right
			dv += transform.right * attr.maxSpeed * safeDistance / dist;
			
		return dv;	
	}
	
	public Vector3 Follow(PathAttributes p)
	{
		Vector3 predict = transform.forward;
		predict.Normalize();
		predict *= 25;
		Vector3 predictLoc = transform.position + predict;
		
		Vector3 normal = Vector3.zero;
		Vector3 target = Vector3.zero;
		float worldRecord = float.MaxValue;
		
		int first;
		int last;
		if (curWP >= 0) {
			first = curWP-1;
			if(first == -1) first = p.waypoints.Count -1;
			last = (first + 3);
		}
		else {
			first = 0;
			last = p.waypoints.Count;
		}
		
		for (int i = first; i < last; i++) {
			
			Vector3 a = p.waypoints[i%p.waypoints.Count];
			Vector3 b = p.waypoints[(i+1)%p.waypoints.Count];
			
			Vector3 normalPoint = GetNormalPoint(predictLoc, a, b);
			
			Vector3 dir = b - a;
			
			Vector3 min = Vector3.Min (a,b);
			Vector3 max = Vector3.Max (a,b);
			
			if (normalPoint.x < min.x || normalPoint.x > max.x || 
			    normalPoint.y < min.y || normalPoint.y > max.y ||
			    normalPoint.z < min.z || normalPoint.z > max.y)
			{
				normalPoint = b;
				a = p.waypoints[(i+1)%p.waypoints.Count];
				b = p.waypoints[(i+2)%p.waypoints.Count];
				dir = b-a;
			}
			
			float d = Vector3.Distance(predictLoc, normalPoint);
			
			if( d < worldRecord ) {
				worldRecord = d;
				normal = normalPoint;
				curWP = i % p.waypoints.Count;
				dir.Normalize();
				dir *= 25;
				target = normal;
				target += dir;
			}			
		}
		
		if (worldRecord > p.radius){
			return Seek (target);
		}
		else {
			return Vector3.zero;
		}
		
	}
	
	
	//grab perpendicular 
	public Vector3 GetNormalPoint(Vector3 p, Vector3 a, Vector3 b) {
		Vector3 ap = p - a;
		Vector3 ab = b - a;
		
		ab.Normalize();
		ab *= Vector3.Dot(ap, ab);
		Vector3 normalPoint = a + ab;
		return normalPoint;
	}
	
}
