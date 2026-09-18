using System; 
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum KeyAction
{ 
    UP,
    DOWN,
    LEFT,
    RIGHT,

    LOCKONTARGET,
    INTERACT,

    ATTACK_NORMAL,
    ATTACK_SPECIAL,
    ATTACK_FINISH,
    DASH,

    MENU,

    KEYCOUNT
}

public enum SceneName
{
    Area_0_Tutorial,
    Area_1,
    Area_2,
    Area_3,
    Area_4_FirstBoss
}

public enum AreaName
{
    Area_00, //���� �� ����
    Area_01, //���� �Ա� 1

    Area_10, //���� �Ա� 2
    Area_11, //���� �߾ӱ�

    Area_20, //�������� ���±�
    Area_21, //������ �ٸ� 1
    Area_22, //������ �ٸ� 2
    Area_23, //������ �ٸ� 3
    Area_24, //������ �ٸ� 4

    Area_30, //���� ���� 1
    Area_31, //���� ���� 2
    Area_32, //���� ���� 3
    Area_33, //���� ���� 4
    Area_34, //���� ���� 5

    Area_40 //������ ��
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private Player player;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private MenuUI menuUI;
    [SerializeField] private MaskChange maskChange;

    #region playState
    private int currentIndex;
    public int CurrentIndex
    {
        get
        {
            return currentIndex;
        }
        set
        {
            currentIndex = value;
        }
    }

    private string[] filePath = new string[4];
    public string[] Filepath
    {
        get { return filePath; }
    }

    private int? selectedIndex = null;
    public int? SelectedIndex
    {
        get
        {
            return selectedIndex;
        }
        set
        {
            selectedIndex = value;
        }
    }

    private int currentHP;
    public int CurrentHP
    {
        get 
        {
            return currentHP; 
        } 
        set
        {
            currentHP = value;
        }
    }

    private bool isHumanMask;
    public bool IsHumanMask
    {
        get
        {
            return isHumanMask;
        }
        set
        {
            isHumanMask = value;
        }
    }


    [SerializeField] private List<GameObject> lights; // Light ������Ʈ�� �����ϴ� ����Ʈ�� ����
    [SerializeField] private List<GameObject> postProcessVolumes; // PostProcessVolume�� ���Ե� �� ������Ʈ ����Ʈ�� ����
    #endregion

    #region Position
    private int currentSceneIndex;
    public int CurrentSceneIndex
    {
        get
        {
            return currentSceneIndex;
        }
        set
        {
            currentSceneIndex = value;
        }
    }
    
    private int currentAreaIndex;
    public int CurrentAreaIndex
    {
        get 
        {
            return currentAreaIndex; 
        }
        set 
        {
            currentAreaIndex = value; 
        }
    }
    
    private Vector3 currentPosition;
    public Vector3 CurrentPosition
    {
        get
        {
            return currentPosition;
        }
        set
        {
            currentPosition = value;
        }
    }

    private Transform startPosition;
    #endregion

    #region Sound
    public float masterVolume { get; private set; }
    public float BGMVolume { get; private set; }
    public float enemySFXVolume { get; private set; }
    public float playerSFXVolume { get; private set; }

    private string soundSettingFilePath;
    #endregion

    #region Input
    //Key
    private string inputKeySettingFilePath;
    private Dictionary<KeyAction, KeyCode> inputKeys = new Dictionary<KeyAction, KeyCode>();
    public Dictionary<KeyAction, KeyCode> InputKeys { get { return inputKeys; } }

    private KeyCode[] defaultKeys = new KeyCode[]
    { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D,
    KeyCode.LeftShift, KeyCode.X,
    KeyCode.Mouse0, KeyCode.Q, KeyCode.F, KeyCode.Mouse1,
    KeyCode.Escape};

    //Mouse 

