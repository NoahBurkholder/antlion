using UnityEngine;
using System.Collections;

public class Chunk : MonoBehaviour {

	public static int EMPTY = 0;
	public static int DIRT = 1;
	public static int PLANK = 2;
	public static int STONE = 3;
	public static int BOULDER = 4;
	public static int EDGE = 5;


	public static bool LEFT = true;
	public static bool RIGHT = false;

	public bool side;
	public bool isBottomChunk;

	public bool waitingForDeletion;

	public int id;

	public string seedData;
	public int[,] intData;

	public int chunkXY;

	public Object prefab0;
	public Object prefab1;
	public Object prefab2;
	public Object prefab3;
	public Object prefab4;

	public int heightIndex;

	public float unpredictability;

	// Use this for initialization
	void Start () {

		// Serialized chunks, designed in notepad.
		string[] chunks = new string[] {
			"0000000000000000000000000000000000001222221111110000100000000001221110000000000100000000220000000000000000000000122130000003112200000000000000000000000000000000000000000000000011122000000000220000000000000000000000000000000011122000000011110000000001111111",
			"0000000000000000000000000000000022222222000000000000000000000000000000000000000011110000000000000000000222000400000000000000110000000001110000000000111100000000000000000000000000000000000000002000000000000000000000000000000000000000000000001111000000000000",
			"0000000000000111000000000000000100000001100000000000000000000000000000000000011100000000000000010000001111000000000000000000000000000000000001110000000000000001000001111110000000000000000000000000000000000111000000000000000100001111111100000000111111110000",
			"0000000000000000000000000000000000111111111111110000000000000001000000000000000100000000000000001111110000111111000000000000000100000000000000010000000000000001000000111100000100000000000000000000000000000000000000000000000011111000000000000000000000000000",
			"0000000000000000000000000000000000111100011111110000000011000001000000000000000100000000004000001111330000111111000000000000000100000000000000010000000000000001000000000000000100000033330000000000000000000000000000000000000011111000000000000000000000000000",
			"0000000000000000000000000000000000001222231111110000100000000001221110004000000100000000220000000000000000000000111130000003111100000000000000000000000000000000000000000000000033003300330033000000000000000000000000000000000011111000001111110000000000000000",
			"0000000000000000333300000000000000000002222222220000000000000000000000000000000011111111000000000101011111000000000000011110000000000001111100000000000000000000000000000000000000000000000000010000000000000011000000000000110000000000001110000000000001111111",
			"1111110000011111111100003000000010100001110000001100000111000011111000211120011001110000000011000011100000011100000111000011100000001110011100000000000000000000000000000000000011111111111110010000000000000000000000000000000000000000000000001111100000111111",
			"0000000000000000000000000000000000111111111111110000000000000001000000000000000100000000000000011111111111111001000000000000000100000000000000010000000000000001001111111111111100000000000000000000000000000000000000000000000011111000000000000000000000000000",
			"1110000000001111111222222222111111100000000011111111122222221111111111110000111111112222222211101111000000111100112222211111100011000011110000001122220000000000110000000000000011000000000000001122000000000000000000000000000000000000000000001111000000001111",
			"0000000000000000000000000000000000111111111111110000000000000001000000000000000100000000000000011111111111111001000000000000000100000000000000010000000000000001001111111111111100000000000000000000000000000000000000000000000011111000000000000000000000000000",
			"0000000000000000000000000000000000111111111111120000000000000010000000000000001000000000000000101111111111110012000000000000001000000000000000100000000000000010001111111111111200000000000000000000000000000000000000000000000011111000000011110000000000000000",
			"0000000000000000000000000000000000002221122200000000000110000000100000011000000011110001100001111000000110000000000000011000000000000001100000000000222112220000000000011000000000000001100000000000001111000000000000000000000000000000000000001110000000000111",
			"0000000040000000221200222220012200100000000001000020000000000100001000000000010000100000002221222210000000000100001222000000000000100000000000000010000000000000001000000000012222100000000001000010000000222100000000000000000000000000000000001111000000001111",
			"0000000000000000000000000000040000000002222222210000000000000001000000000000000100000000000000011110000000000001000000000000000100000000000000000000000000000000000000000022210000000000000001110000000000000111000000000001111100000000000111111111110000111111",
			"0000000000000000222222222222222200000000000000000000000000000000000000000000000011300000000000000000000000000000000000000000000000000000000000040000000000002222111300000000000000000000000000000000000000000000000000000000000000000000000000001111300000000000"
		};

		// Make sure bottom chunks are always the same.
		if (isBottomChunk) {
			id = 0;
		} else { // Otherwise randomize chunks.
			id = Random.Range (0, chunks.Length);
		}

		// Set the seed data.
		seedData = chunks [id];

		// Initialize 2D array for grid.
		intData = new int [chunkXY,chunkXY];

		// De-serialize chunk data.
		optimizeSeed (seedData);

		// Handle bottom-tile setup.
		if (isBottomChunk) {
			for (int x = -1; x < chunkXY+1; x++) { // Iterate column.
				GameObject t = (GameObject)Instantiate (prefab4); // Edge
				modifyTile (t, x, -1, side);
			}
		}
		// Walls of bottom tiles.
		for (int y = 0; y < chunkXY; y++) { // Iterate row.
			GameObject t = (GameObject)Instantiate (prefab4); // Edge
			modifyTile (t, -1, y, side);
			for (int x = 0; x < chunkXY; x++) { // Iterate column.
				randomizeTile (x, y);
				placeTile(x, y, side);
			}
		}




	}

