using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public partial class Player : MonoBehaviour, IDamageable
{
    public static Player instance;

    #region �ܺ�
    [SerializeField] private PlayerController playerController;
    [SerializeField] private MaskChange maskChange;
    public HpHUD hpHUD;

    [SerializeField] private HumanMaskSkill humanSkill;
    [SerializeField] private AnimalMaskSkill animalSkill;

    private SaveManager saveManager;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private PlayerSound playerSound;
    

    [SerializeField] private PlayerCameraEffect playerCameraEffect;
    [SerializeField] private LoadingUI loadingUI;
    [SerializeField] private UIEffect UIEffect;

    //������
    private PlayerCommonData commonData;
    #endregion

    //����ü��
    public float currentHP;

    //�ǰ�
    private float hitStartTime;
    [SerializeField] private bool canUseHitAction;
    
    //TODO: ��¼�� RestrictPlayer�� �ߺ� ����Ҽ���
    private bool isPerformingHitAction = false;
    public bool IsPerformingHitAction
    {
        get { return isPerformingHitAction; }
    }

    private bool isPerformingHitActionAnim;
    private bool isPlayerFalling;

    private void Awake()
    {
        #region �̱���
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this.gameObject);
        #endregion
    }
    private void Start()
    {
        commonData = PlayerCommonData.Instance;
        saveManager = SaveManager.instance;
        
        SetUp();
    }

    public void SetUp()
    {
        //�������� ü�°�������

        currentHP = commonData.maxHp;
        canUseHitAction = true;
        isPlayerFalling = false;

    }

    public void InitializeSkill()
    {
        isPerformingHitActionAnim = false;
        isPerformingHitAction = false;
    }

    public void FollowCharacterObject()
    {
        this.gameObject.transform.position = maskChange.CurrentMask.transform.position;
        this.gameObject.transform.rotation = maskChange.CurrentMask.transform.rotation;
    }

    //�ܺο��� ApplyDamage �Լ��� ������.
    public bool ApplyDamage(DamageMessage damageMessage)
    {
        #region ���� ����
        //�÷��̾ ���� ����, �������� = 0, ���׼� ��Ÿ��, �������� -> ����
        if ((playerState.playerCurrentState == PlayerStateType.DEAD) 
            || (damageMessage.amount <= 0) 
            || !canUseHitAction
            || playerState.isInvincible 
            || currentHP <= 0)
        {
            return false;
        }

        if (playerState.playerCurrentState == PlayerStateType.GHOST_FINISHSKILL)
        {
            return false;
        }
        #endregion

        currentHP -= damageMessage.amount;
        hpHUD.ChangeHPStack((int)currentHP);
        canUseHitAction = false;
        hitStartTime = Time.time;

        if (currentHP <= 0) 
        {
            playerState.ChangePlayerState(PlayerStateType.DEAD);
            playerState.ChangePlayerSubState(PlayerSubStateType.DEAD_HPZERO);
            DieAction();
        }
        else if (!playerState.isSuperArmor)
        {
            playerState.ChangePlayerState(PlayerStateType.HIT);
            playerState.ChangePlayerSubState(PlayerSubStateType.NONE);
            StartCoroutine(HitAction());
        }

        return true;
    }


    #region ġƮ
    public void RestoreHealth(DamageMessage damageMessage)
    {
        if (currentHP + damageMessage.amount < 20)
        {
            currentHP += damageMessage.amount;
            hpHUD.ChangeHPStack((int)currentHP);
        }
        else
        {
            currentHP = 20;
            hpHUD.ChangeHPStack((int)currentHP);
        }
    }
    #endregion

    #region ��ư
    public void Loading()
    {
        loadingUI.StartCoroutine(loadingUI.Loading());
    }
    #endregion

}

