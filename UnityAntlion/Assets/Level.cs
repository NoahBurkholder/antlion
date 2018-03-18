using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

	public static bool LEFT = true;
	public static bool RIGHT = false;

	//public List<GameObject> chunks;
	public int numChunks;
	public bool isBottomChunk;
	public Object chunkPrefab;

	public bool side; // Left or right-hand tile.
	public int height; // Height index.

	public float tileSize, chunkSize;
	public int chunkXY;

	// Variable for modifying chunk unpredictability.
	public float unpredictability;


	public GameObject player;
	public float playerHeight;

	public GameObject monster;
	public float monsterHeight;

	public int chunkIndex;

	// Use this for initialization
	void Start () {
		// Initialize game world things.
		Application.targetFrameRate = 60;
		player = GameObject.Find ("Player");
		monster = GameObject.Find ("Monster");
		monsterHeight = monster.transform.position.y;
		isBottomChunk = true;

		chunkXY = 16; // Data length of the edge of a chunk.
		tileSize = 1.0f; // Tile size in-spatial matrix.
		chunkSize = chunkXY * tileSize; // Length of the edge of a chunk.

		unpredictability = 1.0f; // Base randomization factor.

		GameObject cl = (GameObject)Instantiate (chunkPrefab);
		Chunk clScript = cl.GetComponent<Chunk> ();
		clScript.side = LEFT;
		cl.transform.position = new Vector3 (0, height * chunkSize, 0) + this.transform.position;
		cl.transform.SetParent (this.transform);
		cl.transform.name = "Left" + height.ToString();
		clScript.chunkXY = chunkXY;
		clScript.unpredictability = unpredictability;
		clScript.isBottomChunk = isBottomChunk;

		clScript.heightIndex = height;


		GameObject cr = (GameObject)Instantiate (chunkPrefab);
		Chunk crScript = cr.GetComponent<Chunk> ();
		crScript.side = RIGHT;
		cr.transform.position = new Vector3 (chunkXY, height * chunkSize, 0) + this.transform.position;
		cr.transform.SetParent (this.transform);
		cr.transform.name = "Right" + height.ToString();
		crScript.chunkXY = chunkXY;
		crScript.unpredictability = unpredictability;
		crScript.isBottomChunk = isBottomChunk;
		isBottomChunk = false;

		crScript.heightIndex = height;

		height ++;

	}

	// Make new chunks.
	void stackChunk() {
		GameObject cl = (GameObject)Instantiate (chunkPrefab);
		Chunk clScript = cl.GetComponent<Chunk> ();
		clScript.side = LEFT;
		cl.transform.position = new Vector3 (0, height * chunkSize, 0) + this.transform.position;
		cl.transform.SetParent (this.transform);
		cl.transform.name = "Left" + height.ToString();
		clScript.chunkXY = chunkXY;
		clScript.unpredictability = unpredictability;

		GameObject cr = (GameObject)Instantiate (chunkPrefab);
		Chunk crScript = cr.GetComponent<Chunk> ();
		crScript.side = RIGHT;
		cr.transform.position = new Vector3 (chunkXY, height * chunkSize, 0) + this.transform.position;
		cr.transform.SetParent (this.transform);
		cr.transform.name = "Right" + height.ToString();
		crScript.chunkXY = chunkXY;
		crScript.unpredictability = unpredictability;

		height++;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("escape")) {
			Application.LoadLevel (0);
		}
		if (Input.GetKey ("r")) {
			Application.LoadLevel (Application.loadedLevel);
		}
		if (player != null) { // Check if player exists.
			// Kill player if monster catches up.
			if (player.transform.position.y < monster.transform.position.y) {
				Player p = player.GetComponent<Player> ();
				p.kill (); //
			}

			if (playerHeight < player.transform.position.y) {
				playerHeight = player.transform.position.y;
			}
			if (playerHeight > (chunkXY * chunkIndex)) {
				chunkIndex++;
				// Make higher chunks less predictable.
				unpredictability += 0.5f;
				stackChunk ();
			}
		}

	}
}
