using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class CheatMode : MonoBehaviour
{
    public static CheatMode instance;
    
    //외부 스크립트
    private SaveManager saveManager;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private Player player;
    [SerializeField] private MaskChange maskChange;
    [SerializeField] private GameTimeScale gameTimeScale;

    [SerializeField] private GhostMaskSkill ghostMaskSkill;
    [SerializeField] private MenuUI menuUI;

    //외부 수치
    private CheatData cheatData;

    public bool isCheatMode { get; private set; }
    [SerializeField] private TextMeshProUGUI isCheatModeText;
    [SerializeField] GameObject ActivateCheatModeText;

    public bool isGameSlowed { get; private set; }
    [SerializeField] GameObject gamePauseText;
    public bool isDamageMax { get; private set; }
    [SerializeField] GameObject damageMaxText;
    public bool isPaintOverlapMax { get; private set; }
    [SerializeField] GameObject paintOverlapMaxText;
    public bool isMoveSpeedUp { get; private set; }
    [SerializeField] GameObject moveFastText;
    public bool isFlyMode { get; private set; }
    [SerializeField] GameObject flyModeText;
    public bool isFlying { get; private set; }
    public bool isMapVisible { get; private set; }
    [SerializeField] GameObject showMapText;

}

public partial class CheatMode : MonoBehaviour
{
    private void Awake()
    {
        #region 싱글톤
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this.gameObject);
        #endregion
    }

    private void Start()
    {
        cheatData = CheatData.Instance;
        saveManager = SaveManager.instance;

        ActivateCheatModeText.SetActive(false);
        gamePauseText.SetActive(false);
        damageMaxText.SetActive(false);
        paintOverlapMaxText.SetActive(false);
        moveFastText.SetActive(false);
        flyModeText.SetActive(false);
        showMapText.SetActive(false);
    }

    private void Update()
    {
        return;

        if (menuUI.MainMenu.activeSelf)
        {
            return;
        }

        if (Input.GetKeyDown(cheatData.activateCheatMode))
        {
            ActivateCheatMode();
        }

        if (!isCheatMode)
        {
            return;
        }

        if (Input.GetKeyDown(cheatData.saveData))
        {
            SaveData();
        }
        else if (Input.GetKeyDown(cheatData.loadData))
        {
            LoadData();
        }
        else if (Input.GetKeyDown(cheatData.setGameTimeRate))
        {
            SetGameTimeScale();
        }
        else if (Input.GetKeyDown(cheatData.damageMax))
        {
            DamageMax();
        }
        else if (Input.GetKeyDown(cheatData.paintOverlapMax))
        {
            PaintOverlapMax();
        }
        else if (Input.GetKeyDown(cheatData.moveSpeedUp))
        {
            MoveFast();
        }
        else if (Input.GetKeyDown(cheatData.blink))
        {
            BlinkForward();
        }
        else if (Input.GetKeyDown(cheatData.flyMode))
        {
            Fly();
        }

        if (Input.GetKey(cheatData.moveUp))
        {
            if (isFlyMode)
            {
                isFlying = true;
            }
        }
        else
        {
            isFlying = false;
        }

        if (Input.GetKey(cheatData.minHealth))
        {
            MinHealth();
        }
        else if (Input.GetKeyDown(cheatData.maxHealth))
        {
            MaxHealth();
        }
        else if (Input.GetKeyDown(cheatData.dieFromZeroHealth))
        {
            DieFromZeroHealth();
        }
        else if (Input.GetKeyDown(cheatData.dieFromFall))
        {
            DieFromFall();
        }

        else if (Input.GetKeyDown(cheatData.clearEnemy))
        {
            ClearEnemyNearPlayer();
        }
    }
}

public partial class CheatMode : MonoBehaviour
{
    public void ShowUI()
    {
        if (isCheatMode)
        {
            //함수가 호출될때마다 On/Off

            //UI Image의 enabled = false
            //게임 저장하고 플레이해서 테스트하는게 먼저임
        }
 
    }

