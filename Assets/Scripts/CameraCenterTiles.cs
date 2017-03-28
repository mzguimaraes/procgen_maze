using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCenterTiles : MonoBehaviour {
	//attach to camera

	private Camera cam;

	void Start () {
		//ensure camera is orthographic
		cam = gameObject.GetComponent<Camera>();
		cam.orthographic = true;
		//center camera at start time
		transform.position = new Vector3(0f, 40f, 0f);
		//look down
		transform.rotation = Quaternion.Euler(90f, 0f, 0f);
	}

	void Update () {
		//find the global min and max values for x and z among all tiles spawned 
		float xMin = 0f, xMax = 0f,  zMin = 0f, zMax = 0f;
		foreach(Transform tile in Pathmaker.allTiles) {
			if (tile.position.x < xMin) xMin = tile.position.x;
			if (tile.position.x > xMax) xMax = tile.position.x;
			if (tile.position.z < zMin) zMin = tile.position.z;
			if (tile.position.z > zMax) zMax = tile.position.z;
		}
		float xAvg = (xMin + xMax) / 2;
		float zAvg = (zMin + zMax) / 2;

		//calculate ortho size that would put all tiles in view
		cam.orthographicSize = Mathf.Max(xMax - xAvg, zMax - zAvg) + 5; //Add 5 bc this was clipping it a bit

		transform.position = new Vector3(xAvg, transform.position.y, zAvg);
	}
}
