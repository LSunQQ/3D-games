# Homework 4

### 1、基本操作演练【建议做

- **下载 Fantasy Skybox FREE， 构建自己的游戏场景**

  - 首先在Unity Store中下载Fantasy Skybox，这里面有很多的素材可以用来构造游戏的场景
    <img src="images\image6.png" style="zoom:60%;" />

    

  - 然后创建一个新场景，在`Windows -> Lighting -> Settings`中添加我们的天空背景
    <img src="images\image2.png" style="zoom:60%;" />

    

  - 天空盒里面提供了一些好看的背景，将其中一张拖到`Settings`的框里面就可以将默认的背景替换成这个好看的背景了

    <img src="images\image3.png" style="zoom:60%;" />

    

  - 接下来我们新建一个Terrain，它可以帮助我们构造一个地形，我这里使用了一个它构造好的地形，不过可以在`Inspector`中自行调整其它的属性
    <img src="images\image4.png" style="zoom:60%;" />

    

  - 这里我演示一个种树的功能，点击第五个选项，拖入下面任意一种植物，即可在刚刚的地形上面种上很多的植物
    <img src="images\image5.png" style="zoom:60%;" />

    

  - 最后展示一下我最终的游戏场景

    <img src="images\image1.png" style="zoom:60%;" />

  

  

- **写一个简单的总结，总结游戏对象的使用**
  游戏对象是场景中所有实体的基类，它是一个具体的实例。但是这个游戏对象不会做任何事情，赋予它一个属性之后它才能够成为一个游戏角色、游戏特效或者其他的东西

  也就是说，游戏对象只是一些散装的部件，我们需要将它们组合起来，在`C#`等类似的文件中赋予它们一些动作或者是调整它们的颜色、形状等属性来满足游戏的需要，才能够运用在整个游戏场景之中。不同的游戏对象有着不同的功能，将正确的游戏对象使用在正确的地方也是一个十分关键的因素。





### 2、编程实践

- 牧师与魔鬼 动作分离版
  - 【2019新要求】：设计一个裁判类，当游戏达到结束条件时，通知场景控制器游戏结束

