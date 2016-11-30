using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count {
		public int minimum;
		public int maximum;

		public Count (int min, int max) {
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 8;
	public int rows = 8;

	public Count wallCount = new Count (5, 9); // range for how many walls per level
	public Count foodCount = new Count (1, 5);

	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;

	private Transform boardHolder; // child all the objects to this for hierarchy
	private List <Vector3> gridPositions = new List<Vector3>();

	void InitialiseList() {
		gridPositions.Clear();

		// leave 0 and columns empty (and ditto for rows) so there's an empty outer border
		for (int x = 1; x < columns - 1; x++) {
			for (int y = 1; y < rows -1; y++) {
				// list of possible positions
				gridPositions.Add (new Vector3 (x, y, 0f));
			}
		}
	}

	void BoardSetup() {
		boardHolder = new GameObject ("Board").transform;

		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				// randomly pick a floor tile
				GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];

				// or an outer wall if we're along the edge
				if (x == -1 || x == columns || y == -1 || y == rows)
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];

				// instantiate object (z is 0 because we're 2D, and Quaternion.identity <-> no rotation
				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);
			}
		}
	}

	Vector3 RandomPosition() { // returns a random (unoccupied) position
		// generate random number
		int randomIndex = Random.Range (0, gridPositions.Count);

		// pick a random location out of possible grid positions
		Vector3 randomPosition = gridPositions [randomIndex];

		// remove it so we don't pick it again
		gridPositions.RemoveAt (randomIndex);

		return randomPosition;
	}

	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum) {
		int objectCount = Random.Range (minimum, maximum + 1); // how many of a given object we will spawn

		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition ();
			// pick a random tile
			GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];
			// instantiate it
			Instantiate (tileChoice, randomPosition, Quaternion.identity);
		}
	}

	// PUBLIC - will be called by GameManager
	public void SetupScene (int level) {
		BoardSetup ();
		InitialiseList ();

		LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
		LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);

		// enemy count increases logarithmatically with level
		int enemyCount = (int)Mathf.Log (level, 2f);
		LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);

		// exit always in upper right
		Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
