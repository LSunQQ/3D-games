# Homework 5

### 编写一个简单的鼠标打飞碟（Hit UFO）游戏

- **游戏内容要求：**
  1. 游戏有 n 个 round，每个 round 都包括10 次 trial；
  2. 每个 trial 的飞碟的色彩、大小、发射位置、速度、角度、同时出现的个数都可能不同。它们由该 round 的 ruler 控制；
  3. 每个 trial 的飞碟有随机性，总体难度随 round 上升；
  4. 鼠标点中得分，得分规则按色彩、大小、速度不同计算，规则可自由设定。
- **游戏的要求：**
  - 使用带缓存的工厂模式管理不同飞碟的生产与回收，该工厂必须是场景单实例的！具体实现见参考资源 Singleton 模板类
  - 近可能使用前面 MVC 结构实现人机交互与游戏模型分离



#### 游戏设计过程：

首先我制定了游戏规则：

- 用鼠标点击UFO即可消灭它们，并获得一定的分数
- 在飞碟掉落到地面之前未能消灭它则扣掉一条生命
- 游戏开始就会获得6条生命



然后开始飞碟的设计：

- 样式：我的飞碟是扁圆柱型的
  <img src="images\image1.png" style="zoom:80%;" />
  <br/>
  
- 种类：我设计了三种不同颜色的飞碟，分别是黑色、黄色和绿色
  <br/>

- 模式：我为整个游戏设计了三种难度，在获得一定的分数之后就会进入到下一个难度的模式，一开始是简单模式，随着难度的升级，UFO出现的速度以及频率都会相应的增加

  ```c#
  private int score_round2 = 10; //去到中等难度所需分数
  private int score_round3 = 25; //去到最高难度所需分数
  
  //难度升级
  if (score_recorder.score >= score_round2 && round == 1) {
      round = 2;
      //缩小飞碟发送间隔
      speed = speed - 0.6f;
      CancelInvoke("LoadResources");
      playing_game = false;
  } else if (score_recorder.score >= score_round3 && round == 2) {
      round = 3;
      speed = speed - 0.5f;
      CancelInvoke("LoadResources");
      playing_game = false;
  }
  ```
  <br/>

- 生成与销毁处理：根据不同的难度发出不同的飞碟以及被点击消灭或者落到地面上之后要让它消失

  ```c#
  public GameObject GetDisk(int round) {
      int choice = 0;
      int scope1 = 1, scope2 = 4, scope3 = 7;           //随机的范围
      float start_y = -10f;                             //刚实例化时的飞碟的竖直位置
      string tag;
      disk_prefab = null;
  
      //根据回合，随机选择要飞出的飞碟
      if (round == 1) {
          choice = Random.Range(0, scope1);
      } else if(round == 2) {
          choice = Random.Range(0, scope2);
      } else {
          choice = Random.Range(0, scope3);
      }
  
      //将要选择的飞碟的tag
      if(choice <= scope1) {
          tag = "disk1";
      } else if(choice <= scope2 && choice > scope1) {
          tag = "disk2";
      } else {
          tag = "disk3";
      }
  
      //寻找相同tag的空闲飞碟
      for(int i = 0;i < free.Count; i++) {
          if(free[i].tag == tag) {
              disk_prefab = free[i].gameObject;
              free.Remove(free[i]);
              break;
          }
      }
  
      //如果空闲列表中没有，则重新实例化飞碟
      if(disk_prefab == null) {
          if (tag == "disk1") {
              disk_prefab = Instantiate(Resources.Load<GameObject>("Prefabs/disk1"), new Vector3(0, start_y, 0), Quaternion.identity);
          } else if (tag == "disk2") {
              disk_prefab = Instantiate(Resources.Load<GameObject>("Prefabs/disk2"), new Vector3(0, start_y, 0), Quaternion.identity);
          } else {
              disk_prefab = Instantiate(Resources.Load<GameObject>("Prefabs/disk3"), new Vector3(0, start_y, 0), Quaternion.identity);
          }
  
          //给新实例化的飞碟赋予其他属性
          float ran_x = Random.Range(-1f, 1f) < 0 ? -1 : 1;
          disk_prefab.GetComponent<Renderer>().material.color = disk_prefab.GetComponent<DiskData>().color;
          disk_prefab.GetComponent<DiskData>().direction = new Vector3(ran_x, start_y, 0);
          disk_prefab.transform.localScale = disk_prefab.GetComponent<DiskData>().scale;
      }
  
      //添加到使用列表中
      used.Add(disk_prefab.GetComponent<DiskData>());
      return disk_prefab;
  }
  
  //回收飞碟
  public void FreeDisk(GameObject disk) {
      for(int i = 0;i < used.Count; i++) {
          if (disk.GetInstanceID() == used[i].gameObject.GetInstanceID()) {
              used[i].gameObject.SetActive(false);
              free.Add(used[i]);
              used.Remove(used[i]);
              break;
          }
      }
  }
  ```

  <br/>
  
- 飞行处理：根据$v = a * t$的物理公式来对整个UFO的飞行进行处理，然后通过位移的模拟来更新UFO出现的位置

  ```c#
  public static UFOFlyAction GetSSAction(Vector3 direction, float angle, float power) {
      //初始化物体将要运动的初速度向量
      UFOFlyAction action = CreateInstance<UFOFlyAction>();
      if (direction.x == -1) {
          action.start_vector = Quaternion.Euler(new Vector3(0, 0, -angle)) * Vector3.left * power;
      } else {
          action.start_vector = Quaternion.Euler(new Vector3(0, 0, angle)) * Vector3.right * power;
      }
      return action;
  }
  
  public override void Update() {
      //计算物体的向下的速度,v = at
      time += Time.fixedDeltaTime;
      gravity_vector.y = gravity * time;
  
      //位移模拟
      transform.position += (start_vector + gravity_vector) * Time.fixedDeltaTime;
      current_angle.z = Mathf.Atan((start_vector.y + gravity_vector.y) / start_vector.x) * Mathf.Rad2Deg;
      transform.eulerAngles = current_angle;
  
      //如果物体y坐标小于-10，动作就做完了
      if (this.transform.position.y < -10) {
          this.destroy = true;
          this.callback.SSActionEvent(this);      
      }
  }
  ```

  



