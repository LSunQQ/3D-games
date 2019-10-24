# Homework 7

智能巡逻兵

- 提交要求：
- 游戏设计要求：
  - 创建一个地图和若干巡逻兵(使用动画)；
  - 每个巡逻兵走一个3~5个边的凸多边型，位置数据是相对地址。即每次确定下一个目标位置，用自己当前位置为原点计算；
  - 巡逻兵碰撞到障碍物，则会自动选下一个点为目标；
  - 巡逻兵在设定范围内感知到玩家，会自动追击玩家；
  - 失去玩家目标后，继续巡逻；
  - 计分：玩家每次甩掉一个巡逻兵计一分，与巡逻兵碰撞游戏结束；
- 程序设计要求：
  - 必须使用订阅与发布模式传消息
    - subject：OnLostGoal
    - Publisher: ?
    - Subscriber: ?
  - 工厂模式生产巡逻兵
- 友善提示1：生成 3~5个边的凸多边型
  - 随机生成矩形
  - 在矩形每个边上随机找点，可得到 3 - 4 的凸多边型
  - 5 ?
- 友善提示2：参考以前博客，给出自己新玩法



### 游戏设计规则：

利用WASD操控目标进行移动，玩家每次甩掉一个巡逻兵计一分，与巡逻兵碰撞游戏结束，如果在60秒内没有碰到巡逻兵，则游戏胜利。



### 游戏设计过程：

- 首先介绍一下本次设计中运用到的模型：
  - 巡逻兵模型
    <img src="images\image1.png" style="zoom:60%;" />我用的是一个免费的Soldiers模型，里面有三种士兵，我将一个作为玩家操控的，再选另外一个当做巡逻兵
  - 地形
    <img src="images\image2.png" style="zoom:60%;" />这是用来隔开每一个巡逻兵的栅栏，因为每个巡逻兵都有自己的移动范围，同时为了增加游戏的难度，所以我就增加了几个栅栏来进行空间分隔。

  这些资源下载下来之后点击`import`即可导入到项目里面进行使用。

  <br/>

- 然后开始设计订阅与发布模式

  在“发布者-订阅者”模式中，称为发布者的消息发送者不会将消息编程为直接发送给称为订阅者的特定接收者。这意味着发布者和订阅者不知道彼此的存在。存在第三个组件，称为代理或消息代理或事件总线，它由发布者和订阅者都知道，它过滤所有传入的消息并相应地分发它们。换句话说，pub-sub是用于在不同系统组件之间传递消息的模式，而这些组件不知道关于彼此身份的任何信息。经纪人如何过滤所有消息？实际上，有几个消息过滤过程。最常用的方法有：基于主题和基于内容的。

  所以我就先设计了订阅与发布模式的入口类：

  ```c#
  public class GameEventManager : MonoBehaviour {
      // 玩家逃脱事件
      public delegate void EscapeEvent(GameObject patrol);
      public static event EscapeEvent OnGoalLost;
      // 巡逻兵追击事件
      public delegate void FollowEvent(GameObject patrol);
      public static event FollowEvent OnFollowing;
      // 游戏失败事件
      public delegate void GameOverEvent();
      public static event GameOverEvent GameOver;
      // 游戏胜利事件
      public delegate void WinEvent();
      public static event WinEvent Win;
  
      // 玩家逃脱
      public void PlayerEscape(GameObject patrol) {
          if (OnGoalLost != null) {
              OnGoalLost(patrol);
          }
      }
  
      // 巡逻兵追击
      public void FollowPlayer(GameObject patrol) {
          if (OnFollowing != null) {
              OnFollowing(patrol);
          }
      }
  
      // 玩家被捕
      public void OnPlayerCatched() {
          if (GameOver != null) {
              GameOver();
          }
      }
  
      // 时间结束
      public void TimeIsUP() {
          if (Win != null) {
              Win();
          } 
      }
  }
  ```

  <br/>

  然后这个是订阅者的设计

  ```c#
  // 失去目标，巡逻兵放弃追击
  public void OnGoalLost(GameObject patrol) {
      patrolActionManager.Patrol(patrol);
      scoreRecorder.Record();
  }
  
  // 玩家进入范围，巡逻兵开始追击
  public void OnFollowing(GameObject patrol) {
      patrolActionManager.Follow(player, patrol);
  }
  
  // 失败
  public void GameOver() {
      gameState = GameState.LOSE;
      StopAllCoroutines();
      patrolFactory.PausePatrol();
      player.GetComponent<Animator>().SetTrigger("death");
      patrolActionManager.DestroyAllActions();
  }
  
  // 胜利
  public void Win() {
      gameState = GameState.WIN;
      StopAllCoroutines();
      patrolFactory.PausePatrol();
  }
  ```

  <br/>

