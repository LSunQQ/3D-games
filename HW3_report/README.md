# Homework 3

### 编程实践

- 阅读以下游戏脚本

> Priests and Devils
>
> Priests and Devils is a puzzle game in which you will help the Priests and Devils to cross the river within the time limit. There are 3 priests and 3 devils at one side of the river. They all want to get to the other side of this river, but there is only one boat and this boat can only carry two persons each time. And there must be one person steering the boat from one side to the other side. In the flash game, you can click on them to move them and click the go button to move the boat to the other direction. If the priests are out numbered by the devils on either side of the river, they get killed and the game is over. You can try it in many > ways. Keep all priests alive! Good luck!

程序需要满足的要求：

- play the game ( http://www.flash-game.net/game/2535/priests-and-devils.html )

<img src="images\image1.png" style="zoom:60%;" />

- 列出游戏中提及的事物（Objects）
  - 魔鬼
  - 牧师
  - 船
  - 两岸
  - 河水
  - 时间
- 用表格列出玩家动作表（规则表），注意，动作越少越好

| 规则     | 条件                                                        |
| -------- | ----------------------------------------------------------- |
| 上船     | 船上人数少于2，人和船在岸的同一侧                           |
| 过河     | 船上有人                                                    |
| 下船     | 船上有人                                                    |
| 成功     | 在规定时间内所有牧师与魔鬼都过到左边的岸上了                |
| 失败     | 至少一个牧师被吃、超时                                      |
| 牧师被吃 | （某一边岸上）或者（船上+船那一侧的岸边）的牧师人数少于魔鬼 |

- 请将游戏中对象做成预制
- 在 GenGameObjects 中创建 长方形、正方形、球 及其色彩代表游戏中的对象。
- 使用 C# 集合类型 有效组织对象
- 整个游戏仅 主摄像机 和 一个 Empty 对象， **其他对象必须代码动态生成！！！** 。 整个游戏不许出现 Find 游戏对象， SendMessage 这类突破程序结构的 通讯耦合 语句。 **违背本条准则，不给分**
- 请使用课件架构图编程，**不接受非 MVC 结构程序**
- 注意细节，例如：船未靠岸，牧师与魔鬼上下船运动中，均不能接受用户事件！

### 实验过程：

- 首先将对象做成预制并且存放在`Assets\Resources\Prefabs`目录下，然后运行的时候利用下面的代码进行加载

  ```c#
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
  ```

  

- 然后对于左岸、船上以及右岸的牧师和魔鬼，我是利用一个栈来存放的，注意`C#`的`Pop()`函数是获取栈顶元素再删除，与`C++`的不一样

  ```c#
  // use the stack to store the priests and the devils
  Stack<GameObject> leftPriests = new Stack<GameObject>();
  Stack<GameObject> rightPriests = new Stack<GameObject>();
  Stack<GameObject> leftDevils = new Stack<GameObject>();
  Stack<GameObject> rightDevils = new Stack<GameObject>();
  ```

  

- 接着就是根据MVC结构来进行编程了。我分别创建了Model、View和Controller三个文件，Model里面是一些函数模型，提供给游戏中的object使用，而View负责的是显示功能，将游戏中需要的object显示在屏幕上，最后的Controller是用来控制object的，利用Model中的函数模型使得object能够做出一些相应的操作。这里挑几个比较重要的函数进行介绍，完整的代码请到[github](https://github.com/LSunQQ/3D-games/tree/homework3)上查看

  - `check()`：这个函数是用来判断游戏是否结束的。如果3个牧师和3个魔鬼都过到了河的右侧，那么游戏胜利，如果在其中的任何一步出现一侧有牧师而且该侧所有牧师数量（如果船在该侧则船上的也算）少于同一侧魔鬼的数量（如果船在该侧则船上的也算），那么游戏失败

    ```c#
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
            }
        }
        
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
        
        // if the number of the priests more the zero and less than the number of the devils in the same side, lose
        if ((priestsOnLeftBank != 0 && priestsOnLeftBank < devilsOnLeftBank) || 
            (priestsOnRightBank != 0 && priestsOnRightBank < devilsOnRightBank)) {
            game.state = State.Lose;
        }
    }
    ```

  - `getOnShip()`：这个函数的功能是让一个object上船，关键点在于让这个object与船连成一体，使得船调用移动函数的时候能够让船上的物体一起过河

    ```c#
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
    ```

  - `leftPriestGetOnShip()`：这个函数是让左岸的牧师上船，其它的object是类似的。上船主要有三个条件：船在该侧、船没满以及该侧有牧师

    ```c#
    public void leftPriestGetOnShip() {
        if (leftPriests.Count != 0 && isShipFull() != 2 && game.state == State.Left) {
            getOnShip(leftPriests.Pop()); // Pop(): get the top element then delete it
        }
    }
    ```



- 最后来看看游戏的界面
  由于显示比例问题，所以在运行之前请选中Maximize On Play
  <img src="images\image4.png" style="zoom:60%;" />
  
  
  
  如果按了reset之后object颜色发生了变化，请在`Window->lighting->setting`中取消勾选`Auto Generate `并点击一下`Generate Lighting`，加载完成后即可
  <img src="images\image5.png" style="zoom:60%;" /> <img src="images\image6.png" style="zoom:60%;" />
  
  
  
  
  
  - 这是开始时的界面
    <img src="images\image2.png" style="zoom:60%;" />
  - 这是胜利时的界面
    <img src="images\image3.png" style="zoom:60%;" />