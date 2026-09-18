using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    //������: �ڽĿ�����Ʈ�� ��Ȱ��ȭ�� ���¿��� �����ϸ� �ڽĿ�����Ʈ�� ��ũ��Ʈ�� Awake, Start �Լ��� ȣ����� ����

    public static PlayerController instance;

    private SaveManager saveManager;
    [SerializeField] private MenuUI menuUI;
    [SerializeField] private PlayerSkillInput playerSkillInput;

    #region �ܺ�
    [SerializeField] public PlayerMovement playerMovement;
    [SerializeField] public MaskChange maskChange;
    [SerializeField] public HumanMaskSkill humanMaskSkill;
    [SerializeField] public AnimalMaskSkill animalMaskSkill;
    [SerializeField] public GhostMaskSkill ghostMaskSkill;
    [SerializeField] public Player player;

    [SerializeField] private PlayerSkillMove playerSkillMove;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private PlayerAnimation playerAnimation;

    //������
    private PlayerCommonData commonData;
    private PlayerHumanMaskData humanData;
    private PlayerAnimalMaskData animalData;
    #endregion

    private void Awake()
   {
        #region �̱���
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this.gameObject);
        #endregion

        commonData = PlayerCommonData.Instance;
        humanData = PlayerHumanMaskData.Instance;
        animalData = PlayerAnimalMaskData.Instance;

        maskChange = GetComponent<MaskChange>();
        playerMovement = GetComponent<PlayerMovement>();
        
        humanMaskSkill = GetComponentInChildren<HumanMaskSkill>();
        animalMaskSkill = GetComponentInChildren<AnimalMaskSkill>();
        ghostMaskSkill = GetComponentInChildren<GhostMaskSkill>(    );

        player = GetComponentInChildren<Player>();

        maskChange.InitialSetUp();
    }
    private void Start()
    {
        saveManager = SaveManager.instance;
    }
    private void Update()
    {
        #region ���� �Է�

        if (Input.GetKeyDown(saveManager.InputKeys[KeyAction.MENU]))
        {
            menuUI.MenuSwitch();
        }

        if (menuUI.isPlayerControlDisabled)
        {
            return;
        }

        
        #endregion

        if (playerState.playerCurrentState == PlayerStateType.DEAD) return;

        #region �÷��̾� ����
        //�ִϸ��̼� ����
        playerAnimation.UpdateAnimationState();

        //��������
        if (!humanMaskSkill.canUseDash ||
            !humanMaskSkill.canUseInkShape ||
            !animalMaskSkill.canUseDash ||
            !animalMaskSkill.canUseLeapStrike ||
            !ghostMaskSkill.canUseFinishSkill ||
            CameraController.instance.isTargetDetected)
        {
            UIEffect.instance.IsPlayerHUDFading(true);
        }
        else
        {
            UIEffect.instance.IsPlayerHUDFading(false);
        }
        #endregion

        #region ��Ÿ��
        maskChange.ChangeMaskCooldown();
        player.HitCooldown();

        //�������� ���� ��Ȱ��ȭ�Ǿ �۵��� �ȵ�
        if (maskChange.CurrentMask == maskChange.HumanMask)
        {
            humanMaskSkill.InkFloorCooldown();
            humanMaskSkill.InkShapeCooldown();
            humanMaskSkill.DashCooldown();
        }
        else
        {
            animalMaskSkill.LeapStrikeCooldown();
            animalMaskSkill.roarCooldown();
            animalMaskSkill.DashCooldown();
        }

        ghostMaskSkill.FinishSkillCooldown();

        #endregion

        if (playerState.playerCurrentState == PlayerStateType.GHOST_FINISHSKILL)
        {
            return;
        }

        #region ī�޶� �Է�
        if (Input.GetKeyDown(saveManager.InputKeys[KeyAction.LOCKONTARGET]))
        {
            CameraController.instance.LockOnTarget();
        }
        #endregion

        #region ���� �Է�
        //�Է� �� ����
        playerMovement.InputMovement();
        #endregion

        if (playerState.doNotAct)
        {
            return;
        }

        #region Ž��
        ghostMaskSkill.DetectTargetToFinish();
        #endregion

        if (!PlatformSwitcher.instance.IsPCPlatform)
        {
            return;
        }

        #region �÷��� �Է�

        if (player.IsPerformingHitAction)
        {
            return;
        }


        if (maskChange.CurrentMask == maskChange.HumanMask)
        {
            if (Input.GetKeyDown(saveManager.InputKeys[KeyAction.ATTACK_NORMAL]))
            {
                playerSkillInput.StoreInput(PlayerStateType.HUMAN_NORMALATTACK);

                if (!playerState.isPerfomingSklill) humanMaskSkill.NormalAttack();
            }
            else if (Input.GetKeyDown(saveManager.InputKeys[KeyAction.ATTACK_SPECIAL]))
            {
                playerSkillInput.StoreInput(PlayerStateType.HUMAN_INKSHAPE);
                if (!playerState.isPerfomingSklill) humanMaskSkill.InkShape();
            }
            else if (Input.GetKeyDown(saveManager.InputKeys[KeyAction.DASH]))
            {
                playerSkillInput.StoreInput(PlayerStateType.DASH);
                if (!playerState.isPerfomingSklill) humanMaskSkill.Dash();
            }
            else if (Input.GetKeyDown(saveManager.InputKeys[KeyAction.ATTACK_FINISH]))
            {
                playerSkillInput.StoreInput(PlayerStateType.GHOST_FINISHSKILL);
                if (!playerState.isPerfomingSklill) ghostMaskSkill.Finish();
            }
        }
        else
        {
            if (Input.GetKeyDown(saveManager.InputKeys[KeyAction.ATTACK_NORMAL]))
            {
                playerSkillInput.StoreInput(PlayerStateType.ANIMAL_NORMALATTACK);

                if (!playerState.isPerfomingSklill) animalMaskSkill.NormalAttack();
            }
            else if (Input.GetKeyDown(saveManager.InputKeys[KeyAction.ATTACK_SPECIAL]))
            {
                playerSkillInput.StoreInput(PlayerStateType.ANIMAL_LEAPSTRIKE);
                if (!playerState.isPerfomingSklill) animalMaskSkill.LeapStrike();
            }
            else if (Input.GetKeyDown(saveManager.InputKeys[KeyAction.DASH]))
            {
                playerSkillInput.StoreInput(PlayerStateType.DASH);
                if (!playerState.isPerfomingSklill) animalMaskSkill.Dash();
            }
        }

        if (Input.GetKeyDown(saveManager.InputKeys[KeyAction.ATTACK_FINISH]))
        {
            playerSkillInput.StoreInput(PlayerStateType.GHOST_FINISHSKILL);
            if (!playerState.isPerfomingSklill) ghostMaskSkill.Finish();
        }

        #endregion
    }
    private void FixedUpdate()
    {
        #region Player ������Ʈ�� ĳ���� ������Ʈ ���󰡱�
        player.FollowCharacterObject();
        #endregion

        #region �߰����� �߷°�
        if (!CheatMode.instance.isFlyMode)
        {
            playerMovement.AddGravity();
        }
        #endregion


        if (menuUI.isPlayerControlDisabled)
        {
            return;
        }

        #region ��ų�� ���� �̵�
        playerSkillMove.UpdateSkillMovement();
        #endregion

        if (playerState.playerCurrentState == PlayerStateType.DEAD) return;

        if (playerState.playerCurrentState == PlayerStateType.GHOST_FINISHSKILL)
        {
            return;
        }

        if (playerState.doNotAct)
        {
            return;
        }

        #region ȸ��, �̵�
        if (!playerState.doNotRotate && !player.IsPerformingHitAction)
        {
            playerMovement.CharacterRotate();
        }

        if (!playerState.doNotMove && !player.IsPerformingHitAction)
        {
            playerMovement.CharacterMove();
        }
        #endregion
    }
}