using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour {

	private IUserAction action;
	public int life = 6;                   //血量
	//每个GUI的style
	GUIStyle bold_style = new GUIStyle();
	GUIStyle score_style = new GUIStyle();
	GUIStyle text_style = new GUIStyle();
	GUIStyle over_style = new GUIStyle();
	private int high_score = 0;            //最高分
	private bool game_start = false;       //游戏开始

	void Start () {
		action = SSDirector.GetInstance().CurrentScenceController as IUserAction;
	}

	void OnGUI () {
		bold_style.normal.textColor = new Color(1, 0, 0);
		bold_style.fontSize = 16;
		text_style.normal.textColor = new Color(0, 0, 0, 1);
		text_style.fontSize = 16;
		score_style.normal.textColor = new Color(1, 0, 1, 1);
		score_style.fontSize = 16;
		over_style.normal.textColor = new Color(1, 0, 0);
		over_style.fontSize = 25;

		if (game_start) {
			
			//用户射击
			if (Input.GetButtonDown("Fire1")) {
				Vector3 pos = Input.mousePosition;
				action.Hit(pos);
			}

			GUI.Label(new Rect(10, 5, 200, 50), "Score: ", text_style);
			GUI.Label(new Rect(70, 5, 200, 50), action.GetScore().ToString(), score_style);

			GUI.Label(new Rect(Screen.width - 150, 5, 20, 20), "Life:", text_style);
			
			//显示当前血量
			for (int i = 0; i < life; i++)
				GUI.Label(new Rect(Screen.width - 100 + 15 * i, 5, 50, 50), "❤", bold_style);
			
			//游戏结束
			if (life == 0) {
				high_score = high_score > action.GetScore() ? high_score : action.GetScore();
				GUI.Label(new Rect(Screen.width / 2 - 40, Screen.width / 2 - 250, 100, 100), "Game Over!", over_style);
				GUI.Label(new Rect(Screen.width / 2 - 35, Screen.width / 2 - 200, 50, 50), "Highest Score:", text_style);
				GUI.Label(new Rect(Screen.width / 2 + 75, Screen.width / 2 - 200, 50, 50), high_score.ToString(), text_style);
				if (GUI.Button(new Rect(Screen.width / 2 - 20, Screen.width / 2 - 150, 100, 50), "Restart")) {
					life = 6;
					action.ReStart();
					return;
				}
				action.GameOver();
			}
		} else {
			GUI.Label(new Rect(Screen.width / 2 - 30, Screen.width / 2 - 350, 100, 100), "Welcome!", over_style);
			GUI.Label(new Rect(Screen.width / 2 - 75, Screen.width / 2 - 220, 150, 100), "点击飞碟消灭它们，你有6条命！", text_style);
			if (GUI.Button(new Rect(Screen.width / 2 - 20, Screen.width / 2-150, 100, 50), "Start")) {
				game_start = true;
				action.BeginGame();
			}
		}
	}

	public void ReduceBlood() {
		if(life > 0)
			life--;
	}
}