#region PlayerDamageReaction(��ũ��Ʈ �и���Ű��)
public partial class Player : MonoBehaviour, IDamageable
{
    IEnumerator HitAction()
    {
        isPerformingHitAction = true;

        if (maskChange.HumanMask.activeSelf)
        {
            humanSkill.InitializeSkill();
        }
        else
        {
            animalSkill.InitializeSkill();
        }


        maskChange.CurrentRigidbody.velocity = Vector3.zero;

        //���۾ƸӰ� �ƴϸ� �����鼭 �ൿ����
        if (maskChange.AnimalMask) maskChange.CurrentAnimator.CrossFade(playerAnimation.Animal_Hit, 0);
        if (maskChange.HumanMask) maskChange.CurrentAnimator.CrossFade(playerAnimation.Human_Hit, 0);


        hitStartTime = Time.time;

        #region while ����
        bool activeSoundOnce = false;
        #endregion

        while (true)
        {
            #region Animation State
            var animatorStateInfo = maskChange.CurrentAnimator.GetCurrentAnimatorStateInfo(0);
            var animationHash = animatorStateInfo.shortNameHash;

            if (animationHash == playerAnimation.Human_Hit || animationHash == playerAnimation.Animal_Hit)
            {
                isPerformingHitActionAnim = true;
            }
            else
            {
                if (isPerformingHitActionAnim)
                {
                    InitializeSkill();
                    break;
                }
            }
            #endregion

            #region Restriction
            //������ �ʿ���
            //playerState.RestrictPlayer(commonData.hitRestrict, hitStartTime);
            #endregion

            #region �Ҹ�
            if (!activeSoundOnce)
            {
                playerSound.Initialize();

                playerSound.SetPlayerSound(commonData.hitSound, Player.instance.transform.position, hitStartTime);
                playerSound.SetPlayerSound(commonData.heartBeatSound, Player.instance.transform.position, hitStartTime);
                activeSoundOnce = true;
            }
            #endregion

            yield return null;
        }
    }

    public void HitCooldown()
    {
        if (canUseHitAction) return;

        if (Time.time < hitStartTime + commonData.hitCooldown)
        {
            return;
        }
        canUseHitAction = true;
    }

    public void DieAction()
    {
        StartCoroutine(CoDieAction());
    }

    public IEnumerator CoDieAction()
    {
        //PlayerCurrentSubState�� ���� �Ϲ� ��������, �������� �����ϱ� 
        //�ڷ�ƾ���� ����

        if (playerState.playerCurrentState != PlayerStateType.DEAD)
        {
            yield break;
        }

        if (playerState.playerCurrentSubState == PlayerSubStateType.DEAD_FALL)
        {
            if(currentHP > 3)
            {
                //Recomposer
                playerCameraEffect.StartCoroutine(playerCameraEffect.ToggleCameraRecomposer(commonData.fallDeathCameraRecomposer));
                UIEffect.ShowFadeScreen(false, 1f);

                yield return new WaitForSeconds(1f);

                //���� ������ ������ ������ �ҷ�����
                //���̺� ����Ʈ���� ������ ��� �ε��� +1
                //�ҷ����⸦ �� ���
                saveManager.MoveToPreviousIndex();
                saveManager.LoadSlotData();

                //hp���� �� �ٽ� ����
                currentHP -= 3;
                hpHUD.ChangeHPStack((int)currentHP);
                saveManager.SaveSloatData();
                saveManager.MoveToNextIndex();

                yield return new WaitForSeconds(1f);

                UIEffect.ShowFadeScreen(true, 1f);
                playerState.ChangePlayerState(PlayerStateType.IDLE);
                playerState.ChangePlayerSubState(PlayerSubStateType.NONE);

                UIEffect.instance.ShowPlayerHUDFadeEffect();

                yield break;
            }

            //5���� ������ ���� �������� �Ѿ
            currentHP -= 3;
            hpHUD.ChangeHPStack((int)currentHP);
        }


        //HP 0 ���� ����� ��
        if (playerState.playerCurrentSubState == PlayerSubStateType.DEAD_HPZERO || currentHP <= 0)
        {
            //ü�� 0
            currentHP = 0;
            hpHUD.ChangeHPStack((int)currentHP);

            MenuUI.instance.DisablePlayerControl(true);

            //����
            maskChange.CurrentRigidbody.velocity = Vector3.zero;
            UIEffect.instance.ShowPlayerHUDFadeEffect();

            //�ִϸ��̼�
            if (maskChange.AnimalMask) maskChange.CurrentAnimator.CrossFade(playerAnimation.Animal_Die, 0);
            if (maskChange.HumanMask) maskChange.CurrentAnimator.CrossFade(playerAnimation.Human_Die, 0);

            //�Ҹ�
            playerSound.SetPlayerSound(commonData.dieSound, Player.instance.transform.position, Time.time);
            playerSound.SetPlayerSound(commonData.afterDeadSound, Player.instance.transform.position, Time.time);

            //Recomposer
            playerCameraEffect.StartCoroutine(playerCameraEffect.ToggleCameraRecomposer(commonData.zeroHealthDeathCameraRecomposer));
            yield return new WaitForSeconds(.5f);
            UIEffect.ShowFadeScreen(false, 1f);

            yield return new WaitForSeconds(3);
            UIEffect.StartCoroutine(UIEffect.ShowDeathScreen());

            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(0);
        }

        //�������� ��
    }

    //�׾��� ���� ��ɵ� 
    public bool CheckDie()
    {
        return playerState.playerCurrentState == PlayerStateType.DEAD;
    }

}
#endregion