    private string mouseSettingFilePath;
    public float mouseSpeedWithXAxis { get; private set; } //�¿�
    public float mouseSpeedWithYAxis { get; private set; } //����
    #endregion

    private void Awake()
    {
        #region �̱���
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this.gameObject);
        #endregion

        //filePath���� ������ ����
        for (int i = 0; i < filePath.Length; i++)
        {
            filePath[i] = Path.Combine(Application.persistentDataPath, $"CharacterSlotData{i}");
        }

        //�Ҹ����� ��� ����. ����� ������, �ƴϸ� ����Ʈ ������ �ҷ�����
        soundSettingFilePath = Path.Combine(Application.persistentDataPath, "SoundSettingData");
        if (!File.Exists(soundSettingFilePath))
        {
            ChangeVolumeSetting(0, 0, 0, 0);
            SaveSoundData();
        }
        LoadSoundData();

        //Ű���� ��� ����. ����� ������, �ƴϸ� ����Ʈ Ű �ҷ�����
        inputKeySettingFilePath = Path.Combine(Application.persistentDataPath, "InputKeySettingData");
        if (!File.Exists(inputKeySettingFilePath))
        {
            ChangeKeysSetting(defaultKeys);
            SaveInputKeyData();
        }
        LoadInputKeyData();

        //���콺 ��� ����. ����� ������, �ƴϸ� ����Ʈ ������ �ҷ�����
        mouseSettingFilePath = Path.Combine(Application.persistentDataPath, "MouseSettingData");
        if (!File.Exists(mouseSettingFilePath))
        {
            ChangeMouseSetting(250, 15);
            SaveMouseData();
        }
        LoadMouseData();
    }
    private void Start()
    {
        player = PlayerController.instance.player;

        //���۽� ����� ������ ���Կ� ���
        menuUI.ResetSlot();
        for (int i = 0; i < filePath.Length; i++)
        {
            WriteSlotDate(i);
        }
        menuUI.SortSlots();

        selectedIndex = null;
    }

    #region PlayerState Data
    //���� ���� ��η� �ѱ�� �Լ�(currentIndex�� )
    public void MoveToNextIndex()
    {
        //���� ������ �ϳ��� ������ ��ȯ
        bool useFunction = false;
        for (int i = 0; i < filePath.Length; i++)
        {
            if (File.Exists(filePath[i]))
            {
                useFunction = true;
            }
        }
        if (!useFunction)
        {
            return;
        }

        CurrentIndex = (CurrentIndex + 1) % filePath.Length;
    }

    public void MoveToPreviousIndex()
    {
        //���� ������ �ϳ��� ������ ��ȯ
        bool useFunction = false;
        for (int i = 0; i < filePath.Length; i++)
        {
            if (File.Exists(filePath[i]))
            {
                useFunction = true;
            }
        }
        if (!useFunction)
        {
            return;
        }

        CurrentIndex = (filePath.Length + CurrentIndex - 1) % filePath.Length;
    }

    public void SelectIndex(int index)
    {
        //������ �ִٸ� �����ϰ� ��������� currentIndex�� �״�� ����
        selectedIndex = index;
    }

    //selectedIndex�� currentIndex�� Ȯ������
    public void SetCurrentIndex()
    {
        CurrentIndex = selectedIndex.Value;
    }

    // CSV ���Ͽ� ĳ���� ������ ����, ���� �������� �Ѿ
    public void SaveSloatData()
    {//currentIndex, ��¥, ��ġ, Ż����, ����, ����Ʈ ���μ��� ���� �� ���� currentIndex�� �ѱ�
        float health = player.currentHP;
        int maskType = maskChange.HumanMask.activeSelf ? 1 : 0;
        string currentTime = null;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        using (StreamWriter writer = new StreamWriter(filePath[CurrentIndex]))
        {
            // currentIndex
            writer.WriteLine("CurrentIndex");
            writer.WriteLine($"{CurrentIndex}");
            
            // �ð�(�ʴ���)
            writer.WriteLine("Time");
            currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            writer.WriteLine($"{currentTime}");

            // SceneIndex
            writer.WriteLine("SceneIndex");
            writer.WriteLine($"{currentSceneIndex}");

            // AreaIndex
            writer.WriteLine("AreaIndex");
            writer.WriteLine($"{currentAreaIndex}");

            // ĳ���� Ż����
            writer.WriteLine("MaskType");
            int isHumanMask = maskChange.HumanMask.activeSelf ? 1 : 0;
            writer.WriteLine($"{isHumanMask}");

            // ĳ���� ��ġ
            writer.WriteLine("CharacterPosition");
            writer.WriteLine($"{CurrentPosition.x},{CurrentPosition.y},{CurrentPosition.z}");

            // ü��
            writer.WriteLine("Health");
            writer.WriteLine($"{health}");

            // ���� Ȱ��ȭ ����
            writer.WriteLine("LightObject,Enabled");
            foreach (var light in lights)
            {
                int isEnabled = light.activeSelf ? 1 : 0; // Ȱ��ȭ ���¸� 1 �Ǵ� 0���� ����
                writer.WriteLine($"{light.name},{isEnabled}");
            }

            // ����Ʈ ���μ��� Ȱ��ȭ ����
            writer.WriteLine("PostProcessingObject,Enabled");
            foreach (var postProcessVolume in postProcessVolumes)
            {
                int isEnabled = postProcessVolume.activeSelf ? 1 : 0; // Ȱ��ȭ ���¸� 1 �Ǵ� 0���� ����
                writer.WriteLine($"{postProcessVolume.name},{isEnabled}");
            }
        }

        //���� ���Կ� ���
        menuUI.RecordSlot(CurrentIndex, currentSceneIndex, currentAreaIndex, currentTime);
    }
    public void LoadSlotData()
    {
        if (!File.Exists(filePath[CurrentIndex]))
        {
            return;
        }

        using (StreamReader reader = new StreamReader(filePath[CurrentIndex]))
        {
            string line;

            //������ ���� ���� ��
            bool isLightSection = false;
            bool isPostProcessingSection = false;

            //�д°� ���� ������ �ݺ�
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');

                //����
                if (values[0] == "CurrentIndex")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    CurrentIndex = int.Parse(values[0]);
                }

                if (values[0] == "SceneIndex")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    currentSceneIndex = int.Parse(values[0]);
                }

                if (values[0] == "AreaIndex")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    currentAreaIndex = int.Parse(values[0]);
                }

                if (values[0] == "MaskType")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    if (int.Parse(values[0]) == 1)
                    {
                        if (maskChange.AnimalMask.activeSelf)
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
                }

                // ĳ���� ��ġ�� ü�� �ҷ�����
                if (values[0] == "CharacterPosition")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');

                    maskChange.CurrentMask.transform.position = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
                }

                if (values[0] == "Health")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    player.currentHP = int.Parse(values[0]);
                    HpHUD.instance.ChangeHPStack((int)player.currentHP);
                }
                
                // ����Ʈ ������Ʈ Ȱ��ȭ ���� �ҷ�����
                else if (values[0] == "LightObject")
                {
                    isLightSection = true;
                    continue;
                }
                else if (isLightSection && values.Length == 2)
                {
                    foreach (var light in lights)
                    {
                        if (light.name == values[0])
                        {
                            light.SetActive(values[1] == "1");
                            break;
                        }
                    }
                }
                // ����Ʈ ���μ��� ������Ʈ Ȱ��ȭ ���� �ҷ�����
                else if (values[0] == "PostProcessingObject")
                {
                    isPostProcessingSection = true;
                    continue;
                }
                else if (isPostProcessingSection && values.Length == 2)
                {
                    foreach (var postProcessVolume in postProcessVolumes)
                    {
                        if (postProcessVolume.name == values[0])
                        {
                            postProcessVolume.SetActive(values[1] == "1");
                            break;
                        }
                    }
                }
            }
        }
    }

    //������ �����͸� ��°� �ƴ�!
    //currentIndex ����� ��, ����, �÷��̾� ��ġ ������ �������� 
    //current~ ������ �Ҵ�
    public void GetLoadData()
    {
        using (StreamReader reader = new StreamReader(filePath[CurrentIndex]))
        {
            string line;

            //�д°� ���� ������ �ݺ�
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');

                if (values[0] == "CurrentIndex")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    CurrentIndex = int.Parse(values[0]);
                }

                else if (values[0] == "SceneIndex")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    currentSceneIndex = int.Parse(values[0]);
                }


                else if (values[0] == "AreaIndex")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    currentAreaIndex = int.Parse(values[0]);
                }

                else if (values[0] == "MaskType")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');

                    isHumanMask = (int.Parse(values[0]) == 1);
                }

                // ĳ���� ��ġ�� ü�� �ҷ�����
                if (values[0] == "CharacterPosition")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');

                    currentPosition = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
                }

                if (values[0] == "Health")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    currentHP = int.Parse(values[0]);
                }
            }
        }
    }

    public void ResetPlayerPosition()
    {
        //������ saveManager�� ù��° �ڽĿ�����Ʈ�� ��ġ�� �÷��̾� ��ġ
        startPosition = transform.GetChild(0).transform;
        maskChange.CurrentMask.transform.position = startPosition.position;
        maskChange.CurrentMask.transform.rotation= startPosition.rotation;
    }
    public void ResetData()
    {//����� ����, ���� ���� ����

        for (int i = 0; i < filePath.Length; i++)
        {
            if (File.Exists(filePath[i]))
            {
                File.Delete(filePath[i]);
            }

        }

        menuUI.ResetSlot();

        CurrentIndex = 0;
    }

    //����� �����͸� ���Կ� ���. 
    public void WriteSlotDate(int playStateIndex)
    {
        //����� �����Ͱ� ���ٸ� return;
        if (!File.Exists(filePath[playStateIndex]))
        {
            return;
        }

        //����� �����Ͱ� �ִٸ� ���
        using (StreamReader reader = new StreamReader(filePath[playStateIndex]))
        {
            string line;

            string currentTime = null;
            string currentSceneIndex = null;
            string currentAreaIndex = null;

            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');

                //��ϵ� �ð��� ã�Ƽ� ����UI�� ���
                if (values[0] == "Time")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    currentTime = values[0];
                }

                if (values[0] == "SceneIndex")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    currentSceneIndex = values[0];
                }

                if (values[0] == "AreaIndex")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    currentAreaIndex = values[0];
                }
            }
            //"�ڵ� ����" �̶�� �����ֱ�
            //����ġ������

            menuUI.RecordSlot(playStateIndex, int.Parse( currentSceneIndex), int.Parse( currentAreaIndex), currentTime);
        }
    }
    #endregion

    #region Volume
    public void SaveSoundData()
    {//�Ҹ�, ����Ű �� ȯ������ ��� ������ ����

        using (StreamWriter writer = new StreamWriter(soundSettingFilePath))
        {
            //Volume
            writer.WriteLine("Volume");
            writer.WriteLine($"{masterVolume},{BGMVolume},{enemySFXVolume},{playerSFXVolume}" );
        }
    }

    public void ChangeVolumeSetting(float master, float BGM, float enemySFX, float playerSFX )
    {
        masterVolume = master;
        BGMVolume = BGM;
        enemySFXVolume = enemySFX;
        playerSFXVolume = playerSFX;
    }
    public void LoadSoundData()
    {
        if (!File.Exists(soundSettingFilePath))
        {
            return;
        }

        using (StreamReader reader = new StreamReader(soundSettingFilePath))
        {
            string line;

            //�д°� ���� ������ �ݺ�
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');

                //������, BGM, SFX ���� �ҷ�����
                if (values[0] == "Volume")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    masterVolume = float.Parse(values[0]);
                    BGMVolume = float.Parse(values[1]);
                    enemySFXVolume = float.Parse(values[2]);
                    playerSFXVolume = float.Parse(values[3]);
                }
            }
        }
    }
    #endregion

    #region InputKey
    public void SaveInputKeyData()
    {//������ inputKeys�� ���Ͽ� ����
        using (StreamWriter writer = new StreamWriter(inputKeySettingFilePath))
        {
            writer.WriteLine("KeyAction,KeyCode");
            foreach (var dict in inputKeys)
            {
                writer.WriteLine($"{(int)dict.Key},{dict.Value}");
            }
        }
    }

    public void     ChangeKeysSetting(KeyCode[] keycode)
    {//inputKeys ������ ���� �Ҵ�

        for (int i = 0; i < (int)KeyAction.KEYCOUNT; i++)
        {
            KeyAction keyAction = (KeyAction)i;

            if (!inputKeys.ContainsKey(keyAction))
            {//inputKeys�� �ش� key�� ������ ���� �ʴٸ� 
                inputKeys.Add(keyAction, keycode[i]);
            }
            else
            {
                inputKeys[(KeyAction)i] = keycode[i];
            }
        }
    }

    public void ChangeKeySetting(int index, KeyCode keycode)
    {//inputKeys ������ ���� �Ҵ�

        if (!inputKeys.ContainsKey((KeyAction)index))
        {
            inputKeys.Add((KeyAction)index, keycode);
        }
        else
        {
            inputKeys[(KeyAction)index] = keycode;
        }
    }


    public void LoadInputKeyData()
    {//�ҷ��� ������ ���ٸ� inputKeys�� ����ƮŰ�� �ȴ�.

        if (!File.Exists(inputKeySettingFilePath))
        {
            return;
        }

        using (StreamReader reader = new StreamReader(inputKeySettingFilePath))
        {
            string line;
            bool isKeyActionSection = false;

            //�д°� ���� ������ �ݺ�
            while ((line = reader.ReadLine()) != null)
            {
                //values�� ','�� �и����Ѽ� ���ڿ� ����
                var values = line.Split(',');

                //KeyAction �κк��� ����
                if (values[0] == "KeyAction")
                {
                    isKeyActionSection = true;
                    continue;
                }
                else if (isKeyActionSection && values.Length == 2)
                {

                    int keyActionIdex = int.Parse(values[0]);
                    KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), values[1]);

                    ChangeKeySetting(keyActionIdex, keyCode);
                }
            }
        }
    }

    #endregion

    #region Mouse
    public void SaveMouseData()
    {//�Ҹ�, ����Ű �� ȯ������ ��� ������ ����

        using (StreamWriter writer = new StreamWriter(mouseSettingFilePath))
        {
            writer.WriteLine("Mouse");
            writer.WriteLine($"{mouseSpeedWithXAxis},{mouseSpeedWithYAxis}");
        }
    }

    public void ChangeMouseSetting(float xValue, float yValue)
    {
        mouseSpeedWithXAxis = xValue;
        mouseSpeedWithYAxis = yValue;
    }
    public void LoadMouseData()
    {
        if (!File.Exists(mouseSettingFilePath))
        {
            return;
        }

        using (StreamReader reader = new StreamReader(mouseSettingFilePath))
        {
            string line;

            //�д°� ���� ������ �ݺ�
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');

                //������, BGM, SFX ���� �ҷ�����
                if (values[0] == "Mouse")
                {
                    line = reader.ReadLine();
                    values = line.Split(',');
                    mouseSpeedWithXAxis = float.Parse(values[0]);
                    mouseSpeedWithYAxis = float.Parse(values[1]);
                }
            }
        }
    }
    #endregion
}