// Todo:�����ƶ������ж��Ż�
//last update:2022/3/12


using Communication.Proto;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using Playback;
using System.Collections.Concurrent;
using static Communication.Proto.MessageToClient.Types;  //later find that this can simplify the code
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayBackComponent : MonoBehaviour
{

    MessageReader messageReader = new MessageReader(RecordPLName.fileName + ".thuaipb");
    public MessageToClient msg;
    GameObjMessage gemObjMsg;
    private bool isGameStarted;
    public GameObject test;

    private ConcurrentDictionary<long, bool> isPlayersExisted;
    private ConcurrentDictionary<long, bool> isBulletsExisted; 
    private ConcurrentDictionary<long, bool> isPropsExisted;
    private ConcurrentDictionary<long, bool> isBombedBulletsExisted;
    private ConcurrentQueue<KeyValuePair<long, GameObjMessage>> TaskQueue;

    public ConcurrentDictionary<long, PlayerScript> players;
    public static ConcurrentDictionary<long, BulletScript> bullets;
    public ConcurrentDictionary<long, PropScript> props;
    public ConcurrentDictionary<long, BombedBulletScript> bombullets;

    private PropManager propManager;
    private PlayerManager playerManager;
    private BulletManager bulletManager;
    private BombedBulletManager bombulletManager;

    public static int[,] map;
    public GameObject[] mapSquare;

    private float startTime;

    List<long> liveList = new List<long>();
    List<long> allPlayer = new List<long>();

    private Vector3 xscale;

    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 10;

        Vector3 oneMapVec;
        oneMapVec.x = 0;
        oneMapVec.y = 50;
        oneMapVec.z = -1;
        //Instantiate(mapSquare[0], oneMapVec, Quaternion.identity);

                //Instantiate(test, new Vector3(0, 0, 20), Quaternion.identity);
                //bullet = Instantiate(myPrefab, transform.position, transform.rotation)
                //һЩ��ʼ������
                //MessageReader messageReader = new MessageReader("video.thuaipb");   //��ȡ�ط��ļ�
                gemObjMsg = new GameObjMessage();

        startTime =0;

        propManager = GetComponent<PropManager>();
        playerManager = GetComponent<PlayerManager>();
        bulletManager = GetComponent<BulletManager>();
        bombulletManager = GetComponent<BombedBulletManager>();

        TaskQueue = new ConcurrentQueue<KeyValuePair<long, GameObjMessage>>();

        players = new ConcurrentDictionary<long, PlayerScript>();
        bullets = new ConcurrentDictionary<long, BulletScript>();
        props = new ConcurrentDictionary<long, PropScript>();
        bombullets = new ConcurrentDictionary<long, BombedBulletScript>();

        isPlayersExisted = new ConcurrentDictionary<long, bool>();
        isBulletsExisted = new ConcurrentDictionary<long, bool>();
        isPropsExisted = new ConcurrentDictionary<long, bool>();
        isBombedBulletsExisted = new ConcurrentDictionary<long, bool>();

        map = new int[50, 50];  //save the map information

        //---------------------------------
        msg = messageReader.ReadOne();
        if (msg == null)  //if msg is null
        {
            Environment.Exit(0);
            Debug.Log("empty!!!");
        }
        else
        {
            Debug.Log("messageReader.ReadOne() not empty");
            switch (msg.MessageType)
            {
                case MessageType.StartGame:
                    GameStart(msg); break;
                case MessageType.Gaming:
                    Refresh(msg); break;
                case MessageType.EndGame:
                    GameOver(msg); break;
                default: break;
            }
        }

    }



    // Update is called once per frame
    //ÿ��ˢ�µ�ʱ��FlushTaskQueue(��ͼ��һ��
    void Update()
    {
        if (startTime<1)
            startTime += Time.deltaTime;
        if (!TaskQueue.IsEmpty)
        {
            FlushTaskQueue();
        }    
    }
    
    void FixedUpdate()  //�̶�ʱ���ȡһ���µ���Ϣ
    {
        msg = messageReader.ReadOne();
        if (msg == null)  //if msg is null
        {
            Environment.Exit(0);
        }
        else
        {
            switch (msg.MessageType)
            {
                case MessageType.StartGame:
                    GameStart(msg); break;
                case MessageType.Gaming:
                    Refresh(msg); break;
                case MessageType.EndGame:
                    //TO DO:��ʾ�������棬ͬʱ��ʾ�����ĵ÷�
                    GameOver(msg);
                    Debug.Log("�˳���Ϸ");
                    SceneManager.LoadScene(2);  //�����л�һ������
                    //Invoke(nameof(endGame), 2);                    
                    /*
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #else
                        Application.Quit();
                    #endif
                    */
                    // above five lines of code word well to close the whole game
                    break;
                default: break;
            }
        }
    }
    private void endGame()
    {
        Debug.Log("�˳���Ϸ");
        SceneManager.LoadScene(2);  //�����л�һ������
    }
    
    private void Refresh(MessageToClient message)
    {
        liveList.Clear();
        foreach (var obj in msg.GameObjMessage)   //dependon type of msg for diffthings
        {
            switch (obj.ObjCase)
            {
                case GameObjMessage.ObjOneofCase.None:  //����Ϣ
                    Debug.Log("Empty message");
                    break;

                case GameObjMessage.ObjOneofCase.MessageOfCharacter: //������Ϣ
                    if (isPlayersExisted.ContainsKey(obj.MessageOfCharacter.Guid) && (isPlayersExisted[obj.MessageOfCharacter.Guid] == true) && (players.ContainsKey(obj.MessageOfCharacter.Guid)))
                    {
                        if ((obj.MessageOfCharacter.IsResetting==true))
                        {
                            players[obj.MessageOfCharacter.Guid].dead();
                        }
                        else //if (obj.MessageOfCharacter.IsResetting == false)  //����
                        {
                            if (players[obj.MessageOfCharacter.Guid].isDead == true)
                                players[obj.MessageOfCharacter.Guid].recover();                            
                        }
                        players[obj.MessageOfCharacter.Guid].Renew(obj.MessageOfCharacter); //���ھ͸�������
                        //liveList.Add(obj.MessageOfCharacter.Guid);  //��¼���µ������guid
                        //Debug.Log("liveListnum:");
                        //Debug.Log(liveList.Count);
                    }
                    else if (!isPlayersExisted.ContainsKey(obj.MessageOfCharacter.Guid))
                    {
                        isPlayersExisted.TryAdd(obj.MessageOfCharacter.Guid, true);
                        TaskQueue.Enqueue(new KeyValuePair<long, GameObjMessage>(obj.MessageOfCharacter.Guid, obj)); //�����ھͱ�ǲ�����������ͬ��
                    }
                    break;

                case GameObjMessage.ObjOneofCase.MessageOfBullet: //�ӵ���Ϣ                    
                    if (bullets.ContainsKey(obj.MessageOfBullet.Guid) && (isBulletsExisted[obj.MessageOfBullet.Guid] == true) && (isBulletsExisted.ContainsKey(obj.MessageOfBullet.Guid)))
                    {
                        bullets[obj.MessageOfBullet.Guid].Renew(obj.MessageOfBullet); //���ھ͸�������
                    }
                    else if (!isBulletsExisted.ContainsKey(obj.MessageOfBullet.Guid))
                    {
                        isBulletsExisted.TryAdd(obj.MessageOfBullet.Guid, true);
                        TaskQueue.Enqueue(new KeyValuePair<long, GameObjMessage>(obj.MessageOfBullet.Guid, obj)); //�����ھͱ�ǲ�����
                    }

                    break;

                case GameObjMessage.ObjOneofCase.MessageOfProp: //������Ϣ

                    if (props.ContainsKey(obj.MessageOfProp.Guid) && (isPropsExisted.ContainsKey(obj.MessageOfProp.Guid)) && (isPropsExisted[obj.MessageOfProp.Guid] == true))
                    {
                        props[obj.MessageOfProp.Guid].Renew(obj.MessageOfProp); //���ھ͸�������
                    }
                    else if (!isPropsExisted.ContainsKey(obj.MessageOfProp.Guid))
                    {
                        isPropsExisted.TryAdd(obj.MessageOfProp.Guid, true);
                        TaskQueue.Enqueue(new KeyValuePair<long, GameObjMessage>(obj.MessageOfProp.Guid, obj)); //�����ھͱ�ǲ�����
                    }
                    else if(isPropsExisted.ContainsKey(obj.MessageOfProp.Guid)&&(isPropsExisted[obj.MessageOfProp.Guid] == false))
                    {
                        isPropsExisted[obj.MessageOfProp.Guid] = true;
                        props[obj.MessageOfProp.Guid].Renew(obj.MessageOfProp); //���ھ͸�������
                        props[obj.MessageOfProp.Guid].unPicked();  //�������״̬
                    }       
                    break;

                case GameObjMessage.ObjOneofCase.MessageOfBombedBullet:
                        if (isBulletsExisted.ContainsKey(obj.MessageOfBombedBullet.MappingID) &&isBulletsExisted[obj.MessageOfBombedBullet.MappingID] == true)
                        {
                            bullets[obj.MessageOfBombedBullet.MappingID].die();
                            isBulletsExisted[obj.MessageOfBombedBullet.MappingID] = false;
                        }                          
                        
                        isBombedBulletsExisted.TryAdd(obj.MessageOfBombedBullet.MappingID, true);
                        TaskQueue.Enqueue(new KeyValuePair<long, GameObjMessage>(obj.MessageOfBombedBullet.MappingID, obj)); //�����ھͱ�ǲ�����                   
                    break;

                case GameObjMessage.ObjOneofCase.MessageOfMap:
                    //Debug.Log("mapMsg");
                    //obj.MessageOfMap.Row[1].Col[2].
                    //the map has been initialized
                    break;

                case GameObjMessage.ObjOneofCase.MessageOfPickedProp:
                    //Debug.Log("Should Pick");                    
                    isPropsExisted[obj.MessageOfPickedProp.MappingID] = false;
                    props[obj.MessageOfPickedProp.MappingID].Picked();    //������ݻ�                    
                    break;


                default: Debug.Log("good luck to debug!"); break;
            }
        }
        /*
        if ((liveList.Count!=allPlayer.Count)&&(startTime>0.5))  //someone died
        {
            //Debug.Log(liveList.Count);
            //Debug.Log(allPlayer.Count);
            foreach (long temp in allPlayer)
            { 
                if (!liveList.Contains(temp))  //dead
                {
                    //Debug.Log(temp);
                    players[temp].dead();
                }
            }
        }
        */
    }

    private void GameStart(MessageToClient message)  //��Ϸ��ʼ����Ϣ
    {
        //���ݳ�ʼ����Ϣ��ʼ��������ɫ(((ΪʲôҪ��������?
        foreach (var obj in msg.GameObjMessage)
        {
            if(obj.ObjCase == GameObjMessage.ObjOneofCase.MessageOfMap)
            {
                Debug.Log("mapmap!!!");
                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        map[i, j] = obj.MessageOfMap.Row[i].Col[j];
                    }
                }
                Debug.Log("finish the map information saving!");
                DrawMap();
            }
            else  if (obj.ObjCase == GameObjMessage.ObjOneofCase.MessageOfCharacter)
            {
                if (isPlayersExisted.ContainsKey(obj.MessageOfCharacter.Guid)&&(players.ContainsKey(obj.MessageOfCharacter.Guid)))
                {
                    players[obj.MessageOfCharacter.Guid].Renew(obj.MessageOfCharacter); //���ھ͸�������
                }
                else if (!players.ContainsKey(obj.MessageOfCharacter.Guid))
                {
                    allPlayer.Add(obj.MessageOfCharacter.Guid);   //�����е������guid����¼����
                    isPlayersExisted.TryAdd(obj.MessageOfCharacter.Guid, true);
                    TaskQueue.Enqueue(new KeyValuePair<long, GameObjMessage>(obj.MessageOfCharacter.Guid, obj)); //�����ھͱ�ǲ�����������ͬ��
                }
            }
        }
        isGameStarted = true;
    }

    private void GameOver(MessageToClient message)
    {
        foreach (var obj in msg.GameObjMessage)
        {
            if (obj.ObjCase == GameObjMessage.ObjOneofCase.MessageOfCharacter)
            {
                if (isPlayersExisted.ContainsKey(obj.MessageOfCharacter.Guid) && (players.ContainsKey(obj.MessageOfCharacter.Guid)))
                {
                    players[obj.MessageOfCharacter.Guid].Renew(obj.MessageOfCharacter); //���ھ͸�������
                }
                else if (!players.ContainsKey(obj.MessageOfCharacter.Guid))
                {
                    allPlayer.Add(obj.MessageOfCharacter.Guid);   //�����е������guid����¼����
                    isPlayersExisted.TryAdd(obj.MessageOfCharacter.Guid, true);
                    TaskQueue.Enqueue(new KeyValuePair<long, GameObjMessage>(obj.MessageOfCharacter.Guid, obj)); //�����ھͱ�ǲ�����������ͬ��
                }
            }
        }
        //Thread.Sleep(2000);        
    }

    private void FlushTaskQueue()
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
    private void DrawMap()
    {
        Vector3 oneMapVec;
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                oneMapVec.x =  (float)j + 0.5f;
                oneMapVec.y = (float)50 - (float)i - 0.5f;
                oneMapVec.z = -1;
                switch (map[i, j])
                {
                    case 1:
                        Instantiate(mapSquare[0],oneMapVec, Quaternion.identity);
                        break;
                    case 2:
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

    private void MyInstantiate(long objGuid, GameObjMessage objMsg)
    {
        GameObject isnullobj,bbobj;
        float x, y;
        Vector2 posi;
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
                    Debug.Log("�����˵���");
                    break;
                }                
                break;
            case GameObjMessage.ObjOneofCase.MessageOfCharacter:
                //Debug.Log("1");
                //Debug.Log(playerManager.Jkk());
                //Instantiate(playerManager.PlayerMap(objMsg));
                //Debug.Log("1.5");
                isnullobj = playerManager.PlayerMap(objMsg);
                if (isnullobj != null)
                {
                    var j = Instantiate(isnullobj);
                    //playerManager.PlayerMap(objMsg);
                    //Debug.Log("2");
                    players.TryAdd(objGuid, j.GetComponent<PlayerScript>());
                    players[objGuid].Renew(objMsg.MessageOfCharacter);
                    Debug.Log("�����˽�ɫ");
                    break;
                }               
                break;
            case GameObjMessage.ObjOneofCase.MessageOfBullet:
                isnullobj = bulletManager.BulletMap(objMsg);
                if (isnullobj != null)
                {
                    x = (float)objMsg.MessageOfBullet.Y / (float)1000;
                    y = (float)50 - (float)objMsg.MessageOfBullet.X / (float)1000;
                    var z = Instantiate(isnullobj, new Vector3(x, y, 0), Quaternion.identity);
                    //var z = Instantiate(bulletManager.BulletMap(objMsg));
                    bullets.TryAdd(objGuid, z.GetComponent<BulletScript>());
                    bullets[objGuid].Renew(objMsg.MessageOfBullet);
                    Debug.Log("�������ӵ�");
                    break;
                }
                break;
            case GameObjMessage.ObjOneofCase.MessageOfBombedBullet:
                //yeah!!! Finally de all the bugs of bomedbullets
                bbobj = bombulletManager.BombedBulletMap(objMsg);
                if (bbobj != null)
                {
                    Debug.Log("�����˱�ը");
                    if (objMsg.MessageOfBombedBullet.Type!= BulletType.LineBullet)
                    {
                        x = (float)objMsg.MessageOfBombedBullet.Y / (float)1000;
                        y = (float)50 - (float)objMsg.MessageOfBombedBullet.X / (float)1000;
                        Vector2 kk = new Vector2(x, y);           
                    }
                    else
                    {
                        Debug.Log(objMsg.MessageOfBombedBullet.FacingDirection);
                        x = (float)objMsg.MessageOfBombedBullet.Y / (float)1000 +(float)3*(float)Mathf.Sin((float)objMsg.MessageOfBombedBullet.FacingDirection);
                        y = (float)50 - (float)objMsg.MessageOfBombedBullet.X / (float)1000 - (float)3 * (float)Mathf.Cos((float)objMsg.MessageOfBombedBullet.FacingDirection);
                        Vector2 kk = new Vector2(x, y);
                    }
                    
                    //Debug.Log(kk);

                    //Debug.Log("playback");
                    var ii = Instantiate(bbobj, new Vector3(x, y, 0), Quaternion.identity);
                    
                    xscale = ii.transform.localScale;
                    
                    switch (objMsg.MessageOfBombedBullet.Type)
                    {
                        case BulletType.AtomBomb:
                            xscale = xscale * (float)objMsg.MessageOfBombedBullet.BombRange / (float)7500;
                            break;
                        case BulletType.CommonBullet2:
                            xscale = xscale * (float)objMsg.MessageOfBombedBullet.BombRange / (float)2500;
                            break;
                        case BulletType.FastBullet:
                            xscale = xscale * (float)objMsg.MessageOfBombedBullet.BombRange / (float)1500;
                            break;
                        case BulletType.NullBulletType:
                            xscale = xscale * (float)objMsg.MessageOfBombedBullet.BombRange / (float)2500;
                            break;
                        case BulletType.OrdinaryBullet:
                            xscale = xscale * (float)objMsg.MessageOfBombedBullet.BombRange / (float)2500;
                            break;
                        case BulletType.LineBullet:
                            xscale.y= (float)objMsg.MessageOfBombedBullet.BombRange/(float)2500;
                            break;

                        default:                            
                            break;
                    }
                    ii.transform.localScale = xscale;
                    //Thread.Sleep(5);
                    bombullets.TryAdd(objGuid, ii.GetComponent<BombedBulletScript>());
                    // the following two lines once was a bug.
                    // but I use a gruff method to fix it.
                    if (bombullets[objGuid]!=null)
                    {
                        bombullets[objGuid].Renew(objMsg.MessageOfBombedBullet);  
                    } 
                    break;
                }
                break;

            default: Debug.Log("Empty msg in the MyInstantiate function"); break;
        }
    }

}
