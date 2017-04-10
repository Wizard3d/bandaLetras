using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

	private float speed;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.right * -speed);
	}

	public float Speed {
		get {
			return speed;
		}
		set {
			speed = value;
		}
	}
}
