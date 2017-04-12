using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	private Rigidbody2D rb;

	private bool isGrounded;

	[SerializeField]
	private float jumpForce;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		isGrounded = true;
	}

	void OnCollisionEnter2D(Collision2D other)
	{

		if (other.gameObject.tag == "ground")
			isGrounded = true;

	}

	// Update is called once per frame
	void FixedUpdate () {

		/*if (Input.GetKeyDown (KeyCode.Space) && isGrounded) 
		{

		}*/
		
	}

	public	void Jump(){
		if(isGrounded)
		{
		isGrounded = false;
		rb.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
		}
	}
}