接下来就是各个部分的详细设计了

1. 记录分数：这个模块需要根据击中的UFO来对当前得分进行改变。初始得分和重置游戏后的得分均为0，游戏过程中的分数根据具体的游戏过程进行相应的变换

   ```c#
   // Use this for initialization
   void Start () {
       score = 0;
   }
   
   //记录分数
   public void Record(GameObject disk) {
       int temp = disk.GetComponent<DiskData>().score;
       score = temp + score;
       //Debug.Log(score);
   }
   
   //重置分数
   public void Reset() {
       score = 0;
   }
   ```
   <br/>

2. 动作类和导演类：它们需要判断一些动作是否是有效的，比如点击到空白的地方也就是没有UFO的地方是不应该有任何的处理的，只有点击到有UFO的地方的时候才要进入爆炸效果的处理

   ```c#
   public class SSAction : ScriptableObject {
   
   	public bool enable = true;            //是否正在进行此动作
   	public bool destroy = false;          //是否需要被销毁
   	public GameObject gameobject;         //动作对象
   	public Transform transform;           //动作对象的transform
   	public ISSActionCallback callback;    //动作完成后的消息通知者
   
   	protected SSAction() {
   
   	}
   
   	//子类可以使用下面这两个函数
   	public virtual void Start() {
   		throw new System.NotImplementedException();
   	}
   	public virtual void Update() {
   		throw new System.NotImplementedException();
   	}
   }
   
   public class SSDirector : System.Object {
   
   	private static SSDirector _instance;                  //导演类的实例
   	public ISceneController CurrentScenceController { get; set; }
   	public static SSDirector GetInstance() {
   		if (_instance == null)
   			_instance = new SSDirector();
   		return _instance;
   	}
   }
   ```

   <br/>
   
3. 显示：需要根据飞碟的轨迹不断更新飞碟的位置，以及获得分数的变化，还有剩余生命值的变化

   ```c#
   void OnGUI () {
       bold_style.normal.textColor = new Color(1, 0, 0);
       bold_style.fontSize = 16;
       text_style.normal.textColor = new Color(0, 0, 0, 1);
       text_style.fontSize = 16;
       score_style.normal.textColor = new Color(1, 0, 1, 1);
       score_style.fontSize = 16;
       over_style.normal.textColor = new Color(1, 0, 0);
       over_style.fontSize = 25;
   
       if (game_start) {
   
           //用户射击
           if (Input.GetButtonDown("Fire1")) {
               Vector3 pos = Input.mousePosition;
               action.Hit(pos);
           }
   
           GUI.Label(new Rect(10, 5, 200, 50), "Score: ", text_style);
           GUI.Label(new Rect(70, 5, 200, 50), action.GetScore().ToString(), score_style);
   
           GUI.Label(new Rect(Screen.width - 150, 5, 20, 20), "Life:", text_style);
   
           //显示当前血量
           for (int i = 0; i < life; i++)
               GUI.Label(new Rect(Screen.width - 100 + 15 * i, 5, 50, 50), "❤", bold_style);
   
           //游戏结束
           if (life == 0) {
               high_score = high_score > action.GetScore() ? high_score : action.GetScore();
               GUI.Label(new Rect(Screen.width / 2 - 40, Screen.width / 2 - 250, 100, 100), "Game Over!", over_style);
               GUI.Label(new Rect(Screen.width / 2 - 35, Screen.width / 2 - 200, 50, 50), "Highest Score:", text_style);
               GUI.Label(new Rect(Screen.width / 2 + 75, Screen.width / 2 - 200, 50, 50), high_score.ToString(), text_style);
               if (GUI.Button(new Rect(Screen.width / 2 - 20, Screen.width / 2 - 150, 100, 50), "Restart")) {
                   life = 6;
                   action.ReStart();
                   return;
               }
               action.GameOver();
           }
       } else {
           GUI.Label(new Rect(Screen.width / 2 - 30, Screen.width / 2 - 350, 100, 100), "Welcome!", over_style);
           GUI.Label(new Rect(Screen.width / 2 - 75, Screen.width / 2 - 220, 150, 100), "点击飞碟消灭它们，你有6条命！", text_style);
           if (GUI.Button(new Rect(Screen.width / 2 - 20, Screen.width / 2-150, 100, 50), "Start")) {
               game_start = true;
               action.BeginGame();
           }
       }
   }
   
   public void ReduceBlood() {
       if(life > 0)
           life--;
   }
   ```



最后来看一下整个游戏场景的效果，我加上了自己的SkyBox效果
<img src="images\image2.png" style="zoom:80%;" />

<br/>

<img src="images\image3.png" style="zoom:80%;" />

<br/>

<img src="images\image4.png" style="zoom:80%;" />



最后附上
[完整代码](https://github.com/LSunQQ/3D-games/tree/homework5)
[视频演示](https://v.youku.com/v_show/id_XNDM5MDQ3NzI3Mg==.html?spm=a2h3j.8428770.3416059.1)