# Homework 8

本次作业基本要求是**三选一**

1、简单粒子制作

- 按参考资源要求，制作一个粒子系统，[参考资源](http://www.cnblogs.com/CaomaoUnity3d/p/5983730.html)
- 使用 3.3 节介绍，用代码控制使之在不同场景下效果不一样

2、完善官方的“汽车尾气”模拟

- 使用官方资源资源 Vehicle 的 car， 使用 Smoke 粒子系统模拟启动发动、运行、故障等场景效果

3、参考 http://i-remember.fr/en 这类网站，使用粒子流编程控制制作一些效果， 如“粒子光环”

- 可参考以前作业

我选择的是**粒子光环**



### 制作过程

#### 首先构造一个默认的粒子系统

创建一个空的`GameObject`，然后在这个空对象上右击，选择`Effects->Particle System`

<img src="images\image1.png" style="zoom:80%;" />



<img src="images\image2.png" style="zoom:80%;" />



#### 接着创建一个`C#`文件来控制粒子的行为

1. 声明几个关于粒子的参数，以便后面对粒子进行各种变换

   ```c#
   public ParticleSystem particleSystem; //粒子系统对象
   public int particleNumber = 5000;     //发射的最大粒子数
   public float size = 0.05f;            //粒子的大小
   public float maxRadius = 12.0f;       //粒子的旋转半径
   public float minRadius = 4.0f;
   public float speed = 0.05f;           //粒子的运动速度
   private float[] particleAngle;        //粒子的偏转角
   private float[] particleRadius;       //粒子的运动半径
   
   private ParticleSystem.Particle[] particlesArray;
   ```

   <br/>

2. 然后对这些数据进行初始化以及内存空间的分配，可以在`Start`函数中进行实现

   ```c#
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
   ```

   在`init`函数中，对所有的粒子生成随机的半径、运动角度，另外，由于用弧度制直接生成的运动角度不是我想要的效果，所以先使用了角度制生成一个角度再换回弧度制

   ```c#
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
   ```

   到这个时候，生成的时候粒子已经是圆形的形状了，但是还是会散开。

   <br/>

3. 让粒子旋转运动，使得能够一直保持圆环状

   我使用了两个环，一个顺时针转，另外一个逆时针转

   ```c#
   void Update() {
       colorTimeOut += Time.deltaTime;
       for (int i = 0; i < particleNumber; i++) {
           particlesArray[i].color = setColor;
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
   ```

   - 其中速度那里设置为`speed * (i % 10 + 1)`是为了能够让所有的粒子速度不一样

   <br/>



### 实现效果

<img src="images\image3.png" style="zoom:80%;" />



[视频演示](https://v.youku.com/v_show/id_XNDQyNjM0Nzc0OA==.html?spm=a2h3j.8428770.3416059.1)

[实验代码](https://github.com/LSunQQ/3D-games/tree/homework8)

