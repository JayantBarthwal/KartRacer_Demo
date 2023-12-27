using PathCreation;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class KartController : MonoBehaviour
{
    [Header("Float")]
    public float maxSpeed = 20f;
    [HideInInspector] public float h, v;
    public float acceleration=1f;
    public float deAcceleration = 1f;
    public float handling = 1f;
    public float localX = 0;
    public float clampX = 3.8f;
    public float tiltZ = 5;
    [HideInInspector] public float targetSpeed, crrSpeed;
    [HideInInspector] public float extraSpeed;
    [HideInInspector] public float boosterForce;
    [HideInInspector] public float extraHandeling;
    public float finalSpeed;
    float distanceTravelled;

    [Header("Gameobjects")]
    public Transform carPivot;
    public Transform player;
    public GameObject finishCam;
    public GameObject RaceOverCanvas;

    [Header("OtherVariables")]
    public bool mobileControl = false;
    public int totalLaps = 1;
    public Vector3 rotOffset;
    public bool raceOver = false;
    public bool isBot = false;

    [Header("Extra")]
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    [HideInInspector] public KartBody kb;
    [HideInInspector] public int crrLap = 0;

    void Awake()
    {
        kb = GetComponentInChildren<KartBody>();
    }
  
    void Update()
    {
        if (!GameManager.instance.GameStarted) return;
       
        if(!raceOver&&!isBot)GetInputs();
        ManageHandling();
        ManageForwardMovement();
    }
  
    void GetInputs() 
    {
        if (!mobileControl)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }
        else {
            if (SystemInfo.supportsAccelerometer)
            {
                Vector3 acceleration = Input.acceleration;
                h = acceleration.x * 2;
            }
        }
    }
    void ManageHandling() {
        //left right
        float crrVel = crrSpeed / maxSpeed;
        localX += h * handling *crrVel  * Time.deltaTime;
        localX += extraHandeling *crrVel * Time.deltaTime;//extra handeling is only applied during curves 

        localX = Mathf.Clamp(localX, -clampX, clampX);

        carPivot.transform.localPosition = Vector3.Lerp(carPivot.transform.localPosition, new Vector3(localX, 0, 0), Time.deltaTime * 10f);//actual body movement

        //little tilt animation for kart
        float tiltVal = h * v * tiltZ;
        carPivot.transform.localRotation = Quaternion.Slerp(carPivot.transform.localRotation, Quaternion.Euler(0f, tiltVal, -tiltVal), Time.deltaTime * 5f);
        player.transform.localRotation = Quaternion.Slerp(carPivot.transform.localRotation, Quaternion.Euler(0f, 0f,-tiltVal), Time.deltaTime * 5f);


    }

    void ManageForwardMovement() {
        if (pathCreator != null)
        {
            targetSpeed = maxSpeed * v;
            if (v>0)
                crrSpeed = Mathf.Lerp(crrSpeed, targetSpeed, acceleration * Time.deltaTime);
            else
                crrSpeed = Mathf.Lerp(crrSpeed, targetSpeed, deAcceleration * Time.deltaTime);

            finalSpeed = crrSpeed + extraSpeed + boosterForce;

            distanceTravelled += finalSpeed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            Quaternion rot = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

            //extra rotation for body according to spline
            Vector3 targetRotation = rot.eulerAngles + rotOffset;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * 8f);
        }
    }

    public void ScreenDown() {//User pressing screen
        if (!raceOver)
        {
            v = 1f;
        } 
    }
    public void ScreenUp() {//user released finger
        if (!raceOver)
        {
            v = 0;
        }
    }


    public void RaceOverFn(int pos) {
        raceOver = true;
        v = 0;
        finishCam.SetActive(true);
        RaceOverCanvas.SetActive(true);
        if (pos!=1)
        {
            RaceOverCanvas.transform.Find("YouWon").gameObject.SetActive(false);
            RaceOverCanvas.transform.Find("Position").gameObject.SetActive(true);
            RaceOverCanvas.transform.Find("Position").GetComponent<TextMeshProUGUI>().text ="You finished "+ pos;
        }
    }
}
