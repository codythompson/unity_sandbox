using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

	public bool movingLeft = false;
	public bool movingRight = false;
	public bool isJumping = false;
	public bool firstJumpUsed = false;
	public bool facingLeft = false;
	public bool facingRight = false;

	public float horizontalMoveSpeed = 1.0f;
	public float maxVerticalSpeed = 2.0f;
	public float jumpSpeed = 1;
	public float currentFirstJumpTime = 0;
	public float jumpTimerIncrement = 1;
	public float firstJumpTimerMax = 10;

	public GameObject animationObject = null;
	public Animator animator = null;

	public Rigidbody2D rigidbody2DReference = null;

	// Use this for initialization
	void Start () {

		animator = animationObject.GetComponent<Animator>();

		rigidbody2DReference = this.gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		PerformPlayerKeyListening();
		UpdatePlayerFacing();
		UpdateAnimatorState();

		Vector3 tempForce = PerformPlayerMovementCalculation();
		if(tempForce.y > maxVerticalSpeed) {
			tempForce.y = maxVerticalSpeed;
		}

		rigidbody2DReference.AddForce(tempForce);
		if(!movingLeft && !movingRight) {
			rigidbody2DReference.velocity = new Vector2(0, rigidbody2DReference.velocity.y);
		}
	}

	public void PerformPlayerKeyListening() {
		if(Input.GetAxis("Horizontal") < 0) {
			movingLeft = true;
			movingRight = false;
		}
		else if(Input.GetAxis("Horizontal") > 0) {
			movingRight = true;
			movingLeft = false;
		}
		else {
			movingLeft = false;
			movingRight = false;
		}

		if(Input.GetKeyDown(KeyCode.Space)) {
			if(!firstJumpUsed) {
				isJumping = true;
			}
		}
		if(Input.GetKeyUp(KeyCode.Space)) {
			isJumping = false;
			if(!firstJumpUsed) {
				firstJumpUsed = true;
			}
		}

		if(isJumping
		   && !firstJumpUsed) {
			//Debug.Log("jumping");
			currentFirstJumpTime += jumpTimerIncrement;
			if(currentFirstJumpTime > firstJumpTimerMax) {
				firstJumpUsed = true;
			}
		}

		if(firstJumpUsed) {
			isJumping = false;
		}
	}

	public Vector2 PerformPlayerMovementCalculation() {
		Vector2 newPositionOffset = Vector2.zero;

		if(movingLeft) {
			newPositionOffset.x -= horizontalMoveSpeed;
		}
		else if(movingRight) {
			newPositionOffset.x += horizontalMoveSpeed;
		}

		if(isJumping) {
			newPositionOffset.y += jumpSpeed;
		}

		return newPositionOffset;
	}

	public void UpdatePlayerFacing() {
		if(movingLeft) {
			facingLeft = true;
			facingRight = false;
		}
		else if(movingRight) {
			facingLeft = false;
			facingRight = true;
		}

		Vector3 flipOrientation = new Vector3(1, 1, 1);
		if(facingRight) {
			flipOrientation.x = 1;
		}
		else if(facingLeft) {
			flipOrientation.x = -1;
		}

		transform.localScale = new Vector3(flipOrientation.x,
		                                   flipOrientation.y,
		                                   flipOrientation.z);
	}

	public void UpdateAnimatorState() {
		animator.SetBool("MovingLeft", movingLeft);
		animator.SetBool("MovingRight", movingRight);
		animator.SetBool("IsJumping", isJumping);
		//animator.SetBool("FirstJumpUsed", firstJumpUsed);
	}

	public void OnCollisionEnter2D(Collision2D collision2D) {
		if(collision2D.gameObject.CompareTag("Ground")) {
			//Debug.Log("ground collision");
			if(firstJumpUsed) {
				ResetJump();
				animator.SetBool("PlayerLanded", true);
				//animator.Play("Standing");
			}
			else {
				animator.SetBool("PlayerLanded", false);
			}
		}
		else {
			animator.SetBool("PlayerLanded", false);
		}
	}

	public void OnCollisionStay2D(Collision2D collision2D) {
	}

	public void ResetJump() {
		currentFirstJumpTime = 0;
		firstJumpUsed = false;
		isJumping = false;
	}
}
