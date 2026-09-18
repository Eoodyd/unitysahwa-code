using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuUI : MonoBehaviour
{
    public static MenuUI instance;

    #region �ܺ�
    [Header("�ܺ�")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private MaskChange maskChange;
    [SerializeField] private PlayerSound playerSound;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private SoundSettingUI soundUI;
    [SerializeField] private InputKeySettingUI inputKeyUI;
    [SerializeField] private MouseSettingUI mouseUI;
    [SerializeField] private GameTimeScale gameTimeScale;

    private TimelineHelper timelineHelper;
    private LoadingUI loadingUI;

    private PlayerCommonData commonData;
    #endregion

    #region �޴�
    [Space(20)]
    [Header("�޴�")]
    [SerializeField] private GameObject mainMenu;
    public GameObject MainMenu
    {
        get
        {
            return mainMenu;
        }
    }

    [SerializeField] GameObject pauseMenu;
    public GameObject PauseMenu
    {
        get
        {
            return pauseMenu;
        }
    }
    #endregion

    #region Window
    [Space(20)]
    [Header("���â")]
    [SerializeField] private GameObject newGameWindow;
    public GameObject NewGameWindow
    {
        get
        {
            return newGameWindow;
        }
    }

    [SerializeField] private Button newGameButton;
    [SerializeField] private Button[] StoryImage;
    [SerializeField] private GameObject storyImageWindow;
    public GameObject StoryImageWindow
    {
        get
        {
            return storyImageWindow;
        }
    }

    [SerializeField] private GameObject loadSlotWindow;
    [SerializeField] private GameObject loadWindow;
    [SerializeField] private Button loadButton;

    [SerializeField] private GameObject settingWindow;
    [SerializeField] private GameObject soundWindow;
    [SerializeField] private GameObject inputKeyWindow;
    public GameObject InputKeyWindow
    {
        get { return inputKeyWindow; }
        set { inputKeyWindow = value; }
    }

    [SerializeField] private GameObject goToMainMenuWindow;
    [SerializeField] private Button goToMainMenuButton;

    [SerializeField] private GameObject quitWindow;
    [SerializeField] private Button quitButton;

    [SerializeField] private Button openSettingButton;

    [SerializeField] private Button setPCButton;
    [SerializeField] private Button setMobileButton;

    //TODO: ��Ʋ HUD���� PC HUD �� �ۼ��ϱ�
    #endregion

    public bool isPlayerControlDisabled { get; private set; }
    public bool canShowPauseMenu { get; private set; }

    #region Save Slot
    [Header("����(������� �ֱ�)")]
    [SerializeField] private Button[] loadSlots;
    [SerializeField] private GameObject[] selectImage;
    [SerializeField] private TextMeshProUGUI[] areaText;
    [SerializeField] private TextMeshProUGUI[] saveTypeText;
    [SerializeField] private TextMeshProUGUI[] playTimeText;
    [SerializeField] private TextMeshProUGUI[] dateText;
    [SerializeField] private TextMeshProUGUI[] noDataText;
    //���� ���� ���ϱ� ����
    private int?[] timeIndex;
    #endregion

    #region HUD 
    [Space(20)]
    [Header("HUD")]
    [SerializeField] private GameObject inputHUD;
    public GameObject InputHUD
    {
        get { return inputHUD; }
        set { inputHUD = value; }
    }

    [SerializeField] private GameObject battleHUD;
    public GameObject BattleHUD
    {
        get { return battleHUD; }
        set { battleHUD = value; }
    }

    [SerializeField] private GameObject battleGuideHUD;
    public GameObject BattleGuideHUD
    {
        get { return battleGuideHUD; }
        set { battleGuideHUD = value; }
    }

    #endregion

    #region ���͹ڽ�
    [Space(20)]
    [Header("LetterBox")]
    [SerializeField] private Image letterBoxImage;
    [SerializeField] private Image letterBoxMaskImage;
    [SerializeField] private Mask letterBoxMask;
    #endregion

    #region ����
    [Space(20)]
    [Header("��� ��ü")]
    [SerializeField] private Button changeToKorean;
    [SerializeField] private Button changeToEnglish;
    #endregion

    #region ���� ��ư �Է�
    [Space(20)]
    [Header("���� ��ư")]
    [SerializeField] private GameObject[] settingElements;
    [SerializeField] private Image[] settingTextBG;
    [SerializeField] private TextMeshProUGUI[] settingText;
    [SerializeField] private Button[] settingButton;

    #endregion


    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this.gameObject);

        timeIndex = new int?[loadSlots.Length];
        for (int i = 0; i < timeIndex.Length; i++)
        {
            timeIndex[i] = null;
        }
    }

    void Start()
    {
        timelineHelper = TimelineHelper.instance;
        loadingUI = LoadingUI.instance;

        //�޴�â ��ġ����  
        SetPosition();
        SetUIElements();
        SetButtonFunction();

        //PlayGuide�� DontDestroy��
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            PlayGuide.instance.IsTutorialStart = false;
        }


        if (mainMenu.activeSelf && !SceneSwitcher.instance.IsNotPlayingMainMenu)
        {
            //���θ޴��� ���
            canShowPauseMenu = false;
            isPlayerControlDisabled = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            //�ٷ� �ΰ��� ����
            canShowPauseMenu = true;
            isPlayerControlDisabled = false;

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
        }
    }
    #region Set
    private void SetPosition()
    {
        RectTransform starMenuRT = mainMenu.GetComponent<RectTransform>();
        RectTransform pauseMenuRT = pauseMenu.GetComponent<RectTransform>();
        RectTransform settingWindowRT = settingWindow.GetComponent<RectTransform>();

        SetRectTransform(starMenuRT);
        SetRectTransform(settingWindowRT);
        SetRectTransform(pauseMenuRT);
    }
    private void SetRectTransform(RectTransform rectTransform)
    {
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.anchoredPosition3D = Vector3.zero;
    }
    private void SetUIElements()
    {
        newGameWindow.SetActive(false);

        pauseMenu.SetActive(false);
        goToMainMenuWindow.SetActive(false);

        settingWindow.SetActive(false);
        inputKeyWindow.SetActive(false);
        loadSlotWindow.SetActive(false);
        loadWindow.SetActive(false);
        quitWindow.SetActive(false);
    }

    private void SetButtonFunction()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            //������ ���� �̺�Ʈ
            newGameButton.onClick.AddListener(() => { ShowStory(); });

            //���丮 ���� ���� ���� ��ɵ��� ����
            StoryImage[StoryImage.Length - 1].onClick.AddListener(() => { SaveManager.instance.ResetData(); });
            StoryImage[StoryImage.Length - 1].onClick.AddListener(() => { timelineHelper.StartTutorialTimeline(); });
            StoryImage[StoryImage.Length - 1].onClick.AddListener(() => { SaveManager.instance.SelectIndex(0); });
            StoryImage[StoryImage.Length - 1].onClick.AddListener(() => { PlayGuide.instance.IsTutorialStart = true; });

            for (int i = 0; i < StoryImage.Length - 1; i++)
            {
                int index = i;

                StoryImage[i].onClick.AddListener(() => { StoryImage[index].gameObject.SetActive(false); });
            }
        }

        //�ҷ����� �̺�Ʈ(�ҷ����� ���� �̺�Ʈ�� ����)
        for (int i = 0; i < loadSlots.Length; i++)
        {
            loadSlots[i].onClick.AddListener(() =>
            {
                for (int j = 0; j < loadSlots.Length; j++)
                {
                    if (i == j)
                    {
                        selectImage[j].SetActive(true);
                    }
                    else
                    {
                        selectImage[j].SetActive(false);
                    }
                }
            }
            );
        }

        //���õ� ������ index Ȯ������ ���ҷ�����
        loadButton.onClick.AddListener(() => SaveManager.instance.SetCurrentIndex());
        loadButton.onClick.AddListener(() => SceneSwitcher.instance.LoadScene());
        //�ҷ����⸦ ���� �÷��̾ ��ġ�� �� MoveToNextIndex()�� ����Ǵ� ���� ����
        loadButton.onClick.AddListener(() => SceneSwitcher.instance.SkipRespawnSave = true);


        //�Ͻ����� �޴����� ���θ޴��� ���ư� ���� �Լ�
        goToMainMenuButton.onClick.AddListener(() => { SceneManager.LoadScene(0); });
        goToMainMenuButton.onClick.AddListener(() => { Time.timeScale = 1; });
        goToMainMenuButton.onClick.AddListener(() => { SceneSwitcher.instance.IsNotPlayingMainMenu = false; });


        quitButton.onClick.AddListener(() => { Quit(); });

        openSettingButton.onClick.AddListener(() => { MobileInput.instance.OpenMenu(); });
        openSettingButton.onClick.AddListener(() => { ActivateLetterBox(true); });

        //�÷��� ����ġ
        setPCButton.onClick.AddListener(() => { PlatformSwitcher.instance.SetPCPlatform(true); });
        setMobileButton.onClick.AddListener(() => { PlatformSwitcher.instance.SetPCPlatform(false); });

        //��ü
        changeToKorean.onClick.AddListener(() => { LanguageManager.Instance.ChangeLanguage(0); });
        changeToKorean.onClick.AddListener(() => { ChangeSlotLanguage(); });
        changeToEnglish.onClick.AddListener(() => { LanguageManager.Instance.ChangeLanguage(1); });
        changeToEnglish.onClick.AddListener(() => { ChangeSlotLanguage(); });

        for (int i = 0; i < settingText.Length; i++)
        {
            int firstIndex = i;
            for (int j = 0; j < settingText.Length; j++)
            {
                int secondIndex = j;
                //������ Ŭ���Ǿ��� ��
                if (firstIndex == secondIndex)
                {
                    settingButton[firstIndex].onClick.AddListener(() => { settingElements[secondIndex].SetActive(true); });
                    settingButton[firstIndex].onClick.AddListener(() => { settingTextBG[secondIndex].enabled = true; });
                    settingButton[firstIndex].onClick.AddListener(() => { settingText[secondIndex].color = Color.white; });
                }

                else if (firstIndex != secondIndex)
                {
                    settingButton[firstIndex].onClick.AddListener(() => { settingElements[secondIndex].SetActive(false); });
                    settingButton[firstIndex].onClick.AddListener(() => { settingTextBG[secondIndex].enabled = false; });
                    settingButton[firstIndex].onClick.AddListener(() => { settingText[secondIndex].color = Color.black; });
                }
            }
        }

    }
    #endregion
    
    public void ShowStory()
    {
        storyImageWindow.SetActive(true);

        for (int i = 0;i < StoryImage.Length;i++)
        {
            StoryImage[i].gameObject.SetActive(true);  
        }
    }

    public void DisablePlayerControl(bool actSwitch)
    {
        isPlayerControlDisabled = actSwitch;
    }

    public void MenuSwitch()
    {//Esc������ �� ����. StartMenu�� ������ ����.
        //�ε��߿��� �������� ����.
        if (loadingUI.LoadBG.activeSelf)
        {
            return;
        }
        else if (settingWindow.activeSelf)
        {
            if (soundWindow.activeSelf)
            {
                //����â�� ���� �� ���Ͽ� ����(+��ư���� �Լ� ���)
                soundUI.SaveVolumeData();
            }

            else if (inputKeyWindow.activeSelf)
            {
                if (inputKeyUI.completeEditingKey)
                {
                    inputKeyUI.CompleteEditingKey(false);
                    return;
                }

                mouseUI.SaveMouseData();
            }

            //����â ��ü�� ����
            settingWindow.SetActive(false);
        }
        else if (loadSlotWindow.activeSelf)
        {
            Debug.Log(3);

            SaveManager.instance.SelectedIndex = null;

            if (loadWindow.activeSelf)
            {
                loadWindow.SetActive(false);
            }
            else
            {
                loadSlotWindow.SetActive(false);
            }
        }
        else if (quitWindow.activeSelf)
        {
            quitWindow.SetActive(false);
        }
        else if (pauseMenu.activeSelf)
        {//�Ͻ����� �޴�
            if (goToMainMenuWindow.activeSelf)
            {
                goToMainMenuWindow.SetActive(false);
            }
            else
            {
                //�Ͻ����� -> �÷���
                gameTimeScale.SetTimeScale(1);

                if (!timelineHelper.IsTimelinePlaying())
                {
                    isPlayerControlDisabled = false;
                    playerSound.TogglePlayingAudioPause(false);

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
                }
                else
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                }

                pauseMenu.SetActive(false);
                ActivateLetterBox(false);
            }
        }
        else if (mainMenu.activeSelf)
        {//��ŸƮ �޴�
            SaveManager.instance.SelectedIndex = null;

            if (settingWindow.activeSelf)
            {
                settingWindow.SetActive(false);
            }
            else if (newGameWindow.activeSelf)
            {
                newGameWindow.SetActive(false);
            }
        }
        else
        {//pauseMenu Ȱ��ȭ ��Ű��(pauseMenu Ȱ��ȭ �ȵǴ� ��Ȳ�� ������)
            if (!canShowPauseMenu)
            {
                return;
            }

            gameTimeScale.SetTimeScale(0);

            isPlayerControlDisabled = true;

            playerSound.TogglePlayingAudioPause(true);

            pauseMenu.SetActive(true);
            ActivateLetterBox(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    public void CanShowPauseMenu(bool canShow)
    {
        canShowPauseMenu = canShow;
    }

    public void ActivateLetterBox(bool activate)
    {
        letterBoxImage.enabled = activate;

        letterBoxMaskImage.enabled = activate;
        letterBoxMask.enabled = activate;
    }

    public void SetPCPlatform(bool activate)
    {
        inputKeyWindow.transform.GetChild(0).gameObject.SetActive(activate);
        battleGuideHUD.SetActive(activate);

        inputHUD.SetActive(!activate);
    }

    #region Button
    public void Restart()
    {
        Time.timeScale = 1f;
        isPlayerControlDisabled = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif
    }
    #endregion

    #region Load, Save(��� â�� �������� �����߰���)
    public void RecordSlot(int filePathIndex, int sceneIndex, int areaIndex, string time)
    {
        //���� ��� index�� ������ index�� recordButtons[filePathIndex] 
        //�ð� ������ timeIndex�� ���
        loadSlots[filePathIndex].gameObject.SetActive(true);
        timeIndex[filePathIndex] = sceneIndex * 100 + areaIndex;

        //������Ʈ Ȱ��ȭ �� �̺�Ʈ �ο�
        loadSlots[filePathIndex].interactable = true;
        loadSlots[filePathIndex].onClick.RemoveAllListeners();
        loadSlots[filePathIndex].onClick.AddListener(() => loadWindow.SetActive(true));
        loadSlots[filePathIndex].onClick.AddListener(() => SaveManager.instance.SelectIndex(filePathIndex));

        #region ���� �ؽ�Ʈ
        string areaName;
        if (PlatformSwitcher.instance.IsKorean)
        {
            areaName = areaIndex switch
            {
                0 => "���� �� ����",
                1 => "���� �Ա� 1",

                2 => "���� �Ա� 2",
                3 => "���� �߾ӱ�",

                4 => "�������� ���� ��",
                5 => "������ �ٸ� 1",
                6 => "������ �ٸ� 2",
                7 => "������ �ٸ� 3",
                8 => "������ �ٸ� 4",

                9 => "���� ���� 1",
                10 => "���� ���� 2",
                11 => "���� ���� 3",
                12 => "���� ���� 4",
                13 => "���� ���� 5",

                14 => "������ ��",

                _ => "��������"
            };
        }
        else
        {
            areaName = areaIndex switch
            {
                0 => "Grace Cave Side Path",
                1 => "Grace Cave Entrance 1",

                2 => "Grace Cave Entrance 2",
                3 => "Hyeonmu Central Road",

                4 => "The path to the west",
                5 => "Wisu's Bridge 1",
                6 => "Wisu's Bridge 2",
                7 => "Wisu's Bridge 3",
                8 => "Wisu's Bridge 4",

                9 => "Ruined Wisu's Garden",
                10 => "Ruined Wisu's Garden",
                11 => "Ruined Wisu's Garden",
                12 => "Ruined Wisu's Garden",
                13 => "Ruined Wisu's Garden",

                14 => "Wisu's Area",

                _ => "Error"
            };
        }

        areaText[filePathIndex].text = areaName;
        #endregion

        #region SaveType
        if (PlatformSwitcher.instance.IsKorean)
        {
            saveTypeText[filePathIndex].text = "�ڵ� ����";
        }
        else
        {
            saveTypeText[filePathIndex].text = "Auto Save";
        }

        #endregion

        #region PlayTime
        playTimeText[filePathIndex].text = "";
        #endregion

        #region Time
        dateText[filePathIndex].text = $"{time}";
        #endregion

        #region No Data
        noDataText[filePathIndex].text = "";
        #endregion

        //�ؽ�Ʈ �ۼ�

        //���� ����
        SortSlots();
    }
    public void ChangeSlotLanguage()
    {
        ResetSlot();
        for (int i = 0; i < SaveManager.instance.Filepath.Length; i++)
        {
            SaveManager.instance.WriteSlotDate(i);
        }
        SortSlots();
    }

    public void ResetSlot()
    {
        for (int i = 0; i < timeIndex.Length; i++)
        {
            areaText[i].text = "";
            saveTypeText[i].text = "";
            playTimeText[i].text = "";
            dateText[i].text = "";
            if (PlatformSwitcher.instance.IsKorean)
            {
                noDataText[i].text = "������ ����";
            }
            else
            {
                noDataText[i].text = "No Data";
            }

            timeIndex[i] = null;
        }
    }
    public void SortSlots()
    {
        //�����Ͱ� �ִ� �����̶�� ���ؼ� ����, �󽽷��� ��Ȱ��ȭ

        //timeIndex ������� transform ��ġ ����
        bool[] isSelected = new bool[loadSlots.Length];

        for (int i = 0; i < loadSlots.Length; i++)
        {
            int index = 0;
            int biggestNumber = 0;

            for (int j = 0; j < loadSlots.Length; j++)
            {
                //���� ������ ���ٸ� ��Ȱ��ȭ�ϰ� ���� ������
                if (!timeIndex[j].HasValue)
                {
                    continue;
                }


                //���õ� index ����
                if (isSelected[j]) continue;

                if (biggestNumber < timeIndex[j].Value)
                {
                    biggestNumber = timeIndex[j].Value;
                    index = j;
                }
            }

            //�����ִ� �� �߿� ���� ū ���� index

            isSelected[index] = true;
            loadSlots[index].transform.SetAsFirstSibling();
        }
    }

    public void CloseWindowUsingButton()
    {
        settingWindow.SetActive(false);
    }
    #endregion

}