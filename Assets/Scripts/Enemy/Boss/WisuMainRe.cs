using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WisuMainRe : Enemy
{
    private WisuSpawnPhase spawnPhase;
    private WisuAttackPatternA1 patternA1;
    private WisuAttackPatternA2 patternA2;
    private WisuAttackPatternA3 patternA3;
    private WisuAttackPatternB1 patternB1;
    private WisuAttackPatternB2 patternB2;
    private WisuAttackPatternB3 patternB3;
    private BoxCollider boxCollider;

    [HideInInspector] public bool isPatternFinished = false; // ���� �Ϸ� �÷���
    [HideInInspector] public bool isControllerActive = false; // ���� ����� �μ��� Ȯ��
    [HideInInspector] public bool isGroggy = false; // �׷α� ���� Ȯ��
    [HideInInspector] public bool isSpawnPhaseFinished = false; // ��ȯ ���� ���� Ȯ��    
    [HideInInspector] public bool isGroggyRoutineRunning = false; // �ڷ�ƾ ���� ���� ���� ����
    [HideInInspector] public bool isInvincible = false;  // �������� ����
    [HideInInspector] public float fullHP;
    [HideInInspector] public float currentHpValue; // hp�� �ε巴�� �ϱ� ���� ����
    [HideInInspector] public bool isHPphase = false;
    [HideInInspector] public int currentImageIndex = -1;   // ���� Ȱ��ȭ�� �̹��� �ε���

    #region ���� ����
    [Header("<<<<<< �⺻ ���� >>>>>>")]
    public MainState mainstate;

    [Header("<<<<<< �Ǽ� ��� ���� A1 >>>>>>")]
    public Pattern_A1 pattern_A1;

    [Header("<<<<<< �Ǽ� ��� ���� A2 >>>>>>")]
    public Pattern_A2 pattern_A2;

    [Header("<<<<<< �Ǽ� ��� ���� A3 >>>>>>")]
    public Pattern_A3 pattern_A3;

    [Header("<<<<<< ���� ���� B1 >>>>>>")]
    public Pattern_B1 pattern_B1;

    [Header("<<<<<< ���� ���� B2 >>>>>>")]
    public Pattern_B2 pattern_B2;

    [Header("<<<<<< ���� ���� B3 >>>>>>")]
    public Pattern_B3 pattern_B3;

    #region ��ȯ ����
    [Header("<<<<<< ��ȯ ���� >>>>>>")]
    [Header("1�ܰ� ��ȯ ����")]
    public List<GameObject> phase1Enemies = new List<GameObject>();
    [Header("1�ܰ� ��ȯ ��")]
    public List<int> phase1Counts = new List<int>();
    [Header("1�ܰ� �� ��ȯ ��Ÿ��")]
    public List<float> spawn1CoolTime = new List<float>();

    [Header("2�ܰ� ��ȯ ����")]
    public List<GameObject> phase2Enemies = new List<GameObject>();
    [Header("2�ܰ� ��ȯ ��")]
    public List<int> phase2Counts = new List<int>();
    [Header("2�ܰ� �� ��ȯ ��Ÿ��")]
    public List<float> spawn2CoolTime = new List<float>();

    [Header("3�ܰ� ��ȯ ����")]
    public GameObject phase3Enemy;
    [Header("3�ܰ� ��ȯ ��Ÿ��")]
    public float spawn3CoolTime;

    [Header("��ȯ ��ġ��")]
    public List<Transform> spawnPoints;

    [Header("�� �ܰ� ��Ÿ��")]
    public float delayBetweenSpawnPhase;

    #endregion

    #region ���� ����
    [Header("<<<<<< ���� >>>>>>")]
    [Header("1. ���� Ŀ���� ����")]
    public float localScaleUp;
    #endregion

    [Header("<<<<<< �߰������� �ִ� �� >>>>>>")]
    public WisuSuppressionController suppressionController;
    public GameObject phase1Sword;
    public GameObject phase2Sword;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject dangerZone_Bolt;
    public GameObject dangerZone_SmallPillar;
    public GameObject dangerZone_LargePillar;
    public GameObject dangerZone_B1;
    public Transform dangerZonePosition_B1;
    public Slider bossHPSlider;
    public GameObject bossHPObject;
    public float lerpSpeed;
    #endregion


    #region state
    public enum bState
    {
        ready,
        Idle,
        SpawnPhase,
        AttackPatternA1,
        AttackPatternA2,
        AttackPatternA3,
        AttackPatternB1,
        AttackPatternB2,
        AttackPatternB3,
        Die
    }
    private bState patternState;

    // ������ ����� ������ ������ ����
    private bState lastExecutedPattern = bState.Idle;
    private Transform lookatTransform;
    #endregion

    protected override void Awake()
    {

    }

    protected override void Start()
    {
        mainstate.phase = 1;

        patternA1 = GetComponent<WisuAttackPatternA1>();
        patternA2 = GetComponent<WisuAttackPatternA2>();
        patternA3 = GetComponent<WisuAttackPatternA3>();
        patternB1 = GetComponent<WisuAttackPatternB1>();
        patternB2 = GetComponent<WisuAttackPatternB2>();
        patternB3 = GetComponent<WisuAttackPatternB3>();
        spawnPhase = GetComponent<WisuSpawnPhase>();

        target = GameObject.FindWithTag("PlayerScript").GetComponent<Player>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;

        mainstate.phase2Rate /= 100;
        mainstate.phase3Rate /= 100;

        patternState = bState.ready;
    }

    protected override void Update()
    {
        if (patternState == bState.ready)
        {
            if (isHPphase)
            {
                currentHpValue = Mathf.Lerp(currentHpValue, fullHP, Time.deltaTime * lerpSpeed);
                bossHPSlider.value = currentHpValue;
            }
            return;
        }
        //Debug.Log(fullHP);
        currentHpValue = Mathf.Lerp(currentHpValue, fullHP, Time.deltaTime * lerpSpeed);
        bossHPSlider.value = currentHpValue;



        //���� �̻� ü�� ���̸� �ó׸ӽ� ���
        if (mainstate.phase == 1 && fullHP < hp * mainstate.phase2Rate)
        {
            mainstate.phase++;
            StopAllCoroutines();
            AfterGroggy();
            isInvincible = true;
            patternState = bState.SpawnPhase;
            StartCoroutine(BossStateManager());
        }

        if (isControllerActive)
        {
            isControllerActive = false;
            StopPattern();
            if (patternState == bState.SpawnPhase)
            {
                return;
            }
            StopAllCoroutines();
            animator.speed = 1;
            animator.Play("groggy3", -1, 0f);
            animator.SetBool("GroggyTime", true);

            patternState = bState.Idle;
        }

        if (isSpawnPhaseFinished)
        {
            isInvincible = false;
            isSpawnPhaseFinished = false;
            animator.SetBool("SpawnTime", false);
            animator.SetTrigger("SecondPhaseStart");
            isPatternFinished = true;
            patternState = bState.Idle;
        }

        if (isPatternFinished)
        {
            //���� ���� ����
            isPatternFinished = false;
            bState nextPattern = GetRandomPattern(mainstate.phase);
            lastExecutedPattern = nextPattern;
            patternState = nextPattern;

            StartCoroutine(BossStateManager());
        }
    }
    protected override void FixedUpdate()
    {

    }

    public void StopPattern()
    {
        patternA1.StopPattern();
        patternA2.StopPattern();
        patternA3.StopPattern();
        patternB1.StopPattern();
        patternB2.StopPattern();
        patternB3.StopPattern();
    }

    public void ActiveSword()
    {
        phase1Sword.SetActive(false);
        phase2Sword.SetActive(true);
    }

    public void BossGroggyStart()
    {
        if (isDead)
        {
            return;
        }
        StartCoroutine(BossGroggy());
    }

    #region ���� ����
    public IEnumerator BossStateManager()
    {
        yield return new WaitForSeconds(mainstate.coolTime);
        switch (patternState)
        {
            case bState.Idle:
                isPatternFinished = true;
                break;
            case bState.SpawnPhase:
                animator.SetBool("SpawnTime", true);
                spawnPhase.StartPattern();
                break;
            case bState.AttackPatternA1:
                animator.SetTrigger("A1");
                break;

            case bState.AttackPatternA2:
                animator.SetTrigger("A2");
                break;

            case bState.AttackPatternA3:
                animator.SetTrigger("A3");
                break;

            case bState.AttackPatternB1:
                animator.SetTrigger("B1");
                break;

            case bState.AttackPatternB2:
                animator.SetTrigger("B2");
                break;

            case bState.AttackPatternB3:
                animator.SetTrigger("B3");
                break;
        }
    }
    #endregion

    #region ���� ����

    public void StartPatternA1()
    {
        patternA1.StartPattern();
    }
    public void StartPatternA2()
    {
        patternA2.StartPattern();
    }
    public void StartPatternA3()
    {
        patternA3.StartPattern();
    }
    public void StartPatternB1()
    {
        patternB1.StartPattern();
    }
    public void StartPatternB2()
    {
        patternB2.StartPattern();
    }
    public void StartPatternB3()
    {
        patternB3.StartPattern();
    }
    #endregion

    #region ���� ���� ǥ��
    public void DisplayDangerZoneA1()
    {

    }

    public void DisplayDangerZoneB1()
    {
        GameObject dangerZoneInst = Instantiate(dangerZone_Bolt, dangerZonePosition_B1.position, Quaternion.identity);
    }
    #endregion


    private bState GetRandomPattern(float num)
    {
        bState[] randomPatterns;

        if (mainstate.phase == 1)
        {
            randomPatterns = new bState[]
            {
                bState.AttackPatternA1,
                bState.AttackPatternA2,
                bState.AttackPatternA3
            };
        }
        else
        {
            randomPatterns = new bState[]
            {
                bState.AttackPatternB1,
                bState.AttackPatternB2,
                bState.AttackPatternB3
            };
        }

        // ������ ����� ������ ������ ���� ��� ����
        List<bState> availablePatterns = new List<bState>(randomPatterns);
        availablePatterns.Remove(lastExecutedPattern);

        // �������� ���ο� ���� ����
        int randomIndex = Random.Range(0, availablePatterns.Count);
        return availablePatterns[randomIndex];
    }

    public IEnumerator BossGroggy()
    {
        boxCollider.enabled = true;
        isGroggy = true;
        yield return new WaitForSeconds(mainstate.groggyTime);

        AfterGroggy();

        yield return new WaitForSeconds(3f);

        isPatternFinished = true;
        suppressionController.ResetController();
    }

    public void AfterGroggy()
    {
        isControllerActive = false;
        boxCollider.enabled = false;
        animator.SetBool("GroggyTime", false);
        isGroggy = false;
    }

    public void BossStageStart()
    {
        patternState = bState.Idle;
        StartCoroutine(BossStateManager());
    }

    public void HPUIOn()
    {
        fullHP = hp;
        phaseHPs = new List<float>();
        phaseHP = hp / hpBarCount;
        for (var i = 0; i < hpBarCount; i++)
        {
            phaseHPs.Add(phaseHP);
        }

        bossHPObject.SetActive(true);
        bossHPSlider.maxValue = fullHP;
        isHPphase = true;
    }

    public void AnimationSpeed(float speed)
    {
        animator.speed = speed;
    }
    public void ShowBossPatternImage(int index)
    {
        int imageIndex = index + 1;

        if (imageIndex == 0) // index�� 0�̸� ��� �̹����� ��Ȱ��ȭ
        {
            HideAllImages();
            return;
        }

        // ����Ʈ ���� Ȯ�� �� Ư�� �̹��� Ȱ��ȭ
        if (imageIndex > 0 && imageIndex <= mainstate.bossPatternImages.Count)
        {
            // ���ο� �̹��� Ȱ��ȭ
            currentImageIndex = imageIndex - 1;
            mainstate.bossPatternImages[currentImageIndex].gameObject.SetActive(true);

            // ���� �ð� �� �ش� �̹����� ��Ȱ��ȭ
            StartCoroutine(HideImageAfterDuration(mainstate.displayDuration));
        }
    }

    private IEnumerator HideImageAfterDuration(float duration)
    {
        // ������ �ð���ŭ ���
        yield return new WaitForSeconds(duration);

        // ��� �̹����� ��Ȱ��ȭ
        HideAllImages();
    }

    private void HideAllImages()
    {
        foreach (var image in mainstate.bossPatternImages)
        {
            if (image != null)
            {
                image.gameObject.SetActive(false);
            }
        }
        currentImageIndex = -1;
    }


    #region ������ ������
    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        //TODO: [���] ���� �� ������ ���� �߻�
        if (fullHP <= 0 || phaseHPs[0] <= 0.5f)
        {
            return false;
        }
        if (Time.time < lastDamagedTime + MIN_TIME_BET_DAMAGE || damageMessage.damager == gameObject || isDead || isInvincible)
        {
            return false;
        }

        fullHP--;
        phaseHPs[phaseHPs.Count-1] -= damageMessage.amount;
        lastDamagedTime = Time.time;

        //TODO: [���] HP�� 0�� �������� �� �ٷ� ����ó��
        //phaseHPs[phaseHPs.Count - 1]�� float�� 0�� ��������ٰ� ���� �ε����� �Ѿ�� 0�� ������� ���� ���󺹱��� -> �׷��� ü�� HUD ����� �ݿ��� �ȵ�
        if (fullHP <= 0 || phaseHPs[0] <= 0.5f)
        {
            ClearBossHPBar();
            return true;
        }

        if (hpBarCount > 0 && damageMessage.amount != 0)
        {
            if (calliSystem != null)
            {
                if (!stackUI.activeSelf) stackUI.SetActive(true);
                calliSystem.Painting(damageMessage.color, damageMessage.value);

                if (calliSystem.paintOver < calliSystem.MaxPaintOver)
                {
                    for (var i = 0; i < calliSystem.paintOver + 1; i++)
                    {
                        paintOverStacks[i].SetActive(true);
                    }
                }
                else
                {
                    for (var i = 0; i < calliSystem.MaxPaintOver + 1; i++)
                    {
                        paintOverStacks[i].SetActive(true);
                        paintOverStacks[i].GetComponent<Image>().color = Color.white;
                    }
                    paintOverMax = true;
                }
            }
        }
        return true;
    }
    #endregion

    #region ó�� ���Ҷ�
    public override void Execution()
    {
        if (paintOverMax && !isDead && isGroggy)
        {
            //todo ó�� ���ϴ� ��ǰ� ����
            ClearHPBar();
        }
    }
    #endregion

    #region ���ó��
    public override void DieAction()
    {
        Debug.Log("Wisu Die");

        StopAllCoroutines();
        animator.SetBool("GroggyTime", false);
        animator.SetTrigger("Die");

        if (isDead)
        {
            return;
        }
        isDead = true;
        phaseHPs.Clear();
        stackUI.SetActive(false);

        UIEffect.instance.StartCoroutine(UIEffect.instance.ShowBossDefeatedScreen());
    }
    public void DestroyBoss()
    {
    }
    #endregion
}


