using UnityEngine;
using System.Collections;


//Obstacle is just a GameObject that we need to avoid

public class ObstacleScript : MonoBehaviour {
	public float radius;
	
	public void Start ()
	{
		float s = transform.localScale.x / 2.0f;
		radius = Mathf.Sqrt (s * s + s * s);
		//Debug.Log ("radius = " + radius);
	}
	
	public float Radius {
		get { return radius; }
	}
	
	
}

	