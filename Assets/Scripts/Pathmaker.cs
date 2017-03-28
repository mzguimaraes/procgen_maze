using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathmaker : MonoBehaviour {

	public static List<Transform> allTiles = new List<Transform>();
	public static List<Transform> allPathmakers = new List<Transform>();

	private int counter = 0; //tracks ATTEMPTS at placing a floor tile

	public Transform floorPrefab;
	public Transform pathmakerSpherePrefab;
	public Transform treasureChestPrefab;
	public Transform staircasePrefab;
	public Transform demonPrefab;
	public int globalTileMax = 300;

	private int counterMax = 50;
	private float roomProbability = 5f; //percentage

	private float tileDim = 5f;

	void Start () {
		//if this is the initial Pathmaker, add it to allPathmakers and spawn an Entrance tile
		if (allPathmakers.Count == 0) {
			allPathmakers.Add(transform);
			Instantiate(staircasePrefab, transform.position + new Vector3(0f, 1.5f, 0f), Quaternion.Euler(-180f, 90f, 90f));
		}

		counterMax = Random.Range(30, 100);
		roomProbability = Random.Range(0f, .07f);
		Debug.Log("New Pathmaker: allPathmakers.Count == " + allPathmakers.Count + ", counterMax == " + counterMax + ", roomProbability == " + roomProbability);
	}

	void connect(int xPos, int zPos) {
		//draws a hallway between Pathmaker position on call and the point described by (xPos, zPos)
		//moves Pathmaker to (xPos, zPos)
		Debug.Log("connect(" + xPos + ", " + zPos + ")");
		for ( ; xPos > 0; xPos --) {
			if (!Physics.Raycast(transform.position, -transform.up)) {
				allTiles.Add(Instantiate(floorPrefab, transform.position - transform.up, Quaternion.identity));
			}
			transform.Translate(tileDim, 0f, 0f);
		}

		for ( ; zPos > 0; zPos --) {
			if (!Physics.Raycast(transform.position, -transform.up)) {
				allTiles.Add(Instantiate(floorPrefab, transform.position - transform.up, Quaternion.identity));
			}
			transform.Translate(0f, 0f, tileDim);
		}

	}

	void spawnRoom(bool isTreasureRoom = false) {

		Vector3 entrancePos = transform.position;

		int roomZ = Random.Range(2, 5);
		int roomX = Random.Range(roomZ, roomZ + roomZ / 2);
		//more randomness!!! (this is probably pointless but it's fun so idc)
		if (Random.Range(1, 100) <= 50) {
			int roomXTemp = roomZ;
			roomZ = roomX;
			roomX = roomXTemp;
		}

		//first check if there are tiles in these locations already
		//if so, don't spawn a room here
		//make a hallway leading from current location to that room instead
		for (int i = 0; i < roomZ; i ++) {
			for (int j = 0; j < roomX; j ++) {
				if (Physics.Raycast(transform.position, -transform.up)) {
					Debug.Log("Room aborted");
					connect(j, i);
					return;
				}
				else {
					transform.Translate(tileDim, 0f, 0f);
				}
			}
			transform.Translate(-tileDim * roomX, 0f, tileDim);
		}

		//reset position
		transform.position = entrancePos;

		//lay down tiles in a grid
		for (int i = 0; i < roomZ; i ++) {

			for (int j = 0; j < roomX; j ++) {
				allTiles.Add(Instantiate(floorPrefab, transform.position - transform.up, Quaternion.identity));
				if (isTreasureRoom && Random.Range(1, 100) <= 50) {
					Instantiate(treasureChestPrefab, transform.position + new Vector3(0f, 2.25f, 0f), Quaternion.Euler(180f, -90f, -90f));
				}
				else if (Random.Range(1, 100) <= 50) {
					Instantiate(demonPrefab, transform.position, Quaternion.Euler(-90f, 0f, 180f));
				}
				transform.Translate(tileDim, 0f, 0f);
			}

			transform.Translate(-tileDim * roomX, 0f, tileDim);
		}

		//easier than the following step--move the spawner to the center of the room, 
		//then let the spawner's random pathing move it away from the room
		//just do that, don't do what I did.  I'm so full of regrets

		//resume pathfinding at a random point on room's perimeter that isn't the entrance
		//start by moving to where we started (we'll use this as the origin)
		transform.position = entrancePos;
		//a tile is at the perimeter if its X coord, Z coord, or both are 0 or its max value
		float x, z;
		//first, decide whether X, Z, or both should be locked to an extreme value
		//then fill in the unlocked value (if any) with a random value between 0 and the max
		int rng = Random.Range(1, 100);
		if (rng <= 33) {
			//lock X
			x = (Random.Range(1, 100) <= 50) ? 0 : roomX * tileDim;
			//exclude 0 because the else case handles corners
			z = Random.Range(1, roomZ) * tileDim;
		}
		else if (rng <= 67) {
			//lock Z
			x = Random.Range(1, roomX) * tileDim;
			z = (Random.Range(1, 100) <= 50) ? 0 : roomZ * tileDim;
		}
		else {
			//lock X and Z
			x = (Random.Range(1, 100) <= 50) ? 0 : roomX * tileDim;
			z = (Random.Range(1, 100) <= 50) ? 0 : roomZ * tileDim;
		}
		//move to exit location
		transform.Translate(x, 0f, z);
		//not bothering to rotate--if facing inwards then it'll just exit out a different perimeter tile
	}

	void Update () {
		if (counter < counterMax && allTiles.Count < globalTileMax) {
			
			float rng = Random.Range(0f, 1f);
			if (rng < .25f) {
				transform.Rotate(0f, 90f, 0f);
			}
			else if (rng < .5f) {
				transform.Rotate(0f, -90f, 0f);
			}
			else if (rng < .5f + roomProbability) {
				spawnRoom();
			}
			//Pathmaker spawn chance is dependent on the number of pathmakers
			//this prevents the probability increasing exponentially with the number of pathmakers
			else if (rng >= .98f + .005f * allPathmakers.Count) {
				allPathmakers.Add(Instantiate(pathmakerSpherePrefab, transform.position, transform.rotation));
			}

			if (!Physics.Raycast(transform.position, -transform.up)) {
				allTiles.Add(Instantiate(floorPrefab, transform.position - transform.up, Quaternion.identity));
			}
			else {
				Debug.Log("Skipped a tile!");
			}

			counter++;
			transform.Translate(0f, 0f, tileDim);
		}
		else {
			Debug.Log("Tiles created: " + allTiles.Count);
			spawnRoom(true);
			if (allTiles.Count < 100) {
				allPathmakers.Add(Instantiate(pathmakerSpherePrefab, transform.position, transform.rotation));
			}
			allPathmakers.Remove(transform);
			Destroy(this.gameObject);
		}
	}
}