#region Ŭ����

#region main
[System.Serializable]
public class MainState
{
    [Header("1. �׷α� �ð�")]
    public float groggyTime;

    [Header("2. 2������ ü�º���")]
    public float phase2Rate;

    [Header("3. 3������ ü�º���")]
    public float phase3Rate;

    [Header("4. ���� �� ��Ÿ��")]
    public float coolTime;

    [Header("5. ��� �̹���")]
    public List<Image> bossPatternImages;

    [Header("6. ��� �̹��� ǥ�� �ð�")]
    public float displayDuration;

    //������ �ܰ�
    [Header("������ �ܰ�")]
    public float phase;
}
#endregion

#region �Ǽ� ����_A1
[System.Serializable]
public class Pattern_A1
{
    [Header("1. �Ҳ� �߻�ü ������")]
    public List<GameObject> A1_prefabs;

    [Header("2. �߻��ϴ� ��")]
    public List<Transform> A1_points;

    [Header("3. �߻� ����")]
    public List<float> A1_intervals;

    [Header("4. �⺻ �߻� ����")]
    public float A1_defaultInterval;

    [Header("5. ��� ���� �Ϸ��ϰ� ���ð�")]
    public float A1_waitingTime;
}
#endregion

#region �Ǽ� ����_A2
[System.Serializable]
public class Pattern_A2
{
    [Header("1. �ұ�� ������")]
    public List<GameObject> A2_prefabs;

