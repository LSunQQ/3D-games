using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jzq : MonoBehaviour {
	private int turn = 1; // 记录当前是谁的回合
	private int[,] chess = new int[3, 3]; // 棋盘

	// Use this for initialization
	void Start() {
		reset();
	}

	void reset() { // 重置棋盘
		turn = 1;
		for (int i = 0; i < 3; ++i) {
			for (int j = 0; j < 3; ++j) {
				chess[i, j] = 0;
			}
		}
	}

	int check() { // 判断游戏胜负
		// 横着连，如果满足的话某一行的第一个一定是有棋子的
		for (int i = 0; i < 3; ++i) {
			if (chess[i, 0] != 0 && chess[i, 0] == chess[i, 1] && chess[i, 1] == chess[i, 2]) {
				return chess[i, 0];
			}
		}

		// 竖着连，如果满足的话某一列的第一个一定是有棋子的
		for (int j = 0; j < 3; ++j) {
			if (chess[0, j] != 0 && chess[0, j] == chess[1, j] && chess[1, j] == chess[2, j]) {
				return chess[0, j];
			}
		}

		// 斜着连, 如果满足的话中心一定是有棋子的
		if (chess[1, 1] != 0 &&
			chess[0, 0] == chess[1, 1] && chess[1, 1] == chess[2, 2] ||
			chess[0, 2] == chess[1, 1] && chess[1, 1] == chess[2, 0]) {
			return chess[1, 1];
		}
		return 0;
	}

	void OnGUI() {
		if (GUI.Button(new Rect((Screen.width / 2) - 50, (Screen.height / 4 * 3), 100, 50), "Reset"))
			reset();
		int result = check();
		if (result == 1) {
			GUI.Label(new Rect((Screen.width / 2) - 25, (Screen.height / 2) + 60, 100, 100), "O wins!");
		}
		else if (result == 2) {
			GUI.Label(new Rect((Screen.width / 2) - 25, (Screen.height / 2) + 60, 100, 100), "X wins!");
		}
		for (int i = 0; i < 3; ++i) {
			for (int j = 0; j < 3; ++j) {
				if (chess[i, j] == 1)
					GUI.Button(new Rect(Screen.width / 2 - 25 + (i - 1) * 50, 30 + j * 50, 50, 50), "O");
				if (chess[i, j] == 2)
					GUI.Button(new Rect(Screen.width / 2 - 25 + (i - 1) * 50, 30 + j * 50, 50, 50), "X");
				if (GUI.Button(new Rect(Screen.width / 2 - 25 + (i - 1) * 50, 30 + j * 50, 50, 50), "")) {
					// 这个循环的前提是点击到了没被点过的格子
					if (result == 0) {
						if (turn == 1) chess[i, j] = 1;
						else chess[i, j] = 2;
						turn = -turn; // 切换当前玩家
					}
				}
			}
		}
	}
}