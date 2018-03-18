using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodPlank : MonoBehaviour {

	public float durability;
	Rigidbody2D rb;
	SpriteRenderer sr;
	// Use this for initialization
	void Start () {
		// Initial durability.
		durability = 1.0f;
		rb = GetComponent<Rigidbody2D> ();
		sr = GetComponent <SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		sr.color = new Color (1f, durability, durability);
		if (durability <= 0) {
			rb.constraints = RigidbodyConstraints2D.None;
		} else {
			if (durability < 1f) {
				durability += 0.001f;
			}
		}
	}
}
