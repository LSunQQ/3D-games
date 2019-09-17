using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;

public class Model : MonoBehaviour {

	// use the stack to store the priests and the devils
	Stack<GameObject> leftPriests = new Stack<GameObject>();
	Stack<GameObject> rightPriests = new Stack<GameObject>();
	Stack<GameObject> leftDevils = new Stack<GameObject>();
	Stack<GameObject> rightDevils = new Stack<GameObject>();

	// the ship can hold to people most
	GameObject[] ship = new GameObject[2];
	GameObject ship_obj;

	SSDirector game;

	// these are the positions before moving or after moving
	Vector3 shipLeftPos = new Vector3(-2f, -0.5f, 0);
	Vector3 shipRightPos = new Vector3(2f, -0.5f, 0);
	Vector3 bankLeftPos = new Vector3(-7f, -5f, 0);
	Vector3 bankRightPos = new Vector3(7f, -5f, 0);

	float gap = 0.8f;
	Vector3 priestsLeftPos = new Vector3(-7.7f, 0.25f, 0);
	Vector3 priestsRightPos = new Vector3(6.8f, 0.25f, 0);
	Vector3 devilsLeftPos = new Vector3(-4.7f, 0.25f, 0);
	Vector3 devilsRightPos = new Vector3(9.1f, 0.25f, 0);

	// Use this for initialization
	void Start () {
		game = SSDirector.GetInstance();
		game.setModel(this);
		loadSource();
	}
	
	// Update is called once per frame
	void Update () {
		// update the people's position
		setPosition(leftPriests, priestsLeftPos);
		setPosition(rightPriests, priestsRightPos);
		setPosition(leftDevils, devilsLeftPos);
		setPosition(rightDevils, devilsRightPos);

		if (game.state == State.LtoR) { // the ship move from left to right
			ship_obj.transform.position = Vector3.MoveTowards(ship_obj.transform.position, shipRightPos, Time.deltaTime * 20);
			if (ship_obj.transform.position == shipRightPos) {
				game.state = State.Right;
			}
		}
		else if (game.state == State.RtoL) { // the ship move from right to left
			ship_obj.transform.position = Vector3.MoveTowards(ship_obj.transform.position, shipLeftPos, Time.deltaTime * 20);
			if (ship_obj.transform.position == shipLeftPos) {
				game.state = State.Left;
			}
		}
		else check();
	}

	void loadSource() {
		// clear the stacks
		while (leftPriests.Count > 0) {
			leftPriests.Pop();
		}
		while (rightPriests.Count > 0) {
			rightPriests.Pop();
		}
		while (leftDevils.Count > 0) {
			leftDevils.Pop();
		}
		while (rightDevils.Count > 0) {
			rightDevils.Pop();
		}

		// load the bank
		Instantiate(Resources.Load("Prefabs/bank"), bankLeftPos, Quaternion.identity);
		Instantiate(Resources.Load("Prefabs/bank"), bankRightPos, Quaternion.identity);

		// load the ship
		ship_obj = Instantiate(Resources.Load("Prefabs/ship"), shipLeftPos, Quaternion.identity) as GameObject;

		//load the priests and the devils
		for (int i = 0; i < 3; ++i) {
			leftPriests.Push(Instantiate(Resources.Load("Prefabs/Priest")) as GameObject);
			leftDevils.Push(Instantiate(Resources.Load("Prefabs/Devil")) as GameObject);
		}
	}

	void setPosition(Stack<GameObject> s, Vector3 p) {
		GameObject[] temp = s.ToArray();
		for (int i = 0; i < s.Count; ++i) {
			temp[i].transform.position = p + new Vector3(-gap * i, 0, 0);
		}
	}

