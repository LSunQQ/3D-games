using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

public class UserGUI : MonoBehaviour {

	private UserAction action;
	public int status = 0;
	GUIStyle style;
	GUIStyle style2;
	GUIStyle buttonStyle;

	// Use this for initialization
	void Start () {
		action = Director.getInstance ().currentSceneController as UserAction;
		style = new GUIStyle();
		style.fontSize = 40;
		style.alignment = TextAnchor.MiddleCenter;

		style2 = new GUIStyle();
		style2.fontSize = 20;

		buttonStyle = new GUIStyle("button");
		buttonStyle.fontSize = 30;
	}
	
	void OnGUI() {
		GUI.Label(new Rect(30, 10, 50, 25), "游戏规则：", style2);
		GUI.Label(new Rect(30, 40, 75, 25), "球是魔鬼，方块是牧师", style2);
		GUI.Label(new Rect(30, 70, 75, 25), "任意一侧魔鬼多于牧师则游戏结束", style2);
		GUI.Label(new Rect(30, 100, 75, 25), "所有牧师与魔鬼都过到左侧即为胜利", style2);
		if (status == 1) {
			GUI.Label(new Rect(Screen.width/2-50, Screen.height/2-85, 100, 50), "Gameover!", style);
			if (GUI.Button(new Rect(Screen.width/2-70, Screen.height/2, 140, 70), "Restart", buttonStyle)) {
				status = 0;
				action.restart ();
			}
		} else if(status == 2) {
			GUI.Label(new Rect(Screen.width/2-50, Screen.height/2-85, 100, 50), "You win!", style);
			if (GUI.Button(new Rect(Screen.width/2-70, Screen.height/2, 140, 70), "Restart", buttonStyle)) {
				status = 0;
				action.restart ();
			}
		}
	}
}