    [Header("2. �����Ǵ� ��")]
    public List<Transform> A2_points;

    [Header("3. �߻� ����")]
    public List<float> A2_intervals;

    [Header("4. �⺻ �߻� ����")]
    public float A2_defaultInterval;

    [Header("5. ��� ���� �Ϸ��ϰ� ���ð�")]
    public float A2_waitingTime;
}
#endregion

#region �Ǽ� ����_A3
[System.Serializable]
public class Pattern_A3
{
    [Header(" ���� 1 ")]
    [Header("1. ���� ����")]
    public List<Transform> A3_area1;

    [Header("2. �ұ�� ������")]
    public GameObject A3_area1_prefab;

    [Header("3. ���� ��� �ð�")]
    public float A3_area1_Interval;

    [Header(" ���� 2 ")]
    [Header("1. ���� ����")]
    public List<Transform> A3_area2;

    [Header("2. �ұ�� ������")]
    public GameObject A3_area2_prefab;

    [Header("3. ���� ��� �ð�")]
    public float A3_area2_Interval;

    [Header(" ���� 3 ")]
    [Header("1. ���� ����")]
    public List<Transform> A3_area3;

    [Header("2. �ұ�� ������")]
    public GameObject A3_area3_prefab;

    [Header("3. ���� ��� �ð�")]
    public float A3_area3_Interval;

