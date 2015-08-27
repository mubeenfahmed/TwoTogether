﻿using System.Collections;
using UnityEngine;

public class BigCubeController : MonoBehaviour {

	private Rigidbody2D rigidbody;
	private float speed = 1.25f;
	private float jumpForce = 200f;
	private bool collidedOnRight = false;
	private bool collidedOnLeft = false;
	private bool fadeIn = false;
	private bool fadeOut = false;
	private SpriteRenderer renderer;
	private bool settingUp = false;
	private bool jumping = false;

    // Delegates that allow for easier modification of character movement
    delegate void MoveDelegate();
    MoveDelegate moveLeft;
    MoveDelegate moveRight;

    // Delegate to modify how collisions are detected for sides
    public delegate void CollideDelegate(string side, bool collided);
    public CollideDelegate SetCollided;

    void Awake() {
		rigidbody = GetComponent<Rigidbody2D>();
		settingUp = true;
        moveLeft = MoveLeftNormal;
        moveRight = MoveRightNormal;
        SetCollided = CollidedNormal;
    }

	IEnumerator Start() {
		renderer = GetComponent<SpriteRenderer>();
		renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.002f);
		yield return new WaitForSeconds(2);
		FadeIn();
		settingUp = false;
	}

	void Update() {
		if (fadeIn) {
			if (!(renderer.color.a > 1.0f)) {
				Color color = renderer.color;
				color.a *= 1.0625f;
				renderer.color = color;
			} else {
				fadeIn = false;
			}
		}
		if (fadeOut) {
			if (!(renderer.color.a < 0)) {
				Color color = renderer.color;
				color.a /= 1.0625f;
				renderer.color = color;
			} else {
				fadeOut = false;
			}
		}

		if (!(fadeOut || fadeOut || settingUp)) {
			if (Input.GetKey(KeyCode.A) && !collidedOnLeft) {
                moveLeft();
			}
			if (Input.GetKey(KeyCode.D) && !collidedOnRight) {
                moveRight();
			}
			if (Input.GetKeyDown(KeyCode.W) && !jumping) {
				rigidbody.AddForce(Vector2.up * jumpForce);
				jumping = true;
			}
		}
	}

    /* Start Movement Controllers
       Uses Transform based movement, these methods should be called from the delegates moveLeft/moveRight respectively
   */
    void MoveLeftNormal() {
        transform.Translate(Vector3.left * Time.deltaTime * speed);
    }

    void MoveRightNormal() {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }
    // End Movement Controllers

    // Changes movement and collision detection
    public void SetControlType(ControlType type) {
        if (type == ControlType.Normal) {
            moveLeft = MoveLeftNormal;
            moveRight = MoveRightNormal;
            SetCollided = CollidedNormal;
        } else if (type == ControlType.Inverted) {
            moveLeft = MoveRightNormal;
            moveRight = MoveLeftNormal;
            SetCollided = CollidedInverted;
        }
    }

    // Public fuction for fading in the character
    public void FadeIn() {
		fadeIn = true;
	}

    // Public fuction for fading out the character
    public void FadeOut() {
		fadeOut = true;
	}

    // Returns the current ControlType
    void OnCollisionEnter2D(Collision2D other) {
		if (jumping) {
			jumping = false;
		}
	}

	// Getter and Setter methods
	public void CollidedNormal(string side, bool collided) {
		if (side.Equals("RIGHT")) {
			collidedOnRight = collided;
		} else if (side.Equals("LEFT")) {
			collidedOnLeft = collided;
		}
	}

    void CollidedInverted(string side, bool collided) {
        if (side.Equals("LEFT")) {
            collidedOnRight = collided;
        } else if (side.Equals("RIGHT")) {
            collidedOnLeft = collided;
        }
    }
}
