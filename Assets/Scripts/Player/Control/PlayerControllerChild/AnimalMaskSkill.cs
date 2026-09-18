using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMaskSkill : PlayerSkill
{
    #region �ܺ�
    [SerializeField] private SkillHUD skillHUD;

    //��ų ���
    [SerializeField] private PlayerCameraEffect playerCameraEffect;
    [SerializeField] private PlayerSkillMove playerSkillMove;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private PlayerEffect playerEffect;
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private PlayerSound playerSound;
    [SerializeField] private PlayerSkillInput playerSkillInput;
    [SerializeField] private PlayerHitBox playerHitBox;
    [SerializeField] private GameTimeScale gameTimeScale;

    #endregion

    #region ��밡�� ����
    public bool canUseFirstAttack { get; private set; }
    public bool canUseSecondAttack { get; private set; }
    public bool canUseThirdAttack { get; private set; }
    public bool canUseLeapStrike {get; private set;}
    public bool canUseRoar {get; private set;}
    public bool canUseDash { get;  private set;}
    #endregion

    #region ��ų ������ ����
    private bool isPerformingLeapStrike;
    private bool isPerformingRoar;
    private bool isPerformingDash;

    //�ִ� ����
    private bool isPerformingFirstAttackAnim;
    private bool isPerformingSecondAttackAnim;
    private bool isPerformingThirdAttackAnim;
    private bool isPerformingLeapStrikeAnim;
    private bool isPerformingRoarAnim;
    private bool isPerformingDashAnim;
    #endregion

    #region �����ð�
    private float firstAttackStartTime;
    private float secondAttackStartTime;
    private float thirdAttackStartTime;
    private float leapStrikeStartTime;
    private float roarStartTime;
    private float dashStartTime;
    #endregion

    #region ��ų �ڷ�ƾ
    public Coroutine coFirstAttack { get; private set; }
    public Coroutine coSecondAttack { get; private set; }
    public Coroutine coThirdAttack { get; private set; }
    public Coroutine coLeapStrike { get; private set; }
    public Coroutine coRoar { get; private set; }
    public Coroutine coDash { get; private set; }
    #endregion

    #region ���� ����
    [SerializeField] private GameObject rightHandWeapon;
    [SerializeField] private GameObject leftHandWeapon;
    
    private MeshRenderer rightHandWeaponMesh;
    private MeshRenderer leftHandWeaponMesh;
    #endregion

    #region NoarmalAttack
    [Space(20)]
    [SerializeField] private GameObject firstAttackEffectPosition;
    [SerializeField] private GameObject secondAttackEffectPosition;
    [SerializeField] private GameObject thirdAttackEffectPosition;

    [SerializeField ] private GameObject[] firstAttackEffect;
    [SerializeField] private GameObject[] secondAttackEffect;
    [SerializeField] private GameObject[] thirdAttackEffect;
    #endregion

    #region Leap Strike
    [Space(20)]
    private int leapStrikeHitCount = 0;

    [SerializeField] private GameObject leapStrikeHitBox;
    [SerializeField] private GameObject leapStrikeEffectPosition;
    [SerializeField] private GameObject[] leapStrikeEffect;

    //Position SO �����ͷ� �־���� �ǽð����� �����ϴ°� ���ҵ�. �׽�Ʈ�Ҷ��� 
    [SerializeField] private GameObject leapStrikeTrailPosition;
    [SerializeField] private GameObject[] leapStrikeTrail;
    #endregion

    #region Roar
    [Space(20)]
    private int roarHitCount = 0;
    [SerializeField] private GameObject roarHitBox;
    
    [SerializeField] private GameObject roarEffectPosition;

    [SerializeField] private GameObject[] roarTrailEffect;
    [SerializeField] private GameObject[] roarChargeEffect;
    [SerializeField] private GameObject[] roarDisChargeEffect;
    #endregion

    public delegate IEnumerator CoroutineDelegate();

    private void Start()
    {
        StartSet();
        AnimalMaskStartSet();
        this.gameObject.SetActive(false);
    }

    #region Initialize
    public void InitializeSkill()
    {
        InitializeCoroutine();
        InitializeWeapon();
        InitializeHitBox();
        InitializeState();

        //��ų���
        playerSkillMove.Initialize();
        playerEffect.Initialize();
        playerState.Initialize();
        playerSound.Initialize();
        playerSkillInput.Initialize();
        gameTimeScale.Initialize();
    }

    public void InitializeCoroutine()
    {
        if (coFirstAttack != null) StopCoroutine(coFirstAttack);
        if (coSecondAttack != null) StopCoroutine(coSecondAttack);
        if (coThirdAttack != null) StopCoroutine(coThirdAttack);
        if (coLeapStrike != null) StopCoroutine(coLeapStrike);
        if (coRoar != null) StopCoroutine(coRoar);
        if (coDash != null) StopCoroutine(coDash);
    }

    public void InitializeWeapon()
    {
        rightHandWeaponMesh.enabled = false;
        leftHandWeaponMesh.enabled = false;
    }

    public void InitializeHitBox()
    {
        leapStrikeHitBox.SetActive(false);
        roarHitBox.SetActive(false);

        //������� �ش�
        //�ξ� �ش�
    }

    public void InitializeState()
    {
        #region �⺻����
        isPerformingFirstAttackAnim = false;
        isPerformingSecondAttackAnim = false;
        isPerformingThirdAttackAnim = false;

        canUseFirstAttack = true; //��Ÿ�� ��Ÿ�� ����
        canUseSecondAttack = false; //��Ÿ1 �Լ����� true�� �ٲ�
        canUseThirdAttack = false; //��Ÿ2 �Լ����� true�� �ٲ�
        #endregion

        #region ��ų
        isPerformingLeapStrike = false;
        isPerformingRoar = false;
        isPerformingDash = false;

        isPerformingLeapStrikeAnim = false;
        isPerformingRoarAnim = false;
        isPerformingDashAnim = false;
        #endregion
    }
    #endregion

    private void AnimalMaskStartSet()
    {
        #region canUse
        canUseFirstAttack = true;
        canUseSecondAttack = false;
        canUseThirdAttack = false;
        canUseLeapStrike = true;
        canUseRoar = true;
        canUseDash = true;
        #endregion

        #region perform
        isPerformingLeapStrike = false;
        isPerformingRoar = false;
        isPerformingDash = false;

        isPerformingFirstAttackAnim = false;
        isPerformingSecondAttackAnim = false;
        isPerformingThirdAttackAnim = false;
        isPerformingLeapStrikeAnim = false;
        isPerformingRoarAnim = false;
        isPerformingDashAnim = false;
        #endregion

        #region coroutine
        coFirstAttack = null;
        coSecondAttack = null;
        coThirdAttack = null;
        coLeapStrike = null;
        coRoar = null;
        coDash = null;
        #endregion

        #region ���ӿ�����Ʈ
        leapStrikeHitBox.SetActive(false);

        for (int i = 0; i < firstAttackEffect.Length; i++)
        {
            firstAttackEffect[i].SetActive(false);
        }
        for (int i = 0; i < secondAttackEffect.Length; i++)
        {
            secondAttackEffect[i].SetActive(false);
        }
        for (int i = 0; i < leapStrikeEffect.Length; i++)
        {
            leapStrikeEffect[i].SetActive(false);
        }
        for (int i = 0; i < leapStrikeTrail.Length; i++)
        {
            leapStrikeTrail[i].SetActive(false);
        }
        for (int i = 0; i < roarChargeEffect.Length; i++)
        {
            roarChargeEffect[i].SetActive(false);
        }
        for (int i = 0; i < roarDisChargeEffect.Length; i++)
        {
            roarDisChargeEffect[i].SetActive(false);
        }

        rightHandWeaponMesh = rightHandWeapon.GetComponent<MeshRenderer>();
        leftHandWeaponMesh = leftHandWeapon.GetComponent<MeshRenderer>();

        rightHandWeaponMesh.enabled = false;
        leftHandWeaponMesh.enabled = false;
        #endregion

        leapStrikeHitCount = 0;
        roarHitCount = 0;
    }

    public void UseSkill(bool canUse, Coroutine coroutine, CoroutineDelegate coroutineMethod)
    {
        if (canUse)
        {
            InitializeSkill();

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            UIEffect.ShowPlayerHUDFadeEffect();
            coroutine = StartCoroutine(coroutineMethod());

            canUse = false;
        }
    }

    #region Normal Attack
    public void NormalAttack()
    {
        UseSkill(canUseFirstAttack,coFirstAttack, CoFirstAttack);
    }

    public IEnumerator CoFirstAttack()
    {
        maskChange.CurrentAnimator.CrossFade(playerAnimation.Animal_FirstNormalAttack, 0.1f);
        
        playerState.ChangePlayerState(PlayerStateType.ANIMAL_NORMALATTACK);
        playerState.ChangePlayerSubState(PlayerSubStateType.ANIMAL_FIRSTNORMALATTACK);

        canUseFirstAttack = false;
        firstAttackStartTime = Time.time;

        #region while ����
        bool activeMoveOnce = false;
        bool activeEffectOnce = false;
        bool activeHitBoxOnce = false;
        bool inactiveHitBoxOnce = false;
        bool activeSoundOnce = false;
        bool activeCameraShakeOnce = false;
        #endregion

        playerSkillInput.ProcessInput(animalData.firstNormalAttackInput, firstAttackStartTime);
        while (playerState.playerCurrentSubState == PlayerSubStateType.ANIMAL_FIRSTNORMALATTACK)
        {
            #region Animation State
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;
            if (animationHash == playerAnimation.Animal_FirstNormalAttack)
            {
                isPerformingFirstAttackAnim = true;
            }
            else if ((animationHash == playerAnimation.Animal_Hit) || (animationHash == playerAnimation.Animal_Die))
            {
                InitializeSkill();
                yield break;
            }
            else
            {
                if (isPerformingFirstAttackAnim)
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion

            #region Move
            if (!activeMoveOnce)
            {
                for (int i = 0; i < animalData.firstNormalAttackMove.Length; i++)
                {
                    playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(animalData.firstNormalAttackMove[i]));
                }
                activeMoveOnce = true;
            }
            #endregion

            #region Restriction
            playerState.RestrictPlayer(animalData.firstNormalAttackRestrict, firstAttackStartTime);

            //DoNotAct�� duration���� ���� ��Ȳ���� �����̸� ��ų ���� 
            if (isPerformingFirstAttackAnim && (Time.time >= firstAttackStartTime + animalData.firstNormalAttackRestrict.actRestrictWaitTime))
            {
                canUseSecondAttack = true;

                if (maskChange.CurrentAnimator.GetFloat("moveAmount") > .4f) // �����̸� �ʱ�ȭ
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion

            #region Effect
            if (!activeEffectOnce)
            {
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (animalData.firstNormalAttackSkillEffect, firstAttackEffect, firstAttackEffectPosition));
                activeEffectOnce = true;
            }   
            #endregion

            #region Weapon Shape
            if (Time.time >= firstAttackStartTime + animalData.firstNormalAttackWeaponWaitTime + animalData.firstNormalAttackWeaponDuration)
            {
                rightHandWeaponMesh.enabled = false;
            }
            else if (Time.time >= firstAttackStartTime + animalData.firstNormalAttackWeaponWaitTime)
            {
                rightHandWeaponMesh.enabled = true;
            }
            #endregion

            #region HitBox
            if (!activeHitBoxOnce)
            {
                playerHitBox.StartCoroutine(playerHitBox.TogglePlayerHitBox(animalData.firstNormalAttackHitBox));
                activeHitBoxOnce = true;
            }
            #endregion

            #region Audio
            if (!activeSoundOnce)
            {
                playerSound.SetPlayerSound(animalData.firstNormalAttackSound, player.transform.position,firstAttackStartTime);
                activeSoundOnce = true;
            }
            #endregion

            yield return null;
        }
    }
    public IEnumerator CoSecondAttack()
    {
        playerState.ToggleSuperArmorState(true);

        maskChange.CurrentAnimator.CrossFade(playerAnimation.Animal_SecondNormalAttack, 0.1f);

        playerState.ChangePlayerState(PlayerStateType.ANIMAL_NORMALATTACK);
        playerState.ChangePlayerSubState(PlayerSubStateType.ANIMAL_SECONDNORMALATTACK);

        canUseSecondAttack = false;
        secondAttackStartTime = Time.time;

        #region while ����
        bool activeMoveOnce = false;
        bool activeEffectOnce = false;
        bool activeHitBoxOnce = false;
        bool inactiveHitBoxOnce = false;
        bool activeSoundOnce = false;
        bool activeCameraShakeOnce = false;
        #endregion

        playerSkillInput.ProcessInput(animalData.secondNormalAttackInput, secondAttackStartTime);

        while (playerState.playerCurrentSubState == PlayerSubStateType.ANIMAL_SECONDNORMALATTACK)
        {
            #region Animation State
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;

            if (animationHash == playerAnimation.Animal_SecondNormalAttack)
            {
                isPerformingSecondAttackAnim = true;
            }
            else if ((animationHash == playerAnimation.Animal_Hit) || (animationHash == playerAnimation.Animal_Die))
            {
                InitializeSkill();
                yield break;
            }
            else
            {
                if (isPerformingSecondAttackAnim)
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion

            #region Move
            if (!activeMoveOnce)
            {
                for (int i = 0; i < animalData.secondNormalAttackMove.Length; i++)
                {
                    playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(animalData.secondNormalAttackMove[i]));
                }
                activeMoveOnce = true;
            }
            #endregion

            #region Restrict
            playerState.RestrictPlayer(animalData.secondNormalAttackRestrict, secondAttackStartTime);

            //DoNotAct�� duration���� ���� ��Ȳ���� �����̸� ��ų ����
            if (isPerformingSecondAttackAnim && Time.time >= secondAttackStartTime + animalData.secondNormalAttackRestrict.actRestrictWaitTime)
            {
                canUseThirdAttack = true;

                if (maskChange.CurrentAnimator.GetFloat("moveAmount") > .9f) // �����̸� �ʱ�ȭ
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion

            #region Effect
            if (!activeEffectOnce)
            {
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (animalData.secondNormalAttackSkillEffect, secondAttackEffect, secondAttackEffectPosition));
                activeEffectOnce = true;
            }
            #endregion

            #region Weapon Shape
            if (Time.time >= secondAttackStartTime + animalData.secondNormalAttackWeaponWaitTime + animalData.secondNormalAttackWeaponDuration)
            {
                leftHandWeaponMesh.enabled = false;
            }
            else if (Time.time >= secondAttackStartTime + animalData.secondNormalAttackWeaponWaitTime)
            {
                leftHandWeaponMesh.enabled = true;
            }
            #endregion

            #region HitBox
            if (!activeHitBoxOnce)
            {
                playerHitBox.StartCoroutine(playerHitBox.TogglePlayerHitBox(animalData.secondNormalAttackHitBox));
                activeHitBoxOnce = true;
            }
            #endregion

            #region Sound
            if (!activeSoundOnce)
            {
                playerSound.SetPlayerSound(animalData.secondNormalAttackSound, player.transform.position,secondAttackStartTime);
                activeSoundOnce = true;
            }
            #endregion

            #region Camera Shake
            if (!activeCameraShakeOnce && (Time.time >= secondAttackStartTime + animalData.secondNormalAttackCameraShake.waitTime))
            {
                playerCameraEffect.ShakeCamera(animalData.secondNormalAttackCameraShake);
                activeCameraShakeOnce = true;
            }
            #endregion

            yield return null;
        }
    }
    public IEnumerator CoThirdAttack()
    {
        playerState.ToggleSuperArmorState(true);

        maskChange.CurrentAnimator.CrossFade(playerAnimation.Animal_ThirdNormalAttack, 0.1f);

        playerState.ChangePlayerState(PlayerStateType.ANIMAL_NORMALATTACK);
        playerState.ChangePlayerSubState(PlayerSubStateType.ANIMAL_THIRDNORMALATTACK);

        canUseThirdAttack = false;
        thirdAttackStartTime = Time.time;

        #region while ����
        bool activeMoveOnce = false;
        bool activeEffectOnce = false;
        bool activeHitBoxOnce = false;
        bool inactiveHitBoxOnce = false;
        bool activeSoundOnce = false;
        bool activeCameraShakeOnce = false;
        #endregion

        playerSkillInput.ProcessInput(animalData.thirdNormalAttackInput, thirdAttackStartTime);

        while (playerState.playerCurrentSubState == PlayerSubStateType.ANIMAL_THIRDNORMALATTACK)
        {
            #region Animation State
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;

            if (animationHash == playerAnimation.Animal_ThirdNormalAttack)
            {
                isPerformingThirdAttackAnim = true;
            }
            else if ((animationHash == playerAnimation.Animal_Hit) || (animationHash == playerAnimation.Animal_Die))
            {
                InitializeSkill();
                yield break;
            }
            else
            {
                if (isPerformingThirdAttackAnim)
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion

            #region Move
            if (!activeMoveOnce)
            {
                for (int i = 0; i < animalData.thirdNormalAttackMove.Length; i++)
                {
                    playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(animalData.thirdNormalAttackMove[i]));
                }
                activeMoveOnce = true;
            }
            #endregion

            #region Restrict
            playerState.RestrictPlayer(animalData.thirdNormalAttackRestrict, thirdAttackStartTime);

            //DoNotAct�� duration���� ���� ��Ȳ���� �����̸� ��ų ����
            if (isPerformingThirdAttackAnim && Time.time >= thirdAttackStartTime + animalData.thirdNormalAttackRestrict.actRestrictWaitTime)
            {
                if (maskChange.CurrentAnimator.GetFloat("moveAmount") > .9f) // �����̸� �ʱ�ȭ
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion

            #region Effect
            if (!activeEffectOnce)
            {
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (animalData.thirdNormalAttackSkillEffect, thirdAttackEffect, thirdAttackEffectPosition));
                activeEffectOnce = true;
            }
            #endregion

            #region Weapon Shape
            if (Time.time >= thirdAttackStartTime + animalData.thirdNormalAttackWeaponWaitTime + animalData.thirdNormalAttackWeaponDuration)
            {
                leftHandWeaponMesh.enabled = false;
            }
            else if (Time.time >= thirdAttackStartTime + animalData.thirdNormalAttackWeaponWaitTime)
            {
                leftHandWeaponMesh.enabled = true;

            }
            #endregion

            #region HitBox
            if (!activeHitBoxOnce)
            {
                playerHitBox.StartCoroutine(playerHitBox.TogglePlayerHitBox(animalData.thirdNormalAttackHitBox));
                activeHitBoxOnce = true;
            }
            #endregion

            #region Sound
            if (!activeSoundOnce)
            {
                playerSound.SetPlayerSound(animalData.thirdNormalAttackSound, player.transform.position, thirdAttackStartTime);
                activeSoundOnce = true;
            }
            #endregion

            #region Camera Shake
            if (!activeCameraShakeOnce && (Time.time >= secondAttackStartTime + animalData.secondNormalAttackCameraShake.waitTime))
            {
                playerCameraEffect.ShakeCamera(animalData.secondNormalAttackCameraShake);
                activeCameraShakeOnce = true;
            }
            #endregion

            #region TimeScale

            #endregion

            yield return null;
        }
    }
    #endregion

    #region Leap Strike
    public void LeapStrike()
    {
        UseSkill(canUseLeapStrike, coLeapStrike, CoLeapStrike);
    }

    public IEnumerator CoLeapStrike()
    {
        playerState.ToggleSuperArmorState(true);
        playerState.ChangePlayerState(PlayerStateType.ANIMAL_LEAPSTRIKE);

        if (playerMovement.Movement != Vector3.zero && !cameraController.CurrentTarget)
        {
            maskChange.CurrentMask.transform.forward = playerMovement.Movement;
        }

        isPerformingLeapStrike = true;
        canUseLeapStrike = false;
        leapStrikeStartTime = Time.time;

        playerSkillMove.GetOriginHeight();

        maskChange.CurrentAnimator.CrossFade(playerAnimation.Animal_LeapStrike, 0.1f);


        #region while ����
        bool activeMoveOnce = false;
        bool activeEffectOnce = false;
        bool activeHitBoxOnce = false;
        bool activeSoundOnce = false;
        bool activeCameraShakeOnce = false;
        bool activeTimeScaleOnce = false;
        #endregion

        playerSkillInput.ProcessInput(animalData.leapStrikeInput, leapStrikeStartTime);

        while (isPerformingLeapStrike)
        {
            //�������� �ִϸ��̼� �ϳ��� ����ϴ� ��������
            #region �ִϸ��̼� ����
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;
            if (animationHash == playerAnimation.Animal_LeapStrike)
            {
                isPerformingLeapStrikeAnim = true; //�ִ� ������
            }
            else if (animationHash == playerAnimation.Animal_Die)
            {
                InitializeSkill();
                yield break;
            }
            else
            {
                if (isPerformingLeapStrikeAnim)
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion


            #region ���� �̵�
            if (!activeMoveOnce)
            {
                for (int i = 0; i < animalData.leapStrikeSkillMove.Length; i++)
                {
                    playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(animalData.leapStrikeSkillMove[i]));
                }
                activeMoveOnce = true;
            }
            #endregion

            #region Restrict
            playerState.RestrictPlayer(animalData.leapStrikeRestrict, leapStrikeStartTime);
            #endregion

            #region ����Ʈ
            if (!activeEffectOnce)
            {
                //Ʈ����
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (animalData.leapStrikeTrailEffect, leapStrikeTrail, leapStrikeTrailPosition));

                //������
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (animalData.leapStrikeSlashEffect, leapStrikeEffect, leapStrikeEffectPosition));

                activeEffectOnce = true;
            }
            #endregion

            #region ���� ����
            if (Time.time >= leapStrikeStartTime + animalData.leapStrikeWeaponWaitTime + animalData.leapStrikeWeaponDuration)
            {
                rightHandWeaponMesh.enabled = false;
                leftHandWeaponMesh.enabled = false;

            }
            else if (Time.time >= leapStrikeStartTime + animalData.leapStrikeWeaponWaitTime)
            {
                rightHandWeaponMesh.enabled = true;
                leftHandWeaponMesh.enabled = true;
            }
            #endregion

            #region ��Ʈ�ڽ�
            //Invoke Ȱ��. ����ɾ���� ����Ϸ��� Initialize ���ٰ� bool������ Invoke �����ϵ��� �����

            if (!activeHitBoxOnce)
            {
                if (Time.time >= leapStrikeStartTime + animalData.leapStrikeHitBoxWaitTime)
                {
                    {
                        activeHitBoxOnce = true;
                        leapStrikeHitBox.SetActive(true);
                        rightHandWeapon.SetActive(true);
                        leftHandWeapon.SetActive(true);
                            
                        Invoke("LeapStrikeHitBoxOff", animalData.leapStrikeHitBoxDuration);
                    }
                }
            }
            #endregion

            #region �Ҹ�
            if (!activeSoundOnce)
            {
                playerSound.SetPlayerSound(animalData.leapStrikeJumpSound, player.transform.position,leapStrikeStartTime);
                playerSound.SetPlayerSound(animalData.leapStrikeFloatSound, player.transform.position, leapStrikeStartTime);
                playerSound.SetPlayerSound(animalData.leapStrikeSlashSound, player.transform.position, leapStrikeStartTime);

                activeSoundOnce = true;
            }
            #endregion

            #region ī�޶� ����ũ
            if (!activeCameraShakeOnce)
            {
                if (Time.time >= leapStrikeStartTime + animalData.leapStrikeCameraShake.waitTime)
                {
                    playerCameraEffect.ShakeCamera(animalData.leapStrikeCameraShake);
                    activeCameraShakeOnce = true;
                }
            }
            #endregion

            #region Ÿ�� ������
            if (!activeTimeScaleOnce)
            {
                gameTimeScale.StartCoroutine(gameTimeScale.CoSetTimeScale(animalData.leapStrikeGameTimeScale));
                activeTimeScaleOnce = true;
            }
            #endregion

            yield return null;
        }
    }
    public void LeapStrikeCooldown()
    {
        if (canUseLeapStrike) return;

        float flowTimeRate = (Time.time - leapStrikeStartTime) / animalData.leapStrikeStat.cooldown;
        skillHUD.SkillCooldown(PlayerStateType.ANIMAL_LEAPSTRIKE, flowTimeRate);

        if (Time.time > leapStrikeStartTime + animalData.leapStrikeStat.cooldown)
        {
            canUseLeapStrike = true;
        }
    }
    public void LeapStrikeHitBoxOff()
    {
        if (isPerformingLeapStrike)
        {
            leapStrikeHitBox.SetActive(false);
        }
    }
    #endregion

    #region Roar
    public void Roar()
    {
        UseSkill(canUseRoar, coRoar, CoRoar);
    }
    public IEnumerator CoRoar()
    {
        playerState.ToggleSuperArmorState(true);
        playerState.ChangePlayerState(PlayerStateType.ANIMAL_ROAR);
        playerState.ChangePlayerSubState(PlayerSubStateType.NONE);

        if (playerMovement.Movement != Vector3.zero && !cameraController.CurrentTarget)
        {
            maskChange.CurrentMask.transform.forward = playerMovement.Movement;
        }

        isPerformingRoar = true;
        canUseRoar = false;
        roarStartTime = Time.time;

        maskChange.CurrentAnimator.CrossFade(playerAnimation.Animal_Roar, 0.1f);

        #region while ����
        bool activeMoveOnce = false;
        bool activeEffectOnce = false;
        bool activeHitBoxOnce = false;
        bool activeSoundOnce = false;
        bool activeCameraShakeOnce = false;
        bool activeTimeScaleOnce = false;
        #endregion

        playerSkillInput.ProcessInput(animalData.roarInput, roarStartTime);

        while (isPerformingRoar)
        {
            #region �ִϸ��̼� ����
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;
            if (animationHash == playerAnimation.Animal_Roar)
            {
                isPerformingRoarAnim = true; //�ִ� ������
            }
            else if (animationHash == playerAnimation.Animal_Die)
            {
                InitializeSkill();
                yield break;
            }
            else
            {
                if (isPerformingRoarAnim)
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion

            #region Restrict
            playerState.RestrictPlayer(animalData.roarRestrict, roarStartTime);
            #endregion

            #region ����Ʈ
            if (!activeEffectOnce)
            {
                //Ʈ����
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (animalData.roarTrailEffect, roarTrailEffect, rightHandWeapon));
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (animalData.roarTrailEffect, roarTrailEffect, leftHandWeapon));

                //��¡
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (animalData.roarChargeEffect, roarChargeEffect, roarEffectPosition));

                //��¡
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (animalData.roardisChargeEffect, roarDisChargeEffect, roarEffectPosition));
                activeEffectOnce = true;
            }
            #endregion

            #region ���� ����
            if (Time.time >= roarStartTime + animalData.roarWeaponWaitTime + animalData.roarWeaponDuration)
            {
                rightHandWeaponMesh.enabled = false;
                leftHandWeaponMesh.enabled = false;
            }
            else if (Time.time >= roarStartTime + animalData.roarWeaponWaitTime)
            {
                rightHandWeaponMesh.enabled = true;
                leftHandWeaponMesh.enabled = true;
            }
            #endregion

            #region ��Ʈ�ڽ�
            if (Time.time >= roarStartTime + animalData.roarHitBoxWaitTime)
            {
                if (!activeHitBoxOnce)
                {
                    activeHitBoxOnce = true;

                    roarHitBox.SetActive(true);
                    roarHitBox.transform.position = roarEffectPosition.transform.position;
                    roarHitBox.transform.localScale = animalData.roarHitBoxScale;

                    //Invoke�� �ð��� 0���� �θ� ������ Updata �Լ� ����Ŭ�� �����
                    InvokeRepeating("RoarHitBoxOn", 0.01f, animalData.roarHitInterval);
                }
            }
            #endregion

            #region �Ҹ�
            if (!activeSoundOnce)
            {
                playerSound.SetPlayerSound(animalData.roarChargeSound, player.transform.position, roarStartTime);
                playerSound.SetPlayerSound(animalData.roarDischargeSound, player.transform.position, roarStartTime);
                playerSound.SetPlayerSound(animalData.roarSound, player.transform.position, roarStartTime);

                activeSoundOnce = true;
            }
            #endregion

            #region ī�޶� ����ũ
            if (!activeCameraShakeOnce && (Time.time >= roarStartTime + animalData.roarCameraShake.waitTime))
            {
                playerCameraEffect.ShakeCamera(animalData.roarCameraShake);
                activeCameraShakeOnce = true;
            }
            #endregion

            #region Ÿ�� ������
            if (!activeTimeScaleOnce)
            {
                gameTimeScale.StartCoroutine(gameTimeScale.CoSetTimeScale(animalData.roarGameTimeScale));
                activeTimeScaleOnce = true;
            }
            #endregion

            yield return null;
        }
    }
    public void roarCooldown()
    {
        if (canUseRoar) return;

        if (Time.time > roarStartTime + animalData.roarStat.cooldown)
        {
            canUseRoar = true;
        }
    }
    public void RoarHitBoxOn()
    {
        roarHitCount++;

        roarHitBox.SetActive(true);

        Invoke("RoarHitBoxOff", animalData.roarHitInterval - 0.1f);

        if (roarHitCount >= animalData.roarHitCount)
        {
            CancelInvoke("RoarHitBoxOn");
            roarHitCount = 0;
        }
    }
    public void RoarHitBoxOff()
    {
        roarHitBox.SetActive(false);
    }


    #endregion

    #region Dash
    public void Dash()
    {
        UseSkill(canUseDash, coDash, CoDash);
    }
    public IEnumerator CoDash()
    {
        playerState.ChangePlayerState(PlayerStateType.DASH);
        playerState.ChangePlayerSubState(PlayerSubStateType.NONE);

        if (playerMovement.Movement != Vector3.zero && !cameraController.CurrentTarget)
        {
            maskChange.CurrentMask.transform.forward = playerMovement.Movement;
        }

        isPerformingDash = true;
        dashStartTime = Time.time;
        canUseDash = false;

        bool isFrontDash = false;

        if (cameraController.CurrentTarget)
        {
            isFrontDash = false;
            maskChange.CurrentAnimator.CrossFade(playerAnimation.Animal_BackDash, 0.1f);
            playerSkillInput.ProcessInput(commonData.backDashInput, dashStartTime);
        }
        else
        {
            isFrontDash = true;
            maskChange.CurrentAnimator.CrossFade(playerAnimation.Animal_FrontDash, 0.1f);
            playerSkillInput.ProcessInput(commonData.dashInput, dashStartTime);
        }

        #region ����
        bool activeSoundOnce = false;

        bool activeMoveOnce = false;

        bool activeRestrictOnce = false;
        #endregion

        while (isPerformingDash)
        {
            #region �ִϸ��̼� ����
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;

            if ((animationHash == playerAnimation.Animal_FrontDash) || (animationHash == playerAnimation.Animal_BackDash))
            {
                isPerformingDashAnim = true; //�ִ� ������
            }
            else if ((animationHash == playerAnimation.Animal_Hit) || (animationHash == playerAnimation.Animal_Die))
            {
                InitializeSkill();
                yield break;
            }
            else
            {
                if (isPerformingDashAnim)
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion

            #region ���� �̵�
            if (!isFrontDash)
            {
                if (!activeMoveOnce)
                {
                    for (int i = 0; i < commonData.backDashMove.Length; i++)
                    {
                        playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(commonData.backDashMove[i]));
                    }
                    activeMoveOnce = true;
                }
            }
            else
            {
                if (!activeMoveOnce)
                {
                    for (int i = 0; i < commonData.dashMove.Length; i++)
                    {
                        playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(commonData.dashMove[i]));
                    }
                    activeMoveOnce = true;
                }
            }
            #endregion

            #region ����
            if (!activeRestrictOnce)
            {
                if (isFrontDash)
                {
                    playerState.RestrictPlayer(commonData.dashRestrict, dashStartTime);

                }
                else
                {
                    playerState.RestrictPlayer(commonData.backDashRestrict, dashStartTime);
                }

                activeRestrictOnce = true;
            }
            #endregion

            #region �Ҹ�
            if (!activeSoundOnce)
            {
                if (isFrontDash)
                {
                    playerSound.SetPlayerSound(commonData.dashSound, player.transform.position, dashStartTime);
                }
                else
                {
                    playerSound.SetPlayerSound(commonData.backDashSound, player.transform.position, dashStartTime);
                }

                activeSoundOnce = true;
            }
            #endregion

            yield return null;
        }
    }
    public void DashCooldown()
    {
        if (canUseDash) return;

        float flowTimeRate = (Time.time - dashStartTime) / commonData.dashCooldown;
        skillHUD.SkillCooldown(PlayerStateType.DASH, flowTimeRate);

        if (Time.time > dashStartTime + commonData.dashCooldown)
        {
            canUseDash = true;
            return;
        }
    }
    #endregion
}