- 接下来我就利用之前下载的游戏资源设计了一下整个游戏的环境：
  <img src="images\image3.png" style="zoom:60%;" />

  可以看到整个游戏的地图大致分为了9个部分，每一个部分里面有一个巡逻兵负责，而玩家则可以在这9个部分里面随意走动。下面这个是检测玩家或者巡逻兵移动的函数：

  ```c#
  public class EnterRegion : MonoBehaviour {
      public int region;                         // 当前区域的区域编号
      FirstSceneController sceneController;      // 当前的场记
  
      void OnTriggerEnter(Collider collider) {
          sceneController = Director.GetInstance().CurrentSceneController as FirstSceneController;
          if (collider.gameObject.tag == "Player") {
              // 如果玩家进入区域，则标记玩家当前区域为该区域
              sceneController.playerRegion = region;
          }
      }
  
      private void OnTriggerExit(Collider collider) {
          if (collider.gameObject.tag == "Patrol") {
              // 如果巡逻兵尝试离开区域，则标记巡逻兵发生了碰撞，以控制转向
              collider.gameObject.GetComponent<PatrolData>().isCollided = true;
          }
      }
  }
  ```

  <br/>

- 玩家与巡逻兵的设计

  这两个模型的构造大致如下：

  <img src="images\image4.png" style="zoom:100%;" /><img src="images\image5.png" style="zoom:100%;" />

  <br/>

  我为巡逻兵添加了搜索范围：

  <img src="images\image6.png" style="zoom:100%;" />

  同时， 在 Patrol 上添加一个 Capsule Collider，用于检测巡逻兵与障碍物、玩家的碰撞。在 Bip001 上添加一个 Capsule Collider，用于感知玩家。 

  <br/>

  下面的是与巡逻兵有关的一些参数，用于控制巡逻兵的移动、搜索等

  ```c#
  public class PatrolData : MonoBehaviour {
      public bool isPlayerInRange;    // 玩家是否在侦测范围里
      public bool isFollowing;        // 是否正在追击
      public bool isCollided;         // 是否发生碰撞
      public int patrolRegion;        // 巡逻兵所在区域
      public int playerRegion;        // 玩家所在区域
      public GameObject player;       // 所追击的玩家
  }
  ```

  <br/>

  全部的巡逻兵都是由`Factory`模式进行批量生产的

  ```c#
  public class PatrolFactory : MonoBehaviour {
      public GameObject patrol = null;
      private List<PatrolData> used = new List<PatrolData>(); // 正在使用的巡逻兵
  
      public List<GameObject> GetPatrols() {
          List<GameObject> patrols = new List<GameObject>();
          float[] pos_x = { -4.5f, 1.5f, 7.5f };
          float[] pos_z = { 7.5f, 1.5f, -4.5f };
          for (int i = 0; i < 3; i++) {
              for (int j = 0; j < 3; j++) {
                  patrol = Instantiate(Resources.Load<GameObject>("Prefabs/Patrol"));
                  patrol.transform.position = new Vector3(pos_x[j], 0, pos_z[i]);
                  patrol.GetComponent<PatrolData>().patrolRegion = i * 3 + j + 1;
                  patrol.GetComponent<PatrolData>().playerRegion = 4;
                  patrol.GetComponent<PatrolData>().isPlayerInRange = false;
                  patrol.GetComponent<PatrolData>().isFollowing = false;
                  patrol.GetComponent<PatrolData>().isCollided = false;
                  patrol.GetComponent<Animator>().SetBool("pause", true);
                  used.Add(patrol.GetComponent<PatrolData>());
                  patrols.Add(patrol);
              }
          }
          return patrols;
      }
  
      public void PausePatrol() {
          //切换所有侦查兵的动画
          for (int i = 0; i < used.Count; i++) {
              used[i].gameObject.GetComponent<Animator>().SetBool("pause", true);
          }
      }
  
      public void StartPatrol() {
          //切换所有侦查兵的动画
          for (int i = 0; i < used.Count; i++) {
              used[i].gameObject.GetComponent<Animator>().SetBool("pause", false);
          }
      }
  }
  ```

  <br/>

  为了让巡逻兵能够检测到玩家与障碍物，我为这两个物体设置了`tag`标签：

  <img src="images\image7.png" style="zoom:100%;" />

  ```c#
  public class PatrolCollide : MonoBehaviour {
      void OnCollisionEnter(Collision collision) {
          if (collision.gameObject.tag == "Player") {
              // 当玩家与巡逻兵相撞
              this.GetComponent<Animator>().SetTrigger("shoot");
              Singleton<GameEventManager>.Instance.OnPlayerCatched();
          } else {
              // 当巡逻兵碰到其他障碍物
              this.GetComponent<PatrolData>().isCollided = true;
          } 
      }
  }
  ```

  <br/>

  巡逻兵在没有发现玩家的时候正常随机移动，遇到障碍物则会向后转，接着随机移动，如果发现了玩家，则会开始跟随玩家，直到玩家脱离视野，并向订阅者发送消息。

  ```c#
  public class PatrolAction : Action {
      private float pos_x, pos_z;                 // 移动前的初始x和z方向坐标
      private bool turn = true;                   // 是否选择新方向
      private PatrolData data;                    // 巡逻兵的数据
  
      public static PatrolAction GetAction(Vector3 location) {
          PatrolAction action = CreateInstance<PatrolAction>();
          action.pos_x = location.x;
          action.pos_z = location.z;
          return action;
      } 
  
      public override void Start() {
          data = this.gameObject.GetComponent<PatrolData>();
      }
  
      public override void Update() {
          if (Director.GetInstance().CurrentSceneController.getGameState().Equals(GameState.RUNNING)) {
              // 巡逻兵巡逻
              Patrol();
              // 如果满足要求，而巡逻兵未开始追击，则停止巡逻，开始追击
              if (!data.isFollowing && data.isPlayerInRange && data.patrolRegion == data.playerRegion && !data.isCollided) {
                  this.destroy = true;
                  this.enable = false;
                  this.callback.ActionEvent(this);
                  this.gameObject.GetComponent<PatrolData>().isFollowing = true;
                  Singleton<GameEventManager>.Instance.FollowPlayer(this.gameObject);
              }
          }
      }
  
      void Patrol() {
          if (turn) {
              pos_x = this.transform.position.x + Random.Range(-5f, 5f);
              pos_z = this.transform.position.z + Random.Range(-5f, 5f);
              this.transform.LookAt(new Vector3(pos_x, 0, pos_z));
              this.gameObject.GetComponent<PatrolData>().isCollided = false;
              turn = false;
          }
          float distance = Vector3.Distance(transform.position, new Vector3(pos_x, 0, pos_z));
  
          if (this.gameObject.GetComponent<PatrolData>().isCollided) {
              // 碰撞，则向后转，寻找新位置
              this.transform.Rotate(Vector3.up, 180);
              GameObject temp = new GameObject();
              temp.transform.position = this.transform.position;
              temp.transform.rotation = this.transform.rotation;
              temp.transform.Translate(0, 0, Random.Range(0.5f, 3f));
              pos_x = temp.transform.position.x;
              pos_z = temp.transform.position.z;
              this.transform.LookAt(new Vector3(pos_x, 0, pos_z));
              this.gameObject.GetComponent<PatrolData>().isCollided = false;
              Destroy(temp);
          } else if (distance <= 0.1) {
              turn = true;
          } else {
              // 向前移动巡逻兵
              this.transform.Translate(0, 0, Time.deltaTime);
          }
      }
  }
  ```

  这是跟随玩家的设计，其中如果玩家脱离视野，同样会向发布者发送消息，结束跟随

  ```c#
  public class PatrolFollowAction : Action {
      private float speed = 1.5f;          // 跟随玩家的速度
      private GameObject player;           // 玩家
      private PatrolData data;             // 巡逻兵数据
  
      public static PatrolFollowAction GetAction(GameObject player) {
          PatrolFollowAction action = CreateInstance<PatrolFollowAction>();
          action.player = player;
          return action;
      }
  
      public override void Start() {
          data = this.gameObject.GetComponent<PatrolData>();
      }
  
      public override void Update() {
          if (Director.GetInstance().CurrentSceneController.getGameState().Equals(GameState.RUNNING)) {
              // 追击玩家
              transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
              this.transform.LookAt(player.transform.position);
              // 如果满足要求，而巡逻兵正在追击，则停止追击，开始巡逻
              if (data.isFollowing && (!(data.isPlayerInRange && data.patrolRegion == data.playerRegion) || data.isCollided)) {
                  this.destroy = true;
                  this.enable = false;
                  this.callback.ActionEvent(this);
                  this.gameObject.GetComponent<PatrolData>().isFollowing = false;
                  Singleton<GameEventManager>.Instance.PlayerEscape(this.gameObject);
              }
          }
      }
  }
  ```

  <br/>

  而玩家的设计则要使用键盘输入控制移动，利用WASD控制上下左右移动

  ```c#
  private void Update() {
      action = Director.GetInstance().CurrentSceneController as UserAction;
      controller = Director.GetInstance().CurrentSceneController as SceneController;
      if (controller.getGameState().Equals(GameState.RUNNING)) {
          // 获取键盘输入
          float translationX = Input.GetAxis("Horizontal");
          float translationZ = Input.GetAxis("Vertical");
          //移动玩家
          action.MovePlayer(translationX, translationZ);
      }
  }
  ```

  我将玩家的移动速度在沿直线行走的时候调的较快，能够更容易的脱离巡逻兵的跟随

  ```c#
  public void MovePlayer(float translationX, float translationZ) {
      if (translationX != 0 || translationZ != 0) {
          player.GetComponent<Animator>().SetBool("run", true);
      } else {
          player.GetComponent<Animator>().SetBool("run", false);
      }
      translationX *= Time.deltaTime;
      translationZ *= Time.deltaTime;
  
      player.transform.LookAt(new Vector3(player.transform.position.x + translationX, player.transform.position.y, player.transform.position.z + translationZ));
      if (translationX == 0)
          player.transform.Translate(0, 0, Mathf.Abs(translationZ) * 2);
      else if (translationZ == 0)
          player.transform.Translate(0, 0, Mathf.Abs(translationX) * 2);
      else
          player.transform.Translate(0, 0, Mathf.Abs(translationZ) + Mathf.Abs(translationX));
  }
  ```

  <br/>

