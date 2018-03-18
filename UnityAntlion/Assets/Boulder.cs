using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour {

	Collider2D col;
	public bool isDeadly;

	// Use this for initialization
	void Start () {
		col = GetComponent<Collider2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (col.attachedRigidbody.velocity.y < -0.3f) { // If falling downwards very slightly.
			col.gameObject.layer = LayerMask.NameToLayer("Ghost"); // Fall through other blocks below.
			isDeadly = true; // Makes the boulder capable of damaging the monster.
		}
		if (isDeadly) { // Handle staggering of monster.
			GameObject monster = GameObject.Find ("Monster");
			Monster m = monster.GetComponent<Monster>();
			if (gameObject.transform.position.y < monster.transform.position.y + 5f) {
				m.stagger();
				GameObject.Destroy (gameObject); // Boulder cleanup.
			}
		}
	}
}
