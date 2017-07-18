using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour {

	private float timeDistance = 3.5f; //tiempo que se a demorar recorriendo la distancia x

	private float Distance = 19f; // distancia x, es la distancia que hay desde el personaje hasta el spawn de enemigos

	[SerializeField]
	private Text textWord;

	// Use this for initialization
	void Start () {

	}

	public void SetText(string myText)
	{

		textWord.text = myText;

	}
	
	// Update is called once per frame
	void Update () {

		float currentMove = (Time.deltaTime * Distance) / timeDistance;

		transform.Translate (Vector3.right*-currentMove);
	}

}
