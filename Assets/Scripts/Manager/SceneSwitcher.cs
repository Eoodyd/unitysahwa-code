using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher instance;

    private SaveManager saveManager;
    private MaskChange maskChange;

    private bool isNotPlayingMainMenu;
    public bool IsNotPlayingMainMenu
    {
        set
        {
            isNotPlayingMainMenu = value ;
        }
        get
        {
            return isNotPlayingMainMenu;
        }
    }
    
    //���̺� �������� �ҷ��� �� �ѹ��� ���̺� �Ǵ� ��� �Ϻθ� ��ŵ�ϱ� ����
    private bool skipRespawnSave;
    public bool SkipRespawnSave
    {
        set
        {
            skipRespawnSave = value;
        }
        get
        {
            return skipRespawnSave;
        }
    }

    private bool isSceneLoaded = false;
    public bool IsSceneLoaded
    {
        get { return isSceneLoaded; }
        set { isSceneLoaded = value; }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        isNotPlayingMainMenu = false;
    }
    private void Start()
    {
        saveManager = SaveManager.instance;
        maskChange = MaskChange.instance;

        DontDestroyOnLoad(this);


        //TODO:UI �������� �۵��� �ؾ��Ѵ�
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            MenuUI.instance.ActivateLetterBox(true);
        }
        else
        {
            MenuUI.instance.ActivateLetterBox(false);
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isSceneLoaded = true;
    }

    public void IsNotSceneLoaded()
    {
        isSceneLoaded = false;
    }
    public void SwitchScene(int index) //Ʈ���Ÿ� ���� ���� ������ �Ѿ
    {
        StartCoroutine(CoSwitchScene(index));
    }

    public IEnumerator CoSwitchScene(int index)
    {
        //TODO: �÷��̾ �����ð����� ������ �ʿ䰡 �ִ�. -> bool ������
        //���� �ѳ������ϴϱ� ���ı� ������Ʈ�� �پ��־����

        //���� ���� �ֽŵ����� �ӽ�����, currentIndex�� ������ ���� ����
        float currentHP = Player.instance.currentHP;
        bool isHumanMask = MaskChange.instance.HumanMask.activeSelf;
        int currentIndex = SaveManager.instance.CurrentIndex;
        
        SceneManager.LoadScene(index);
        
        yield return new WaitForSeconds(0.5f);

        CameraController.instance.TerrainLoadCamera.gameObject.SetActive(true);
        CameraController.instance.DefaultCamera.gameObject.SetActive(false);
        CameraController.instance.DefaultCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0;
        CameraController.instance.DefaultCamera.m_RecenterToTargetHeading.m_enabled = true;

        

        //���Ҵ�
        saveManager = SaveManager.instance;
        maskChange = MaskChange.instance;

        if (TimelineHelper.instance.startNewGameCamera != null)
        {
            TimelineHelper.instance.startNewGameCamera.SetActive(false);
        }
        if (MenuUI.instance.MainMenu != null)
        {
            MenuUI.instance.MainMenu.SetActive(false);
        }

        //�ΰ��ӿ��� Ŀ�� ����
        if (PlatformSwitcher.instance.IsPCPlatform)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        //���� ���� ������ �ε�, ������ ���ġ
        Player.instance.currentHP = currentHP;
        HpHUD.instance.ChangeHPStack((int)currentHP);
        if (isHumanMask)
        {
            if (!maskChange.HumanMask.activeSelf)
            {
                maskChange.ChangeCharacter();
            }
        }
        else
        {
            if (maskChange.HumanMask.activeSelf)
            {
                maskChange.ChangeCharacter();
            }
        }

        saveManager.CurrentIndex = currentIndex;

        saveManager.ResetPlayerPosition();

        yield return new WaitForSeconds(0.5f);
        CameraController.instance.TerrainLoadCamera.gameObject.SetActive(false);
        CameraController.instance.DefaultCamera.gameObject.SetActive(true);

        CameraController.instance.DefaultCamera.m_RecenterToTargetHeading.m_RecenteringTime = 1;
        CameraController.instance.DefaultCamera.m_RecenterToTargetHeading.m_enabled = false;
        CameraController.instance.ChangeCamera(CameraType.DEFAULT);
        yield return new WaitForSeconds(1f);
    }

    public void LoadScene() //�ҷ����⸦ ���� ����ȯ(������ ����)
    {
        StartCoroutine(CoLoadScene());
    }
    public IEnumerator CoLoadScene()
    {
        //���[currentIndex]�� �ش��ϴ� ������ ������
        saveManager.GetLoadData();
        
        //LoadingUI.instance.FadeOutInScreen(1, 1);
        LoadingUI.instance.FadeOutScreen(4);

        int currentIndex = saveManager.CurrentIndex;

        if (saveManager.CurrentSceneIndex == 0)
        {
            //���θ޴� �ҷ����� �� �ƴ϶� ���θ޴� ��� ��Ȱ��ȭ ��Ŵ, Ʃ�丮�� ������ �ҷ���. 
            isNotPlayingMainMenu = true;
        }

        //TODO: WaitForSecond�� �� ��� �ð��� ������ ����. �ʹ� �����ϰ� �ڵ��ۼ���
        yield return new WaitForSecondsRealtime (0.5f);

        SceneManager.LoadScene(saveManager.CurrentSceneIndex);

        yield return null;

        saveManager = SaveManager.instance;
        saveManager.CurrentIndex = currentIndex;
        saveManager.LoadSlotData();
        CameraController.instance.TerrainLoadCamera.gameObject.SetActive(true);
        CameraController.instance.DefaultCamera.gameObject.SetActive(false);
        CameraController.instance.DefaultCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0;
        CameraController.instance.DefaultCamera.m_RecenterToTargetHeading.m_enabled = true;

        Time.timeScale = 1;

        //�ΰ��ӿ��� Ŀ�� ����
        if (PlatformSwitcher.instance.IsPCPlatform)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        yield return new WaitForSecondsRealtime(0.5f);
        CameraController.instance.TerrainLoadCamera.gameObject.SetActive(false);
        CameraController.instance.DefaultCamera.gameObject.SetActive(true);
        CameraController.instance.DefaultCamera.m_RecenterToTargetHeading.m_RecenteringTime = 1;
        CameraController.instance.DefaultCamera.m_RecenterToTargetHeading.m_enabled = false;
        CameraController.instance.ChangeCamera(CameraType.DEFAULT);
        
        yield return new WaitForSecondsRealtime(1f);

        MenuUI.instance.ActivateLetterBox(false);
        LoadingUI.instance.FadeInScreen(2);

        yield return new WaitForSecondsRealtime(1f);
    }
}
