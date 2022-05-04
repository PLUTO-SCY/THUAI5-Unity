using Communication.Proto;
using Communication.ClientCommunication;
using System.Collections.Generic;
using UnityEngine;
using System;
//using Playback;
using System.Collections.Concurrent;
using static Communication.Proto.MessageToClient.Types;  //later find that this can simplify the code

/****
 配置文件：9 .dll
 ****/

public class LiveBroadcast : MonoBehaviour
{
    //MessageReader messageReader;
    public MessageToClient msg;
    public GameObjMessage gemObjMsg;
    private bool isGameStarted;
    public GameObject test;

    private ConcurrentDictionary<long, bool> isPlayersExisted;
    private ConcurrentDictionary<long, bool> isBulletsExisted;
    private ConcurrentDictionary<long, bool> isPropsExisted;
    private ConcurrentDictionary<long, bool> isBombedBulletsExisted;
    private ConcurrentQueue<KeyValuePair<long, GameObjMessage>> TaskQueue;

    public ConcurrentDictionary<long, PlayerScript> players;
    public static ConcurrentDictionary<long, LB_BulletScript> bullets;
    public ConcurrentDictionary<long, PropScript> props;
    public ConcurrentDictionary<long, BombedBulletScript> bombullets;

    private PropManager propManager;
    private PlayerManager playerManager;
    private BulletManager bulletManager;
    private BombedBulletManager bombulletManager;

    public static int[,] map;
    public GameObject[] mapSquare;

    private bool isMapRead;
    private bool firstDrawMap;   //第一次刷新地图

