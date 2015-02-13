using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {

	GameObject myGuy;
	GameObject targ;
	
	//private List<GameObject> theGuys;
	
	public GameObject TargetPrefab;
	public GameObject GuyPrefab;
	public GameObject ObstaclePrefab;
	public GameObject PreyPrefab;
	
	public GameObject centroidGO;
	
	//////////////////////////////////////////////////////MY CODE //////////////////////////////////////////////////////////
	
	//create Array of Flockers for Steering
	public List<GameObject> flockers;
	public List<GameObject> prey;
	//center position for Cohesion
	public Vector3 centroid;
	//direction of the flock
	public Vector3 direction;
	
	//////////////////////////////////////////////////////MY CODE //////////////////////////////////////////////////////////
	
	// Things we need to do for flocking
	// Create variables for the centroid, the flock direction and an arraylist of flockers
	// Create accessors for these variables
	// Write functions to update the centroid and flock direction every frame
	// We can make insure that this happens before the flockers calculate steering
	// forces by having the flockers use LateUpdate instead of Update.
	
	// Use this for initialization
	void Start ()
	{
		//create centroid object to follow
		//centroidGO = new GameObject();
		
		flockers = new List<GameObject>();
		prey = new List<GameObject>();

		Vector3 pos = new Vector3 (Random.Range (-40, 40), 4f, Random.Range (-40, 40));
		targ = (GameObject)GameObject.Instantiate (TargetPrefab, pos, Quaternion.identity);
		
		//pos = new Vector3 (0, 1.0f, 0);
		//myGuy = (GameObject)GameObject.Instantiate (GuyPrefab, pos, Quaternion.identity);
		//myGuy.GetComponent<FlockerSteering> ().Target = targ.gameObject;
		
		//make some obstacles
		for (int i=0; i< 40; i++) {
			pos = new Vector3 (Random.Range (-100, 100), 10f, Random.Range (-100, 100));
			Quaternion rot = Quaternion.Euler (0, Random.Range (0, 90), 0);
			GameObject o = (GameObject)GameObject.Instantiate (ObstaclePrefab, pos, rot);
			
			float scal = Random.Range (2f, 7f);
			o.transform.localScale = new Vector3 (scal, scal, scal);
		}
		
		for(int i = 0; i < 10; i++)
		{
			pos = new Vector3 (Random.Range (-40, 40), 5.0f, Random.Range (-40, 40));
			myGuy = (GameObject)GameObject.Instantiate (GuyPrefab, pos, Quaternion.identity);
			myGuy.GetComponent<FlockerSteering> ().Target = targ.gameObject;
			flockers.Add(myGuy);
		}

		for(int i = 0; i < 50; i++)
		{
			pos = new Vector3 (Random.Range (-200, 200), 15.0f, Random.Range (-200, 200));
			myGuy = (GameObject)GameObject.Instantiate (PreyPrefab, pos, Quaternion.identity);
			myGuy.GetComponent<PathSteering> ().Target = targ.gameObject;
			prey.Add(myGuy);
		}
		
		//tell camera to follow myGuy
		Camera.main.GetComponent<SmoothFollow> ().target = centroidGO.transform;
		flockers.Add(centroidGO);
		
	}

	void UpdateCentroid ()
	{
		centroid = Vector3.zero;
		foreach( GameObject go in flockers)
		{
			//sum all the position of the flockers for cohesion
			centroid += go.transform.position;
		}
		//divide by number of flockers for create center position for cohesion.
		centroid = centroid / flockers.Count;
		
		//update centroid object
		//centroidGO.transform.position = centroid;

			
	}

	void UpdateDirection ()
	{
		//for each flocker in array list caluclate the direction
		foreach( GameObject go in flockers)
		{
			//sum all the forward vectors together for allignment
			direction += go.transform.forward;
		}
		
		//update centroid object
		//centroidGO.transform.forward = direction;

			
	}
	
	public Vector3 GetCentroid()
	{
		Vector3 dir = centroidGO.transform.forward;
		dir.Normalize();
		Vector3 cent = centroidGO.transform.position - (5 * dir);
		return cent;
	}
	
	public Vector3 GetDirection()
	{
		return centroidGO.transform.forward;
	}
	
	public List<GameObject> GetFlockers()
	{
		return flockers;
	}

	public List<GameObject> GetPrey()
	{
		return prey;
	}
	
	public GameObject GetCentGO()
	{
		return centroidGO;
	}

	
	// Update is called once per frame
	void Update ()
	{
		GameObject flocker;
		foreach(GameObject f in flockers)
		{
			flocker = f;
			if(Vector3.Distance( flocker.transform.position, targ.transform.position) < 10) 
			{
				targ.transform.position = new Vector3(Random.Range(-100, 100), 4f, Random.Range(-100, 100));
			}
		}

		
		UpdateCentroid();
		UpdateDirection();

		GameObject clicked;
		Ray ray;
		RaycastHit rayHit;

		if (Input.GetMouseButtonDown(0))
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out rayHit))
			{
				clicked = rayHit.collider.gameObject;
				if(clicked != null)
				{
					if(clicked.GetComponent<FlockerSteering> ().Attack == false)
						{
							clicked.GetComponent<FlockerSteering> ().Attack = true;
							Camera.main.GetComponent<SmoothFollow> ().target = clicked.transform;
						}
					else
						{
							clicked.GetComponent<FlockerSteering> ().Attack = false;
							Camera.main.GetComponent<SmoothFollow> ().target = centroidGO.transform;
						}
				}
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha0))
		{
			int num = 0;
			if(flockers[num].GetComponent<FlockerSteering> ().Attack == false)
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = true;
				Camera.main.GetComponent<SmoothFollow> ().target = flockers[num].transform;
			}
			else
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = false;
				Camera.main.GetComponent<SmoothFollow> ().target = centroidGO.transform;
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			int num = 1;
			if(flockers[num].GetComponent<FlockerSteering> ().Attack == false)
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = true;
				Camera.main.GetComponent<SmoothFollow> ().target = flockers[num].transform;
			}
			else
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = false;
				Camera.main.GetComponent<SmoothFollow> ().target = centroidGO.transform;
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			int num = 2;
			if(flockers[num].GetComponent<FlockerSteering> ().Attack == false)
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = true;
				Camera.main.GetComponent<SmoothFollow> ().target = flockers[num].transform;
			}
			else
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = false;
				Camera.main.GetComponent<SmoothFollow> ().target = centroidGO.transform;
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			int num = 3;
			if(flockers[num].GetComponent<FlockerSteering> ().Attack == false)
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = true;
				Camera.main.GetComponent<SmoothFollow> ().target = flockers[num].transform;
			}
			else
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = false;
				Camera.main.GetComponent<SmoothFollow> ().target = centroidGO.transform;
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			int num = 4;
			if(flockers[num].GetComponent<FlockerSteering> ().Attack == false)
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = true;
				Camera.main.GetComponent<SmoothFollow> ().target = flockers[num].transform;
			}
			else
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = false;
				Camera.main.GetComponent<SmoothFollow> ().target = centroidGO.transform;
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha5))
		{
			int num = 5;
			if(flockers[num].GetComponent<FlockerSteering> ().Attack == false)
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = true;
				Camera.main.GetComponent<SmoothFollow> ().target = flockers[num].transform;
			}
			else
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = false;
				Camera.main.GetComponent<SmoothFollow> ().target = centroidGO.transform;
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha6))
		{
			int num = 6;
			if(flockers[num].GetComponent<FlockerSteering> ().Attack == false)
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = true;
				Camera.main.GetComponent<SmoothFollow> ().target = flockers[num].transform;
			}
			else
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = false;
				Camera.main.GetComponent<SmoothFollow> ().target = centroidGO.transform;
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha7))
		{
			int num = 7;
			if(flockers[num].GetComponent<FlockerSteering> ().Attack == false)
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = true;
				Camera.main.GetComponent<SmoothFollow> ().target = flockers[num].transform;
			}
			else
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = false;
				Camera.main.GetComponent<SmoothFollow> ().target = centroidGO.transform;
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha8))
		{
			int num = 8;
			if(flockers[num].GetComponent<FlockerSteering> ().Attack == false)
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = true;
				Camera.main.GetComponent<SmoothFollow> ().target = flockers[num].transform;
			}
			else
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = false;
				Camera.main.GetComponent<SmoothFollow> ().target = centroidGO.transform;
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha9))
		{
			int num = 9;
			if(flockers[num].GetComponent<FlockerSteering> ().Attack == false)
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = true;
				Camera.main.GetComponent<SmoothFollow> ().target = flockers[num].transform;
			}
			else
			{
				flockers[num].GetComponent<FlockerSteering> ().Attack = false;
				Camera.main.GetComponent<SmoothFollow> ().target = centroidGO.transform;
			}
		}

	}

}
