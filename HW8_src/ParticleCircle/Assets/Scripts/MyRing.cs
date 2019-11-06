using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRing : MonoBehaviour {
	public ParticleSystem particleSystem; //粒子系统对象
	public int particleNumber = 5000;     //发射的最大粒子数
	// public float pingPong = 0.05f;        //让粒子能够在一定范围内抖动
	public float size = 0.05f;            //粒子的大小
	public float maxRadius = 12.0f;       //粒子的旋转半径
	public float minRadius = 4.0f;
	public float speed = 0.05f;           //粒子的运动速度
	private float[] particleAngle;        //粒子的偏转角
	private float[] particleRadius;       //粒子的运动半径

	// private float time = 0;   
	private ParticleSystem.Particle[] particlesArray;

	private Color setColor = new Color(255, 255, 255);

	void Start() {
		particleSystem = GetComponent<ParticleSystem>();

		particlesArray = new ParticleSystem.Particle[particleNumber];
		particleSystem.maxParticles = particleNumber;
		particleAngle = new float[particleNumber];
		particleRadius = new float[particleNumber];

		particleSystem.Emit(particleNumber);
		particleSystem.GetParticles(particlesArray);

		init();

		particleSystem.SetParticles(particlesArray, particlesArray.Length);  
	}

	void Update() {
		for (int i = 0; i < particleNumber; i++) {
			// time += Time.deltaTime;
			particlesArray[i].color = setColor;
			// particleRadius[i] += (Mathf.PingPong(time / minRadius / maxRadius, pingPong) - pingPong / 2.0f);
			if (i % 2 == 0) {
				particleAngle[i] += speed * (i % 10 + 1);
			} else {
				particleAngle[i] -= speed * (i % 10 + 1);
			}
			particleAngle[i] = (particleAngle[i] + 360) % 360;
			float rad = particleAngle[i] / 180 * Mathf.PI;

			particlesArray[i].position = new Vector3(particleRadius[i] * Mathf.Cos(rad), particleRadius[i] * Mathf.Sin(rad), 0f);
		}
		particleSystem.SetParticles(particlesArray, particleNumber);
	}

	void init() {
		//对于每个粒子
		for (int i = 0; i < particleNumber; i++) {
			//随机生成角度
			float angle = Random.Range(0.0f, 360.0f);
			//换回弧度制
			float rad = angle / 180 * Mathf.PI;
			//设定粒子的旋转半径
			float midRadius = (maxRadius + minRadius) / 2;
			float rate1 = Random.Range(1.0f, midRadius / minRadius);
			float rate2 = Random.Range(midRadius / maxRadius, 1.0f);
			float r = Random.Range(minRadius * rate1, maxRadius * rate2);
			//设定粒子的大小
			particlesArray[i].size = size;

			particleAngle[i] = angle;
			particleRadius[i] = r;
			//放置粒子
			particlesArray[i].position = new Vector3(r * Mathf.Cos(rad), r * Mathf.Sin(rad), 0.0f);
		}
	}
}
