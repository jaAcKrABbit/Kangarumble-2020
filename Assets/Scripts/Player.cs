using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    public int playerIndex;
    public int score = 0;

    private bool controlGet = false;
    //public bool controllerInput = false;
    public bool myTurn = false;
    public bool imDead = false;
    public bool isSpring = false;
    public bool isInAir = false;
    public bool isFalling = false;
    public bool isRunning = false;
    public bool isIdle = true;
    public bool hitByAnotherPlayerInTheAir = false;
    public bool isStomping = false;
    public bool onMovingPlatform = false;
    public bool onOilFloor = false;
    public bool wallSliding = false;
    public bool canDoubleJump = false;
    public bool onBelt = false;
    public bool playerHitHorizontally = false;
    private bool ableToTriggerSound = false;

    private GameObject camera;
    private GameObject playerVarsObj;
    [HideInInspector]
    public GameObject movingPlatform;
    [HideInInspector]
    public GameObject playerBelow;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float springHeight;
    public float timeToJumpApex = .4f;
    public float bounceValue = 30;
    public float stompSpeed = 50;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float accelerationTimeGroundedOil = .4f;
    float moveSpeed = 15;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    public Vector3 velocity;
    float velocityXSmoothing;
    float velocityXSmoothingOil;

    Controller2D controller;

    public GameObject myAnimatorObj;
    private float animatorObjScale;

    public SoundManager soundmanager;

    public GameObject countdownObj;

    //[SerializeField]
    //private int[] playerOrder;

    private bool animationBlink = false;
    private bool canAnimationBlink = true;
    private bool animationEarFlick = false;
    private bool canAnimationEarFlick = true;

    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem quickStompFallParticle;
    public ParticleSystem quickStompSlamParticle;

    [SerializeField]
    private bool vibratedRoundStart = false;

    private GameObject gameEndObj;

    private Vector2 input;
    private int wallDirX;

    public GameObject playerInputObj;

    void Start() {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        camera = GameObject.Find("Main Camera");
        playerVarsObj = GameObject.Find("PlayerVars");
        countdownObj = GameObject.Find("321Rumble");
        gameEndObj = GameObject.Find("GameEnd");

        movingPlatform = null;
        playerBelow = null;
        animatorObjScale = myAnimatorObj.transform.localScale.x;
    }

    void Update() {

        isIdle = (!isRunning && !isInAir && !wallSliding);

        // update score in PlayerVars script
        playerVarsObj.GetComponent<PlayerVars>().playerScores[playerIndex] = score;

        myTurn = (gameObject == camera.GetComponent<TurnSwitch>().currentActivePlayer && !countdownObj.active);
        if (gameEndObj.GetComponent<GameEnd>().gameEnd) {
            myTurn = false;
        }

        if (GameObject.Find("MenuController").GetComponent<Menus>().isPaused) {
            input = new Vector2(0f, 0f);
        }

        if (myTurn) {
            if (!vibratedRoundStart)
            {
                //Vibration
                if (playerInputObj.GetComponent<PlayerInput>().currentControlScheme == "Gamepads")
                {
                    vibration();
                }
                vibratedRoundStart = true;
            }

            ableToTriggerSound = true;
            isRunning = (Mathf.Abs(input.x - 0)) > 0.1f;

            if (input.x < -0.1f) {
                myAnimatorObj.transform.localScale = new Vector3(-animatorObjScale, animatorObjScale, animatorObjScale);
            }
            if (input.x > 0.1f) {
                myAnimatorObj.transform.localScale = new Vector3(animatorObjScale, animatorObjScale, animatorObjScale);
            }
        }
        else {
            isRunning = false;
        }


        if (!myTurn) {
            vibratedRoundStart = false;
            input = Vector2.zero;
        }
        //Debug.Log("INput: " + input);
        wallDirX = (controller.collisions.left) ? -1 : 1;

        float targetVelocityX = input.x * moveSpeed;

        if (!onOilFloor)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        }
        else
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, 2 * targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGroundedOil : accelerationTimeAirborne);
        }

        velocity.x = Mathf.Clamp(velocity.x, -18f, 18f);

        wallSliding = false;

        //check if player is in the air
        onMovingPlatform = false;
        movingPlatform = null;
        playerBelow = null;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
            wallSliding = true;
            // wall jump optimization 
            if (velocity.y < -wallSlideSpeedMax && controller.collisions.left) {
                if (input.x <= -0.75f && myTurn)
                    velocity.y = -wallSlideSpeedMax;
            }

            if (velocity.y < -wallSlideSpeedMax && controller.collisions.right) {
                if (input.x >= 0.75f && myTurn)
                    velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0) {
                velocityXSmoothing = 0;
                velocity.x = 0;
                if (Mathf.Abs(input.x - wallDirX) >= 0.25f && input.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else {
                timeToWallUnstick = wallStickTime;
            }
        }

        isInAir = Mathf.Abs(velocity.y) > 0 ? true : false;
        isFalling = velocity.y < -2 ? true : false;

        //stepped on spring
        if (isSpring) {
            velocity.y += springHeight;
            isSpring = false;
            canDoubleJump = true;
        }

        if (myTurn) {
            //INPUTS MOVED TO ACTION TRIGGER FUNCTIONS
        }

        //bounce
        if (hitByAnotherPlayerInTheAir) {
            velocity.y += bounceValue;
            hitByAnotherPlayerInTheAir = false;
        }

        velocity.y += gravity * Time.deltaTime;
        if ((camera.GetComponent<CameraFollow>().caughtUpPipe) || !myTurn) {
            controller.Move(velocity * Time.deltaTime, input);
        }


        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }


        AnimationController();
        if (onBelt)
        {
            soundmanager.onBelt = true;
        }
        else if (!onBelt && ableToTriggerSound)
        {
            ableToTriggerSound = false;
            soundmanager.onBelt = false;
        }
        if (countdownObj.active) {
            soundmanager.onBelt = false;
        }

        if (!isInAir && quickStompFallParticle.isPlaying) {
            quickStompFallParticle.Stop();
        }
    }

    public void AnimationController()
    {
        // set animations for AnimationController
        myAnimatorObj.GetComponent<Animator>().SetBool("isRunning", isRunning);
        myAnimatorObj.GetComponent<Animator>().SetBool("isInAir", isInAir);
        myAnimatorObj.GetComponent<Animator>().SetBool("isWallSliding", wallSliding);
        myAnimatorObj.GetComponent<Animator>().SetBool("isFalling", isFalling);
        myAnimatorObj.GetComponent<Animator>().SetBool("yVelocityZero", (velocity.y == 0));


        // blink animation
        animationBlink = false;
        if (canAnimationBlink && isIdle && !animationBlink && Random.Range(1, 80) == 1) {
            animationBlink = true;
            canAnimationBlink = false;
            Invoke("animationBlinkReset", 1.5f);
        }
        myAnimatorObj.GetComponent<Animator>().SetBool("blink", animationBlink);

        // ear flick animation
        animationEarFlick = false;
        if (canAnimationEarFlick && isIdle && !animationEarFlick && Random.Range(1, 80) == 1) {
            animationEarFlick = true;
            canAnimationEarFlick = false;
            Invoke("animationEarFlickReset", 1.5f);
        }
        myAnimatorObj.GetComponent<Animator>().SetBool("earFlick", animationEarFlick);


        // set position for wallJumpParticle
        float wallJumpParticleX = (myAnimatorObj.transform.localScale.x < 0) ? -0.75f : 0.75f;
        wallJumpParticle.transform.position = new Vector3(
            gameObject.transform.position.x + wallJumpParticleX,
            gameObject.transform.position.y + 1f,
            gameObject.transform.position.z - 10f);
    }

    public void animationBlinkReset()
    {
        canAnimationBlink = true;
    }

    public void animationEarFlickReset()
    {
        canAnimationEarFlick = true;
    }

    public void ShakeScore()
    {
        GameObject scoreUIObj = GameObject.Find("UI");
        GameObject timerUIObj = GameObject.Find("TimerUI");

        if (scoreUIObj != null) {
            for (int i = 0; i < 4; i++) {
                scoreUIObj.GetComponent<ScoreUI>().scoreUIShake[i] = 17;
            }
            scoreUIObj.GetComponent<ScoreUI>().scoreUIShake[playerIndex] = 22;
        }
        if (timerUIObj != null) {
            timerUIObj.GetComponent<TimerUI>().textShake = 17;
        }
    }

    public void PlayJumpParticle()
    {
        jumpParticle.Play();
    }
    public void PlayWallJumpParticle()
    {
        wallJumpParticle.Play();
    }
    public void PlayQuickStompFallParticle()
    {
        quickStompFallParticle.Play();
    }
    public void PlayQuickStompSlamParticle()
    {
        quickStompSlamParticle.Play();
    }

    //New Input system Movement Input
    public void Move(Vector2 data)
    {
        //Debug.Log(data);
        input = data;
    }

    //New Input system JumpKeyDown
    public void JumpKeyDown()
    {
        //Debug.Log($"JumpKeyDown triggered on player {playerIndex}");
        if (myTurn)
        {
            if (wallSliding)
            {
                //Wall climbing
                if (Mathf.Abs(input.x - wallDirX) <= 0.25f || Mathf.Abs(input.x - wallDirX) <= 0.95f /*&& input.y >= -0.25f*/)
                {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                //neutral jump off
                else if ((Mathf.Abs(input.x - wallDirX) <= 0.95f && Mathf.Abs(input.x - wallDirX) > 0.25f /*&& input.y < -0.25f*/) ||
                    Mathf.Abs(input.x - wallDirX) > 0.95f && Mathf.Abs(input.x - wallDirX) < 1f)
                {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else if (Mathf.Abs(input.x - wallDirX) >= 1f)
                {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                    //Debug.Log(wallDirX + "   " + input.x + "   " + inputController.x);
                }
                soundmanager.PlayWallJumpSound();
                PlayWallJumpParticle();
            }
            if (controller.collisions.below)
            {
                velocity.y = maxJumpVelocity;
                canDoubleJump = true;
                soundmanager.PlayJumpSound();
                PlayJumpParticle();
            }
            if (isInAir && canDoubleJump && !wallSliding)
            {
                canDoubleJump = false;
                velocity.y = maxJumpVelocity;
                PlayJumpParticle();
            }
        }
    }

    //New Input system JumpKeyUp
    public void JumpKeyUp()
    {
        if (myTurn)
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
            //if (velocity.y > minJumpVelocity) {
            //	velocity.y = minJumpVelocity;
            //}
        }
    }

    //New Input system StompKeyDown
    public void StompKeyDown()
    {
        if (myTurn)
        {
            if (isInAir && !wallSliding)
            {
                velocity.y -= stompSpeed;
                isStomping = true;
                PlayQuickStompFallParticle();
            }
        }
    }

    //Retrieve gameobject and playerinput data for vibration
    public void RetrieveObject(GameObject gameObject)
    {
        playerInputObj = gameObject;
    }

    //Vibration
    IEnumerator StopVibration(Gamepad gamepad, float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        gamepad.SetMotorSpeeds(0f, 0f);
    }
    public void vibration()
    {
        Gamepad gamepad = (Gamepad)playerInputObj.GetComponent<PlayerInput>().devices[0];
        gamepad.SetMotorSpeeds(0.5f, 0.5f);
        StartCoroutine(StopVibration(gamepad, 0.3f));
    }
}