using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class RegularEnemyBulletScript : MonoBehaviour
{
    public enum typeOfAtk
    {
        bullet,
        field,
        chainLightning,
        mortarShell
    }
    public enum typeOfShell //only if is mortarShell duhh...
    {
        indicatorCreatesAttack, //uses atkWayDeathOr
        attackSpawnIndicator
    }
    public enum spawnLocation //only if is mortarShell duhh...
    {
        normal, //comes from source
        aroundTheArena, //from a location from the arena.
        atTheTarget
    }

    [Header("Type of attack")]
    public typeOfAtk typeOfBullet;
    public typeOfShell typeOfMShell;
    public List<GameObject> enemiesHit;
    [Header("Spawn Type")]
    public spawnLocation spawnType;
    public Vector2 centerOfArena;
    public Vector2 changesOfSpace; //only if spawnLocation is aroundTheArena
    [Header("Movement")]
    public float yDisplacement = 0;
    public float moveAheadSpeed; //how much bullet moves forward when it is created.
    public float moveSpeedMin;
    public float moveSpeedMax;
    float moveSpeed;
    public bool rotateWhenMoving = false;
    public float rotateSpeed = 0;
    public int amountOfBounces = 0;
    int whichSideV = 0; //0 = up, 1 = down
    int whichSideH = 0; //0 = right, 1 = left
    public int amountOfPiercing = 0;
    int amountOfEnemiesHit = 0;
    public bool homing = false;
    public float homingSpeed = 0;
    public string homingTag;
    float distancetoenemy;
    public Transform target;
    public ContactFilter2D contactFilter;

    public Vector2 moveDirection;

    [Header("Animation")]
    public bool usesAnimation = false;
    public string animationPlayedOnRepeat;
    Animator animator;

    [Header("Other")]
    public bool ignoreDashing = false;
    public bool breakableForEBullets = true;
    public bool alwaysAt0Degrees = false;
    public float damage;
    public float screenShakeStrength;
    public float screenShakeLength;
    public bool hitsWall = true;
    public float damageMultiplier = 1;

    [Header("Death")]
    public bool hasADeathAttack = false;
    public bool doDAttackWBOrP = false;
    public GameObject deathEffect;
    public bool diesOut = false;
    public float dieOutTime;
    public float colRadius = 1;

    [Header("For Just Fields")]
    public float damageCooldown = 0;
    bool canDoDmg = true;

    [Header("For Just Shells")]
    public Vector2 expanderSize;
    public float expansionTimeMin = 1; //means will take X seconds until the attack is played.
    public float expansionTimeMax = 1;
    public bool destroyAfterExpand = true;
    float expansionTime;
    public Transform expander;
    // Start is called before the first frame update

    private void Awake()
    {
        if (usesAnimation == true)
        {
            animator = GetComponent<Animator>();
        }
        if (alwaysAt0Degrees == true)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        if (typeOfBullet == typeOfAtk.mortarShell && expander == null)
        {
            expander = transform.Find("Expander");
        }
        moveSpeed = Random.Range(moveSpeedMin, moveSpeedMax);
        expansionTime = Random.Range(expansionTimeMin, expansionTimeMax);
        Collider2D collider = transform.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        if (usesAnimation == true)
        {
            animator.Play(animationPlayedOnRepeat);
        }
    }
    void Start()
    {
        if (alwaysAt0Degrees == true)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        if (diesOut == true)
        {
            StartCoroutine(Dieout());
        }
        transform.position = new Vector2(transform.position.x, transform.position.y + yDisplacement);
        if (spawnType == spawnLocation.aroundTheArena)
        {
            float randomX = Random.Range(-changesOfSpace.x, changesOfSpace.x);
            float randomY = Random.Range(-changesOfSpace.y, changesOfSpace.y);

            transform.position = new Vector2(centerOfArena.x + randomX, centerOfArena.y + randomY);
        }
        else if (spawnType == spawnLocation.atTheTarget)
        {
            UpdateTarget();
            transform.position = new Vector2(target.position.x, target.position.y);
        }
        if (typeOfBullet == typeOfAtk.mortarShell)
        {
            if (typeOfMShell == typeOfShell.indicatorCreatesAttack)
            {
                StartCoroutine(mortorShellExpand(expansionTime, true));
            }
        }
        if (moveAheadSpeed > 0)
        {
            transform.Translate(new Vector2(1, 0) * moveAheadSpeed * Time.fixedDeltaTime); //new Vector used to be moveDirection
        }
        if (homing == true)
        {
            UpdateTarget();
        }
        foreach (Transform child in transform)
        {
            RegularEnemyBulletScript childEBScript = child.GetComponent<RegularEnemyBulletScript>();
            if (childEBScript != null)
            {
                childEBScript.damageMultiplier = damageMultiplier;
            }
        }

        Collider2D collider = transform.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameManagerScript.playerHasDied == true)
        //{
        //    StartCoroutine(Death());
        //}
        if (alwaysAt0Degrees == true)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        transform.Translate(new Vector2(1, 0) * moveSpeed * Time.deltaTime);
        if (rotateWhenMoving == true)
        {
            transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime));
            float radians = transform.rotation.z * (Mathf.PI / 180);
            moveDirection = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f);
            //print(moveDirection);
        }
        if (homing == true)
        {
            UpdateTarget();
            if (target != null)
            {
                Vector2 dir = target.position - transform.position;
                moveDirection = dir.normalized;
                float rotateAmount = Vector3.Cross(moveDirection, transform.right).z;
                //Debug.Log(rotateAmount);
                transform.Rotate(new Vector3(0, 0, -rotateAmount * homingSpeed * Time.deltaTime));
            }
        }
        //if (amountOfBounces > 0)
        //{
        //    RaycastHit2D rayUp = Physics2D.Raycast(transform.position, new Vector2(0, 1), colRadius * 2.5f, contactFilter.layerMask);
        //    RaycastHit2D rayRight = Physics2D.Raycast(transform.position, new Vector2(1, 0), colRadius * 2.5f, contactFilter.layerMask);
        //    RaycastHit2D rayLeft = Physics2D.Raycast(transform.position, new Vector2(-1, 0), colRadius * 2.5f, contactFilter.layerMask);
        //    RaycastHit2D rayDown = Physics2D.Raycast(transform.position, new Vector2(0, -1), colRadius * 2.5f, contactFilter.layerMask);
        //    if (rayUp.collider != null) //0 = up, 1 = right, 2 = down, 3= left
        //    {
        //        whichSide = 0;
        //    }
        //    if (rayRight.collider != null)
        //    {
        //        whichSide = 1;

        //    }
        //    if (rayLeft.collider != null)
        //    {
        //        whichSide = 3;
        //    }
        //    if (rayDown.collider != null)
        //    {
        //        whichSide = 2;
        //    }
        //}
        //rb.velocity = moveDirection * moveSpeed
        }

    void UpdateTarget()
    {
        //Debug.Log("targetUpdated");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(homingTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                distancetoenemy = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        if (nearestEnemy != null && shortestDistance <= 999999)
        {
            target = nearestEnemy.transform;

        }
        else
        {
            target = null;
        }
    }

    void UpdateTargetChainLightning()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(homingTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            Collider2D collider = enemy.GetComponent<Collider2D>();
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance && collider.enabled == true)
            {
                int count = 0;
                for (int i = 0; i < enemiesHit.Count; i++)
                {
                    if (enemy == enemiesHit[i])
                    {
                        count = count + 1;
                    }
                }
                if (count == 0)
                {
                    shortestDistance = distanceToEnemy;
                    distancetoenemy = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }
        }
        if (nearestEnemy != null && shortestDistance <= 999999)
        {
            //enemiesHit.Add(nearestEnemy);
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    IEnumerator mortorShellExpand(float time, bool alsoDoAttack)
    {
        int repeatAmount = (int)(time * 100);
        float eIncreaseAmntX = expanderSize.x / repeatAmount;
        float eIncreaseAmntY = expanderSize.y / repeatAmount;
        expander.localScale = new Vector3(0, 0, 1);
        for(int i = 0; i < repeatAmount; i++)
        {
            expander.localScale = new Vector3(expander.localScale.x + eIncreaseAmntX, expander.localScale.y + eIncreaseAmntY, 1);
            yield return new WaitForSeconds(0.01f);
        }
        if (destroyAfterExpand == true)
        {
            if (alsoDoAttack == true)
            {
                StartCoroutine(Death());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetMoveDirection(Vector2 dir, float angle) //when add angles do a , float angle or something
    {
        //transform.position = new Vector2(transform.position.x, transform.position.y + yDisplacement);
        if (alwaysAt0Degrees != true)
        {
            transform.Rotate(new Vector3(0, 0, angle));
        }
        moveDirection = dir;
        //moveDirection.Normalize();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (typeOfBullet == typeOfAtk.bullet)
        {
            if (collision.gameObject.tag == "Wall" && hitsWall == true)
            {
                if (amountOfBounces > 0)
                {
                    RaycastHit2D rayUp = Physics2D.Raycast(transform.position, new Vector2(0, 1), colRadius * 1.1f, contactFilter.layerMask);
                    RaycastHit2D rayRight = Physics2D.Raycast(transform.position, new Vector2(1, 0), colRadius * 1.1f, contactFilter.layerMask);
                    RaycastHit2D rayLeft = Physics2D.Raycast(transform.position, new Vector2(-1, 0), colRadius * 1.1f, contactFilter.layerMask);
                    RaycastHit2D rayDown = Physics2D.Raycast(transform.position, new Vector2(0, -1), colRadius * 1.1f, contactFilter.layerMask);
                    if (rayUp.collider != null) //0 = up, 0 = right, 1 = down, 1 = left
                    {
                        whichSideV = 0;
                    }

                    if (rayRight.collider != null)
                    {
                        whichSideH = 0;
                    }

                    if (rayLeft.collider != null)
                    {
                        whichSideH = 1;
                    }

                    if (rayDown.collider != null)
                    {
                        whichSideV = 1;
                    }

                    if (rayUp.collider == null && rayDown.collider == null)
                    {
                        whichSideV = -1;
                    }
                    else if (rayLeft.collider == null && rayRight.collider == null)
                    {
                        whichSideH = -1;
                    }

                    if (whichSideV == 0 || whichSideV == 1)
                    {
                        moveDirection = new Vector2(moveDirection.x, moveDirection.y * -1);
                    }
                    if (whichSideH == 0 || whichSideH == 1)
                    {
                        moveDirection = new Vector2(moveDirection.x * -1, moveDirection.y);
                    }
                    whichSideH = 0;
                    whichSideV = 0;
                    float radians = Mathf.Atan2(moveDirection.y, moveDirection.x);
                    float angle = (radians * (180 / Mathf.PI));
                    //float angle = Vector2.Angle(moveDirection, newVelocity);
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                    //if (screenShakeStrength > 0)
                    //{
                    //    CameraPosScript mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraPosScript>();
                    //    StartCoroutine(mainCam.shakeTheScreen(screenShakeStrength, screenShakeLength));
                    //}
                    if (doDAttackWBOrP == true)
                    {
                        BounceOrPierceAtkWay();
                    }
                    amountOfBounces = amountOfBounces - 1;
                }
                else
                {
                    StartCoroutine(Death());
                }
            }

            if (collision.gameObject.tag == "Player")
            {
                if (collision.GetComponent<PlayerMovementScript>() != null /*&& GameManagerScript.playerTookDmg == false && GameManagerScript.bossAnimationSignalReceivedINTRO == false*/)
                {
                    collision.GetComponent<PlayerMovementScript>().takeDamage();
                }
                if (amountOfPiercing <= 0)
                {
                    StartCoroutine(Death());
                }
                if (doDAttackWBOrP == true && amountOfPiercing - 1 != 0)
                {
                    BounceOrPierceAtkWay();
                }
                amountOfPiercing = amountOfPiercing - 1;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (typeOfBullet == typeOfAtk.field)
        {
            if (collision.gameObject.tag == "Player")
            {
                //if (deathEffect != null)
                //{
                //    Instantiate(deathEffect, transform.position, Quaternion.identity);
                //}
                if (collision.GetComponent<PlayerMovementScript>() != null && canDoDmg == true)
                {
                    Debug.Log("hit");
                    collision.GetComponent<PlayerMovementScript>().takeDamage();
                    StartCoroutine(Cooldown(damageCooldown));
                    canDoDmg = false;

                }
            }
        }
    }

    IEnumerator Cooldown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canDoDmg = true;
    }

    IEnumerator Dieout()
    {
        yield return new WaitForSeconds(dieOutTime);
        StartCoroutine(Death());
        //remember to make this work with the soon to be AtkWayDeath
    }

    IEnumerator Death()
    {
        moveSpeed = 0;
        yield return new WaitForSeconds(0);
        if (deathEffect != null)
        {
            GameObject deathEf = Instantiate(deathEffect, transform.position, Quaternion.identity);
            RegularEnemyBulletScript deathEBScript = deathEf.GetComponent<RegularEnemyBulletScript>();
            if (deathEBScript != null)
            {
                deathEBScript.damageMultiplier = damageMultiplier;
            }
        }
        if (hasADeathAttack == true)
        {
            Collider2D collider = GetComponent<Collider2D>();
            collider.enabled = false;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;

            //AtkWayDeath atkWayDeath = GetComponent<AtkWayDeath>();
            //atkWayDeath.attackReaction();
            //moveSpeed = 0;
            //while (atkWayDeath.numOfAttacksFinished < atkWayDeath.eAttack.Length)
            //{
            //    yield return new WaitForSeconds(0);
            //}
        }
        Destroy(gameObject);
    }

    void BounceOrPierceAtkWay()
    {
        //AtkWayDeath atkWayDeath = GetComponent<AtkWayDeath>();
        //atkWayDeath.attackReactionBounce();
    }
}
