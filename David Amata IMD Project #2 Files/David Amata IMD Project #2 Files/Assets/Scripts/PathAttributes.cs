using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathAttributes : MonoBehaviour {
	
	public List<Vector3> waypoints;
	
	public float radius;

	// Use this for initialization
	void Start () {
		
		radius = 10.0f;
		waypoints = new List<Vector3>();
		
		
		float r = 100.0f;
		float theta  = 0.0f;
		float numNodes = 400.0f;
		float sides = 40.0f;
		for (int i = 0; i < 400; i++)
		{
			theta += (float)6.28318531/sides;
			//r += radiusIncrement;
			
			float x = r * Mathf.Cos(theta);
			float z = r * Mathf.Sin(theta);
			//float y = Terrain.activeTerrain.SampleHeight(transform.position);
			float y = Terrain.activeTerrain.SampleHeight(new Vector3(x,0,z));
			//float y = 20.0f;
			addPoint (x,y,z);
		}
		
	
	}
	
	void addPoint(float x, float y, float z) {
		
		Vector3 point = new Vector3(x,y,z);
		waypoints.Add(point);
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
