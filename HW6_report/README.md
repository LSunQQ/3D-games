# Homework 6

### 改进飞碟（Hit UFO）游戏：

- **游戏内容要求：**
  1. 按 *adapter模式* 设计图修改飞碟游戏
  2. 使它同时支持物理运动与运动学（变换）运动

<br/>

#### 游戏设计过程：

游戏的规则还有飞碟的设计是和普通版的Hit UFO是一样的

主要就是根据要求使用适配器模式，新增了一个类`ActionManageAdapter`

```c#
public class ActionManagerAdapter : MonoBehaviour, IActionManager {

	public FlyActionManager action_manager;
	public PhysisFlyActionManager phy_action_manager;
	public void playDisk(GameObject disk, float angle, float power,bool isPhy) {
		if(isPhy) {
			phy_action_manager.UFOFly(disk, angle, power);
		} else {
			action_manager.UFOFly(disk, angle, power);
		}
	}

	// Use this for initialization
	void Start () {
		action_manager = gameObject.AddComponent<FlyActionManager>() as FlyActionManager;
		phy_action_manager = gameObject.AddComponent<PhysisFlyActionManager>() as PhysisFlyActionManager;
	}
}
```

同时新增了一个接口

```c#
public interface IActionManager {
	void playDisk(GameObject disk, float angle, float power,bool isPhy);
}
```

这是场景控制器与适配器的接口，由于有可以有很多种飞行的方式，所以需要一个接口来使得不同的飞行模式能够同时使用，并且能够让 场景控制器通过这个接口告诉适配器应该选择哪种实现飞碟飞行动作的方式  

<br/>

相应的在`FirstController`中将`action_manager`的类型从`FlyActionManager`改成了`IActionManager`

```c#
public IActionManager action_manager;
```

并且新增了一个变量`isPhy`来判断是否使用物理运动适配器

```c#
public bool isPhy = false;
```

<br/>

接下来，又设计了一个类`PhysisFlyActionManager`来帮助实现整个物理的飞行过程

```c#
public class PhysisFlyActionManager : SSActionManager {

	public PhysisUFOFlyAction fly;

	// Use this for initialization
	protected void Start () {
		
	}
	
	// 飞碟飞行
	public void UFOFly(GameObject disk, float angle, float power) {
		fly = PhysisUFOFlyAction.GetSSAction(disk.GetComponent<DiskData>().direction, angle, power);
		this.RunAction(disk, fly, this);
	}
}
```

<br/>

最后，就是整个UFO飞行方式的设计了

```c#
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
```



最后展示一下整个游戏界面：

开始界面：
<img src="images\image1.png" style="zoom:100%;" />



游戏界面：
<img src="images\image2.png" style="zoom:100%;" />



结束界面：
<img src="images\image3.png" style="zoom:100%;" />



附上
[项目代码](https://github.com/LSunQQ/3D-games/tree/homework6)
[演示视频](https://v.youku.com/v_show/id_XNDM5Nzk1NDM3Mg==.html?spm=a2h3j.8428770.3416059.1)