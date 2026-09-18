using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostMaskSkill : MonoBehaviour
{
    #region �ܺ�
    private PlayerController playerController;
    private Player player;

    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private CameraController cameraController;
    [SerializeField] private MaskChange maskChange;
    [SerializeField] private HumanMaskSkill humanSkill;

    [SerializeField] private SkillHUD skillHUD;

    [SerializeField] private PlayerCameraEffect playerCameraEffect;
    [SerializeField] private PlayerSkillMove playerSkillMove;
    [SerializeField] private GameTimeScale playerTimeScale;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private PlayerEffect playerEffect;
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private PlayerSound playerSound;

    //������
    private PlayerCommonData commonData;
    private PlayerGhostMaskData ghostData;
    #endregion

    #region ó�� ��ų ������Ʈ
    [Space(20)]
    [SerializeField] private GameObject humanWeapon;
    [SerializeField] private GameObject[] animalWeapon;
    [SerializeField] private GameObject ghostWeaponForHuman;
    [SerializeField] private GameObject ghostWeaponForAnimal;
    [SerializeField] private GameObject[] finishDomeEffect;
    [SerializeField] private GameObject[] finishCutEffect;
    [SerializeField] private GameObject finishDomeEffectPosition;
    [SerializeField] private GameObject finishCutEffectPosition;

    private List<Transform> finishTargetList;
    #endregion

    public bool canUseFinishSkill {get; private set;}   
    
    private float finishSkillStartTime;
    private bool isPerformingFinish = false;
    private bool isPerformingFinishAnim = false;

    private Coroutine coFinishSkill;

    [SerializeField] private CinemachineVirtualCamera skillCamera;
    private Animator skillCameraAnimator;


    private void Start()
    {
        playerController = PlayerController.instance;
        player = Player.instance;

        commonData = PlayerCommonData.Instance;
        ghostData = PlayerGhostMaskData.Instance;
        cameraController = CameraController.instance;

        skillCameraAnimator = skillCamera.GetComponent<Animator>();

        finishTargetList = new();
        canUseFinishSkill = true;
        coFinishSkill = null;

        ghostWeaponForAnimal.SetActive(false);
        ghostWeaponForHuman.SetActive(false);
    }

    public void InitializeSkill()
    {
        //�ڷ�ƾ
        if (coFinishSkill != null) StopCoroutine(coFinishSkill);

        //����
        isPerformingFinish = false;
        isPerformingFinishAnim = false;

        //��� �ʱ�ȭ
        playerSkillMove.Initialize();
        playerEffect.Initialize();
        playerState.Initialize();
        playerSound.Initialize();

        //ī�޶�
        cameraController.ChangeCamera(CameraType.DEFAULT);
    }

    //���Ž��
    public void DetectTargetToFinish()
    {
        if (!canUseFinishSkill)
        { 
            skillHUD.ActivateFinishHUD(false);
            return;
        }

        //����Ʈ Ŭ����
        finishTargetList.Clear();

        //�� ���̾��� Ÿ�� ����
        Collider[] colliders = Physics.OverlapSphere
            (maskChange.CurrentMask.transform.position, CameraData.Instance.detectRange, CameraData.Instance.enemyLayer);

        int maxInkStack = 0;
        Collider targetCenter; //Ÿ�ٱ���(������ ���� ���� Ÿ��) //�̸� �ٽ� ����

        #region Ÿ�ٿ� �߰��Ǵ� ����
        for (int i = 0; i < colliders.Length; i++)
        {
            //���� �ƴ϶�� + �׾��ٸ� �н�
            if (!colliders[i].gameObject.GetComponent<Enemy>()) continue;
            if (colliders[i].gameObject.GetComponent<Enemy>().isDead) continue;

            //��ĥ Ǯ������ �����ϸ� �� ������ �Ѱ轺�ü� ����(�� ū�� ���ü��� ����)
            //�� �ܿ��� ��Ƽ��
            if (!colliders[i].gameObject.GetComponent<CalliSystem>()) continue;
            if (!colliders[i].gameObject.GetComponent<CalliSystem>().IsPaintOverMax()) continue;

            int maxPaintOver = colliders[i].gameObject.GetComponent<CalliSystem>().MaxPaintOver;
            if (maxInkStack < maxPaintOver)
            {
                maxInkStack = maxPaintOver;
            }
        }

        //�ٽ� ������ ������ �����߿�
        //�ش� �����̶� ���ų� ���� ��� ��� ó������Ʈ�� �߰�
        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].gameObject.GetComponent<Enemy>()) continue;
            if (colliders[i].gameObject.GetComponent<Enemy>().isDead) continue;
            if (!colliders[i].gameObject.GetComponent<CalliSystem>()) continue;
            
            int enemyMaxPaintOver = colliders[i].gameObject.GetComponent<CalliSystem>().MaxPaintOver;
            if (maxInkStack >= enemyMaxPaintOver)
            {
                finishTargetList.Add(colliders[i].gameObject.transform);
            }
        }

        //Ÿ�� ���� ���¿� ���� ������ Ȱ��ȭ
        if (finishTargetList.Count == 0 || (finishTargetList == null) || !canUseFinishSkill)
        {
            skillHUD.ActivateFinishHUD(false);
        }
        else
        {
            skillHUD.ActivateFinishHUD(true);
        }
        #endregion
    }

    //�Է½� ���� �Լ�
    public void Finish()
    {
        //���� ������ ��ų ����
        if (!CheckEnableFinish())
        {
            return;
        }

        InitializeSkill();

        if (coFinishSkill != null)
        {
            StopCoroutine(coFinishSkill);
        }

        canUseFinishSkill = false;
        coFinishSkill = StartCoroutine(CoFinish());
    }

    //ī�޶� ����� ī�޶�� �� ������� ���� ����ؼ� ó���������� ���
    public bool CheckEnableFinish()
    {
        if (!canUseFinishSkill) return false;
        if (finishTargetList == null) return false;
        if (finishTargetList.Count <= 0) return false;

        //�þ߰� ���� Ÿ���� �ִٸ� ����
        bool startSkill = false;
        for (int i = 0; i < finishTargetList.Count; i++)
        {
            Vector3 direction = (finishTargetList[i].transform.position - cameraController.MainCamera.transform.position).normalized; //Ÿ�Ϲ��� ����

            if (Vector3.Angle(cameraController.MainCamera.transform.forward, direction) < (ghostData.viewAngle * 0.5f))
            {
                startSkill = true;
                break;
            }
        }

        if (!startSkill)
        {
            return false;
        }

        //��� ���� ������ ����
        return true;
    }

    public IEnumerator CoFinish()
    {
        //����
        playerState.ToggleInvincibleState(true);

        //ī�޶�
        cameraController.ChangeCamera(CameraType.FINISHSKILL);
        SkillCameraAnimation();

        //�ð�
        finishSkillStartTime = Time.time;
        canUseFinishSkill = false;

        //����
        bool isHumanMask = false;

        //����
        playerState.ChangePlayerState(PlayerStateType.GHOST_FINISHSKILL);
        playerState.ChangePlayerSubState(PlayerSubStateType.NONE);

        //��ų ��������� �˸�
        playerSound.StopLoopingAudio();

        //�� ����
        Collider[] colliders = Physics.OverlapSphere
            (maskChange.CurrentMask.transform.position, CameraData.Instance.detectRange, CameraData.Instance.enemyLayer);

        if (colliders.Length > 0 )
        {
            foreach (var target in colliders)
            {
                if (!target) continue;
                if (!target.gameObject.GetComponent<Enemy>()) continue;
                if (!target.gameObject.GetComponent<NavMeshAgent>()) continue;
                if (!target.gameObject.GetComponent<NavMeshAgent>().enabled) continue;
                target.GetComponent<Enemy>().MotionStop(7);
            }
        }
        
        //�ִϸ��̼� ���
        if (maskChange.CurrentMask == maskChange.HumanMask)
        {
            maskChange.CurrentAnimator.CrossFade(playerAnimation.Ghost_HumanHitGround, 0);
            isHumanMask = true;
        }
        else
        {
            maskChange.CurrentAnimator.CrossFade(playerAnimation.Ghost_AnimalSweap, 0);
            
            for (int i = 0; i < animalWeapon.Length; i++)
            {
                animalWeapon[i].SetActive(true);
            }

            isHumanMask = false;
            
        }

        #region while ����
        bool setGhostWeaponOnce = false;
        bool setOriginalWeaponOnce = false;
        bool killTargetsOnce = false;

        bool activeMoveOnce = false;
        bool activeEffectOnce = false;
        bool activeSoundOnce = false;
        bool activeCameraShake1Once = false;
        bool activeCameraShake2Once = false;
        bool activeTimeScaleOnce = false;
        #endregion

        while (true)
        {
            if (isHumanMask)
            {
                #region ����
                if (!setGhostWeaponOnce && (Time.time >= finishSkillStartTime + ghostData.humanSetGhostWeaponTime))
                {
                    maskChange.ChangeMask(MaskType.GHOST,true,false);
                    humanWeapon.SetActive(false);
                    ghostWeaponForHuman.SetActive(true);
                    
                    setGhostWeaponOnce = true;
                }
                if (!setOriginalWeaponOnce && (Time.time >= finishSkillStartTime + ghostData.humanSetOriginalWeaponTime))
                {
                    maskChange.ChangeMask(MaskType.HUMAN,false,false);
                    humanWeapon.SetActive(true);
                    ghostWeaponForHuman.SetActive(false);
                    
                    setOriginalWeaponOnce = true;
                }
                if (!killTargetsOnce && (Time.time >= finishSkillStartTime + ghostData.humanKillTargetTime))
                {
                    if (finishTargetList != null || finishTargetList.Count != 0)
                    {
                        foreach (var target in finishTargetList)
                        {
                            //while�� 
                            if (target.GetComponent<Enemy>() || !target.GetComponent<Enemy>().isDead)
                            {
                                target.GetComponent<Enemy>().Execution();
                            }
                        }

                        if (cameraController.CurrentTarget != null)
                        {
                            cameraController.LockOnTarget();
                        }

                        player.currentHP += 2;
                        if (player.currentHP >= 20)
                        {
                            player.currentHP = 20;
                        }
                        HpHUD.instance.ChangeHPStack((int)player.currentHP);

                        killTargetsOnce = true;
                    }
                }
                #endregion

                #region �ִϸ��̼� ����
                var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
                var animationHash = animatorStateInfo.shortNameHash;

                if (animationHash == playerAnimation.Ghost_HumanHitGround || animationHash == playerAnimation.Ghost_HumanSwing)
                {
                    isPerformingFinishAnim = true;
                }
                else
                {
                    if (isPerformingFinishAnim)
                    {
                        InitializeSkill();
                        yield break;
                    }
                }
                #endregion

                #region �����̵�
                if (!activeMoveOnce)
                {
                    for (int i = 0; i < ghostData.humanSkillMove.Length; i++)
                    {
                        playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(ghostData.humanSkillMove[i]));
                    }
                    activeMoveOnce = true;
                }
                #endregion

                #region ����
                    playerState.RestrictPlayer(ghostData.humanRestrict, finishSkillStartTime);
                #endregion

                #region ����Ʈ
                if (!activeEffectOnce)
                {
                    playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                        (ghostData.humanCutEffect, finishCutEffect, finishCutEffectPosition ));
                    playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                        (ghostData.humanDomeEffect, finishDomeEffect, finishDomeEffectPosition));
                    activeEffectOnce = true;
                }
                #endregion

                #region �Ҹ�
                if (!activeSoundOnce)
                {
                    //waitTime ���� ū struct���� ���� ��ġ
                    playerSound.SetPlayerSound(ghostData.humanHitGroundSound, player.transform.position, finishSkillStartTime);
                    playerSound.SetPlayerSound(ghostData.humanSwingSound, humanWeapon.transform.position, finishSkillStartTime);
                    playerSound.SetPlayerSound(ghostData.humanAfterSwingSound, player.transform.position, finishSkillStartTime);
                    activeSoundOnce = true;
                }
                
                #endregion

                #region ī�޶� ����ũ
                if (!activeCameraShake1Once && (Time.time >= finishSkillStartTime + ghostData.humanFinishHitGroundCameraShake.waitTime))
                {
                    playerCameraEffect.ShakeCamera(ghostData.humanFinishHitGroundCameraShake);
                    activeCameraShake1Once = true;
                }
                if (!activeCameraShake2Once && (Time.time >= finishSkillStartTime + ghostData.humanFinishSwingCameraShake.waitTime))
                {
                    playerCameraEffect.ShakeCamera(ghostData.humanFinishSwingCameraShake);
                    activeCameraShake2Once = true;
                }
                #endregion

                #region Ÿ�� ������
                if (!activeTimeScaleOnce)
                {
                    playerTimeScale.StartCoroutine(playerTimeScale.CoSetTimeScale(ghostData.humanFinishTimeScale));
                    activeTimeScaleOnce = true;
                }
                #endregion
            }
            else
            {
                #region ����
                if (!setGhostWeaponOnce && (Time.time >= finishSkillStartTime + ghostData.animalSetGhostWeaponTime))
                {
                    maskChange.ChangeMask(MaskType.GHOST, true, false);

                    for (int i = 0; i < animalWeapon.Length; i++)
                    {
                        animalWeapon[i].SetActive(false);
                    }

                    ghostWeaponForAnimal.SetActive(true);

                    setGhostWeaponOnce = true;
                }
                if (!setOriginalWeaponOnce && (Time.time >= finishSkillStartTime + ghostData.animalSetOriginalWeaponTime))
                {
                    maskChange.ChangeMask(MaskType.ANIMAL, false,false);

                    //���� ���������� ��ҿ� ��Ȱ��ȭ

                    ghostWeaponForAnimal.SetActive(false);
                    setOriginalWeaponOnce = true;
                }
                if (!killTargetsOnce && (Time.time >= finishSkillStartTime + ghostData.animalKillTargetTime))
                {
                    if (finishTargetList != null || finishTargetList.Count != 0)
                    {
                        foreach (var target in finishTargetList)
                        {
                            target.GetComponent<Enemy>().Execution();
                        }
                        if (cameraController.CurrentTarget != null)
                        {
                            cameraController.LockOnTarget();
                        }

                        player.currentHP += 2;
                        if (player.currentHP >= 20)
                        {
                            player.currentHP = 20;
                        }
                        HpHUD.instance.ChangeHPStack((int)player.currentHP);

                        killTargetsOnce = true;
                    }
                }
                #endregion

                #region �ִϸ��̼� ����
                var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
                var animationHash = animatorStateInfo.shortNameHash;

                if (animationHash == playerAnimation.Ghost_AnimalSweap || animationHash == playerAnimation.Ghost_AnimalSwing)
                {
                    isPerformingFinishAnim = true;
                }
                else
                {
                    if (isPerformingFinishAnim)
                    {
                        InitializeSkill();
                        yield break;
                    }
                }
                #endregion

                #region �����̵�
                if (!activeMoveOnce)
                {
                    for (int i = 0; i < ghostData.animalSkillMove.Length; i++)
                    {
                        playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(ghostData.animalSkillMove[i]));
                    }
                    activeMoveOnce = true;
                }
                #endregion

                #region ����
                playerState.RestrictPlayer(ghostData.animalRestrict, finishSkillStartTime);
                #endregion

                #region ����Ʈ
                if (!activeEffectOnce)
                {
                    playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                        (ghostData.animalCutEffect, finishCutEffect, finishCutEffectPosition));
                    playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                        (ghostData.animalDomeEffect, finishDomeEffect, finishDomeEffectPosition));
                    activeEffectOnce = true;
                }
                #endregion

                #region �Ҹ�
                if (!activeSoundOnce)
                {
                    playerSound.SetPlayerSound(ghostData.animalSweapSound, player.transform.position,finishSkillStartTime);
                    playerSound.SetPlayerSound(ghostData.animalSwingSound, player.transform.position, finishSkillStartTime);
                    playerSound.SetPlayerSound(ghostData.animalAfterSwingSound, player.transform.position, finishSkillStartTime);
                    activeSoundOnce = true;
                }
                #endregion

                #region ī�޶� ����ũ
                if (!activeCameraShake1Once && (Time.time >= finishSkillStartTime + ghostData.animalFinishSweapCameraShake.waitTime))
                {
                    playerCameraEffect.ShakeCamera(ghostData.animalFinishSweapCameraShake);
                    activeCameraShake1Once = true;
                }
                if (!activeCameraShake2Once && (Time.time >= finishSkillStartTime + ghostData.animalFinishSwingCameraShake.waitTime))
                {
                    playerCameraEffect.ShakeCamera(ghostData.animalFinishSwingCameraShake);
                    activeCameraShake2Once = true;
                }
                #endregion

                #region Ÿ�� ������
                if (!activeTimeScaleOnce)
                {
                    playerTimeScale.StartCoroutine(playerTimeScale.CoSetTimeScale(ghostData.animalFinishTimeScale));
                    activeTimeScaleOnce = true;
                }
                #endregion
            }

            yield return null;
        }
    }

    public void FinishSkillCooldown() //ó�� ��Ÿ��
    {
        if (canUseFinishSkill) return;

        float flowTimeRate = (Time.time - finishSkillStartTime) / ghostData.cooldown;
        skillHUD.SkillCooldown(PlayerStateType.GHOST_FINISHSKILL, flowTimeRate);

        if (Time.time > finishSkillStartTime + ghostData.cooldown)
        {
            canUseFinishSkill = true;
        }
    }

    public void SkillCameraAnimation()
    {
        if (maskChange.HumanMask.activeSelf)
        {
            skillCameraAnimator.CrossFade("HumanMaskFinish", 0);
        }
        else
        {
            skillCameraAnimator.CrossFade("AnimalMaskFinish", 0);
        }
    }
}