    // Start is called before the first frame update
    void Start()
    {
        //------------***--------------
        //quantities of Initialization

        //Instantiate(test, new Vector3(0, 0, 20), Quaternion.identity);
        //bullet = Instantiate(myPrefab, transform.position, transform.rotation)
        //一些初始化操作
        
        gemObjMsg = new GameObjMessage();

        propManager = GetComponent<PropManager>();
        playerManager = GetComponent<PlayerManager>();
        bulletManager = GetComponent<BulletManager>();
        bombulletManager = GetComponent<BombedBulletManager>();

        TaskQueue = new ConcurrentQueue<KeyValuePair<long, GameObjMessage>>();

        players = new ConcurrentDictionary<long, PlayerScript>();
        bullets = new ConcurrentDictionary<long, LB_BulletScript>();
        props = new ConcurrentDictionary<long, PropScript>();
        bombullets = new ConcurrentDictionary<long, BombedBulletScript>();  

        isPlayersExisted = new ConcurrentDictionary<long, bool>();
        isBulletsExisted = new ConcurrentDictionary<long, bool>();
        isPropsExisted = new ConcurrentDictionary<long, bool>();
        isBombedBulletsExisted = new ConcurrentDictionary<long, bool>();

        isMapRead = false;
        firstDrawMap = false;

        map = new int[50, 50];  //save the map information

        /*-------------------------------*/
        /*-------connect to server-------*/
        /*-------                 -------*/
        Debug.Log("---BEGIN---");        
        long playerID, teamID;
        playerID = 0;
        teamID = 0;
        ClientCommunication clientCommunication = new ClientCommunication();

        if (clientCommunication.Connect("127.0.0.1", 7777))
        {
            Debug.Log("成功连接Server.");
        }
        else
        {
            Debug.Log("连接Server失败.");
            Application.Quit();
        }

        MessageToServer messageToServer = new MessageToServer();
        messageToServer.MessageType = MessageType.AddPlayer;
        messageToServer.PlayerID = playerID;
        messageToServer.TeamID = teamID;
        messageToServer.ASkill1 = ActiveSkillType.BecomeAssassin;
        messageToServer.PSkill = PassiveSkillType.SpeedUpWhenLeavingGrass;

        try
        {
            clientCommunication.SendMessage(messageToServer);  //发送观展者消息
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
            Debug.Log(e.StackTrace);
        }

        clientCommunication.OnReceive += () =>
        {
            try
            {
                if (clientCommunication.TryTake(out IGameMessage msg) && msg.PacketType == PacketType.MessageToClient)
                {
                    switch (((MessageToClient)msg.Content).MessageType)
                    {
                        case MessageType.StartGame:
                            GameStart((MessageToClient)msg.Content);
                            break;
                        case MessageType.Gaming:
                            Refresh((MessageToClient)msg.Content);
                            break;
                        case MessageType.EndGame:
                            GameOver();
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
                            // above five lines of code work well to close the whole game                        
                            break;
                        default: break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());

                Debug.Log(e.StackTrace);
            }

        };
        //Thread.Sleep(1000);
    }

    // Update is called once per frame
    void Update()
    {
        if ((isMapRead==true) && (firstDrawMap==false))
        {
            DrawMap();
            firstDrawMap = true;
        }
        try
        {
            if (!TaskQueue.IsEmpty)
            {
                FlushTaskQueue();
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
            Debug.Log(e.StackTrace);
        }
        
    }

    private void Refresh(MessageToClient message)
    {
        foreach (var obj in msg.GameObjMessage)   //dependon type of msg for diffthings
        {
            switch (obj.ObjCase)
            {
                case GameObjMessage.ObjOneofCase.None:  //空信息
                    Debug.Log("Empty message");
                    break;

                case GameObjMessage.ObjOneofCase.MessageOfCharacter: //人物信息
                    if (isPlayersExisted.ContainsKey(obj.MessageOfCharacter.Guid) && (isPlayersExisted[obj.MessageOfCharacter.Guid] == true) && (players.ContainsKey(obj.MessageOfCharacter.Guid)))
                    {
                        players[obj.MessageOfCharacter.Guid].Renew(obj.MessageOfCharacter); //存在就更新数据
                    }
                    else if (!isPlayersExisted.ContainsKey(obj.MessageOfCharacter.Guid))
                    {
                        isPlayersExisted.TryAdd(obj.MessageOfCharacter.Guid, true);
                        TaskQueue.Enqueue(new KeyValuePair<long, GameObjMessage>(obj.MessageOfCharacter.Guid, obj)); //不存在就标记并创建，下面同理
                    }
                    break;

                case GameObjMessage.ObjOneofCase.MessageOfBullet: //子弹信息                    
                    if (bullets.ContainsKey(obj.MessageOfBullet.Guid) && (isBulletsExisted[obj.MessageOfBullet.Guid] == true) && (isBulletsExisted.ContainsKey(obj.MessageOfBullet.Guid)))
                    {
                        bullets[obj.MessageOfBullet.Guid].Renew(obj.MessageOfBullet); //存在就更新数据
                    }
                    else if (!isBulletsExisted.ContainsKey(obj.MessageOfBullet.Guid))
                    {
                        isBulletsExisted.TryAdd(obj.MessageOfBullet.Guid, true);
                        TaskQueue.Enqueue(new KeyValuePair<long, GameObjMessage>(obj.MessageOfBullet.Guid, obj)); //不存在就标记并创建
                    }

                    break;

                case GameObjMessage.ObjOneofCase.MessageOfProp: //道具信息

                    if (props.ContainsKey(obj.MessageOfProp.Guid) && (isPropsExisted.ContainsKey(obj.MessageOfProp.Guid)) && (isPropsExisted[obj.MessageOfProp.Guid] == true))
                    {
                        props[obj.MessageOfProp.Guid].Renew(obj.MessageOfProp); //存在就更新数据
                    }
                    else if (!isPropsExisted.ContainsKey(obj.MessageOfProp.Guid))
                    {
                        isPropsExisted.TryAdd(obj.MessageOfProp.Guid, true);
                        TaskQueue.Enqueue(new KeyValuePair<long, GameObjMessage>(obj.MessageOfProp.Guid, obj)); //不存在就标记并创建
                    }
                    else if (isPropsExisted.ContainsKey(obj.MessageOfProp.Guid) && (isPropsExisted[obj.MessageOfProp.Guid] == false))
                    {
                        isPropsExisted[obj.MessageOfProp.Guid] = true;
                        props[obj.MessageOfProp.Guid].Renew(obj.MessageOfProp); //存在就更新数据
                        props[obj.MessageOfProp.Guid].unPicked();  //解除隐身状态
                    }
                    break;

                case GameObjMessage.ObjOneofCase.MessageOfBombedBullet:
                    isBombedBulletsExisted.TryAdd(obj.MessageOfBombedBullet.MappingID, true);
                    TaskQueue.Enqueue(new KeyValuePair<long, GameObjMessage>(obj.MessageOfBombedBullet.MappingID, obj)); //不存在就标记并创建                   
                    break;

                case GameObjMessage.ObjOneofCase.MessageOfMap:
                    //Debug.Log("mapMsg");
                    //obj.MessageOfMap.Row[1].Col[2].
                    //the map has been initialized
                    break;

                case GameObjMessage.ObjOneofCase.MessageOfPickedProp:
                    //Debug.Log("Should Pick");                    
                    isPropsExisted[obj.MessageOfPickedProp.MappingID] = false;
                    props[obj.MessageOfPickedProp.MappingID].Picked();    //在这里摧毁                    
                    break;


                default: Debug.Log("good luck to debug!"); break;
            }
        }
    }

    public void GameStart(MessageToClient message)  //游戏开始的信息
    {
        //根据初始的消息初始化各个角色(((为什么要单独处理?
        foreach (var obj in message.GameObjMessage)
        {
            if (obj.ObjCase == GameObjMessage.ObjOneofCase.MessageOfMap)
            {
                Debug.Log("Game start! Map preparing!!!");
                isMapRead=true;
                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        map[i, j] = obj.MessageOfMap.Row[i].Col[j];
                    }
                }
                Debug.Log("finish the map information saving!");
                isMapRead = true;
            }
            else if (obj.ObjCase == GameObjMessage.ObjOneofCase.MessageOfCharacter)
            {
                if (players.ContainsKey(obj.MessageOfCharacter.Guid))
                {
                    players[obj.MessageOfCharacter.Guid].Renew(obj.MessageOfCharacter); //存在就更新数据
                }
                else if (!players.ContainsKey(obj.MessageOfCharacter.Guid))
                {
                    isPlayersExisted.TryAdd(obj.MessageOfCharacter.Guid, true);
                    TaskQueue.Enqueue(new KeyValuePair<long, GameObjMessage>(obj.MessageOfCharacter.Guid, obj)); //不存在就标记并创建，下面同理
                }
            }
        }
        isGameStarted = true;
    }

