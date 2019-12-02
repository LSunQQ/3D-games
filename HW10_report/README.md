# Homework 10

1、 有趣 AR 小游戏制作

2、 坦克对战游戏 AI 设计

从商店下载游戏：“Kawaii” Tank 或 其他坦克模型，构建 AI 对战坦克。具体要求

- 使用“感知-思考-行为”模型，建模 AI 坦克
- 场景中要放置一些障碍阻挡对手视线
- 坦克需要放置一个矩阵包围盒触发器，以保证 AI 坦克能使用射线探测对手方位
- AI 坦克必须在有目标条件下使用导航，并能绕过障碍。（失去目标时策略自己思考）
- 实现人机对战

3、P&D 过河游戏智能帮助实现，程序具体要求：

- 实现状态图的自动生成
- 讲解图数据在程序中的表示方法
- 利用算法实现下一步的计算
- 参考：[P&D 过河游戏智能帮助实现](https://blog.csdn.net/kiloveyousmile/article/details/71727667)

我选择的任务是制作一个可以多人游戏的坦克对战

首先设计`Tank`类，也就是整个游戏的主体部分，设置它的一些基本属性值以及行为

```c#
public class Tank : NetworkBehaviour {
    private float hp =500.0f;
    // 初始化
    public Tank() {
        hp = 500.0f;
    }

    public float getHP() {
        return hp;
    }

    public void setHP(float hp) {
        this.hp = hp;
    }

    public void beShooted() {
        hp -= 100;
    }

    [Command]
    public void CmdFire(TankType type) {
        GameObject bullet = Singleton<MyFactory>.Instance.getBullets(type);
        
        bullet.transform.position = new Vector3(gameObject.transform.position.x, 1.5f, gameObject.transform.position.z) + gameObject.transform.forward * 1.5f;
        bullet.transform.forward = gameObject.transform.forward; //方向
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 20, ForceMode.Impulse);

        NetworkServer.Spawn(bullet);

        Destroy(bullet, 2.0f);
    }
}
```

<br/>

然后增加一个`Player`类，也就是使得玩家能够控制坦克移动，大致就是将玩家的按到的键值传递给`Update()`函数进行处理，对战的时候的数据传递都是利用`NetworkBehaviour`来实现的

```c#
public class Player : Tank {
    public override void OnStartLocalPlayer() {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    void Start () {
        setHP(500);
	}
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;
        Camera.main.transform.position = new Vector3(gameObject.transform.position.x, 18, gameObject.transform.position.z);

        if (Input.GetKey(KeyCode.W))
            moveForward();

        if (Input.GetKey(KeyCode.S))
            moveBackWard();

        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("space");
            CmdFire(TankType.PLAYER);
        }
        //获取水平轴上的增量，目的在于控制玩家坦克的转向
        float offsetX = Input.GetAxis("Horizontal");
        turn(offsetX);
    }

    //向前移动
    public void moveForward() {
        gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 25;
    }
    //向后移动
    public void moveBackWard() {
        gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * -25;
    }

    //通过水平轴上的增量，改变玩家坦克的欧拉角，从而实现坦克转向
    public void turn(float offsetX) {
        float x = gameObject.transform.localEulerAngles.x;
        float y = gameObject.transform.localEulerAngles.y + offsetX*2;
        gameObject.transform.localEulerAngles = new Vector3(x, y, 0);
    }
}
```

<br/>

其中网路数据传递是利用`Tank`类中的`[Command]`来实现的，主要就是多个玩家之间的数据进行同步，也就是使得不同玩家之间的游戏界面显示的内容是一样的。

然后再添加网络工具，创建一个空对象，设置它的属性值。往该物体添加`NetworkManager`和`NetworkManagerHUD`组件（选中该物体，在菜单栏里选`Component-> Network -> NetworkManager`）

<img src="images\image1.png" style="zoom:100%" />

设置成功之后是能够看到这个网络的信息的：

<img src="images\image2.png" style="zoom:100%;" />

同时，游戏开始之后，会看到生成了`Player`的预制，同样的，也会生成它的网络信息：

<img src="images\image3.png" style="zoom:100%;" />



实验效果：

由于我没有两台电脑，所以我在一台电脑上同时打开了两个页面来进行测试，效果如下：

<img src="images\image4.png" style="zoom:80%;" />



最后附上

[项目地址](https://github.com/LSunQQ/3D-games/tree/homework10)

[视频演示](https://v.youku.com/v_show/id_XNDQ1NTUyNDE2OA==.html?spm=a2h3j.8428770.3416059.1)