using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;

namespace mygame {
	public enum State {Left, LtoR, RtoL, Right, Win, Lose};

	public interface UserAction {
		void priestLGetOnShip();
		void priestRGetOnShip();
		void devilLGetOnShip();
		void devilRGetOnShip();
		void shipMove();
		void getOffShipLeft();
		void getOffShipRight();
	}

	public class SSDirector : System.Object, UserAction {
		private static SSDirector game;

		public Controller currentScenceController;
		public State state = State.Left;
		private Model game_obj;

		public static SSDirector GetInstance() {
			if (game == null) {
				game = new SSDirector();
			}
			return game;
		}

		public Model getModel() {
			return game_obj;
		}

		internal void setModel(Model temp) {
			if (game_obj == null) {
				game_obj = temp;
			}
		}

		public void priestLGetOnShip() {
			game_obj.leftPriestGetOnShip();
		}

		public void priestRGetOnShip() {
			game_obj.rightPriestGetOnShip();
		}

		public void devilLGetOnShip() {
			game_obj.leftDevilGetOnShip();
		}

		public void devilRGetOnShip() {
			game_obj.rightDevilGetOnShip();
		}

		public void shipMove() {
			game_obj.shipMove();
		}

		public void getOffShipLeft() {
			game_obj.getOffShip(0);
		}

		public void getOffShipRight() {
			game_obj.getOffShip(1);
		}
	}
}
 
public class Controller : MonoBehaviour {
	
	// Use this for initialization
	void Start() {
		SSDirector.GetInstance();
	}
}