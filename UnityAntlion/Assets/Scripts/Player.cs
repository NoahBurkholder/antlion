using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	Rigidbody2D rb;

	// Raycasting
	int xRays, yRays;
	SpriteRenderer sr;
	public Vector2 pos, vel, acc, dim, grav;
	float skin = 0.1f;

	public bool cLeft, cRight, cTop, cBottom;
	bool finishedJump, isCrouching;

	// Camera
	GameObject camObject;
	Camera cam;
	float baseZoom = 5.0f;
	float zoomTarget;
	float zoom;

	// Use this for initialization
	void Awake () {
		camObject = gameObject.transform.Find("Main Camera").gameObject;
		cam = camObject.GetComponent<Camera> ();
		zoom = 5.0f;
		zoomTarget = 5.0f;

		grav = new Vector2 (0, 0.01f);
		getScale ();

		sr = GetComponent<SpriteRenderer>();
		yRays = 4;
		xRays = yRays * 2;
		pos = gameObject.transform.position;
		vel = new Vector2(0f, 0f);
		rb = gameObject.GetComponent<Rigidbody2D> ();

		finishedJump = true;
	}

	// For getting player dimensions.
	void getScale() {
		crouchOffset = new Vector2 (0, 0);
		dim = new Vector2 (1f, 1.5f);
	}

	// Update is called once per frame
	void Update () {

		// Ordering of operations.
		move ();
		collisions ();
		cameraZoom ();

	}

	// Experimental. Not in use.
	void cameraZoom() {
		//zoomTarget = (5f + (vel.magnitude * 10f));
		//zoom = ((zoom - zoomTarget)) / 10f;

		//cam.orthographicSize = 5f - zoom;
	}


	int tiredTimer;
	void move() {
		// Acceleration variables.
		float accStep;
		float accAir;

		if (tiredTimer > 0) { // If tired.
			accStep = 0.000f;
			accAir = 0.0000f;
			tiredTimer--;
			sr.color = Color.red;
			if (cBottom) {
				vel.x *= 0.8f;
				acc.x = 0;
			}
		} else { // If not tired.
			accStep = 0.005f;
			accAir = 0.0005f;
			isTired = false;
			sr.color = Color.white;
		} 

		if (cBottom) { // If grounded.
			if (Input.GetAxis ("Horizontal") > 0.6f) { // If right arrow pressed.
				if (Mathf.Sign(vel.x) != Mathf.Sign(Input.GetAxis("Horizontal"))) { // If moving same direction as current velocity.
					acc.x += accStep;
					vel.x += accStep;
				} else { // If moving counter to velocity.
					acc.x += accStep + accStep + accStep;				
				}

			} else if (Input.GetAxis ("Horizontal") < -0.6f) { // If left arrow pressed.
				if (Mathf.Sign(vel.x) != Mathf.Sign(Input.GetAxis("Horizontal"))) { // If moving same direction as current velocity.
					acc.x -= accStep;
					vel.x -= accStep;
				} else { // If moving counter to velocity.
					acc.x -= accStep + accStep + accStep;				
				}
			} else { // If no horizontal arrows pressed.
				vel.x *= 0.8f;
				acc.x = 0;
			}
			if (Input.GetAxis ("Vertical") < -0.6f) { // If down arrow pressed.
				isCrouching = true;
			} else { // If not pressed.
				if (!cTop) { // Then if you have headspace.
					isCrouching = false;
				}
			}
			if (Input.GetAxis ("Jump") > 0.6f) {  // If jumping.
				if (Input.GetAxis ("Vertical") < -0.6f) { // Then if holding down arrow, drop through platforms.
					dropTimer = 0.2f;
				} else { // Otherwise jump normally.
					if (finishedJump) {
						jump (jumpMultiplier);
						finishedJump = false;
					}
				}
			} else {
				finishedJump = true;
			}
		} else { // If in air.
			if (Input.GetAxis ("Horizontal") > 0.6f) { // Then if holding right.
				acc.x += accAir;
			} else if (Input.GetAxis ("Horizontal") < -0.6f) { // Or if holding left.
				acc.x -= accAir;
			} else { // Or if holding nothing.
				acc.x = 0;
			}
			if (Input.GetAxis ("Vertical") < -0.6f) { // Crouching in air.
				isCrouching = true;
			} else { // Uncrouching in air.
				if (!cTop) {
					isCrouching = false;
				}
			}
			if (vel.y > 0) { // Vertical in-air upward influence.
				if (Input.GetAxis ("Jump") > 0.6f) {
					vel.y += 0.005f;
				}
			}
			if (vel.y < 0) { // Vertical in-air downward influence.
				if (Input.GetAxis ("Vertical") < -0.6f) {
					vel.y -= 0.005f;
				} 
			}
		}

		// Cap movement/acceleration.
		capSpeed ();
		crouch ();
		drill ();


		vel += acc;

		if (cTop) {
			if (vel.y > 0) {
				vel.y = 0;
			}
		}

		if (cBottom) {
			if (vel.y < 0) {
				vel.y = 0;
			}
		}

		if (cRight) {
			if (vel.x > 0) {
				vel.x = 0;
			}
		}
		if (cLeft) {
			if (vel.x < 0) {
				vel.x = 0;
			}
		}

		if (vel.x < -0.01f) {
			sr.flipX = true;
		} else if (vel.x > 0.01f) {
			sr.flipX = false;
		}

		if (!cBottom) {
			vel -= grav;
		}
		pos += vel;
		gameObject.transform.position = pos;
	}
		
	// Self explanatory.
	void capSpeed () {
		float maxSpeed;
		float maxAcc;

		// Differentiate crouching and non-crouching speeds.
		if (!isCrouching) {
			maxSpeed = 0.15f;
			maxAcc = 0.008f;
		} else {
			maxSpeed = 0.05f;
			maxAcc = 0.008f;
		}

		if (vel.x > maxSpeed) {
			vel.x = maxSpeed;
		}
		if (vel.x < -maxSpeed) {
			vel.x = -maxSpeed;
		}
		if (acc.x > maxAcc) {
			acc.x = maxAcc;
		}
		if (acc.x < -maxAcc) {
			acc.x = -maxAcc;
		}
	}
		
	// Jump using summed jump height from foot-rays.
	void jump(float tileFactor) {
		float jumpForce;
		if (tiredTimer > 0) {
			jumpForce = 0f;
		} else {
			jumpForce = 0.22f;
		}

		cBottom = false;
		vel.y = jumpForce * tileFactor;
	}

	float dropTimer; // Time in seconds that the player can fall through platforms for.
	void crouch() {
		// Handle avatar dimensions when crouching.
		if (isCrouching) {
			crouchOffset = new Vector2 (0, -0.12f);
			dim = new Vector2 (1f, 1.3f);
		} else {
			crouchOffset = new Vector2 (0, 0);
			dim = new Vector2 (1f, 1.5f);
		}
	}


	public bool doubleUp, doubleDown, doubleLeft, doubleRight;
	public bool isDrilling, isTired;
	public int coolUp, coolDown, coolLeft, coolRight;
	public int drillTimer;
	void drill () {
		// If landed on ground.
		if (cBottom) {
			doubleUp = false;
			doubleDown = false;
			doubleLeft = false;
			doubleRight = false;
		}
		// Tick timers down.
		if (tiredTimer > 0) {
			if (coolUp > 0) {
				coolUp--;
			}
			if (coolDown > 0) {
				coolDown--;
			}
			if (coolRight > 0) {
				coolRight--;
			}
			if (coolLeft > 0) {
				coolLeft--;
			}
		} else { // Otherwise, if player isn't tired.
			if (coolUp > 0) { // Listen
				if (Input.GetKeyDown ("up")) { // If two taps in 20 frames.
					if (!isDrilling) {
						isDrilling = true;
						drillTimer = 20;
						vel.y += 0.25f;
						doubleUp = true;
					}
				}
				coolUp--;
			}
			if (coolDown > 0) {
				if (Input.GetKeyDown ("down")) { // If two taps in 20 frames.
					if (!isDrilling) {
						isDrilling = true;
						drillTimer = 20;
						vel.y -= 0.25f;
						doubleDown = true;
					}
				}
				coolDown--;
			}
			if (coolLeft > 0) {
				if (Input.GetKeyDown ("left")) { // If two taps in 20 frames.
					if (!isDrilling) {
						isDrilling = true;
						drillTimer = 20;
						vel.x -= 0.3f;
						doubleLeft = true;
					}
				}
				coolLeft--;
			}
			if (coolRight > 0) {
				if (Input.GetKeyDown ("right")) { // If two taps in 20 frames.
					if (!isDrilling) {
						isDrilling = true;
						drillTimer = 20;
						vel.x += 0.3f;
						doubleRight = true;
					}
				}
				coolRight--;
			}
		}
		// Set framecount for listening upon hitting key for first time.
		if (Input.GetKeyDown ("up")) {
			coolUp = 10;
		}
		if (Input.GetKeyDown ("down")) {
			coolDown = 10;
		}
		if (Input.GetKeyDown ("right")) {
			coolRight = 10;
		}
		if (Input.GetKeyDown ("left")) {
			coolLeft = 10;
		}

		// When drilling.
		if (drillTimer > 0) {
			drillTimer--;
			isTired = true;
			tiredTimer = 150;
		} else {
			isDrilling = false;
		}



	}

	public void kill() {
		camObject.transform.SetParent (gameObject.transform.parent);
		GameObject.Destroy (gameObject);
	}

	float jumpMultiplier;
	public Vector2 rayVector1, rayVector2;
	public Vector2 rayPos;
	public Vector2 crouchOffset;
	void collisions() {
		cLeft = false;
		cRight = false;
		cBottom = false;
		cTop = false;

		float xOffset = crouchOffset.x + (dim.x / 2.0f) - skin;
		float yOffset = crouchOffset.y + (dim.y / 2.0f) - skin;
		float ySpacing = (yOffset * 2f) * ((float)1f / (float)xRays);
		float xSpacing = (xOffset * 2f) * ((float)1f / (float)yRays);

		rayVector1 = vel;
		rayVector2 = vel;

		if (dropTimer > 0) {
			dropTimer -= Time.deltaTime;
		}


		for (int i = 0; i < xRays; i++) {
			// Left-facing collisions.
			rayPos = pos + new Vector2 (-xOffset, -yOffset + (i * ySpacing) + (ySpacing/2.0f)); // Set position of ray start points.
			RaycastHit2D rayLeft = Physics2D.Linecast (rayPos, rayPos + new Vector2(rayVector1.x - skin, 0)); // Make new ray.

			if (rayLeft) { // If hit.
				rayVector1 = rayLeft.point - (rayPos); // Create local vector based on ray path.
				if (rayLeft.collider.tag == "Thin") {
					Debug.DrawRay (rayPos, new Vector2 (rayVector1.x - skin, 0), Color.magenta); // Debug line.
				} else {
					// Begin drill code.
					if (isDrilling) {
						if (doubleLeft) {
							if (rayLeft.collider.gameObject.layer == LayerMask.NameToLayer ("Solid")) {
								if (rayLeft.collider.gameObject.layer != LayerMask.NameToLayer ("Unbreakable")) {
									GameObject c = rayLeft.collider.gameObject;
									GameObject.Destroy (c);
								}
							}
						}
					}
					// End drill code.
					if (rayLeft.collider.tag == "Boulder") {
						if (rayVector1.x > -skin) { // If ray length hits, and is less than skin.
							if (acc.x < -0.001f) {
								rayLeft.collider.attachedRigidbody.AddTorque (-acc.x * 200f);
								vel.x = acc.x;
								//acc.x = 0;

							}
							cLeft = true; // Officially collided.
							Debug.DrawRay (rayPos, new Vector2 (rayVector2.x + skin, 0), Color.green); // Debug line.
						} else {
							Debug.DrawRay (rayPos, new Vector2 (rayVector2.x + skin, 0), Color.yellow); // Debug line.
						}
					} else {
					vel.x = rayVector1.x + skin; // Make sure not to overshoot.
					Debug.DrawRay (rayPos, new Vector2 (rayVector1.x - skin, 0), Color.cyan); // Debug line.
					if (rayVector1.x > -skin) { // If ray length hits, and is less than skin.
						cLeft = true; // Officially collided.
						vel.x = 0; // Hit.
						Debug.DrawRay (rayPos, new Vector2 (rayVector1.x - skin, 0), Color.green); // Debug line.
					}
					Debug.DrawRay (rayPos, new Vector2 (rayVector1.x - skin, 0), Color.yellow); // Debug line.
					}
				}

			} else { // If no hit.
				Debug.DrawRay (rayPos, new Vector2(rayVector1.x - skin, 0), Color.red); // Debug line.
			}

			// Right-facing collisions.
			rayPos = pos + new Vector2 (xOffset, -yOffset + (i * ySpacing) + (ySpacing/2.0f)); // Set position of ray start points.
			RaycastHit2D rayRight = Physics2D.Linecast (rayPos, rayPos + new Vector2(rayVector2.x + skin, 0)); // Make new ray.

			if (rayRight) { // If hit.
				rayVector2 = rayRight.point - (rayPos); // Create local vector based on ray path.
				if (rayRight.collider.tag == "Thin") {
					Debug.DrawRay (rayPos, new Vector2 (rayVector2.x + skin, 0), Color.magenta); // Debug line.
				} else {
					// Begin drill code.
					if (isDrilling) {
						if (doubleRight) {
							if (rayRight.collider.gameObject.layer == LayerMask.NameToLayer ("Solid")) {
								if (rayRight.collider.gameObject.layer != LayerMask.NameToLayer ("Unbreakable")) {
									GameObject c = rayRight.collider.gameObject;
									GameObject.Destroy (c);
								}
							}
						}
					}
					// End drill code.
					if (rayRight.collider.tag == "Boulder") {
						if (rayVector2.x < skin) { // If ray length hits, and is less than skin.
							if (acc.x > 0.001f) {
								rayRight.collider.attachedRigidbody.AddTorque (-acc.x * 200f);
								vel.x = acc.x;
								//acc.x = 0;

							}
							cRight = true; // Officially collided.
							Debug.DrawRay (rayPos, new Vector2 (rayVector2.x + skin, 0), Color.green); // Debug line.
						} else {
							Debug.DrawRay (rayPos, new Vector2 (rayVector2.x + skin, 0), Color.yellow); // Debug line.
						}

					} else {

					vel.x = rayVector2.x - skin; // Make sure not to overshoot.
					Debug.DrawRay (rayPos, new Vector2 (rayVector2.x + skin, 0), Color.cyan); // Debug line.
					if (rayVector2.x < skin) { // If ray length hits, and is less than skin.
						cRight = true; // Officially collided.
						vel.x = 0; // Hit.
						Debug.DrawRay (rayPos, new Vector2 (rayVector2.x + skin, 0), Color.green); // Debug line.
						} else {
							Debug.DrawRay (rayPos, new Vector2 (rayVector2.x + skin, 0), Color.yellow); // Debug line.
						}
					}
				}

			} else { // If no hit.
				Debug.DrawRay (rayPos, new Vector2(rayVector2.x + skin, 0), Color.red); // Debug line.
			}
		}

		float sumJumpMultiplier = 0; // Jump-force summation.

		rayVector1 = vel;
		rayVector2 = vel;

		// Iterate over the head and feet rays.
		for (int i = 0; i < yRays; i++) {

			// Upward-facing collisions.
			rayPos = pos + new Vector2 (-xOffset + (i * xSpacing) + (xSpacing/2.0f), yOffset); // Set position of ray start points.
			RaycastHit2D rayUp = Physics2D.Linecast (rayPos, rayPos + new Vector2(0, rayVector1.y + skin)); // Make new ray.

			if (rayUp) { // If hit.
				rayVector1 = rayUp.point - (rayPos); // Create local vector based on ray path.
				if (vel.y >= -0.1f) { // Is moving upwards.
					if (rayUp.collider.tag == "Thin") {
						Debug.DrawRay (rayPos, new Vector2 (0, rayVector1.y + skin), Color.magenta); // Debug line.
					} else {
						// Begin drill code.
						if (isDrilling) {
							if (doubleUp) {
								if (rayUp.collider.gameObject.layer == LayerMask.NameToLayer ("Solid")) {
									if (rayUp.collider.gameObject.layer != LayerMask.NameToLayer ("Unbreakable")) {
										GameObject c = rayUp.collider.gameObject;
										GameObject.Destroy (c);
									}
								}
							}
						}
						// End drill code.
						vel.y = rayVector1.y - skin; // Make sure not to overshoot.
						Debug.DrawRay (rayPos, new Vector2 (0, rayVector1.y + skin), Color.cyan); // Debug line.
						if (rayVector1.y < skin) { // If ray length hits, and is less than skin.
							cTop = true; // Officially collided.

							if (rayVector2.y < skin - 0.02f) {
								vel.y = -0.01f; // Hit.
							} else {
								vel.y = 0;
							}

							Debug.DrawRay (rayPos, new Vector2 (0, rayVector1.y + skin), Color.green); // Debug line.
						} else {
							Debug.DrawRay (rayPos, new Vector2 (0, rayVector1.y + skin), Color.yellow); // Debug line.
						}
					}
				}
			} else { // If no hit.
				Debug.DrawRay (rayPos, new Vector2(0, rayVector1.y + skin), Color.red); // Debug line.
			}

			// Downward-facing collisions.
			rayPos = pos + new Vector2 (-xOffset + (i * xSpacing) + (xSpacing/2.0f), -yOffset); // Set position of ray start points.
			RaycastHit2D rayDown = Physics2D.Linecast (rayPos, rayPos + new Vector2(0, rayVector2.y - skin)); // Make new ray.

			if (rayDown) { // If hit.
				sumJumpMultiplier += rayDown.collider.bounciness; // Get bounciness of tile.
				rayVector2 = rayDown.point - (rayPos); // Create local vector based on ray path.
				if (vel.y <= 0.1f) { // Is moving downwards.
					if (rayDown.collider.tag == "Thin") {
						WoodPlank p = rayDown.collider.gameObject.GetComponent<WoodPlank> ();
						p.durability -= 0.003f;
						if (dropTimer <= 0) { // If timer isn't in effect.
							vel.y = rayVector2.y + skin; // Make sure not to overshoot.
							Debug.DrawRay (rayPos, new Vector2 (0, rayVector2.y - skin), Color.cyan); // Debug line.
							if (rayVector2.y > -skin) { // If ray length hits, and is less than skin.

								cBottom = true; // Officially collided.

								if (rayVector2.y > -skin + 0.02f) {
									vel.y = 0.01f; // Hit.
								} else {
									vel.y = 0;
								}
								Debug.DrawRay (rayPos, new Vector2 (0, rayVector2.y - skin), Color.green); // Debug line.
								if (rayVector2.y == 0) {
									pos.y += 0.01f;
								}
							}
						} else { // If fall-through timer is still activated.
							Debug.DrawRay (rayPos, new Vector2 (0, rayVector2.y - skin), Color.yellow); // Debug line.
						}

					} else {
						// Begin drill code.
						if (isDrilling) {
							if (doubleDown) {
								if (rayDown.collider.gameObject.layer == LayerMask.NameToLayer ("Solid")) {
									if (rayDown.collider.gameObject.layer != LayerMask.NameToLayer ("Unbreakable")) {
										GameObject c = rayDown.collider.gameObject;
										GameObject.Destroy (c);
									}
								}
							}
						}
						// End drill code.
						vel.y = rayVector2.y + skin; // Make sure not to overshoot.
						Debug.DrawRay (rayPos, new Vector2 (0, rayVector2.y - skin), Color.cyan); // Debug line.
						if (rayVector2.y > -skin) { // If ray length hits, and is less than skin.

							cBottom = true; // Officially collided.

							if (rayVector2.y > -skin + 0.02f) {
								vel.y = 0.01f; // Hit.
							} else {
								vel.y = 0;
							}
							Debug.DrawRay (rayPos, new Vector2 (0, rayVector2.y - skin), Color.green); // Debug line.
							if (rayVector2.y == 0) {
								pos.y += 0.01f;
							}
						}
					}
				}
			} else { // If no hit.
				Debug.DrawRay (rayPos, new Vector2(0, rayVector2.y - skin), Color.red); // Debug line.
			}



			jumpMultiplier = sumJumpMultiplier / yRays * 10.0f;
		}
	}
}