	// Takes serial chunk and deserializes it.
	public void optimizeSeed (string s) {
		if (s.Length == (chunkXY * chunkXY)) { // Check if correct length.
			char[] chars = new char[chunkXY * chunkXY]; // Chars array for string,
			chars = s.ToCharArray (); // Turn serial chunk into characters.

			for (int y = 0; y < chunkXY; y++) { // Iterate row.
				for (int x = 0; x < chunkXY; x++) { // Iterate column.
					// Set grid data on 2D array.
					intData [x, y] = (int)System.Char.GetNumericValue(chars [((chunkXY * chunkXY) - 1 - (y * chunkXY)) - (chunkXY - 1 - x)]);
				}
			}
		} else {
			Debug.LogError ("String input at ID #" + id + ". " + s.Length + "!= "+ (chunkXY * chunkXY));
		}
	}

	// Self-explanatory.
	public void placeTile(int x, int y, bool s) {

		GameObject t; // Temporary tile variable.

		if (intData [x, y] == DIRT) { // Is this tile a dirt tile?
			t = (GameObject)Instantiate (prefab0);
			modifyTile (t, x, y, s);
		} else if (intData [x, y] == PLANK) { // Etc.
			t = (GameObject)Instantiate (prefab1);
			modifyTile (t, x, y, s);
		} else if (intData [x, y] == STONE) {
			t = (GameObject)Instantiate (prefab2);
			modifyTile (t, x, y, s);
		} else if (intData [x, y] == BOULDER) {
			t = (GameObject)Instantiate (prefab3);
			modifyTile (t, x, y, s);

		}
	}

	// Adds random elements to tile within chunk.
	public void randomizeTile(int x, int y) {
		int p = Random.Range(0, 100);
		if (intData [x, y] == EMPTY) { // If tile is air.
			if ((y > 0) && (y < chunkXY - 1)) {
				if ((x > 0) && (x < chunkXY - 1)) {
					if (intData [x, y - 1] != PLANK) {
				
						int percentProbability = (int)(0.1f * unpredictability); // Set percent chance.
						if (p <= percentProbability) {
							intData [x, y] = DIRT;
						}
					}
				}
			}
		} else if (intData[x, y] == DIRT) { // If tile is dirt.
			if ((y > 0) && (y < chunkXY - 1)) {
				if ((x > 0) && (x < chunkXY - 1)) {
					if ((intData [x, y + 1] == EMPTY) && (intData [x, y - 1] == EMPTY)) {
						int percentProbability = (int)(0.5f * unpredictability); // Set percent chance.

						// If there's adjacent planks, increase probability.
						if (intData [x - 1, y] == PLANK) percentProbability *= 10;
						if (intData [x + 1, y] == PLANK) percentProbability *= 10;

						if (p <= percentProbability) { // Moment of truth.
							intData [x, y] = PLANK; // Create plank.
						}
					} else {
						int percentProbability = (int)(1.5f * unpredictability); // Set percent chance.
						if (p <= percentProbability) { // Moment of truth.
							intData [x, y] = STONE; // Create stone.
						}
					}
							} else {
								int percentProbability = (int)(1.5f * unpredictability);
								if (p <= percentProbability) { // Moment of truth.
						intData [x, y] = STONE; // Create stone.
								}
							}
			} else {
				int percentProbability = (int)(1.5f * unpredictability);
				if (p <= percentProbability) {
					intData [x, y] = STONE; // Create stone.
				}
			}
		} else if (intData[x, y] == PLANK) { // If tile is plank.
			intData [x, y] = PLANK; // Create plank.
		} else if (intData[x, y] == STONE) { // If tile is stone.
			intData [x, y] = STONE; // Create stone.
		} else if (intData[x, y] == BOULDER) { // If tile is boulder.
			intData [x, y] = BOULDER; // Create boulder.
		}
	}

	// Pseudo-constructor for new tile instantiations.
	public void modifyTile(GameObject t, int x, int y, bool s) {
		if (t != null) {
			if (s == RIGHT) {
				t.transform.position = new Vector3 (chunkXY - 1 - x, y, 0) + this.transform.position;
			} else {
				t.transform.position = new Vector3 (x, y, 0) + this.transform.position;
			}

			// Change properties.
			t.transform.name = "x" + x.ToString () + "y" + y.ToString ();
			t.transform.SetParent (this.transform);
		} else {
			Debug.LogError ("Tile is null.");
		}
	}

	GameObject monster;

	// Update is called once per frame
	void Update () {
		if (monster == null) { // Check if monster exists or not.
			monster = GameObject.Find ("Monster");
		} else {
			if (monster.transform.position.y > gameObject.transform.position.y+(2*chunkXY)) {
				GameObject.Destroy (gameObject); // Chunk cleanup below the monster to save memory.
			}
		}
	}
}