    //UI,HUD는 치트와 별개
    public void ActivateCheatMode()
    {
        if (menuUI.MainMenu.activeSelf)
        {
            isCheatMode = false;
            return;
        }

        isCheatMode = !isCheatMode;
        ActivateCheatModeText.SetActive(isCheatMode);

        if (isCheatMode)
        {
            isCheatModeText.text = "Cheat Mode: O";
        }
        else
        {
            isCheatModeText.text = "Cheat Mode: X";

            //모든 치트 해제하기
            if (isGameSlowed)
            {
                SetGameTimeScale();
            }
            if (isDamageMax) 
            {
                DamageMax(); 
            }
            if (isPaintOverlapMax)
            {
                PaintOverlapMax();
            }
            if (isMoveSpeedUp)
            {
                MoveFast();
            }
            if (isFlyMode)
            {
                Fly();
            }
        }
    }
    public void SaveData()
    {
        saveManager.SaveSloatData();
    }
    public void LoadData()
    {
        saveManager.LoadSlotData();
    }

    public void SetGameTimeScale()
    {
        isGameSlowed = !isGameSlowed;

        if (isGameSlowed)
        {
            gameTimeScale.SetTimeScale(cheatData.timeScaleValue);
        }
        else
        {
            gameTimeScale.SetTimeScale(1);
        }
        gamePauseText.SetActive(isGameSlowed);
    }

    public void DamageMax()
    {
        isDamageMax = !isDamageMax;
        damageMaxText.SetActive(isDamageMax);
    }
    public void PaintOverlapMax()
    {
        isPaintOverlapMax = !isPaintOverlapMax;
        paintOverlapMaxText.SetActive(isPaintOverlapMax);
    }

    public void MoveFast()
    {
        isMoveSpeedUp = !isMoveSpeedUp;
        moveFastText.SetActive(isMoveSpeedUp);
    }

    public void BlinkForward()
    {
        maskChange.CurrentMask.transform.position = maskChange.CurrentMask.transform.position + maskChange.CurrentMask.transform.forward * cheatData.blinkDistance;
    }

    public void BlinkUpward()
    {
        maskChange.CurrentMask.transform.position = maskChange.CurrentMask.transform.position + maskChange.CurrentMask.transform.up * cheatData.blinkDistance / 2;
    }

    public void Fly()
    {
        isFlyMode = !isFlyMode;
        flyModeText.SetActive(isFlyMode);
        if (isFlyMode)
        {
            PlayerController.instance.maskChange.CurrentRigidbody.useGravity = false;
            PlayerController.instance.maskChange.CurrentRigidbody.velocity = new Vector3(0, 0, 0);
        }
        else
        {
            PlayerController.instance.maskChange.CurrentRigidbody.useGravity = true;
            PlayerController.instance.maskChange.CurrentRigidbody.velocity = new Vector3(0, 0, 0);
            isFlying = false;
        }
    }

    public void MinHealth()
    {
        if (player.currentHP > 1)
        {
            DamageMessage damageMessage = new DamageMessage();
            damageMessage.amount = player.currentHP - 1;
            player.ApplyDamage(damageMessage);
        }
    }

    public void MaxHealth()
    {
        DamageMessage damageMessage = new DamageMessage();
        damageMessage.amount = 20;
        player.RestoreHealth(damageMessage);
    }


    public void DieFromZeroHealth()
    {
        playerState.ChangePlayerState(PlayerStateType.DEAD);
        playerState.ChangePlayerSubState(PlayerSubStateType.DEAD_HPZERO);
        player.DieAction();
    }

    public void DieFromFall()
    {
        playerState.ChangePlayerState(PlayerStateType.DEAD);
        playerState.ChangePlayerSubState(PlayerSubStateType.DEAD_FALL);
        player.DieAction();
    }

    public void ClearEnemyNearPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, cheatData.clearRange, cheatData.clearLayer);

        if(colliders.Length == 0 ) 
        {
            return; 
        }
        for (int i = 0; i < colliders.Length; i++)
        {
            //적이 아니라면 + 죽었다면 패스
            if (!colliders[i].gameObject.GetComponent<Enemy>()) continue;
            if (colliders[i].gameObject.GetComponent<Enemy>().isDead) continue;

            var atkTarget = colliders[i].gameObject.GetComponent<Enemy>();
            if (atkTarget != null)
            {
                var message = new DamageMessage();
                message.amount = 9999;
                message.damager = maskChange.CurrentMask;
                message.color = 'B' ;
                message.value = 1;
                atkTarget.ApplyDamage(message);
            }
        }
    }
    
}