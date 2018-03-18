using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

	public static bool LEFT = true;
	public static bool RIGHT = false;

	public ArrayList chunks;
	public int numChunks;
	public bool isBottomChunk;
	public Object chunkPrefab;

	public bool side;
	public int height;
	public int chunkXY;

	public float unpredictability;


	// Use this for initialization
	void Start () {
		numChunks = 3;
		isBottomChunk = true;
		chunkXY = 16;

		unpredictability = 1.5f;

		chunks = new ArrayList();
		for (int i = 0; i < numChunks; i++) {
				GameObject cl = (GameObject)Instantiate (chunkPrefab);
				Chunk clScript = cl.GetComponent<Chunk> ();
				clScript.side = LEFT;
				cl.transform.position = new Vector3 (0, height * chunkXY, 0) + this.transform.position;
				cl.transform.SetParent (this.transform);
				cl.transform.name = "Left" + height.ToString();
				clScript.chunkXY = chunkXY;
				clScript.unpredictability = unpredictability;
				clScript.isBottomChunk = isBottomChunk;

				GameObject cr = (GameObject)Instantiate (chunkPrefab);
				Chunk crScript = cr.GetComponent<Chunk> ();
				crScript.side = RIGHT;
				cr.transform.position = new Vector3 (chunkXY, height * chunkXY, 0) + this.transform.position;
				cr.transform.SetParent (this.transform);
				cr.transform.name = "Right" + height.ToString();
				crScript.chunkXY = chunkXY;
				crScript.unpredictability = unpredictability;
				crScript.isBottomChunk = isBottomChunk;
				isBottomChunk = false;


				height++;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