    [Header(" ���� 4 ")]
    [Header("1. ���� ����")]
    public List<Transform> A3_area4;

    [Header("2. �ұ�� ������")]
    public GameObject A3_area4_prefab;

    [Header("3. ���� ��� �ð�")]
    public float A3_area4_Interval;

    [Header(" ���� 5 ")]
    [Header("1. ���� ����")]
    public List<Transform> A3_area5;

    [Header("2. �ұ�� ������")]
    public GameObject A3_area5_prefab;

    [Header("3. ���� ��� �ð�")]
    public float A3_area5_Interval;


    [Header("��� ���� �Ϸ��ϰ� ���ð�")]
    public float A3_waitingTime;

}
#endregion

#region ���� ����_B1
[System.Serializable]
public class Pattern_B1
{
    [Header(" ���� 1 ")]
    [Header("1. ���� ����")]
    public List<Transform> B1_area1;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B1_area1_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B1_area1_Interval;

    [Header(" ���� 2 ")]
    [Header("1. ���� ����")]
    public List<Transform> B1_area2;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B1_area2_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B1_area2_Interval;

    [Header(" ���� 3 ")]
    [Header("1. ���� ����")]
    public List<Transform> B1_area3;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B1_area3_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B1_area3_Interval;

    [Header(" ���� 4 ")]
    [Header("1. ���� ����")]
    public List<Transform> B1_area4;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B1_area4_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B1_area4_Interval;