- 其它设计：

  首先我的游戏时间是60秒，利用`Director`进行时间的设计

  ```c#
  public class Director : System.Object {
      private static Director _instance;                          // 导演的实例
      public SceneController CurrentSceneController { get; set; } // 当前的场记
      public int leaveSeconds = 60;                               // 当前剩余时间
  
      private Director() {}
  
      // 获取导演实例
      public static Director GetInstance() {
          if (_instance == null) {
              _instance = new Director();
          }
          return _instance;
      }
  
      public int GetFPS() {
          return Application.targetFrameRate;
      }
  
      public void SetFPS(int fps) {
          Application.targetFrameRate = fps;
      }
  
      // 游戏倒计时
      public IEnumerator CountDown() {
          while (leaveSeconds > 0) {
              yield return new WaitForSeconds(1f);
              leaveSeconds--;
              if (leaveSeconds == 0) {
                  Singleton<GameEventManager>.Instance.TimeIsUP();
              }
          }
      }
  }
  ```

  <br/>

  最重要的是需要镜头始终跟随玩家移动显示

  ```c#
  public class CameraFollowAction : MonoBehaviour {
      public GameObject player;            //相机跟随的物体
      public float smothing = 5f;          //相机跟随的平滑速度
      Vector3 offset;                      //相机与物体相对偏移位置
  
      void Start() {
          offset = new Vector3(0, 5, -5);
      }
  
      void FixedUpdate() {
          // 设置摄像机目标位置
          Vector3 target = player.transform.position + offset;
          //摄像机自身位置到目标位置平滑过渡
          transform.position = Vector3.Lerp(transform.position, target, smothing * Time.deltaTime);
      }
  }
  ```



### 游戏效果展示

初始界面：

<img src="images\image8.png" style="zoom:60%;" />

失败界面：

<img src="images\image9.png" style="zoom:60%;" />

胜利界面：

<img src="images\image10.png" style="zoom:60%;" />



最后附上

[项目地址](https://github.com/LSunQQ/3D-games/tree/homework7)

[视频演示](https://v.youku.com/v_show/id_XNDQxMDg5MDg3Ng==.html?spm=a2h3j.8428770.3416059.1)

