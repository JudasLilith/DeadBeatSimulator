using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEnemyScript : MonoBehaviour
{
    public float timeTillDisappear = 10;
    public float acceleration = 10;
    public float decellerationMultiplier = 2.25f;
    float curSpeed = 0;
    public float maxSpeed = 50;

    public float turnSpeed = 10;
    float curTurnSpeed = 10;
    public float maxTurnSpeed = 25;
    public bool useBaseMovement = true;
    public bool doDamageOnContact = true;
    Transform target;

    float _obstacleAvoidanceCooldown = 0.5f;
    public LayerMask basicPathfindingContacts;
    RaycastHit2D[] obstacleAvoidanceResult;
    Vector2 _obstacleAvoidanceTargetDirection = Vector2.zero;
    public float distanceOfCircleCast;

    float timerForDamageCooldown = 2.5f;
    // Start is called before the first frame update
    void Start()
    {
        obstacleAvoidanceResult = new RaycastHit2D[10];
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Vector2 dir = target.position - transform.position;
        transform.eulerAngles = new Vector3(0, 0, Mathf.Deg2Rad * Mathf.Atan2(dir.y, dir.x));
        StartCoroutine(startDisappear());
    }

    public void setStartAngle()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Vector2 dir = target.position - transform.position;
        transform.eulerAngles = new Vector3(0, 0, Mathf.Deg2Rad * Mathf.Atan2(dir.y, dir.x));
    }

    // Update is called once per frame
    void Update()
    {
        timerForDamageCooldown += Time.deltaTime;
        float setCurAngle = 0;
        if (HandleObstacles() != -999)
        {
            setCurAngle = HandleObstacles();
        }
        else
        {
            Vector2 dir = target.position - transform.position;
            float wantedAngle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
            setCurAngle = Mathf.LerpAngle(transform.eulerAngles.z, wantedAngle, curTurnSpeed * Time.deltaTime);
        }
        transform.eulerAngles = new Vector3(0, 0, setCurAngle);
        curSpeed = Mathf.Lerp(curSpeed, maxSpeed, acceleration * Time.deltaTime);
        transform.Translate(new Vector2(1, 0) * curSpeed * Time.deltaTime);
    }

    private float HandleObstacles()
    {
        _obstacleAvoidanceCooldown -= Time.deltaTime;

        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(basicPathfindingContacts);
        Vector2 moveDirection = target.position - transform.position;

        int numberOfCollision = Physics2D.CircleCast(
            transform.position,
            0.5f,
            moveDirection,
            contactFilter,
            obstacleAvoidanceResult,
            distanceOfCircleCast
            );
        //if (numberOfCollision <= 1)
        //{
        //    _targetDirection = Mathf.
        //}
        for (int index = 0; index < numberOfCollision; index++)
        {
            var obstacleCollision = obstacleAvoidanceResult[index];
            if (obstacleCollision.collider.gameObject == gameObject || obstacleCollision.collider.gameObject == target)
            {
                continue;
            }
            if (_obstacleAvoidanceCooldown <= 0)
            {
                _obstacleAvoidanceCooldown = 0.5f;
                _obstacleAvoidanceTargetDirection = obstacleCollision.normal;
            }
            float targetRad = Mathf.Atan2(_obstacleAvoidanceTargetDirection.y, _obstacleAvoidanceTargetDirection.x);
            float targetRotate = Mathf.Rad2Deg * targetRad;
            return Mathf.LerpAngle(transform.eulerAngles.z, targetRotate, Time.deltaTime * 2f);
        }
        return -999;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (timerForDamageCooldown > 2.5f)
        {
            PlayerMovementScript pScript = collision.gameObject.GetComponent<PlayerMovementScript>();
            pScript.takeDamage();
            timerForDamageCooldown = 0;
            StartCoroutine(Disappear());
        }
    }

    IEnumerator startDisappear()
    {
        yield return new WaitForSeconds(timeTillDisappear);
        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        useBaseMovement = false;
        for (int i = 0; i < 200; i++)
        {
            yield return new WaitForSeconds(0.01f);
            curSpeed = Mathf.Lerp(curSpeed, -maxSpeed * 3f, acceleration * decellerationMultiplier * 0.01f);
            transform.Translate(new Vector2(1, 0) * curSpeed * Time.deltaTime);
        }
        Destroy(gameObject);
    }
}
