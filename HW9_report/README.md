# Homework 9

**以下作业五选一：**

1、血条（Health Bar）的预制设计。具体要求如下

- 分别使用 IMGUI 和 UGUI 实现
- 使用 UGUI，血条是游戏对象的一个子元素，任何时候需要面对主摄像机
- 分析两种实现的优缺点
- 给出预制的使用方法

2、 UI 效果制作（你仅需要实现以下效果之一）

- 进入NGUI官方网站，使用 UGUI 实现以下效果

  - [Inventory](http://www.tasharen.com/ngui/exampleX.html) 背包系统
  - [Quest Log](http://www.tasharen.com/ngui/example9.html) 公告牌
  - [Scroll View](http://www.tasharen.com/ngui/example7.html) 选择板

- 以上例子需要使用 Unity web player， 仅支持以下操作系统与浏览器，参见

  官方下载

  - Windows 版 **IE11**
  - **Mac OS X 10.7** Safari
  - 出现界面需要等待较长时间，打开页面让它慢慢加载

3、 DOTween 仿写

如果你觉得 UI 不是你的菜，喜欢复杂的设计与代码

- 研究 DOTween 网站 http://dotween.demigiant.com/getstarted.php 网页， 它在 Specific settings 中 transform.DoMove 返回 Tween 对象。请实现该对象，实现对动作的持续管理。
  - 本作业有较大难度，**[务必参考师兄的作业](https://blog.csdn.net/pmlpml)**

4、编写一个组件，提供常用窗口服务

- 修改 Game Jam Menu Template 的脚本
  - 如 ShowPanels 脚本
- 具体要求是实现一个方法
  - 支持某个面板窗口独立显示
  - 支持某个面板窗口模态，其他面板窗口显示在后面
  - 支持几个窗口同时显示，仅一个窗口是活动窗口

5、如果你喜欢凭空构思场景，请自制有趣的 UI 场景

- 例如：“几个小动物（3D）开会，语句从每个动物头上飘出，当达到一个大小，会出现清晰的文字！如果文字较多，会自动滚动”



我选择的是UI 效果制作并制作了一个**公告栏**

最终的成品展示：

<img src="images\image1.png" style="zoom:80%;" />



### 制作过程：

- 首先按照下面的框架构建

  <img src="images\image2.png" style="zoom:80%;" />

  创建一个`Canvas`然后为它创建一个`Image`、`Button`、`Scroll View`和`Scrollbar`子对象

  

- 然后为这个公告栏添加它的背景图片

  <img src="images\image3.png" style="zoom:80%;" />

  选中`Image`对象，然后点击`Source Image`，选择自己喜欢的图片作为背景

  这里再说一下如何将图片导入这个项目之中：

  - 首先将一张图片拖到`Assets`中

  - 然后选中那张图片，将它的`Texture Type`属性值选为`Sprite (2D and UI)`

    <img src="images\image4.png" style="zoom:80%;" />

  

- 接下来就是为公告栏添加文字了

  选择刚刚创建的`Scroll View`子对象，然后为它添加一个子对象`Text`，在文本框中添加自己需要的文本内容
  
  <img src="images\image5.png" style="zoom:80%;" />
  
  
  
- 由于之前添加了`Scrollbar`子对象，所以即使文本内容太多显示不完全，也可以通过滑动条来上下拖动看到所有的文本，而`Button`也可以修改上面的内容，方法是类似的，这里就不多说了。



最后附上

[项目地址](https://github.com/LSunQQ/3D-games/tree/homework9)

[视频演示](https://v.youku.com/v_show/id_XNDQzNjA1OTcyMA==.html?spm=a2h3j.8428770.3416059.1)