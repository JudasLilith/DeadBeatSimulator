using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkWayRandom : MonoBehaviour
{
    public EAttack[] eAttack;
    public float attackRateMin = 1;
    public float attackRateMax = 2;
    public Transform attackLocation;
    public float range = 10000f;
    public float minRange = 0f;

    public bool facePlayerOnStart = true;
    List<EAttack> realAttackList = new List<EAttack>();
    EAttack previouslyChosenAttack;
    public bool removefromL = false; //removes attack chosen from list until empty
    public bool noInARow = false; //will never pick the same attack in a row
    int attackChosen;

    float actualAtkSpeed = 0;
    bool canAttack = true;
    public string targetTag = "Player";

    private float distancetoenemy;
    public Transform target;
    Transform animationTarget;
    Vector3 tPosNoUpdate = Vector3.zero;

    public bool ifNeedToFacePAlsoFlipChild = false;
    float objectScale;

    //EnemyRegularMove eRegularMove;

    int allAttacksFinished;
    bool finishedWaitingShield; //Finished Waiting for Shield, ID = 0

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        objectScale = transform.localScale.x;
        actualAtkSpeed = Random.Range(attackRateMin, attackRateMax);
        if (attackLocation == null)
        {
            attackLocation = transform;
        }
        for(int i = 0; i < eAttack.Length; i++)
        {
            Debug.Log(eAttack[i]);
            Debug.Log(i);
            realAttackList.Add(eAttack[i]);
        }
        attackChosen = 0;
        previouslyChosenAttack = realAttackList[attackChosen];
        UpdateTarget();
        InvokeRepeating("attackReaction", actualAtkSpeed, actualAtkSpeed);
        //if (facePlayerOnStart == true)
        //{
        //    facePlayer();
        //}
    }
    private void Update()
    {
        //if (eScript != null && eScript.isStunned == true)
        //{
        //    stopAllAttack();
        //    pickRandom();
        //    canAttack = true;
        //}

        if (target == null /*&& GameManagerScript.bossAnimationSignalReceivedINTRO == true*/)
        {
            UpdateTarget();
            UpdateAnimationTarget();
        }
    }

    //public void stopAllAttack()
    //{
    //    if (animator != null)
    //    {
    //        animator.Play(eScript.stunnedAnimation);
    //    }
    //    StopAllCoroutines();
    //}

    void UpdateAnimationTarget()
    {
        Debug.Log("targetUpdated");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(targetTag);
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
        if (nearestEnemy != null && shortestDistance <= range)
        {
            animationTarget = nearestEnemy.transform;

        }
        else
        {
            animationTarget = null;
        }
    }

    public IEnumerator animationProcessor(Animator animator, Transform transform, EAttack attack)
    {
        if (attack.idleAnimation != "" && attack.attackingAnimation != "" && attack.usesAnimation == true)
        {
            yield return new WaitForSeconds(attack.idleAnimDelay);
            animator.Play(attack.idleAnimation);
            //facePlayer();
            //Debug.Log("Played Idle Animation");
            yield return new WaitForSeconds(attack.attackAnimDelay);
            animator.Play(attack.attackingAnimation);
            if (attack.animationParticleEffect != null)
            {
                Instantiate(attack.animationParticleEffect, new Vector2(transform.position.x + attack.animPartEffectPos.x, transform.position.y + attack.animPartEffectPos.y), Quaternion.identity);
            }
            yield return new WaitForSeconds(attack.animEndTime);
            if (attack.idleAtEnd == true)
            {
                animator.Play(attack.idleAnimation);
            }
        }
        yield break;
    }

    void attackReaction()
    {
        if (target != null && canAttack == true && Vector2.Distance(transform.position, target.position) >= minRange)
        {
            StartCoroutine(attack());
        }
    }

    void pickRandom()
    {
        if(removefromL == true)
        {
            if (realAttackList.Count == 0)
            {
                for (int i = 0; i < eAttack.Length; i++)
                {
                    realAttackList.Add(eAttack[i]);
                }
            }
        }
        if(noInARow == true)
        {
            while (previouslyChosenAttack == realAttackList[attackChosen])
            {
                attackChosen = Random.Range(0, realAttackList.Count);
            }
            previouslyChosenAttack = realAttackList[attackChosen];
        }
        else
        {
            attackChosen = Random.Range(0, realAttackList.Count);
        }
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(targetTag);
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
        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;

        }
        else
        {
            target = null;
        }
    }

    IEnumerator _combinedAttack(EAttack newAttack)
    {
        Animator anim = GetComponent<Animator>();
        if (newAttack.playAnimationBeforeWait == true)
        {
            StartCoroutine(animationProcessor(anim, transform, newAttack));
        }

        yield return new WaitForSeconds(newAttack.atkDelayS);

        if (newAttack.typeOfAttack == EAttack.attackType.Constant || newAttack.typeOfAttack == EAttack.attackType.Aimed)
        {
            if (newAttack.playAnimationBeforeWait == false)
            {
                StartCoroutine(animationProcessor(anim, transform, newAttack));
            }
            Debug.Log(newAttack);
            for (int d = 0; d <= newAttack.RepeatAmount - 1; d++)
            {
                if (newAttack.alwaysChangeTarget == true)
                {
                    //facePlayer();
                    Invoke("UpdateTarget", 0);
                    tPosNoUpdate = target.position;
                }
                if (newAttack.attackFX != null)
                {
                    Debug.Log("playedSound");
                    //SoundFXManager.instance.PlaySoundFXClip(newAttack.attackFX, transform, newAttack.volumeFX, newAttack.pitchMinFX, newAttack.pitchMaxFX, true);
                }
                StartCoroutine(newAttack.attack(gameObject.transform, attackLocation.position, tPosNoUpdate));
                yield return new WaitForSeconds(newAttack.RepeatRate);
            }
            while (newAttack.canAtk == false)
            {
                yield return new WaitForSeconds(0.0f);
            }
            allAttacksFinished = allAttacksFinished + 1;
        }
        else if (newAttack.typeOfAttack == EAttack.attackType.Movement)
        {
            if (newAttack.playAnimationBeforeWait == false)
            {
                StartCoroutine(animationProcessor(anim, transform, newAttack));
            }
            //eRegularMove = gameObject.GetComponent<EnemyRegularMove>();
            if (newAttack.movementType == EAttack.atkMovementType.dashAndTPTowards)
            {
                if (newAttack.attackFX != null)
                {
                    //SoundFXManager.instance.PlaySoundFXClip(newAttack.attackFX, transform, newAttack.volumeFX, newAttack.pitchMinFX, newAttack.pitchMaxFX, true);
                }
                for (int i = 0; i <= newAttack.repeatAmount - 1; i++)
                {
                    //facePlayer();
                    Instantiate(newAttack.mEffect, transform.position, Quaternion.identity);
                    Vector2 moveDir = target.position - transform.position;
                    transform.Translate(moveDir * newAttack.mAmount);
                    yield return new WaitForSeconds(newAttack.mRate);
                }
                if (newAttack.rDEAtEnd == true)
                {
                    Instantiate(newAttack.mEffect, transform.position, Quaternion.identity);
                }
            }
            else if (newAttack.movementType == EAttack.atkMovementType.dashAndTPAway)
            {
                if (newAttack.attackFX != null)
                {
                    //SoundFXManager.instance.PlaySoundFXClip(newAttack.attackFX, transform, newAttack.volumeFX, newAttack.pitchMinFX, newAttack.pitchMaxFX, true);
                }
                for (int i = 0; i <= newAttack.repeatAmount - 1; i++)
                {
                    //facePlayer();
                    Instantiate(newAttack.mEffect, transform.position, Quaternion.identity);
                    Vector2 moveDir = target.position - transform.position;
                    moveDir = moveDir.normalized;
                    transform.Translate(moveDir * -newAttack.mAmount);
                    yield return new WaitForSeconds(newAttack.mRate);
                }
                if (newAttack.rDEAtEnd == true)
                {
                    Instantiate(newAttack.mEffect, transform.position, Quaternion.identity);
                }
            }
            else if (newAttack.movementType == EAttack.atkMovementType.FullShield)
            {
                if (newAttack.attackFX != null)
                {
                    //SoundFXManager.instance.PlaySoundFXClip(newAttack.attackFX, transform, newAttack.volumeFX, newAttack.pitchMinFX, newAttack.pitchMaxFX, true);
                }
                finishedWaitingShield = false;
                StartCoroutine(_wait(newAttack.mRate, 0));
                if (newAttack.rDEAtEnd == true)
                {
                    Instantiate(newAttack.mEffect, transform.position, Quaternion.identity);
                }
            }
            else if (newAttack.movementType == EAttack.atkMovementType.jumpTowardsTimedBased)
            {
                if (newAttack.attackFX != null)
                {
                    //SoundFXManager.instance.PlaySoundFXClip(newAttack.attackFX, transform, newAttack.volumeFX, newAttack.pitchMinFX, newAttack.pitchMaxFX, true);
                }
                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                GameObject point1 = new GameObject("JumpPoint1");
                GameObject point2 = new GameObject("JumpPoint2");
                GameObject point3 = new GameObject("JumpPoint3");
                point1.transform.position = gameObject.transform.position;
                if (distanceToTarget > newAttack.mAmount)
                {
                    point3.transform.position = Vector2.MoveTowards(point1.transform.position, target.position, newAttack.mAmount);
                    point2.transform.position = Vector2.MoveTowards(point1.transform.position, target.position, newAttack.mAmount / 2);
                }
                else if (distanceToTarget <= newAttack.mAmount)
                {
                    point3.transform.position = target.position;
                    point2.transform.position = Vector2.MoveTowards(point1.transform.position, target.position, distanceToTarget / 2);
                }
                point2.transform.position = new Vector2(point2.transform.position.x, point2.transform.position.y + (distanceToTarget / 4) * newAttack.repeatAmount); //height

                point1.SetActive(true);
                point2.SetActive(true);
                point3.SetActive(true);

                GameObject shadow = Instantiate(newAttack.otherIndicator);
                shadow.SetActive(true);
                GameObject jumpIndicator = Instantiate(newAttack.attackIndicator);
                jumpIndicator.SetActive(true);

                Vector2 point1Pos = point1.transform.position;
                Vector2 point2Pos = point2.transform.position;
                Vector2 point3Pos = point3.transform.position;
                jumpIndicator.transform.position = point3Pos;

                float travelPos = 0f;
                float distanceToPoint3 = Vector2.Distance(point1Pos, point3Pos);
                float travelSpeed = distanceToPoint3 / newAttack.mRate;
                while (travelPos < 1f)
                {
                    travelPos = travelPos + (Time.deltaTime * travelSpeed);
                    transform.position = (1 - travelPos) * ((1 - travelPos) * point1Pos + travelPos * point2Pos) + travelPos * ((1 - travelPos) * point2Pos + travelPos * point3Pos);
                    //shadow.transform.position = new Vector2(transform.position.x, shadow.transform.position.y);
                    shadow.transform.position = Vector2.MoveTowards(point1Pos, point3Pos, distanceToPoint3 * travelPos);
                    yield return new WaitForEndOfFrame();
                }
                Destroy(point1);
                Destroy(point2);
                Destroy(point3);
                Destroy(shadow);
                Destroy(jumpIndicator);
            }
            else if (newAttack.movementType == EAttack.atkMovementType.jumpTowardsDistanceBased)
            {
                if (newAttack.attackFX != null)
                {
                    //SoundFXManager.instance.PlaySoundFXClip(newAttack.attackFX, transform, newAttack.volumeFX, newAttack.pitchMinFX, newAttack.pitchMaxFX, true);
                }
                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                GameObject point1 = new GameObject("JumpPoint1");
                GameObject point2 = new GameObject("JumpPoint2");
                GameObject point3 = new GameObject("JumpPoint3");
                point1.transform.position = gameObject.transform.position;
                if (distanceToTarget > newAttack.mAmount)
                {
                    point3.transform.position = Vector2.MoveTowards(point1.transform.position, target.position, newAttack.mAmount);
                    point2.transform.position = Vector2.MoveTowards(point1.transform.position, target.position, newAttack.mAmount / 2);
                }
                else if (distanceToTarget <= newAttack.mAmount)
                {
                    point3.transform.position = target.position;
                    point2.transform.position = Vector2.MoveTowards(point1.transform.position, target.position, distanceToTarget / 2);
                }
                point2.transform.position = new Vector2(point2.transform.position.x, point2.transform.position.y + (distanceToTarget / 4) * newAttack.repeatAmount); //height

                point1.SetActive(true);
                point2.SetActive(true);
                point3.SetActive(true);

                GameObject shadow = Instantiate(newAttack.otherIndicator);
                shadow.SetActive(true);
                GameObject jumpIndicator = Instantiate(newAttack.attackIndicator);
                jumpIndicator.SetActive(true);

                Vector2 point1Pos = point1.transform.position;
                Vector2 point2Pos = point2.transform.position;
                Vector2 point3Pos = point3.transform.position;
                jumpIndicator.transform.position = point3Pos;

                float travelPos = 0f;
                float distanceToPoint3 = Vector2.Distance(point1Pos, point3Pos);
                while (travelPos < 1f)
                {
                    travelPos = travelPos + (Time.deltaTime * newAttack.mRate);
                    transform.position = (1 - travelPos) * ((1 - travelPos) * point1Pos + travelPos * point2Pos) + travelPos * ((1 - travelPos) * point2Pos + travelPos * point3Pos);
                    //shadow.transform.position = new Vector2(transform.position.x, shadow.transform.position.y);
                    shadow.transform.position = Vector2.MoveTowards(point1Pos, point3Pos, distanceToPoint3 * travelPos);
                    yield return new WaitForEndOfFrame();
                }
                Destroy(point1);
                Destroy(point2);
                Destroy(point3);
                Destroy(shadow);
                Destroy(jumpIndicator);
            }
            else if (newAttack.movementType == EAttack.atkMovementType.followXOnly)
            {
                for (int i = 0; i <= newAttack.repeatAmount - 1; i++)
                {
                    //facePlayer();
                    //Instantiate(realAttackList[attackChosen].mEffect, transform.position, Quaternion.identity);
                    //Vector2 moveDir = target.position - transform.position;
                    transform.position = Vector2.Lerp(transform.position, new Vector2(target.position.x, transform.position.y), newAttack.mAmount);
                    //transform.Translate(moveDir * realAttackList[attackChosen].mAmount);
                    yield return new WaitForSeconds(newAttack.mRate);
                }
            }
            else if (newAttack.movementType == EAttack.atkMovementType.ChargeDownY)
            {
                for (int i = 0; i <= newAttack.repeatAmount - 1; i++)
                {
                    //facePlayer();
                    //Instantiate(realAttackList[attackChosen].mEffect, transform.position, Quaternion.identity);
                    //Vector2 moveDir = target.position - transform.position;
                    transform.Translate(new Vector2(0, -1) * newAttack.mAmount);
                    //transform.Translate(moveDir * realAttackList[attackChosen].mAmount);
                    yield return new WaitForSeconds(newAttack.mRate);
                }
            }
            else if (newAttack.movementType == EAttack.atkMovementType.TeleportToSpecificY)
            {
                Instantiate(newAttack.mEffect, transform.position, Quaternion.identity);
                //Vector2 moveDir = target.position - transform.position;
                transform.position = new Vector2(transform.position.x, newAttack.mAmount);
                Instantiate(newAttack.mEffect, transform.position, Quaternion.identity);
                //transform.Translate(moveDir * realAttackList[attackChosen].mAmount);
                //yield return new WaitForSeconds(realAttackList[attackChosen].mRate);
            }
            allAttacksFinished = allAttacksFinished + 1;
        }
        yield return new WaitForSeconds(newAttack.atkDelayE);
    }

    IEnumerator _wait(float waitAmount, int ID)
    {
        yield return new WaitForSeconds(waitAmount);
        switch (ID)
        {
            case 0:
                finishedWaitingShield = true;
                break;
        }
    }

    IEnumerator attack()
    {
        if (eAttack[attackChosen].facePlayerWhenAnimating == true)
        {
            //facePlayer();
        }
        Invoke("UpdateTarget", 0);
        tPosNoUpdate = target.position;
        canAttack = false;

        Animator anim = GetComponent<Animator>();
        if (realAttackList[attackChosen].playAnimationBeforeWait == true)
        {
            StartCoroutine(animationProcessor(anim, transform, realAttackList[attackChosen]));
        }

        if (realAttackList[attackChosen].typeOfAttack == EAttack.attackType.Constant || realAttackList[attackChosen].typeOfAttack == EAttack.attackType.Aimed)
        {
            yield return new WaitForSeconds(realAttackList[attackChosen].atkDelayS);
            if (realAttackList[attackChosen].playAnimationBeforeWait == false)
            {
                StartCoroutine(animationProcessor(anim, transform, realAttackList[attackChosen]));
            }
            for (int i = 0; i <= realAttackList[attackChosen].RepeatAmount - 1; i++)
            {
                if (realAttackList[attackChosen].alwaysChangeTarget == true)
                {
                    //facePlayer();
                    Invoke("UpdateTarget", 0);
                    tPosNoUpdate = target.position;
                }
                if (realAttackList[attackChosen].attackFX != null)
                {
                    //SoundFXManager.instance.PlaySoundFXClip(realAttackList[attackChosen].attackFX, transform, realAttackList[attackChosen].volumeFX, realAttackList[attackChosen].pitchMinFX, realAttackList[attackChosen].pitchMaxFX, true);
                }
                StartCoroutine(realAttackList[attackChosen].attack(gameObject.transform, attackLocation.position, tPosNoUpdate));
                yield return new WaitForSeconds(realAttackList[attackChosen].RepeatRate);
            }
            while (realAttackList[attackChosen].canAtk == false)
            {
                yield return new WaitForSeconds(0.0f);
            }
            yield return new WaitForSeconds(realAttackList[attackChosen].atkDelayE);
            canAttack = true;
            if (removefromL == true)
            {
                realAttackList.Remove(realAttackList[attackChosen]);
            }
            pickRandom();
        }
        else if (realAttackList[attackChosen].typeOfAttack == EAttack.attackType.Movement)
        {
            canAttack = false;
            yield return new WaitForSeconds(realAttackList[attackChosen].atkDelayS);
            if (realAttackList[attackChosen].attackFX != null)
            {
                //SoundFXManager.instance.PlaySoundFXClip(realAttackList[attackChosen].attackFX, transform, realAttackList[attackChosen].volumeFX, realAttackList[attackChosen].pitchMinFX, realAttackList[attackChosen].pitchMaxFX, true);
            }
            if (realAttackList[attackChosen].playAnimationBeforeWait == false)
            {
                StartCoroutine(animationProcessor(anim, transform, realAttackList[attackChosen]));
            }
            //eRegularMove = gameObject.GetComponent<EnemyRegularMove>();
            if (realAttackList[attackChosen].movementType == EAttack.atkMovementType.dashAndTPTowards)
            {
                for (int i = 0; i <= realAttackList[attackChosen].repeatAmount - 1; i++)
                {
                    //facePlayer();
                    Instantiate(realAttackList[attackChosen].mEffect, transform.position, Quaternion.identity);
                    Vector2 moveDir = target.position - transform.position;
                    transform.Translate(moveDir * realAttackList[attackChosen].mAmount);
                    yield return new WaitForSeconds(realAttackList[attackChosen].mRate);
                }
                if (realAttackList[attackChosen].rDEAtEnd == true)
                {
                    Instantiate(realAttackList[attackChosen].mEffect, transform.position, Quaternion.identity);
                }
            }
            else if (realAttackList[attackChosen].movementType == EAttack.atkMovementType.dashAndTPAway)
            {
                for (int i = 0; i <= realAttackList[attackChosen].repeatAmount - 1; i++)
                {
                    //facePlayer();
                    Instantiate(realAttackList[attackChosen].mEffect, transform.position, Quaternion.identity);
                    Vector2 moveDir = target.position - transform.position;
                    moveDir = moveDir.normalized;
                    transform.Translate(moveDir * -realAttackList[attackChosen].mAmount);
                    yield return new WaitForSeconds(realAttackList[attackChosen].mRate);
                }
                if (realAttackList[attackChosen].rDEAtEnd == true)
                {
                    Instantiate(realAttackList[attackChosen].mEffect, transform.position, Quaternion.identity);
                }
            }
            else if (realAttackList[attackChosen].movementType == EAttack.atkMovementType.FullShield)
            {
                finishedWaitingShield = false;
                StartCoroutine(_wait(realAttackList[attackChosen].mRate, 0));
                if (realAttackList[attackChosen].rDEAtEnd == true)
                {
                    Instantiate(realAttackList[attackChosen].mEffect, transform.position, Quaternion.identity);
                }
                StopCoroutine(_wait(realAttackList[attackChosen].mRate, 0));
                finishedWaitingShield = false;
            }
            else if (eAttack[attackChosen].movementType == EAttack.atkMovementType.jumpTowardsTimedBased)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                GameObject point1 = new GameObject("JumpPoint1");
                GameObject point2 = new GameObject("JumpPoint2");
                GameObject point3 = new GameObject("JumpPoint3");
                point1.transform.position = gameObject.transform.position;
                if (distanceToTarget > eAttack[attackChosen].mAmount)
                {
                    point3.transform.position = Vector2.MoveTowards(point1.transform.position, target.position, eAttack[attackChosen].mAmount);
                    point2.transform.position = Vector2.MoveTowards(point1.transform.position, target.position, eAttack[attackChosen].mAmount / 2);
                }
                else if (distanceToTarget <= eAttack[attackChosen].mAmount)
                {
                    point3.transform.position = target.position;
                    point2.transform.position = Vector2.MoveTowards(point1.transform.position, target.position, distanceToTarget / 2);
                }
                point2.transform.position = new Vector2(point2.transform.position.x, point2.transform.position.y + (distanceToTarget / 4) * eAttack[attackChosen].repeatAmount); //height

                point1.SetActive(true);
                point2.SetActive(true);
                point3.SetActive(true);

                GameObject shadow = Instantiate(eAttack[attackChosen].otherIndicator);
                shadow.SetActive(true);
                GameObject jumpIndicator = Instantiate(eAttack[attackChosen].attackIndicator);
                jumpIndicator.SetActive(true);

                Vector2 point1Pos = point1.transform.position;
                Vector2 point2Pos = point2.transform.position;
                Vector2 point3Pos = point3.transform.position;
                jumpIndicator.transform.position = point3Pos;

                float travelPos = 0f;
                float distanceToPoint3 = Vector2.Distance(point1Pos, point3Pos);
                float travelSpeed = distanceToPoint3 / eAttack[attackChosen].mRate;
                while (travelPos < 1f)
                {
                    travelPos = travelPos + (Time.deltaTime * travelSpeed);
                    transform.position = (1 - travelPos) * ((1 - travelPos) * point1Pos + travelPos * point2Pos) + travelPos * ((1 - travelPos) * point2Pos + travelPos * point3Pos);
                    //shadow.transform.position = new Vector2(transform.position.x, shadow.transform.position.y);
                    shadow.transform.position = Vector2.MoveTowards(point1Pos, point3Pos, distanceToPoint3 * travelPos);
                    yield return new WaitForEndOfFrame();
                }
                Destroy(point1);
                Destroy(point2);
                Destroy(point3);
                Destroy(shadow);
                Destroy(jumpIndicator);
            }
            else if (eAttack[attackChosen].movementType == EAttack.atkMovementType.jumpTowardsDistanceBased)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                GameObject point1 = new GameObject("JumpPoint1");
                GameObject point2 = new GameObject("JumpPoint2");
                GameObject point3 = new GameObject("JumpPoint3");
                point1.transform.position = gameObject.transform.position;
                if (distanceToTarget > eAttack[attackChosen].mAmount)
                {
                    point3.transform.position = Vector2.MoveTowards(point1.transform.position, target.position, eAttack[attackChosen].mAmount);
                    point2.transform.position = Vector2.MoveTowards(point1.transform.position, target.position, eAttack[attackChosen].mAmount / 2);
                }
                else if (distanceToTarget <= eAttack[attackChosen].mAmount)
                {
                    point3.transform.position = target.position;
                    point2.transform.position = Vector2.MoveTowards(point1.transform.position, target.position, distanceToTarget / 2);
                }
                point2.transform.position = new Vector2(point2.transform.position.x, point2.transform.position.y + (distanceToTarget / 4) * eAttack[attackChosen].repeatAmount); //height

                point1.SetActive(true);
                point2.SetActive(true);
                point3.SetActive(true);

                GameObject shadow = Instantiate(eAttack[attackChosen].otherIndicator);
                shadow.SetActive(true);
                GameObject jumpIndicator = Instantiate(eAttack[attackChosen].attackIndicator);
                jumpIndicator.SetActive(true);

                Vector2 point1Pos = point1.transform.position;
                Vector2 point2Pos = point2.transform.position;
                Vector2 point3Pos = point3.transform.position;
                jumpIndicator.transform.position = point3Pos;

                float travelPos = 0f;
                float distanceToPoint3 = Vector2.Distance(point1Pos, point3Pos);
                while (travelPos < 1f)
                {
                    travelPos = travelPos + (Time.deltaTime * eAttack[attackChosen].mRate);
                    transform.position = (1 - travelPos) * ((1 - travelPos) * point1Pos + travelPos * point2Pos) + travelPos * ((1 - travelPos) * point2Pos + travelPos * point3Pos);
                    //shadow.transform.position = new Vector2(transform.position.x, shadow.transform.position.y);
                    shadow.transform.position = Vector2.MoveTowards(point1Pos, point3Pos, distanceToPoint3 * travelPos);
                    yield return new WaitForEndOfFrame();
                }
                Destroy(point1);
                Destroy(point2);
                Destroy(point3);
                Destroy(shadow);
                Destroy(jumpIndicator);
            }
            else if (eAttack[attackChosen].movementType == EAttack.atkMovementType.followXOnly)
            {
                for (int i = 0; i <= realAttackList[attackChosen].repeatAmount - 1; i++)
                {
                    //facePlayer();
                    //Instantiate(realAttackList[attackChosen].mEffect, transform.position, Quaternion.identity);
                    //Vector2 moveDir = target.position - transform.position;
                    transform.position = Vector2.Lerp(transform.position, new Vector2(target.position.x, transform.position.y), realAttackList[attackChosen].mAmount);
                    //transform.Translate(moveDir * realAttackList[attackChosen].mAmount);
                    yield return new WaitForSeconds(realAttackList[attackChosen].mRate);
                }
            }
            else if (eAttack[attackChosen].movementType == EAttack.atkMovementType.ChargeDownY)
            {
                for (int i = 0; i <= realAttackList[attackChosen].repeatAmount - 1; i++)
                {
                    //facePlayer();
                    //Instantiate(realAttackList[attackChosen].mEffect, transform.position, Quaternion.identity);
                    //Vector2 moveDir = target.position - transform.position;
                    transform.Translate(new Vector2(0, -1) * realAttackList[attackChosen].mAmount);
                    //transform.Translate(moveDir * realAttackList[attackChosen].mAmount);
                    yield return new WaitForSeconds(realAttackList[attackChosen].mRate);
                }
            }
            else if (eAttack[attackChosen].movementType == EAttack.atkMovementType.TeleportToSpecificY)
            {
                Instantiate(realAttackList[attackChosen].mEffect, transform.position, Quaternion.identity);
                //Vector2 moveDir = target.position - transform.position;
                transform.position = new Vector2(transform.position.x, realAttackList[attackChosen].mAmount);
                //transform.Translate(moveDir * realAttackList[attackChosen].mAmount);
                //yield return new WaitForSeconds(realAttackList[attackChosen].mRate);
            }
            yield return new WaitForSeconds(realAttackList[attackChosen].atkDelayE);
            canAttack = true;
            if (removefromL == true)
            {
                realAttackList.Remove(realAttackList[attackChosen]);
            }
            pickRandom();
        }
        else if (realAttackList[attackChosen].typeOfAttack == EAttack.attackType.Combine)
        {
            yield return new WaitForSeconds(realAttackList[attackChosen].atkDelayS);
            allAttacksFinished = 0;
            canAttack = false;
            EAttack[] combinedAttack = realAttackList[attackChosen].attacksToCombine;
            foreach (EAttack newAttack in combinedAttack)
            {
                StartCoroutine(_combinedAttack(newAttack));
            }
            while (allAttacksFinished < combinedAttack.Length)
            {
                yield return new WaitForSeconds(0f);
            }
            yield return new WaitForSeconds(realAttackList[attackChosen].atkDelayE);
            canAttack = true;
            if (removefromL == true)
            {
                realAttackList.Remove(realAttackList[attackChosen]);
            }
            pickRandom();
        }
        else if (eAttack[attackChosen].typeOfAttack == EAttack.attackType.Sequential)
        {
            yield return new WaitForSeconds(realAttackList[attackChosen].atkDelayS);
            allAttacksFinished = 0;
            canAttack = false;
            EAttack[] combinedAttack = eAttack[attackChosen].attacksToCombine;
            for (int i = 0; i < combinedAttack.Length; i++)
            {
                StartCoroutine(_combinedAttack(eAttack[attackChosen].attacksToCombine[i]));
                while (allAttacksFinished < i + 1)
                {
                    yield return new WaitForSeconds(0f);
                }
            }
            yield return new WaitForSeconds(realAttackList[attackChosen].atkDelayE);
            canAttack = true;
            if (removefromL == true)
            {
                realAttackList.Remove(realAttackList[attackChosen]);
            }
            pickRandom();
        }

        if (eAttack[attackChosen].deleteSelfAfterAtk == true)
        {
            Destroy(gameObject);
        }
    }
}
