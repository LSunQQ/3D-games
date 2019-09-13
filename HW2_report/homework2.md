## 1、简答题

##### 解释 游戏对象（GameObjects） 和 资源（Assets）的区别与联系。

###### 游戏对象

游戏对象是场景中所有实体的基类，它是一个具体的实例。但是这个游戏对象不会做任何事情，赋予它一个属性之后它才能够成为一个游戏角色、游戏特效或者其他的东西

###### 资源

Assets中有很多的对象、C#文件、材质、场景等文件，还有一些的资源可以从Unity外导入，比如一些音频、图片等。

<img src="images\image1.png" alt="image1" style="zoom:60%;" />

###### 联系

一个对象是很多资源组合的一个的具体表现；而一个资源可以可以作为组件被一个或多个对象使用



##### 编写一个代码，使用 debug 语句来验证MonoBehaviour基本行为或事件触发的条件

- 基本行为包括 Awake() Start() Update() FixedUpdate() LateUpdate()
- 常用事件包括 OnGUI() OnDisable() OnEnable()

测试代码如下：

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

	void Awake () {
		Debug.Log("Test Awake!");
	}

	// Use this for initialization
	void Start () {
		Debug.Log("Test Start!");
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("Test Update!");
	}

	void FixedUpdate () {
		Debug.Log("Test FixedUpdate!");
	}

	void LateUpdate () {
		Debug.Log("Test LateUpdate!");
	}

	void OnGUI () {
		Debug.Log("Test OnGUI!");
	}

	void OnDisable () {
		Debug.Log("Test OnDisable!");
	}

	void OnEnable () {
		Debug.Log("Test OnEnable!");
	}
}
```

实验结果如下：
<img src="images\image2.png" alt="image2" style="zoom:60%;" />



可以看到，前面的Awake、OnEnable、Start和最后的OnDisable都只执行了一次而中间的4个执行了很多次

- Awake：在加载脚本实例时调用
- Start：在第一次调用任何Update方法之前启用脚本时，将在框架上调用Start。
- Update：如果启用了MonoBehaviour，那么Update在每一帧被调用。
- FixedUpdate：这是为了物理计算而调用的
- LateUpdate：如果某个行为是能用的，那么将在每一帧调用此函数
- OnDisable：当某个行为被禁用时，将调用此函数。
- OnEnable：当对象启用并激活时，将调用此函数。
- OnGUI：它是用来呈现和处理GUI事件的



##### 查找脚本手册，了解GameObject，Transform，Component 对象

- 分别翻译官方对三个对象的描述（Description）

  - GameObject: Base class for all entities in Unity Scenes.（统一场景中所有实体的基类）
  - Transform: Position, rotation and scale of an object. （物体的位置、旋转和比例）
  - Component: Base class for everything attached to GameObjects. （所有附加到GameObjects的东西的基类。）

- 描述下图中 table 对象（实体）的属性、table 的 Transform 的属性、 table 的部件
  - 本题目要求是把可视化图形编程界面与 Unity API 对应起来，当你在 Inspector 面板上每一个内容，应该知道对应 API。
  - 例如：table 的对象是 GameObject，第一个选择框是 activeSelf 属性。
  
  第二个选择框是Transform：可以定义对象的Position、Rotation、Scale
  
  第三个选择框是Box Collider：可以调整Material、Center、Size
  
  第四个选择框是Test(Script)：可以放入C#文件给对象一些行为



- 用 UML 图描述三者的关系（请使用 UMLet 14.1.1 stand-alone版本出图）

<img src="images\image3.png" alt="image3" style="zoom:90%;" />



##### 整理相关学习资料，编写简单代码验证以下技术的实现：

- 查找对象
- 添加子对象
- 遍历对象树
- 清除所有子对象

```c#
// 查找对象：
public static GameObject Find(string name) // 通过名字查找单个对象
public static GameObject FindWithTag(string tag) // 通过标签查找单个对象
public static GameObject[] FindGameObjectsWithTag(string tag) // 通过标签查找多个对象

// 添加子对象：
public static GameObect CreatePrimitive(PrimitiveTypetype)

// 遍历对象树：
foreach (Transform child in transform)

// 清除所有子对象：
foreach (Transform child in transform) {
	Destroy(child.gameObject);
}
```



##### 资源预设（Prefabs）与 对象克隆 (clone)

- 预设（Prefabs）有什么好处？

这可以当成一个模板，降低出错的概率，而且修改的时候改了预设其他的也就会作用于和他相连的其他实例

- 预设与对象克隆 (clone or copy or Instantiate of Unity Object) 关系？

两者都可以看成一个模板，上面说到预设修改之后其他的也会自动修改，但是克隆的不会。

- 制作 table 预制，写一段代码将 table 预制资源实例化成游戏对象

```objective-c
public GameObject table;
void Start () {
	Debug.Log("Init Start");
	GameObject newTable = (GameObject)Instantiate(table.gameObject);
}
```





##  2、编程实践，小游戏

- 游戏内容： 井字棋
- 技术限制： 仅允许使用 IMGUI 构建 UI
- 作业目的：
  - 提升 debug 能力
  - 提升阅读 API 文档能力



##### 代码解释

###### 准备部分

```c#
private int turn = 1; // 记录当前是谁的回合，利用turn = -turn来切换回合
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
```

- 这里使用一个变量turn来表示当前是谁的游戏回合，不断切换实现两个玩家轮流玩
- 井字棋盘是一个3 * 3的矩阵，用一个二维数组来表示，其中0表示没人填，1和2分别表示玩家1和玩家2填的
- 每次开始的时候就调用reset函数，来重置棋盘和当前回合使用者



###### 判断部分

```c#
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
```

- 游戏的赢的情况大致分为3种：横着连3个，竖着连3个，或者斜着连3个。其中每种情况都可以利用一些技巧来减少循环的次数。
- 判断哪个玩家赢就可以利用数组里面存的是1还是2来区分



###### 界面部分

```c#
void OnGUI() {
    if (GUI.Button(new Rect((Screen.width / 2) - 50, (Screen.height / 4 * 3), 100, 50), "Reset"))
        reset();
    int result = check();
    if (result == 1) { // 玩家1获胜
        GUI.Label(new Rect((Screen.width / 2) - 25, (Screen.height / 2) + 60, 100, 100), "O wins!");
    }
    else if (result == 2) { // 玩家2获胜
        GUI.Label(new Rect((Screen.width / 2) - 25, (Screen.height / 2) + 60, 100, 100), "X wins!");
    }
    for (int i = 0; i < 3; ++i) {
        for (int j = 0; j < 3; ++j) {
            if (chess[i, j] == 1)
                GUI.Button(new Rect(Screen.width / 2 - 25 + (i - 1) * 50, 30 + j * 50, 50, 50), "O"); // 如果是玩家1填的就显示"O"
            if (chess[i, j] == 2)
                GUI.Button(new Rect(Screen.width / 2 - 25 + (i - 1) * 50, 30 + j * 50, 50, 50), "X"); // 如果是玩家2填的就显示"X"
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
```

- 利用check()函数来判断游戏是否结束和谁输谁赢
- 根据当前游戏者显示他下的棋子
- 利用turn变量来切换游戏者
- Button是按钮，可以点击；Label是一个标签，可以显示一段文字。它们都可以根据需要调整位置



最后展示一下游戏：
<img src="images\image4.png" alt="image4" style="zoom:60%;" />