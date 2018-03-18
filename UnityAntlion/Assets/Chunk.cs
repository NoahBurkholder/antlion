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

	public int id;

	public string seedData;
	public int[,] intData;

	public int chunkXY;

	public Object prefab0;
	public Object prefab1;
	public Object prefab2;
	public Object prefab3;

	public float unpredictability;

	// Use this for initialization
	void Start () {
		string[] chunks = new string[] {
			"0000000000000000111100000000000000000000000000000000000000111100000000000000000011110000000000000000000000000000000000000011110000000001111000000000111100000000000000000000000000000000000000000000000000000000111100000000000000000000000000000000000000000000",
			"0000000000000111000000000000000100000011110000000000000000000000000000000000011100000000000000010000001111000000000000000000000000000000000001110000000000000001000000111100000000000000000000000000000000000111000000000000000100000011110000000000000000000000",
			"0000000000000000111100000000000000000000000000000000000000000000000000000000000011110000000000000000000000000000000000000011110000000001111000000000111100000000000000000000000000000000000000000000000000000000111100000000000000000000000000000000000000000000"
		};

		id = Random.Range (0, chunks.Length);
		seedData = chunks [id];
		intData = new int [chunkXY,chunkXY];
		optimizeSeed (seedData);

		if (isBottomChunk) {
			for (int x = -1; x < chunkXY+1; x++) { // Iterate column.
				GameObject t = (GameObject)Instantiate (prefab3);
				modifyTile (t, x, -1, side);
			}
		}

		for (int y = 0; y < chunkXY; y++) { // Iterate row.
			GameObject t = (GameObject)Instantiate (prefab3);
			modifyTile (t, -1, y, side);
			for (int x = 0; x < chunkXY; x++) { // Iterate column.
				randomizeTile (x, y);
				placeTile(x, y, side);
			}
		}




	}

	public void optimizeSeed (string s) {
		if (s.Length == (chunkXY * chunkXY)) {
			char[] chars = new char[chunkXY * chunkXY];
			chars = s.ToCharArray ();

			for (int y = 0; y < chunkXY; y++) { // Iterate row.
				for (int x = 0; x < chunkXY; x++) { // Iterate column.
					intData [x, y] = (int)System.Char.GetNumericValue(chars [((chunkXY * chunkXY) - 1 - (y * chunkXY)) - (chunkXY - 1 - x)]);
				}
			}
		} else {
			Debug.LogError ("String input at ID #" + id + ". " + s.Length + "!= "+ (chunkXY * chunkXY));
		}
	}

	public void placeTile(int x, int y, bool s) {

		GameObject t;

		if (intData [x, y] == DIRT) { // Is this tile a dirt tile?
			t = (GameObject)Instantiate (prefab0);
			modifyTile (t, x, y, s);
		} else if (intData [x, y] == PLANK) {
			t = (GameObject)Instantiate (prefab1);
			modifyTile (t, x, y, s);
		} else if (intData [x, y] == STONE) {
			t = (GameObject)Instantiate (prefab2);
			modifyTile (t, x, y, s);
		}
	}

	public void randomizeTile(int x, int y) {
		int p = Random.Range(0, 100);

		if (intData [x, y] == EMPTY) {
			int percentProbability = (int)(1.0f * unpredictability);
			if (p <= percentProbability) {
				intData [x, y] = DIRT;
			}
		} else if (intData[x, y] == DIRT) {
			if ((y > 0) && (y < chunkXY - 1)) {
				if ((x > 0) && (x < chunkXY - 1)) {
					if ((intData [x, y + 1] == EMPTY) && (intData [x, y - 1] == EMPTY)) {
						int percentProbability = (int)(25.0f * unpredictability);
						if (intData [x - 1, y] == PLANK) percentProbability *= 2;
						if (intData [x + 1, y] == PLANK) percentProbability *= 2;
						if (p <= percentProbability) {
							intData [x, y] = PLANK;
						}
					} else {
						int percentProbability = (int)(5.0f * unpredictability);
						if (p <= percentProbability) {
							intData [x, y] = STONE;
						}
					}
							} else {
								int percentProbability = (int)(5.0f * unpredictability);
								if (p <= percentProbability) {
									intData [x, y] = STONE;
								}
							}
			} else {
				int percentProbability = (int)(5.0f * unpredictability);
				if (p <= percentProbability) {
					intData [x, y] = STONE;
				}
			}
		}
	}

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
	// Update is called once per frame
	void Update () {
	
	}
}