    [Header(" ���� 5 ")]
    [Header("1. ���� ����")]
    public List<Transform> B1_area5;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B1_area5_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B1_area5_Interval;

    [Header("��� ���� �Ϸ��ϰ� ���ð�")]
    public float B1_waitingTime;
}
#endregion

#region ���� ����_B2
[System.Serializable]
public class Pattern_B2
{
    [Header(" ���� 1 ")]
    [Header("1. ���� ����")]
    public List<Transform> B2_area1;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B2_area1_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B2_area1_Interval;

    [Header(" ���� 2 ")]
    [Header("1. ���� ����")]
    public List<Transform> B2_area2;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B2_area2_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B2_area2_Interval;

    [Header(" ���� 3 ")]
    [Header("1. ���� ����")]
    public List<Transform> B2_area3;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B2_area3_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B2_area3_Interval;

    [Header(" ���� 4 ")]
    [Header("1. ���� ����")]
    public List<Transform> B2_area4;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B2_area4_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B2_area4_Interval;


    [Header(" ���� 5 ")]
    [Header("1. ���� ����")]
    public List<Transform> B2_area5;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B2_area5_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B2_area5_Interval;

    [Header(" ���� 6 ")]
    [Header("1. ���� ����")]
    public List<Transform> B2_area6;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B2_area6_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B2_area6_Interval;

