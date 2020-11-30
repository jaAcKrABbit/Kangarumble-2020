using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour{

    public GameObject camera;

    public List<Controller2D> target;
    public Vector2 focusAreaSize;

    private GameObject menuControllerObj;

    private GameObject countdownObj;
    public Vector2 countdownFocusPos;
    public float camSize = 11;
    public float camSizeNormal = 13;
    public float camSizeStart = 9;
    public float camSizeCountdown = 12;
    public float camSizeMax = 20f;
    private float camSizeDest = 11;
    public float camPanOutSpeed = 0.4f;

    public float verticalOffset;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public int targetIndex = 0;

    FocusArea focusArea;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityX;
    float smoothVelocityY;

    public float shakePosAmount = 0;
    public float shakeRotAmount = 0;

    public float minX = 22;
    public float maxX = 42;
    public float minY = 4.7f;
    public float maxY = 4.6f;

    public float originalMinX = 0;
    public float originalMaxX = 0;
    public float originalMinY = 0;
    public float originalMaxY = 0;

    public float screenWidth = 0;
    public float screenHeight = 0;
    public float screenRatio = 0;

    public bool caughtUp = true;
    public float caughtUpDistance = 10;
    public float distanceToFocus = 0;
    public bool caughtUpPipe = true;
    public bool canCatchUpPipe = true;
    bool lookAheadStopped;

    void Start() {
        focusArea = new FocusArea(target[targetIndex].collider.bounds, focusAreaSize);
        camera = GameObject.Find("Main Camera");
        countdownObj = GameObject.Find("321Rumble");
        camSize = camSizeStart;
        camSizeDest = camSizeCountdown;
        gameObject.GetComponent<Camera>().orthographicSize = camSize;
        menuControllerObj = GameObject.Find("MenuController");

        originalMinX = minX;
        originalMaxX = maxX;
        originalMinY = minY;
        originalMaxY = maxY;
    }

    void LateUpdate() {

        screenWidth = Screen.width;
        screenHeight = Screen.height;
        screenRatio = screenWidth / screenHeight;
        minX = originalMinX * (screenRatio / (16f / 9f));
        maxX = originalMaxX * ((16f / 9f) / screenRatio);
       
        focusArea.Update(target[targetIndex].collider.bounds);

        Vector2 focusPosition = focusArea.centre + (Vector2.up * verticalOffset);
        if (countdownObj.active) {
            focusPosition = countdownFocusPos;
            camSizeDest = camSizeCountdown;
            if (camSizeCountdown < camSizeMax) {
                camSizeCountdown += camPanOutSpeed * Time.deltaTime;
            }
        }
        else {
            camSizeDest = camSizeNormal; 
        }
        camSize = Damp(camSize, camSizeDest, 12f, Time.deltaTime);
        camSize = Mathf.Clamp(camSize, camSizeStart, camSizeMax);

        gameObject.GetComponent<Camera>().orthographicSize = camSize;

        focusPosition.x = Mathf.Clamp(focusPosition.x, minX, maxX);
        focusPosition.y = Mathf.Clamp(focusPosition.y, minY, maxY);

        // deleted useless part so far
        //if (focusArea.velocity.x != 0) {
        //    lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
        //    if (Mathf.Sign(target[targetIndex].playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target[targetIndex].playerInput.x != 0) {
        //        lookAheadStopped = false;
        //        targetLookAheadX = lookAheadDirX * lookAheadDstX;
        //    } else {
        //        if (!lookAheadStopped) {
        //            lookAheadStopped = true;
        //            targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 2f;
        //        }

        //    }
        //}

        //currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        //add smooth effects on X axes
        focusPosition.x = Mathf.SmoothDamp(transform.position.x, focusPosition.x, ref smoothVelocityX, verticalSmoothTime);
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);

        Vector2 camPosition = new Vector2(transform.position.x, transform.position.y);
        distanceToFocus =  Vector2.Distance(camPosition, focusPosition);
        caughtUp = (distanceToFocus <= caughtUpDistance);
        if (canCatchUpPipe && caughtUp) {
            caughtUpPipe = true;
        }



        //focusPosition.x += Random.Range(-5, 5);

        // screen shake for position & rotation
        if (menuControllerObj.GetComponent<Menus>().isPaused) {
            shakePosAmount = 0;
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else {
            if (shakePosAmount > 0) {
                shakePosAmount -= 11.0f * Time.deltaTime;
                float shakeX = Random.Range(-shakePosAmount, shakePosAmount);
                float shakeY = Random.Range(-shakePosAmount, shakePosAmount);
                focusPosition.x += shakeX;
                focusPosition.y += shakeY;
            }
            else {
                shakePosAmount = 0;
            }
            ShakeRot();
        }


        //focusPosition += Vector2.right * currentLookAheadX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }
    // change character before reupdating focus position
    void Update() {

        targetIndex = camera.GetComponent<TurnSwitch>().currentPlayerTurn;
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.centre, focusAreaSize);
    }

    void ShakeRot() {
        float shakeRot = 0;
        if (shakeRotAmount > 0) {
            shakeRotAmount -= 15.0f * Time.deltaTime;
            shakeRot = Random.Range(-shakeRotAmount, shakeRotAmount);
        }
        else {
            shakeRotAmount = 0;
        }
        gameObject.transform.eulerAngles = new Vector3(0, 0, shakeRot);
    }

    struct FocusArea {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top,bottom;

        public FocusArea(Bounds targetBounds, Vector2 size) {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;
            velocity = Vector2.zero;

            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds) {
            float shiftX = 0;
            if (targetBounds.min.x < left) {
                shiftX = targetBounds.min.x - left;
            }else if (targetBounds.max.x > right) {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if(targetBounds.min.y < bottom) {
                shiftY = targetBounds.min.y - bottom; 
            } else if (targetBounds.max.y > top) {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;

            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);

        }
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }

    public void ResetCatchUpPipe()
    {
        canCatchUpPipe = true;
    }
}