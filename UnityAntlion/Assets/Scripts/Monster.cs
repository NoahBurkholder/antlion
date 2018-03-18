using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
	public float basePace;
	public float paceModifier;
	public bool isHurt;
	public float staggerTimer;
	public float chaseTime;
	public int tailgate;

	GameObject player;

	public Vector2 pos, vel, acc;

	// Use this for initialization
	void Start () {
		pos = new Vector2 (16f, 0);
		pos = gameObject.transform.position;
		player = GameObject.Find ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null) { // Check for player existence.
			if (isHurt) { // If the monster is stunned.
				if (staggerTimer <= 0) { // Check if the stun is over.
					isHurt = false;
				}
				// Slow velocity.
				vel.y *= 0.95f;

				// Tick timer.
				staggerTimer--;

			} else { // If monster is healthy.
				
				chaseTime = Time.frameCount; // This is causing issues because it's tied to the application's entire lifecycle, not the gameplay.

				paceModifier = chaseTime * 0.000001f; // Increase monster pace over subtle amount of time.

				strafe (); // Follow player horizontally.
				catchUp (); // Tailgate player vertically.
				vel.y = basePace + paceModifier;
				vel += acc;
			}
			pos += vel;
			gameObject.transform.position = pos;
		}
	}
	void strafe() {
		
			vel.x = (player.transform.position.x - pos.x) / 100f;


	}
	void catchUp() {
		if (player.transform.position.y > pos.y + tailgate) {
			pos.y = player.transform.position.y - tailgate;
		}
	}
	public void stagger() {
		isHurt = true;
		chaseTime -= 10f;
		staggerTimer = 3.0f;
	}
}