    public void GameOver()
    {
        //TO DO:显示结束画面，同时显示两方的得分
        Debug.Log("退出游戏");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }

    public void FlushTaskQueue()
    {
        while (!TaskQueue.IsEmpty)
        {
            bool isGood = TaskQueue.TryDequeue(out KeyValuePair<long, GameObjMessage> res);
            if (isGood)
            {
                MyInstantiate(res.Key, res.Value);
            }
        }
    }
    public void DrawMap()
    {
        Vector3 oneMapVec;
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                oneMapVec.x = (float)j / (float)1000;
                oneMapVec.y = (float)50 - (float)i / (float)1000;
                oneMapVec.z = -1;
                switch (map[i, j])
                {
                    case 1:
                        Instantiate(mapSquare[0], oneMapVec, Quaternion.identity);
                        break;
                    case 2:
                        break;
                    case 3:
                    case 4:
                        Instantiate(mapSquare[1], oneMapVec, Quaternion.identity);
                        break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        Instantiate(mapSquare[2], oneMapVec, Quaternion.identity);
                        break;
                    case 13:
                        Instantiate(mapSquare[3], oneMapVec, Quaternion.identity);
                        break;
                }
            }
        }
    }

    public void MyInstantiate(long objGuid, GameObjMessage objMsg)
    {
        GameObject isnullobj, bbobj;
        float x, y;
        //Vector2 posi;
        switch (objMsg.ObjCase)
        {
            case GameObjMessage.ObjOneofCase.MessageOfProp:

                isnullobj = propManager.PropMap(objMsg);
                if (isnullobj != null)  //actually at the start ,this is not need. but later without this will cause a bug
                {
                    x = (float)objMsg.MessageOfProp.Y / (float)1000;
                    y = (float)50 - (float)objMsg.MessageOfProp.X / (float)1000;
                    //var i = Instantiate(propManager.PropMap(objMsg));
                    var i = Instantiate(isnullobj, new Vector3(x, y, 0), Quaternion.identity);
                    props.TryAdd(objGuid, i.GetComponent<PropScript>());
                    props[objGuid].Renew(objMsg.MessageOfProp);
                    Debug.Log("生成了道具");
                    break;
                }
                break;
            case GameObjMessage.ObjOneofCase.MessageOfCharacter:
                //Debug.Log("1");
                //Debug.Log(playerManager.Jkk());
                //Instantiate(playerManager.PlayerMap(objMsg));
                //Debug.Log("1.5");
                var j = Instantiate(playerManager.PlayerMap(objMsg));
                //playerManager.PlayerMap(objMsg);
                //Debug.Log("2");
                players.TryAdd(objGuid, j.GetComponent<PlayerScript>());
                players[objGuid].Renew(objMsg.MessageOfCharacter);
                Debug.Log("生成了角色");
                break;
            case GameObjMessage.ObjOneofCase.MessageOfBullet:
                isnullobj = bulletManager.BulletMap(objMsg);
                if (isnullobj != null)
                {
                    x = (float)objMsg.MessageOfBullet.Y / (float)1000;
                    y = (float)50 - (float)objMsg.MessageOfBullet.X / (float)1000;
                    var z = Instantiate(isnullobj, new Vector3(x, y, 0), Quaternion.identity);
                    //var z = Instantiate(bulletManager.BulletMap(objMsg));
                    bullets.TryAdd(objGuid, z.GetComponent<LB_BulletScript>());
                    bullets[objGuid].Renew(objMsg.MessageOfBullet);
                    Debug.Log("生成了子弹");
                    break;
                }
                break;
            case GameObjMessage.ObjOneofCase.MessageOfBombedBullet:
                bbobj = bombulletManager.BombedBulletMap(objMsg);
                if (bbobj != null)
                {
                    x = (float)objMsg.MessageOfBombedBullet.Y / (float)1000;
                    y = (float)50 - (float)objMsg.MessageOfBombedBullet.X / (float)1000;
                    //Debug.Log("playback");
                    var ii = Instantiate(bbobj, new Vector3(x, y, 0), Quaternion.identity);
                    bombullets.TryAdd(objGuid, ii.GetComponent<BombedBulletScript>());
                    bombullets[objGuid].Renew(objMsg.MessageOfBombedBullet);
                    Debug.Log("生成了爆炸");
                    break;
                }
                break;

            default: Debug.Log("Empty msg in the MyInstantiate function"); break;
        }
    }

}
