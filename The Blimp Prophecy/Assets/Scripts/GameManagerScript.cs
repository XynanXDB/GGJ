using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

	private static bool _bIsStartGame = false;
	public static bool bIsStartGame { get { return _bIsStartGame;} set { _bIsStartGame = value;} }

	public static bool bIsDefend = false;

	public GameObject player1Blimp;
	public GameObject player2Blimp;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
