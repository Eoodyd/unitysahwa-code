using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanMaskSkill : PlayerSkill
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
    [SerializeField] private PlayerHitBox playerHitBox;
    [SerializeField] private PlayerSkillInput playerSkillInput;
    [SerializeField] private GameTimeScale gameTimeScale;
    #endregion

    #region ��밡�� ����
    public bool canUseFirstAttack {get; private set;}
    public bool canUseSecondAttack {get; private set;}
    public bool canUseThirdAttack {get; private set;}
    public bool canUseInkShape {get; private set;}
    public bool canUseInkFloor { get; private set; }
    public bool canUseDash { get; private set; }
    #endregion

    #region ��ų ������ ����
    private bool isPerformingFirstAttack;
    private bool isPerformingSecondAttack;
    private bool isPerformingThirdAttack;
    private bool isPerformingInkShape;
    private bool isPerformingInkFloor;
    private bool isPerformingDash;


    //�ִ� ����
    private bool isPerformingFirstAttackAnim;
    private bool isPerformingSecondAttackAnim;
    private bool isPerformingThirdAttackAnim;
    private bool isPerformingInkShapeAnim;
    private bool isPerformingInkFloorAnim;
    private bool isPerformingDashAnim;
    #endregion

    #region �����ð�
    private float firstAttackStartTime;
    private float secondAttackStartTime;
    private float thirdAttackStartTime;
    private float inkShapeStartTime;
    private float inkFloorStartTime;
    private float dashStartTime;
    #endregion

    #region ��ų �ڷ�ƾ
    public Coroutine coFirstAttack {get; private set;}
    public Coroutine coSecondAttack {get; private set;}
    public Coroutine coThirdAttack {get; private set;}
    public Coroutine coInkShape {get; private set;}
    public Coroutine coInkFloor {get; private set;}
    public Coroutine coDash {get; private set;}
    #endregion

    #region NoarmalAttack
    [Space(20)]
    [SerializeField] private GameObject normalAttackHitBox;

    [SerializeField] private GameObject firstAttackEffectPosition;
    [SerializeField] private GameObject secondAttackEffectPosition;
    [SerializeField] private GameObject thirdAttackEffectPosition;

    [SerializeField] private GameObject[] firstAttackEffect;
    [SerializeField] private GameObject[] secondAttackEffect;
    [SerializeField] private GameObject[] thirdAttackEffect;
    #endregion  

    #region InkShape
    [Space(20)]
    private int inkShapeHitCount = 0;

    [SerializeField] private GameObject inkShapeHitBox;

    [SerializeField] private GameObject inkShapeEffectPosition;
    [SerializeField] private GameObject inkShapeTrailPosition;

    [SerializeField] private GameObject[] inkShapeSplashEffect;
    [SerializeField] private GameObject[] inkShapeTrail;
    #endregion

    #region InkFloor
    private int[] inkFloorHitCount = new int[2];

    [Space(20)]
    [SerializeField] private GameObject[] inkFloorProjectileEffect;
    [SerializeField] private GameObject[] inkFloorHitBox;
    [SerializeField] private GameObject inkFloorProjectileEffectPosition;

    #endregion

    public delegate IEnumerator CoroutineDelegate();

    private void Start()
    {
        StartSet();
        HumanMaskStartSet();
    }
    
    #region Initialize
    public void InitializeSkill()
    {
        InitializeCoroutine();
        InitializeState();

        playerSkillMove.Initialize();
        playerEffect.Initialize();
        playerState.Initialize();
        playerSound.Initialize();
        playerHitBox.Initialize();
        playerSkillInput.Initialize();
        gameTimeScale.Initialize();

    }

    public void InitializeCoroutine()
    {
        if (coFirstAttack != null) StopCoroutine(coFirstAttack);
        if (coSecondAttack != null) StopCoroutine(coSecondAttack);
        if (coThirdAttack != null) StopCoroutine(coThirdAttack);
        if (coInkShape != null) StopCoroutine(coInkShape);
        if (coInkFloor != null) StopCoroutine(coInkFloor);
        if (coDash != null) StopCoroutine(coDash);
    }

    public void InitializeState()
    {
        #region �⺻����
        isPerformingFirstAttack = false;
        isPerformingSecondAttack = false;
        isPerformingThirdAttack = false;

        isPerformingFirstAttackAnim = false;
        isPerformingSecondAttackAnim = false;
        isPerformingThirdAttackAnim = false;

        //��뿩��(��Ÿ ��Ÿ��X)
        canUseFirstAttack = true; //��Ÿ�� ��Ÿ�� ����
        canUseSecondAttack = false; //��Ÿ1 �Լ����� true�� �ٲ�
        canUseThirdAttack = false; //��Ÿ2 �Լ����� true�� �ٲ�
        #endregion

        #region ��ų
        isPerformingInkShape = false;
        isPerformingInkFloor = false;
        isPerformingDash = false;
        
        isPerformingInkShapeAnim = false;
        isPerformingInkFloorAnim = false;
        isPerformingDashAnim = false;
        #endregion
    }

    //���� �ʱ�ȭ �Լ� ���� ����
    #endregion

    #region Skill
    private void HumanMaskStartSet()
    {
        #region canUse
        canUseFirstAttack = true;
        canUseSecondAttack = false;
        canUseThirdAttack = false;
        canUseInkShape = true;
        canUseInkFloor = true;
        canUseDash = true;
        #endregion

        #region Perform
        isPerformingFirstAttack = false;
        isPerformingSecondAttack = false;
        isPerformingThirdAttack = false;
        isPerformingInkShape = false;
        isPerformingInkFloor = false;
        isPerformingDash = false;

        isPerformingFirstAttackAnim = false;
        isPerformingSecondAttackAnim = false;
        isPerformingThirdAttackAnim = false;
        isPerformingInkShapeAnim = false;
        isPerformingInkFloorAnim = false;
        isPerformingDashAnim = false;
        #endregion

        coFirstAttack = null;
        coSecondAttack = null;
        coThirdAttack = null;
        coInkShape = null;
        coInkFloor = null;
        coDash = null;

        inkShapeHitCount = 0;
        inkFloorHitCount[0] = 0;
        inkFloorHitCount[1] = 0;

        //���ӿ�����Ʈ �¿�Ƽ�� �ʿ�
        normalAttackHitBox.SetActive(false);
        inkShapeHitBox.SetActive(false);
        for (int i = 0; i < inkShapeSplashEffect.Length; i++)
        {
            inkShapeSplashEffect[i].SetActive(false);
        }
        for (int i = 0; i < inkShapeTrail.Length; i++)
        {
            inkShapeTrail[i].SetActive(false);
        }
        for (int i = 0; i < inkFloorProjectileEffect.Length; i++)
        {
            inkFloorProjectileEffect[i].SetActive(false);
        }
        for (int i = 0; i < inkFloorHitBox.Length; i++)
        {
            inkFloorHitBox[i].SetActive(false);
        }
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

    #endregion

    #region Normal Attack
    public void NormalAttack()
    {
        {
            UseSkill(canUseFirstAttack, coFirstAttack, CoFirstAttack);
        }
    }

    //NormalizedTime <= 0.9f �϶��� ���� ��Ÿ �������� ���� �ٿ��� �ɵ���(�Ǵ� ������)
    public IEnumerator CoFirstAttack()
    {

        maskChange.CurrentAnimator.CrossFade(playerAnimation.Human_FirstNormalAttack, 0.1f);

        playerState.ChangePlayerState(PlayerStateType.HUMAN_NORMALATTACK);
        playerState.ChangePlayerSubState(PlayerSubStateType.HUMAN_FIRSTNORMALATTACK);

        isPerformingFirstAttack = true;
        canUseFirstAttack = false;
        firstAttackStartTime = Time.time;

        #region while ����
        bool activeMoveOnce = false;
        bool activeEffectOnce = false;
        bool activeHitBoxOnce = false;
        bool activeSoundOnce = false;
        bool activeCameraShakeOnce = false;
        #endregion

        playerSkillInput.ProcessInput(humanData.firstNormalAttackInput, firstAttackStartTime);
        while (isPerformingFirstAttack)
        {
            #region Animation State
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;

            if (animationHash == playerAnimation.Human_FirstNormalAttack)
            {
                isPerformingFirstAttackAnim = true;
            }
            else if ((animationHash == playerAnimation.Human_Hit) || (animationHash == playerAnimation.Human_Die))
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

            #region ���� �̵�
            if (!activeMoveOnce)
            {
                for (int i = 0; i < humanData.firstNormalAttackMove.Length; i++)
                {
                    playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(humanData.firstNormalAttackMove[i]));
                }
                activeMoveOnce = true;
            }
            #endregion

            #region ����
            playerState.RestrictPlayer(humanData.firstNormalAttackRestrict,firstAttackStartTime);

            //DoNotAct�� duration���� ���� ��Ȳ���� �����̸� ��ų ���� 
            if (isPerformingFirstAttackAnim && (Time.time >= firstAttackStartTime + humanData.firstNormalAttackRestrict.actRestrictWaitTime))
            {
                canUseSecondAttack = true;

                if (maskChange.CurrentAnimator.GetFloat("moveAmount") > .4f) // �����̸� �ʱ�ȭ
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion

            #region ����Ʈ
            if (!activeEffectOnce)
            {
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (humanData.firstNormalAttackSkillEffect, firstAttackEffect, firstAttackEffectPosition));
                activeEffectOnce = true;
            }
            #endregion

            #region ��Ʈ�ڽ�
            if (!activeHitBoxOnce)
            {
                playerHitBox.StartCoroutine(playerHitBox.TogglePlayerHitBox(humanData.firstNormalAttackHitBox));
                activeHitBoxOnce = true;
            }
            #endregion

            #region �Ҹ�
            if (!activeSoundOnce)
            {
                playerSound.SetPlayerSound(humanData.firstNormalAttackSound, player.transform.position, firstAttackStartTime);
                activeSoundOnce = true;
            }
            #endregion

            #region ī�޶� ����ũ
            if (!activeCameraShakeOnce && (Time.time >= firstAttackStartTime + humanData.firstNormalAttackCameraShake.waitTime))
            {
                playerCameraEffect.ShakeCamera(humanData.firstNormalAttackCameraShake);
                activeCameraShakeOnce = true;
            }
            #endregion

            yield return null;
        }
    }
    public IEnumerator CoSecondAttack()
    {
        playerState.ToggleSuperArmorState(true);

        maskChange.CurrentAnimator.CrossFade(playerAnimation.Human_SecondNormalAttack, 0.1f);

        playerState.ChangePlayerState(PlayerStateType.HUMAN_NORMALATTACK);
        playerState.ChangePlayerSubState(PlayerSubStateType.HUMAN_SECONDNORMALATTACK);

        isPerformingSecondAttack = true;
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

        playerSkillInput.ProcessInput(humanData.secondNormalAttackInput, secondAttackStartTime);

        while (isPerformingSecondAttack)
        {
            #region Animation State
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;

            if (animationHash == playerAnimation.Human_SecondNormalAttack)
            {
                isPerformingSecondAttackAnim = true;
            }
            else if ((animationHash == playerAnimation.Human_Hit) || (animationHash == playerAnimation.Human_Die))
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
                for (int i = 0; i < humanData.secondNormalAttackMove.Length; i++)
                {
                    playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(humanData.secondNormalAttackMove[i]));
                }
                activeMoveOnce = true;
            }
            #endregion

            #region Restrict
            playerState.RestrictPlayer(humanData.secondNormalAttackRestrict, secondAttackStartTime);


            //DoNotAct�� duration���� ���� ��Ȳ���� �����̸� ��ų ����
            if (isPerformingSecondAttackAnim && Time.time >= secondAttackStartTime + humanData.secondNormalAttackRestrict.actRestrictWaitTime)
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
                    (humanData.secondNormalAttackSkillEffect, secondAttackEffect, secondAttackEffectPosition));
                activeEffectOnce = true;
            }
            #endregion

            #region HitBox
            if (!activeHitBoxOnce)
            {
                playerHitBox.StartCoroutine(playerHitBox.TogglePlayerHitBox(humanData.secondNormalAttackHitBox));
                activeHitBoxOnce = true;
            }
            #endregion

            #region Sound
            if (!activeSoundOnce)
            {
                playerSound.SetPlayerSound(humanData.secondNormalAttackSound, player.transform.position, secondAttackStartTime);
                activeSoundOnce = true;
            }
            #endregion

            #region ī�޶� ����ũ
            if (!activeCameraShakeOnce && (Time.time >= secondAttackStartTime + humanData.secondNormalAttackCameraShake.waitTime))
            {
                playerCameraEffect.ShakeCamera(humanData.secondNormalAttackCameraShake);
                activeCameraShakeOnce = true;
            }
            #endregion

            yield return null;
        }
    }
    public IEnumerator CoThirdAttack()
    {
        playerState.ToggleSuperArmorState(true);

        maskChange.CurrentAnimator.CrossFade(playerAnimation.Human_ThirdNormalAttack, 0.1f);

        playerState.ChangePlayerState(PlayerStateType.HUMAN_NORMALATTACK);
        playerState.ChangePlayerSubState(PlayerSubStateType.HUMAN_THIRDNORMALATTACK);

        isPerformingThirdAttack = true;
        canUseThirdAttack = false;
        thirdAttackStartTime = Time.time;

        #region while ����
        bool activeSoundOnce = false;
        bool activeEffectOnce = false;
        bool activeHitBoxOnce = false;
        bool inactiveHitBoxOnce = false;

        bool activeMoveOnce = false;

        bool activeCameraShakeOnce = false;

        bool activeRestrictOnce = false;

        bool activeTimeScaleOnce = false;

        int? actIndex = null;
        #endregion

        playerSkillInput.ProcessInput(humanData.thirdNormalAttackInput, thirdAttackStartTime);

        while (isPerformingThirdAttack)
        {
            #region �ִϸ��̼� ����
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;

            if (animationHash == playerAnimation.Human_ThirdNormalAttack)
            {
                isPerformingThirdAttackAnim = true;
            }
            else if ((animationHash == playerAnimation.Human_Hit) || (animationHash == playerAnimation.Human_Die))
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

            #region ���� �̵�
            if (!activeMoveOnce)
            {
                for (int i = 0; i < humanData.thirdNormalAttackMove.Length; i++)
                {
                    playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(humanData.thirdNormalAttackMove[i]));
                }
                activeMoveOnce = true;
            }

            #endregion

            #region ����
            playerState.RestrictPlayer(humanData.thirdNormalAttackRestrict, thirdAttackStartTime);


            //DoNotAct�� duration���� ���� ��Ȳ���� �����̸� ��ų ���� 
            if (isPerformingThirdAttackAnim && Time.time >= thirdAttackStartTime + humanData.thirdNormalAttackRestrict.actRestrictWaitTime)
            {
                if (maskChange.CurrentAnimator.GetFloat("moveAmount") > .9f) // �����̸� �ʱ�ȭ
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion 
            
            #region ����Ʈ
            if (!activeEffectOnce)
            {
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (humanData.thirdNormalAttackSkillEffect, thirdAttackEffect, thirdAttackEffectPosition));
                activeEffectOnce = true;
            }
            #endregion
            
            #region HitBox
            if (!activeHitBoxOnce)
            {
                playerHitBox.StartCoroutine(playerHitBox.TogglePlayerHitBox(humanData.thirdNormalAttackHitBox));
                activeHitBoxOnce = true;
            }
            #endregion

            #region Sound
            if (!activeSoundOnce)
            {
                playerSound.SetPlayerSound(humanData.thirdNormalAttackSound, player.transform.position, thirdAttackStartTime);
                activeSoundOnce = true;
            }

            #endregion

            #region ī�޶� ����ũ
            if (!activeCameraShakeOnce && (Time.time >= thirdAttackStartTime + humanData.thirdNormalAttackCameraShake.waitTime))
            {
                playerCameraEffect.ShakeCamera(humanData.thirdNormalAttackCameraShake);
                activeCameraShakeOnce = true;
            }
            #endregion

            #region Ÿ�� ������
            if (!activeTimeScaleOnce)
            {
                gameTimeScale.StartCoroutine( gameTimeScale.CoSetTimeScale(humanData.thirdNormalAttackGameTimeScale));
                activeTimeScaleOnce = true;
            }
            #endregion

            yield return null;
        }
    }
    #endregion

    #region InkShape
    public void InkShape()
    {
        UseSkill(canUseInkShape, coInkShape, CoInkShape);
    }   
    public IEnumerator CoInkShape()
    {
        playerState.ToggleSuperArmorState(true);
        playerState.ChangePlayerState(PlayerStateType.HUMAN_INKSHAPE);
        playerState.ChangePlayerSubState(PlayerSubStateType.NONE);

        if (playerMovement.Movement != Vector3.zero && !cameraController.CurrentTarget)
        {
            maskChange.CurrentMask.transform.forward = playerMovement.Movement;
        }

        isPerformingInkShape = true;
        canUseInkShape = false;
        inkShapeStartTime = Time.time;

        playerSkillMove.GetOriginHeight(); //�տ� ��ֹ� Ž��

        maskChange.CurrentAnimator.CrossFade(playerAnimation.Human_InkShape, .1f);

        #region while ����
        bool activeMoveOnce = false;
        bool activeEffectOnce = false;
        bool activeHitBoxOnce = false;
        bool activeSoundOnce = false;
        bool activeCameraShakeOnce = false;
        bool activeTimeScaleOnce = false;
        #endregion

        playerSkillInput.ProcessInput(humanData.inkShapeInput, inkShapeStartTime);

        while (isPerformingInkShape)
        {
            #region �ִϸ��̼� ����
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;
            if (animationHash == playerAnimation.Human_InkShape)
            {
                isPerformingInkShapeAnim = true; //�ִ� ������
            }
            else
            {
                if (isPerformingInkShapeAnim)
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion

            #region ���� �̵�
            if (!activeMoveOnce)
            {
                for (int i = 0; i < humanData.inkShapeMove.Length; i++)
                {
                    playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(humanData.inkShapeMove[i]));
                }
                activeMoveOnce = true;
            }
            #endregion

            #region Restrict
            playerState.RestrictPlayer(humanData.inkShapeRestrict, inkShapeStartTime);
            #endregion

            #region ����Ʈ
            if (!activeEffectOnce)
            {
                //Ʈ����
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (humanData.inkShapeSpinTrailEffect, inkShapeTrail, inkShapeTrailPosition));
                
                //���÷���
                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (humanData.inkShapeSplashEffect, inkShapeSplashEffect, inkShapeEffectPosition));
                activeEffectOnce = true;
            }
            #endregion

            #region ��Ʈ�ڽ�
            if (Time.time >= inkShapeStartTime + humanData.inkShapeHitBoxWaitTime)
            {
                if (!activeHitBoxOnce)
                {
                    activeHitBoxOnce = true;

                    inkShapeHitBox.SetActive(true);
                    inkShapeHitBox.transform.position = inkShapeEffectPosition.transform.position;
                    inkShapeHitBox.transform.rotation = inkShapeEffectPosition.transform.rotation;

                    InvokeRepeating("InkShapeHitBoxOn", 0, humanData.inkShapeHitInterval);
                }
            }
            #endregion

            #region �Ҹ�
            if (!activeSoundOnce)
            {
                playerSound.SetPlayerSound(humanData.inkShapeSpinSound, player.transform.position, inkShapeStartTime);
                playerSound.SetPlayerSound(humanData.inkShapeSplashSound, player.transform.position, inkShapeStartTime);
                activeSoundOnce = true;
            }
            #endregion

            #region ī�޶� ����ũ
            if (!activeCameraShakeOnce && (Time.time >= inkShapeStartTime + humanData.inkShapeCameraShake.waitTime))
            {
                playerCameraEffect.ShakeCamera(humanData.inkShapeCameraShake);
                activeCameraShakeOnce = true;
            }
            #endregion

            #region Ÿ�� ������
            if (!activeTimeScaleOnce)
            {
                gameTimeScale.StartCoroutine( gameTimeScale.CoSetTimeScale(humanData.inkShapeGameTimeScale));
                activeTimeScaleOnce = true;
            }
            #endregion

            yield return null;
        }
    }
    public void InkShapeCooldown()
    {
        if (canUseInkShape) return;

        float flowTimeRate = (Time.time - inkShapeStartTime) / humanData.inkShapeStat.cooldown;
        skillHUD.SkillCooldown(PlayerStateType.HUMAN_INKSHAPE, flowTimeRate);

        if (Time.time > inkShapeStartTime + humanData.inkShapeStat.cooldown)
        {
            canUseInkShape = true;
        }
    }
    public void InkShapeHitBoxOn()
    {
        inkShapeHitCount++;

        inkShapeHitBox.SetActive(true);
        Invoke("InkShapeHitBoxOff", humanData.inkShapeHitInterval - 0.1f);

        if (inkShapeHitCount >= humanData.inkShapeHitCount)
        {
            CancelInvoke("InkShapeHitBoxOn");
            inkShapeHitCount = 0;
        }
    }
    public void InkShapeHitBoxOff()
    {
        inkShapeHitBox.SetActive(false);
    }
    #endregion

    #region InkFloor
    public void InkFloor()
    {
        if (cameraController.CurrentTarget == null)
        {
            return;
        }

        UseSkill(canUseInkFloor, coInkShape, CoInkFloor);
    }
    public IEnumerator CoInkFloor()
    {
        Collider target = null;
        target = cameraController.CurrentTarget;

        playerState.ToggleSuperArmorState(true);
        playerState.ChangePlayerState(PlayerStateType.HUMAN_INKFLOOR);
        playerState.ChangePlayerSubState(PlayerSubStateType.NONE);

        if (playerMovement.Movement != Vector3.zero && !cameraController.CurrentTarget)
        {
            maskChange.CurrentMask.transform.forward = playerMovement.Movement;
        }

        isPerformingInkFloor = true;
        canUseInkFloor = false;
        inkFloorStartTime = Time.time;

        maskChange.CurrentAnimator.CrossFade(playerAnimation.Human_InkFloor, 0.1f);

        #region while ����
        bool activeSound1Once = false;
        bool activeSound2Once = false;

        bool activeMoveOnce = false;

        bool activeEffectOnce = false;
        bool inactiveEffectOnce = false;

        bool activeHitBoxOnce = false;
        bool inactiveHitBoxOnce = false;

        bool activeCameraShakeOnce = false;

        bool activeRestrictOnce = false;

        bool activeTimeScaleOnce = false;

        #endregion

        playerSkillInput.ProcessInput(humanData.inkFloorInput, inkFloorStartTime);


        while (isPerformingInkFloor)
        {
            #region �ִϸ��̼� ���� ��Ȳ
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;

            if (animationHash == playerAnimation.Human_InkFloor)
            {
                isPerformingInkFloorAnim = true; //�ִ� ������
            }
            else if (animationHash == playerAnimation.Human_Die)
            {
                InitializeSkill();
                yield break;
            }
            else
            {
                if (isPerformingInkFloorAnim)
                {
                    InitializeSkill();
                    yield break;
                }
            }
            #endregion

            #region ���� �̵�
            if (!activeMoveOnce)
            {
                for (int i = 0; i < humanData.inkFloorMove.Length; i++)
                {
                    playerSkillMove.StartCoroutine(playerSkillMove.SkillMove(humanData.inkFloorMove[i]));
                }
                activeMoveOnce = true;
            }
            #endregion

            #region ����
            playerState.RestrictPlayer(humanData.inkFloorRestrict, inkFloorStartTime);

            #endregion

            #region �߻�ü ����Ʈ
            if (!activeEffectOnce)
            {
                inkFloorProjectileEffectPosition.transform.position = target.transform.position;
                inkFloorProjectileEffectPosition.transform.rotation = Quaternion.identity;

                playerEffect.StartCoroutine(playerEffect.TogglePlayerEffect
                    (humanData.inkFloorProjectileEffect, inkFloorProjectileEffect, inkFloorProjectileEffectPosition));
                activeEffectOnce = true;
            }
            #endregion

            #region ��Ʈ�ڽ�_�߻�ü -> Ƚ�� �����ؾ���
            if (Time.time >= inkFloorStartTime + humanData.inkFloorHitBoxWaitTime)
            {
                if (!cameraController.CurrentTarget) { }
                else if (!cameraController.CurrentTarget.gameObject.GetComponent<Enemy>()) { }
                else if (cameraController.CurrentTarget.gameObject.GetComponent<Enemy>().isDead) { }
                else
                {
                    if (!activeHitBoxOnce)
                    {
                        activeHitBoxOnce = true;
                        if (inkFloorHitBox[0].activeSelf)
                        {
                            inkFloorHitBox[1].transform.localScale = humanData.inkFloorScale;
                            inkFloorHitBox[1].transform.position = cameraController.CurrentTarget.transform.position;
                            InvokeRepeating("InkFloorHitBoxOn2", 0, humanData.inkFloorHitInterval);
                        }
                        else
                        {
                            inkFloorHitBox[0].transform.localScale = humanData.inkFloorScale;
                            inkFloorHitBox[0].transform.position = cameraController.CurrentTarget.transform.position;
                            InvokeRepeating("InkFloorHitBoxOn1", 0, humanData.inkFloorHitInterval);
                        }
                    }
                }
            }
            #endregion

            #region ����, �߻�ü �Ҹ�
            if (!activeSound1Once)
            {
                playerSound.SetPlayerSound(humanData.inkFloorSwingSound, player.transform.position, inkFloorStartTime);
                activeSound1Once = true;
            }
            if (!activeSound2Once)
            {
                playerSound.SetPlayerSound(humanData.inkFloorProjectileSound, target.transform.position, inkFloorStartTime);
                activeSound2Once = true;
            }
            #endregion

            #region ī�޶� ����ũ
            if (!activeCameraShakeOnce && (Time.time >= inkFloorStartTime + humanData.inkFloorCameraShake.waitTime))
            {
                playerCameraEffect.ShakeCamera(humanData.inkFloorCameraShake);
                activeCameraShakeOnce = true;
            }
            #endregion

            #region Ÿ�� ������
            if (!activeTimeScaleOnce)
            {
                gameTimeScale.StartCoroutine(  gameTimeScale.CoSetTimeScale(humanData.inkFloorGameTimeScale));
                activeTimeScaleOnce = true;
            }
            #endregion

            yield return null;
        }
    }
    public void InkFloorCooldown()
    {
        if (canUseInkFloor) return;

        if (Time.time > inkFloorStartTime + humanData.inkFloorStat.cooldown)
        {
            canUseInkFloor = true;
        }
    }
    public void InkFloorHitBoxOn1()
    {
        inkFloorHitCount[0]++;

        inkFloorHitBox[0].SetActive(true);
        
        Invoke("InkFloorHitBoxOff1", humanData.inkFloorHitInterval - 0.1f);

        if (inkFloorHitCount[0] >= humanData.inkFloorHitCount)
        {
            CancelInvoke("InkFloorHitBoxOn1");
            inkFloorHitCount[0] = 0;
        }
    }
    public void InkFloorHitBoxOff1()
    {
        inkFloorHitBox[0].SetActive(false);
    }
    public void InkFloorHitBoxOn2()
    {
        inkFloorHitCount[1]++;

        inkFloorHitBox[1].SetActive(true);

        Invoke("InkFloorHitBoxOff2", humanData.inkFloorHitInterval - 0.1f);

        if (inkFloorHitCount[1] >= humanData.inkFloorHitCount)
        {
            CancelInvoke("InkFloorHitBoxOn2");
            inkFloorHitCount[1] = 0;
        }
    }
    public void InkFloorHitBoxOff2()
    {
        inkFloorHitBox[0].SetActive(false);
    }
    #endregion

    #region Dash
    public void Dash()
    {
        UseSkill(canUseDash, coDash, CoDash);
    }
    public IEnumerator CoDash()
    {
        if (playerMovement.Movement != Vector3.zero && !cameraController.CurrentTarget)
        {
            maskChange.CurrentMask.transform.forward = playerMovement.Movement;
        }

        playerState.ChangePlayerState(PlayerStateType.DASH);
        playerState.ChangePlayerSubState(PlayerSubStateType.NONE);

        isPerformingDash = true;
        dashStartTime = Time.time;
        canUseDash = false;
        
        bool isFrontDash = false;

        if (cameraController.CurrentTarget)
        {
            isFrontDash = false;
            maskChange.CurrentAnimator.CrossFade(playerAnimation.Human_BackDash, 0.1f);
            playerSkillInput.ProcessInput(commonData.backDashInput, dashStartTime);
        }
        else
        {
            isFrontDash = true;
            maskChange.CurrentAnimator.CrossFade(playerAnimation.Human_FrontDash, 0.1f);
            playerSkillInput.ProcessInput(commonData.dashInput, dashStartTime);
        }

        #region ����
        bool activeSoundOnce = false;

        bool activeMoveOnce = false;
        #endregion


        while (isPerformingDash)
        {
            #region �ִϸ��̼� ����
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;

            if ((animationHash == playerAnimation.Human_FrontDash) || (animationHash == playerAnimation.Human_BackDash))
            {
                isPerformingDashAnim = true; //�ִ� ������
            }
            else if ((animationHash == playerAnimation.Human_Hit) || (animationHash == playerAnimation.Human_Die))
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
            if (isFrontDash)
            {
                playerState.RestrictPlayer(commonData.dashRestrict, dashStartTime);

            }
            else
            {
                playerState.RestrictPlayer(commonData.backDashRestrict, dashStartTime);
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
        }
    }
    #endregion
}