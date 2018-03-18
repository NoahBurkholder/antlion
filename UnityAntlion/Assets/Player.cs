using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	bool collisionBottom, collisionTop, collisionRight, collisionLeft;
	Rigidbody2D rb;
	Vector3 pos;
	Vector3 vel;

	// Use this for initialization
	void Awake () {
		collisionBottom = false;
		pos = new Vector3 (2f, 2f, 0f);
		vel = new Vector3(0f, 0f, 0f);
		rb = gameObject.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		move ();
	}

	void move() {
		gameObject.transform.position += vel;
		collisionBottom = checkCollision (gameObject.transform.position, new Vector3 (0f, -1f, 0f));
		collisionTop = checkCollision (gameObject.transform.position, new Vector3 (0f, 1f, 0f));
		collisionRight = checkCollision (gameObject.transform.position, new Vector3 (0.5f, 0f, 0f));
		collisionLeft = checkCollision (gameObject.transform.position, new Vector3 (-0.5f, 0f, 0f));

		if (collisionBottom) {
			Debug.Log ("On ground: " + vel);
			vel.y = 0f;
			gameObject.transform.position.y +=
		} else {
			vel.y -= 0.01f;
		}



	}
	bool checkCollision(Vector3 v1, Vector3 v2) {
		RaycastHit2D tf = Physics2D.Linecast (v1, v1 + v2);
		if (tf) {
			Debug.DrawRay (v1, v2, Color.green);
		} else {
			Debug.DrawRay (v1, v2, Color.red);
		}
		return tf;
	}
}
