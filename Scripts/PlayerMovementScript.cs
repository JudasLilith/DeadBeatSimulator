using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementScript : MonoBehaviour
{
    public int playerMaxHits = 3;
    bool playerAlive = true;
    public float accelerationSpeed = 10;
    public float decellerationMultiplier = 2.25f;
    public float maxMovementSpeed = 50;
    public float actualMovementSpeed = 0;
    public float turnSpeed = 10;
    float curTurnSpeed = 0;
    public float maxTurnSpeed = 50;
    int angleUp = 0;
    int angleDown = 0;
    int angleLeft = 0;
    int angleRight = 0;
    Vector2 angleVec = Vector2.zero;
    Vector2 angleVecOld = Vector2.zero;
    public int moveDirection = 0;
    int dirUp = 0;
    int dirDown = 0;

    public Image healthIndicator;
    public GameObject smoke; //indicator for low health
    public Sprite hitOnce;
    public Sprite hitTwice;


    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void changeTurnSpeed()
    {
        if (moveDirection != 0)
        {
            curTurnSpeed = Mathf.Lerp(curTurnSpeed, maxTurnSpeed, turnSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            float wantedAngle = Mathf.Rad2Deg * Mathf.Atan2(angleVecOld.y, angleVecOld.x);
            float curAngle = Mathf.LerpAngle(transform.eulerAngles.z, wantedAngle, curTurnSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, 0, curAngle);
        }
        else
        {
            curTurnSpeed = Mathf.Lerp(curTurnSpeed, 0, decellerationMultiplier * turnSpeed * Time.deltaTime);
        }
    }

    public void takeDamage()
    {
        playerMaxHits -= 1;
        if (playerMaxHits == 2)
        {

        }
        if (playerMaxHits == 1)
        {

        }
        if (playerMaxHits <= 0)
        {
            playerAlive = false;
            //die
        }
    }

    void FixedUpdate()
    {
        if (playerAlive)
        {
            //Turning stuff
            angleVec = new Vector2(angleLeft + angleRight, angleUp + angleDown);
            if (Input.GetKey(KeyCode.UpArrow))
            {
                angleUp = 1;
                angleVecOld = new Vector2(angleVec.x, angleVec.y);
            }
            else
            {
                angleUp = 0;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                angleDown = -1;
                angleVecOld = new Vector2(angleVec.x, angleVec.y);
            }
            else
            {
                angleDown = 0;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                angleLeft = -1;
                angleVecOld = new Vector2(angleVec.x, angleVec.y);
            }
            else
            {
                angleLeft = 0;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                angleRight = 1;
                angleVecOld = new Vector2(angleVec.x, angleVec.y);
            }
            else
            {
                angleRight = 0;
            }
            changeTurnSpeed();

            //moving stuff
            if (Input.GetKey(KeyCode.PageUp)) //move Forward
            {
                dirUp = 1;
            }
            else
            {
                dirUp = 0;
            }
            if (Input.GetKey(KeyCode.PageDown)) //move Backwards
            {
                dirDown = -1;
            }
            else
            {
                dirDown = 0;
            }
            moveDirection = dirUp + dirDown;

            if (moveDirection == 1)
            {
                actualMovementSpeed = Mathf.Lerp(actualMovementSpeed, maxMovementSpeed, accelerationSpeed * Time.deltaTime);
            }
            else if (moveDirection == -1)
            {
                actualMovementSpeed = Mathf.Lerp(actualMovementSpeed, -maxMovementSpeed, accelerationSpeed * Time.deltaTime);
            }
            else
            {
                actualMovementSpeed = Mathf.Lerp(actualMovementSpeed, 0, decellerationMultiplier * accelerationSpeed * Time.deltaTime);
            }

            transform.Translate(new Vector2(1, 0) * actualMovementSpeed * Time.deltaTime);
        }
    }
}