	void getOnShip(GameObject temp) {
		// let the object connected with the ship
		temp.transform.parent = ship_obj.transform;
		int judge = isShipFull();
		if (judge != 2) {
			if (ship[0] == null) {
				ship[0] = temp;
				temp.transform.localPosition = new Vector3(-0.25f, 0.75f, 0);
			}
			else {
				ship[1] = temp;
				temp.transform.localPosition = new Vector3(0.25f, 0.75f, 0);
			}
		}
	}

	// judge whether the ship is full or not
	int isShipFull() {
		int count = 0;
		for (int i = 0; i < 2; ++i) {
			if (ship[i] != null) ++count;
		}
		return count;
	}

	public void shipMove() {
		int judge = isShipFull();
		if (judge != 0) {
			if (game.state == State.Left) {
				game.state = State.LtoR;
			}
			else if (game.state == State.Right) {
				game.state = State.RtoL; 
			}
		}
	}

	public void getOffShip(int place) {
		if (ship[place] != null) {
			ship[place].transform.parent = null;
			if (game.state == State.Left) {
				if (ship[place].tag == "Priest") {
					leftPriests.Push(ship[place]);
				}
				else {
					leftDevils.Push(ship[place]);
				}
			}
			else if (game.state == State.Right) {
				if (ship[place].tag == "Priest") {
					rightPriests.Push(ship[place]);
				}
				else {
					rightDevils.Push(ship[place]);
				}
			}
			ship[place] = null;
		}
	}

	// check whether the game is over or not
	void check() {
		if (rightDevils.Count == 3 && rightPriests.Count == 3) {
			game.state = State.Win;
			return;
		}
		int priestsOnShip = 0, devilsOnShip = 0;
		for (int i = 0; i < 2; ++i) {
			if (ship[i] != null) {
				if (ship[i].tag == "Priest")
					++priestsOnShip;
				else ++devilsOnShip;
				// Debug.Log(ship[i].tag);
			}
		}
		// Debug.Log(priestsOnShip);
		// Debug.Log(devilsOnShip);
		int priestsOnLeftBank = leftPriests.Count, priestsOnRightBank = rightPriests.Count;
		int devilsOnLeftBank = leftDevils.Count, devilsOnRightBank = rightDevils.Count;
		if (game.state == State.Left) {
			priestsOnLeftBank += priestsOnShip;
			devilsOnLeftBank += devilsOnShip;
		}
		else if (game.state == State.Right) {
			priestsOnRightBank += priestsOnShip;
			devilsOnRightBank += devilsOnShip;
		}
		// Debug.Log(priestsOnLeftBank);
		// Debug.Log(devilsOnLeftBank);
		// Debug.Log(priestsOnRightBank);
		// Debug.Log(devilsOnRightBank);

		// if the number of the priests more the zero and less than the number of the devils in the same side, lose
		if ((priestsOnLeftBank != 0 && priestsOnLeftBank < devilsOnLeftBank) || 
			(priestsOnRightBank != 0 && priestsOnRightBank < devilsOnRightBank)) {
			game.state = State.Lose;
		}
	}

	// objects get on ship
	public void leftPriestGetOnShip() {
		if (leftPriests.Count != 0 && isShipFull() != 2 && game.state == State.Left) {
			getOnShip(leftPriests.Pop());
			// Debug.Log(leftPriests.Count);
		}
	}

	public void rightPriestGetOnShip() {
		if (rightPriests.Count != 0 && isShipFull() != 2 && game.state == State.Right) {
			getOnShip(rightPriests.Pop());
			// Debug.Log(rightPriests.Count);
		}
	}

	public void leftDevilGetOnShip() {
		if (leftDevils.Count != 0 && isShipFull() != 2 && game.state == State.Left) {
			getOnShip(leftDevils.Pop());
			// Debug.Log(leftDevils.Count);
		}
	}

	public void rightDevilGetOnShip() {
		if (rightDevils.Count != 0 && isShipFull() != 2 && game.state == State.Right) {
			getOnShip(rightDevils.Pop());
			// Debug.Log(rightDevils.Count);
		}
	}
}