using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysisUFOFlyAction : SSAction {

	private Vector3 start_vector;                              //初速度向量
	public float power;
	private PhysisUFOFlyAction() {

	}
	
	public static PhysisUFOFlyAction GetSSAction(Vector3 direction, float angle, float power) {
		//初始化物体将要运动的初速度向量
		PhysisUFOFlyAction action = CreateInstance<PhysisUFOFlyAction>();
		if (direction.x == -1) {
			action.start_vector = Quaternion.Euler(new Vector3(0, 0, -angle)) * Vector3.left * power;
		} else {
			action.start_vector = Quaternion.Euler(new Vector3(0, 0, angle)) * Vector3.right * power;
		}
		action.power = power;
		return action;
	}

	public override void FixedUpdate() {
		//判断是否超出范围
		if (this.transform.position.y < -10) {
			this.destroy = true;
			this.callback.SSActionEvent(this);
		}
	}

	// Use this for initialization
	public override void Start () {
		
	}
	
	// Update is called once per frame
	public override void Update () {
		//使用重力以及给一个初速度
		gameobject.GetComponent<Rigidbody>().velocity = power / 35 * start_vector;
		gameobject.GetComponent<Rigidbody>().useGravity = true;
	}
}