[完整代码链接](https://github.com/LSunQQ/3D-games/tree/homework4)

[视频演示](https://v.youku.com/v_show/id_XNDM3Nzc0ODgwOA==.html?spm=a2h3j.8428770.3416059.1)

这道题目我是根据老师给出的提示来进行的

<img src="images\image9.png" style="zoom:75%;" />

我的代码主要分为5个部分，

首先是控制部分，第一个版本的控制是将每个对象的移动分别实现，而这里是让一个类来实现，不同的对象根据自己的需要在继承过程中进行适当的修改

```c#
readonly Vector3 water_pos = new Vector3(0,0.5f,0);

UserGUI userGUI;

public CoastController fromCoast;
public CoastController toCoast;
public BoatController boat;
private MyCharacterController[] characters;

void Awake() {
    Director director = Director.getInstance ();
    director.currentSceneController = this;
    userGUI = gameObject.AddComponent <UserGUI>() as UserGUI;
    characters = new MyCharacterController[6];
    loadResources ();
}

public void loadResources() {
    GameObject water = Instantiate (Resources.Load ("Perfabs/Water", typeof(GameObject)), water_pos, Quaternion.identity, null) as GameObject;
    water.name = "water";

    fromCoast = new CoastController ("from");
    toCoast = new CoastController ("to");
    boat = new BoatController ();

    loadCharacter ();
}

private void loadCharacter() {
    for (int i = 0; i < 3; i++) {
        MyCharacterController cha = new MyCharacterController ("priest");
        cha.setName("priest" + i);
        cha.setPosition (fromCoast.getEmptyPosition ());
        cha.getOnCoast (fromCoast);
        fromCoast.getOnCoast (cha);

        characters [i] = cha;
    }

    for (int i = 0; i < 3; i++) {
        MyCharacterController cha = new MyCharacterController ("devil");
        cha.setName("devil" + i);
        cha.setPosition (fromCoast.getEmptyPosition ());
        cha.getOnCoast (fromCoast);
        fromCoast.getOnCoast (cha);

        characters [i+3] = cha;
    }
}


public void moveBoat() {
    if (boat.isEmpty ())
        return;
    boat.Move ();
    userGUI.status = check_game_over ();
}

public void characterIsClicked(MyCharacterController characterCtrl) {
    if (characterCtrl.isOnBoat ()) {
        CoastController whichCoast;
        if (boat.get_to_or_from () == -1) { // to->-1; from->1
            whichCoast = toCoast;
        } else {
            whichCoast = fromCoast;
        }

        boat.GetOffBoat (characterCtrl.getName());
        characterCtrl.moveToPosition (whichCoast.getEmptyPosition ());
        characterCtrl.getOnCoast (whichCoast);
        whichCoast.getOnCoast (characterCtrl);

    } else {									// character on coast
        CoastController whichCoast = characterCtrl.getCoastController ();

        if (boat.getEmptyIndex () == -1) {		// boat is full
            return;
        }

        if (whichCoast.get_to_or_from () != boat.get_to_or_from ())	// boat is not on the side of character
            return;

        whichCoast.getOffCoast(characterCtrl.getName());
        characterCtrl.moveToPosition (boat.getEmptyPosition());
        characterCtrl.getOnBoat (boat);
        boat.GetOnBoat (characterCtrl);
    }
    userGUI.status = check_game_over ();
}

int check_game_over() {	// 0->not finish, 1->lose, 2->win
    int from_priest = 0;
    int from_devil = 0;
    int to_priest = 0;
    int to_devil = 0;

    int[] fromCount = fromCoast.getCharacterNum ();
    from_priest += fromCount[0];
    from_devil += fromCount[1];

    int[] toCount = toCoast.getCharacterNum ();
    to_priest += toCount[0];
    to_devil += toCount[1];

    if (to_priest + to_devil == 6)		// win
        return 2;

    int[] boatCount = boat.getCharacterNum ();
    if (boat.get_to_or_from () == -1) {	// boat at toCoast
        to_priest += boatCount[0];
        to_devil += boatCount[1];
    } else {	// boat at fromCoast
        from_priest += boatCount[0];
        from_devil += boatCount[1];
    }
    if (from_priest < from_devil && from_priest > 0) {		// lose
        return 1;
    }
    if (to_priest < to_devil && to_priest > 0) {
        return 1;
    }
    return 0;			// not finish
}

public void restart() {
    boat.reset ();
    fromCoast.reset ();
    toCoast.reset ();
    for (int i = 0; i < characters.Length; i++) {
        characters [i].reset ();
    }
}
```



然后是显示部分，分为用户点击的显示与一些默认显示两个部分

```c#
UserAction action;
MyCharacterController characterController;

public void setController(MyCharacterController characterCtrl) {
    characterController = characterCtrl;
}

// Use this for initialization
void Start () {
    action = Director.getInstance ().currentSceneController as UserAction;
}

// Update is called once per frame
void Update () {

}

void OnMouseDown() {
    if (gameObject.name == "boat") {
        action.moveBoat ();
    } else {
        action.characterIsClicked (characterController);
    }
}
```

```c#
private UserAction action;
public int status = 0;
GUIStyle style;
GUIStyle style2;
GUIStyle buttonStyle;

// Use this for initialization
void Start () {
    action = Director.getInstance ().currentSceneController as UserAction;
    style = new GUIStyle();
    style.fontSize = 40;
    style.alignment = TextAnchor.MiddleCenter;

    style2 = new GUIStyle();
    style2.fontSize = 20;

    buttonStyle = new GUIStyle("button");
    buttonStyle.fontSize = 30;
}

void OnGUI() {
    GUI.Label(new Rect(30, 10, 50, 25), "游戏规则：", style2);
    GUI.Label(new Rect(30, 40, 75, 25), "球是魔鬼，方块是牧师", style2);
    GUI.Label(new Rect(30, 70, 75, 25), "任意一侧魔鬼多于牧师则游戏结束", style2);
    GUI.Label(new Rect(30, 100, 75, 25), "所有牧师与魔鬼都过到左侧即为胜利", style2);
    if (status == 1) {
        GUI.Label(new Rect(Screen.width/2-50, Screen.height/2-85, 100, 50), "Gameover!", style);
        if (GUI.Button(new Rect(Screen.width/2-70, Screen.height/2, 140, 70), "Restart", buttonStyle)) {
            status = 0;
            action.restart ();
        }
    } else if(status == 2) {
        GUI.Label(new Rect(Screen.width/2-50, Screen.height/2-85, 100, 50), "You win!", style);
        if (GUI.Button(new Rect(Screen.width/2-70, Screen.height/2, 140, 70), "Restart", buttonStyle)) {
            status = 0;
            action.restart ();
        }
    }
}
```



然后是牧师、魔鬼、船、岸等对象的生成

```c#
public GameObject[] priests_start = new GameObject[3];
public GameObject[] priests_end = new GameObject[3];
public GameObject[] devils_start = new GameObject[3];
public GameObject[] devils_end = new GameObject[3];
public GameObject[] boat = new GameObject[2];
//用数组存储在船上的gameobject
public GameObject boat_obj;
//获取船的gameobject
public Vector3 shoreStartPos = new Vector3(8, 0, 0);
//起点的岸的坐标
public Vector3 shoreEndPos = new Vector3(-8, 0, 0);
//终点的岸的坐标
public Vector3 boatStartPos = new Vector3(6, 0, 0);
public Vector3 boatEndPos = new Vector3(-14, 0, 0);
//记录船的两个位置
public float gap = 1.0f;
public int boatCapacity = 2;
//纪录船的容量
public int boat_position = 0;
//纪录船的位置
public int game = 0;
public int find = 0;

public Vector3 priestStartPos = new Vector3(6, 0, 0);
public Vector3 priestEndPos = new Vector3(-6, 0, 0);
public Vector3 devilStartPos = new Vector3(7, 0, 0);
public Vector3 devilEndPos = new Vector3(-7, 0, 0);

public Vector3 waterPos = new Vector3(0, 0, 0);
public Vector3 waterPos1 = new Vector3(0, 0, 0);

public void LoadResources()
    //载入资源
{
    // shore  
    Instantiate(Resources.Load("Prefabs/Stone"), shoreStartPos, Quaternion.identity);
    Instantiate(Resources.Load("Prefabs/Stone"), shoreEndPos, Quaternion.identity);
    Instantiate(Resources.Load("Prefabs/Water"), waterPos, Quaternion.identity);
    Instantiate(Resources.Load("Prefabs/Water"), waterPos1, Quaternion.identity);
    // boat  
    boat_obj = Instantiate(Resources.Load("Prefabs/Boat"), boatStartPos, Quaternion.identity) as GameObject;
    // priests & devils  
    for (int i = 0; i < 3; ++i)
    {
        priests_start[i] = (Instantiate(Resources.Load("Prefabs/Priest")) as GameObject);
        priests_end[i] = null;
        devils_start[i] = (Instantiate(Resources.Load("Prefabs/Devil")) as GameObject);
        devils_end[i] = null;
    }
}

void setCharacterPositions(GameObject[] array, Vector3 pos)
    //设置人物位置
{
    for (int i = 0; i < 3; ++i)
    {
        if (array[i] != null)
            array[i].transform.position = new Vector3(pos.x + gap * i, pos.y, pos.z);
    }
}

// Use this for initialization
void Start () {

}

// Update is called once per frame
void Update () {
    setCharacterPositions(priests_start, priestStartPos);
    setCharacterPositions(priests_end, priestEndPos);
    setCharacterPositions(devils_start, devilStartPos);
    setCharacterPositions(devils_end, devilEndPos);
}
```

最后则是不同动作根据需要进行不同的实现



下面展示一下我加了天空盒之后的场景

<img src="images\image10.png" style="zoom:60%;" />

这是胜利的界面

<img src="images\image11.png" style="zoom:60%;" />





### 3、材料与渲染联系【可选】

从 Unity 5 开始，使用新的 Standard Shader 作为自然场景的渲染。

- 阅读官方 [Standard Shader](https://docs.unity3d.com/Manual/shader-StandardShader.html) 手册 。
- 选择合适内容，如 [Albedo Color and Transparency](https://docs.unity3d.com/Manual/StandardShaderMaterialParameterAlbedoColor.html)，寻找合适素材，用博客展示相关效果的呈现



下面展示一下Albedo Color 和透明度调整的过程

**Albedo：**

<img src="images\image12.png" style="zoom:60%;" />

首先创建一个对象，然后为它更改材质，然后在`Main Maps`中选择`Albedo`选择none，并双击右边的颜色框即可进入调色板，里面有很多的默认颜色可以使用

这是我创建的不同颜色的对象

<img src="images\image7.png" style="zoom:60%;" />



**透明度：**

同样的也要先创建对象，添加材质，然后在`Shader`中选择`Legacy Shaders -> Transparent -> Diffuse`，材质更改为`none`

<img src="images\image14.png" style="zoom:60%;" />

<img src="images\image13.png" style="zoom:60%;" />

然后同样进入调色板，调整`A`即可调整物体的透明度了

<img src="images\image15.png" style="zoom:60%;" />

这是我创建的不同透明度的对象

<img src="images\image8.png" style="zoom:60%;" />