    [Header(" ���� 7 ")]
    [Header("1. ���� ����")]
    public List<Transform> B2_area7;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B2_area7_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B2_area7_Interval;

    [Header(" ���� 8 ")]
    [Header("1. ���� ����")]
    public List<Transform> B2_area8;

    [Header("2. ��ȭ �ұ�� ������")]
    public GameObject B2_area8_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B2_area8_Interval;

    [Header("��� ���� �Ϸ��ϰ� ���ð�")]
    public float B2_waitingTime;
}
#endregion

#region ���� ����_B3
[System.Serializable]
public class Pattern_B3
{
    [Header(" ���� 1 ")]
    [Header("1. ���� ����")]
    public List<Transform> B3_area1;

    [Header("2. �ұ�� ������")]
    public GameObject B3_area1_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B3_area1_Interval;

    [Header(" ���� 2 ")]
    [Header("1. ���� ����")]
    public List<Transform> B3_area2;

    [Header("2. �ұ�� ������")]
    public GameObject B3_area2_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B3_area2_Interval;

    [Header(" ���� 3 ")]
    [Header("1. ���� ����")]
    public List<Transform> B3_area3;

    [Header("2. �ұ�� ������")]
    public GameObject B3_area3_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B3_area3_Interval;

    [Header(" ���� 4 ")]
    [Header("1. ���� ����")]
    public List<Transform> B3_area4;

    [Header("2. �ұ�� ������")]
    public GameObject B3_area4_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B3_area4_Interval;

    [Header(" ���� 5 ")]
    [Header("1. ���� ����")]
    public List<Transform> B3_area5;

    [Header("2. �ұ�� ������")]
    public GameObject B3_area5_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B3_area5_Interval;

    [Header("��� ���� �Ϸ��ϰ� ���ð�")]
    public float B3_waitingTime;

    [Header(" ���� 6 ")]
    [Header("1. ���� ����")]
    public List<Transform> B3_area6;

    [Header("2. �ұ�� ������")]
    public GameObject B3_area6_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B3_area6_Interval;

    [Header(" ���� 7 ")]
    [Header("1. ���� ����")]
    public List<Transform> B3_area7;

    [Header("2. �ұ�� ������")]
    public GameObject B3_area7_prefab;

    [Header("3. ���� ��� �ð�")]
    public float B3_area7_Interval;
}
#endregion

#endregion