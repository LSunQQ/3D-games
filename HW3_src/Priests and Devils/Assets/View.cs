using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using mygame;

public class View : MonoBehaviour {
	
	SSDirector game;
	UserAction click;

	// Use this for initialization
	void Start () {
		game = SSDirector.GetInstance();
		click = SSDirector.GetInstance() as UserAction;
	}
	
	private void OnGUI () {
		GUI.skin.label.fontSize = 30;

		GUI.Label(new Rect(10,10,700,50),"White are Devils and Black are Priests");
		if (game.state == State.Win) {
			if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 100), "YOU WIN\n(click here to reset)")) {
				game.state = State.Left;
				SceneManager.LoadScene(0);
			}
		}
		if (game.state == State.Lose) {
			if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 100), "YOU LOSE\n(click here to reset)")) {
				game.state = State.Left;
				SceneManager.LoadScene(0);
			}
		}
		if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 5, 150, 50), "Move the ship")) {
			click.shipMove();
		}
		if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 5 * 4, 100, 50), "Left gets off")) {
			click.getOffShipLeft();
		}
		if (GUI.Button(new Rect(Screen.width / 2 + 50, Screen.height / 5 * 4, 100, 50), "Right gets off")) {
			click.getOffShipRight();
		}
		if (GUI.Button(new Rect(Screen.width / 7 - 37, Screen.height / 2 + 75, 100, 50), "Priest gets on")) {
			click.priestLGetOnShip();
		}
		if (GUI.Button(new Rect(Screen.width / 7 * 2 - 62, Screen.height / 2 + 75, 100, 50), "Devil gets on")) {
			click.devilLGetOnShip();
		}
		if (GUI.Button(new Rect(Screen.width / 7 * 5 - 37, Screen.height / 2 + 75, 100, 50), "Priest gets on")) {
			click.priestRGetOnShip();
		}
		if (GUI.Button(new Rect(Screen.width / 7 * 6 - 62, Screen.height / 2 + 75, 100, 50), "Devil gets on")) {
			click.devilRGetOnShip();
		}
	}
}