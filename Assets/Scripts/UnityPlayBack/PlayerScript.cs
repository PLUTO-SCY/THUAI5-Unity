/* Don't forget to delete the Unused library files 
 * Last change:2022_3_6
 * 目前的坐标映射：x/2500-10
 */

using Communication.Proto;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public int life;
    public bool isDead;

    //  nwo we have 27 attributes unbelievable!!!!!
    public float moveSpeed;
    public Vector2 position;
    public Vector2 lastposition;
    public Vector2 nowposition;
    private Animator _animator;
    private double attackRange;
    public int bulletNum; 
    private double timeUntilCommonSkillAvailable;
    private double timeUntilUltimateSkillAvailable = 8;
    public int gemNum = 0;                    //   ...这个是啥？？？
    private BuffType[] buff;                   //buff是一个数组了
    private PropType prop;
    private PlaceType place;                   //人物所在位置
    private double vampire;                    //吸血率
    private BulletType bulletType;             //子弹类型
    private bool isResetting;                  //是否在复活中
    private PassiveSkillType PassiveSkillType; //被动技能
    ActiveSkillType ActiveSkillType;//可见
    public long guid;     //操作方法：Client和Server互相约定guid。非负整数中，1-8这8个guid预留给8个人物，其余在子弹或道具被创造/破坏时分发和回收。Client端用向量[guid]储存物体信息和对应的控件实例。
                         //0号guid存储单播模式中每人Client对应的GUID。
    private bool canMove;
    private int radius;  //这个是啥
    private int CD;
    private int lifeNum;		//第几次复活
    public int score;   //
    public long teamID;
    public long playerID;
    private bool isInvisible;

    //values that are Not attributes
    private float moveThreshold = 5; //the basic gap that determines whether to do move animation
    private bool isfirstUpdate;
    private int buffNum;             //buff成为数组，记录同时有几个buff在身
    Vector2 diffVec2;
    int i;

    private GameObject _player;
    private GameObject _EnergyBall;
    //public GameObject jamSignal;

    private SpriteRenderer _spriteRenderer1;
    private SpriteRenderer _spriteRenderer2;
    private SpriteRenderer _spriteRenderer3;

    public GameObject addLifebuff = null;
    public GameObject moveSpeedbuff = null;
    public GameObject Shieldbuff = null;
    public GameObject Spearbuff = null;

    private bool isAddLife;
    private bool isMoveSpeed;
    private bool isShield;
    private bool isSpear;

    private GameObject _HP;
    private GameObject _HPValue;
    private GameObject _ID;
    private Slider _HPSlider;
    private TMP_Text idTmpro;

    private int lifeMax;

    public GameObject AddSpeed0 = null;
    public GameObject AddSpeed1 = null;
    public GameObject AddSpeed2 = null;

    private float addSpeedTime;
    private bool ifAddSpeed;

    // Start is called before the first frame update
    void Start()
    {
        //do some initialization
        moveSpeed = 0.0f;
        position.x = 2.0f;
        position.x = 2.0f;
        lastposition = position;

        life = 20;
        buff = new BuffType[6];              // I set it casually
        prop = PropType.AddLife;
        place = PlaceType.Land;
        vampire = 0.0f;
        score = 0;
        isDead = false;

        isInvisible = false;    
        isfirstUpdate =true;
        //animator = GetComponent<Animator>();
        //animator.SetBool("isMoving", false); // havn't been set

        _player = transform.GetChild(0).gameObject;
        _EnergyBall = transform.GetChild(1).gameObject;

        _animator = _player.GetComponent<Animator>();

        _spriteRenderer1 = GetComponent<SpriteRenderer>();
        _spriteRenderer2 = _player.GetComponent<SpriteRenderer>();
        _spriteRenderer3 = _EnergyBall.GetComponent<SpriteRenderer>();

        _HP = transform.GetChild(2).gameObject;
        _HPValue = _HP.transform.GetChild(0).gameObject;
        _ID = _HP.transform.GetChild(1).gameObject;
        idTmpro = _ID.GetComponent<TMP_Text>();       

        _HPSlider = _HPValue.GetComponent<Slider>();
        _HPSlider.value = 0;

        addSpeedTime = 0;
        ifAddSpeed = false;
    }

    /* Update is called once per frame
     * if player is alive , need to update the state
     * but the problem is the state is more complicated than last year
     * because the hero need to move and attack and do other things
     * no!!!
     */

    void Update()
    {
        if (moveSpeed>500)
        {
            if (ifAddSpeed==false)  //开始计时间
            {
                ifAddSpeed = true;
                GameObject as0 = Instantiate(AddSpeed0,this.transform.position, Quaternion.identity);
                GameObject as1 = Instantiate(AddSpeed1, this.transform.position, Quaternion.identity);
                GameObject as2 = Instantiate(AddSpeed2, this.transform.position, Quaternion.identity);
                as0.transform.parent = transform;
                as1.transform.parent = transform;
                as2.transform.parent = transform;
                addSpeedTime += Time.deltaTime;
            }
            else addSpeedTime += Time.deltaTime;
        }
        else 
        {
            if (addSpeedTime>1.2)
            {
                addSpeedTime = 0;
                ifAddSpeed = false;
                foreach (Transform child in transform)
                {
                    if (child.gameObject.tag == "AddSpeed")
                        Destroy(child.gameObject);
                }
            }
            else addSpeedTime += Time.deltaTime;
        }

        Move();

        /*  //双倍人数Bug已修复
        if ((transform.position.x>58)|(transform.position.y > 58))
        {
            Destroy(this.gameObject);
        }
        */
        //if (life>0 || isResetting == true)

        /*
        if (canMove == true)
        {
            Move();
            //animator.SetBool("Right_Run", isMoving);
            //other things
        }
        */
        /*
        else
        {
            //死亡的位置
            Vector2 diePos = transform.position;
            diePos.x = -1000f;
            diePos.y = -1000f;
            transform.position = diePos;
        }*/
        /*
        nowposition.x = transform.position.x;
        nowposition.y = transform.position.y;
        if (nowposition!=lastposition)
        {
            Debug.Log("true;");
            animator.SetBool("Right_Run", true);
            lastposition = nowposition;
        }
        else
        {
            Debug.Log("false;");
            animator.SetBool("Right_Run", false);
            lastposition = nowposition;
        }*/

        _EnergyBall.transform.RotateAround(transform.position, Vector3.up, 150 * Time.deltaTime);
        _EnergyBall.transform.Rotate(0, -150 * Time.deltaTime, 0);
        /*
        //控制草丛隐身效果
        if (!isInvisible)
        {
            //GameObject _jamSignal = Instantiate(jamSignal, transform);
            //GameObject _recoverAfterBattle = Instantiate(RecoverAfterBattle, transform);
            _spriteRenderer1.color = new Color(1, 1, 1, 0f);
            _spriteRenderer2.color = new Color(1, 1, 1, 0.5f);
            _spriteRenderer3.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            _spriteRenderer1.color = new Color(1, 1, 1, 1);
            _spriteRenderer2.color = new Color(1, 1, 1, 1);
            _spriteRenderer3.color = new Color(1, 1, 1, 1);
        }
        */
        _HPSlider.value = (float)life / (float)lifeMax;       

    }

    public void FixedUpdate()
    {   _EnergyBall.transform.RotateAround(transform.position, Vector3.up, 100 * Time.deltaTime);
        _EnergyBall.transform.Rotate(0, -100 * Time.deltaTime, 0);
    }


    void Move()
    {
        //transform.position = position;

        Vector2 diffVec;  //Calculate the difference between vectors
        diffVec.x = position.x - transform.position.x;
        diffVec.y = position.y - transform.position.y;
        
        if (diffVec.magnitude > moveThreshold)
        {
            transform.position = position;  //vec3 = vec2 actually
        }
        
        else
        {
            //use Mathf.Lerp  named"线性差值"
            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, position.x, moveSpeed * Time.deltaTime);
            pos.y = Mathf.Lerp(pos.y, position.y, moveSpeed * Time.deltaTime);
            transform.position = pos;
            //animator.SetBool("isMoving", true);
        }       
        
    }

    public void dead()
    {
        //Debug.Log("Picked");
        //Destroy(this.gameObject); //
        gameObject.SetActive(false);        
        if (!isDead)
        {
            isDead = true;
            Debug.Log("I dead");
        }
            
    }
    public void recover()
    {
        //Debug.Log("Picked");
        //Destroy(this.gameObject); //
        if (isDead)
        {
            gameObject.SetActive(true);
            Debug.Log("I live again.");
            isDead = false;
        }        
    }


    //this function is used to update the basic information
    //this is called in the 'PlayBack.cs'
    public void Renew(MessageOfCharacter msgOfCharacter)
    {
        if (isfirstUpdate)
        {
            //msgOfCharacter.
            CD = msgOfCharacter.CD;
            attackRange = msgOfCharacter.AttackRange;
            isfirstUpdate = false;
            isAddLife = false;
            isMoveSpeed = false;
            isShield = false;
            isSpear = false;

            playerID = msgOfCharacter.PlayerID;
            teamID = msgOfCharacter.TeamID;
            //Debug.Log("Renew");
            //Debug.Log(msgOfCharacter.TeamID);
            guid = msgOfCharacter.Guid;
            idTmpro.text = playerID.ToString();


            life = msgOfCharacter.Life;
            lifeMax = life;            
        }

        position.x = (float)msgOfCharacter.Y / (float)1000;
        position.y = (float)50 - (float)msgOfCharacter.X / (float)1000;

        life = msgOfCharacter.Life;
        //teamID = msgOfCharacter.TeamID;

        moveSpeed = msgOfCharacter.Speed;
        bulletNum = msgOfCharacter.BulletNum;
        
        lifeNum = msgOfCharacter.LifeNum;
        isInvisible = msgOfCharacter.IsInvisible;
        canMove = msgOfCharacter.CanMove;

        vampire = msgOfCharacter.Vampire;

        buffNum = msgOfCharacter.Buff.Count;

        score = msgOfCharacter.Score;

        gemNum = msgOfCharacter.GemNum;

        /*
        Debug.Log(buffNum);
        for (i=0;i<=buffNum;i++)
        {
            buff[i] = msgOfCharacter.Buff[i];   
        }*/

        var bufff = msgOfCharacter.Buff;
        /*
        foreach (var abuff in bufff)
        {
            GameObject a = null;
            switch (abuff)
            {
                case BuffType.AddLife:
                    if (!isAddLife)
                    {
                        a = Instantiate(addLifebuff, this.transform.position,Quaternion.identity);
                        a.transform.parent = this.transform;
                        isAddLife = true;
                    }                    
                    break;

                case BuffType.MoveSpeed:
                    if (!isMoveSpeed)
                    {
                        a = Instantiate(moveSpeedbuff, this.transform.position, Quaternion.identity);
                        a.transform.parent = this.transform;
                        isMoveSpeed = true;
                    }                    
                    break;

                case BuffType.NullBuffType:
                    break;

                case BuffType.ShieldBuff:
                    if (!isShield)
                    {
                        a = Instantiate(Shieldbuff, this.transform.position, Quaternion.identity);
                        a.transform.parent = this.transform;
                        isShield = true;
                    }        
                    break;

                case BuffType.SpearBuff:                    
                    if (!isSpear)
                    {
                        a = Instantiate(Spearbuff, this.transform.position, Quaternion.identity);
                        a.transform.parent = this.transform;
                        isSpear = true;
                    }         
                    break;
            }
        }
        */
        prop = msgOfCharacter.Prop;
        place = msgOfCharacter.Place;
    }
}


/*血条测试：
         * 
        if (guid != 0)
        {
            Debug.Log(guid);
            Debug.Log("guid-life");
            Debug.Log(life);
        }   
        if (guid != 0)
        { 
           Debug.Log("life:");
           Debug.Log(lifeMax);
           Debug.Log((float)life / (float)lifeMax);        
        }
        */