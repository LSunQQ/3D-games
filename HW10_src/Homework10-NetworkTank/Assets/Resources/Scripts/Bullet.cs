using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : MonoBehaviour {
    public float explosionRadius = 3.0f;
    private TankType tankType;

    //设置发射子弹的坦克类型, 因为如果射到队友是不能算伤害的.
    public void setTankType(TankType type) {
        tankType = type;
    }
    
    private void OnCollisionEnter(Collision collision) {
        MyFactory factory = Singleton<MyFactory>.Instance;
        ParticleSystem explosion = factory.getParticleSystem();
        explosion.transform.position = gameObject.transform.position;
        explosion.Play();
    }
    
}
