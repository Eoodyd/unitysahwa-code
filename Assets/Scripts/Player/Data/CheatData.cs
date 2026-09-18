using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public partial class CheatData: ScriptableObject
{
    [Header("�Է�Ű ���� ã�� ���: ���ϴ� �Է�Ű�� ù���ڰ� ���ö����� ù���� ��� �Է��ϸ�ǿ�. \n���� ��� 'Space' ��� �Ѵٸ� S ��� ������ ���� Space�� ������ \n����Ű�� alpha0,1,2... �Դϴ�")]
    [Header("������ Ȯ��")]
    public int frameTextSize = 25;
    public Color frameTextColor = Color.green;
    public KeyCode showFrame;
    public KeyCode frameLimitOff;
    public KeyCode frame30;
    public KeyCode frame60;
    public KeyCode frame144;

    [Header("���� ��ɰ��� ������ �۵�-----------------------------------------------------------------")]
    [Header("ġƮ Ȱ��ȭ")]
    public KeyCode activateCheatMode;

    [Header("����, �ҷ�����(ù��° ���Կ� ����)")]
    public KeyCode saveData;
    public KeyCode loadData;
    
    [Header("���Ӽӵ�(0�̸� �Ͻ�����)")]
    public KeyCode setGameTimeRate;
    public float timeScaleValue;

    [Header("�������ƽ�, Ǯ����")]
    public KeyCode damageMax;
    public KeyCode paintOverlapMax;

    [Header("�̵��ӵ�")]
    public KeyCode moveSpeedUp;
    public float moveSpeed;

    [Header("����")]
    public KeyCode blink;
    public float blinkDistance;

    [Header("�ö��̸��")]
    public KeyCode flyMode;
    public KeyCode moveUp;
    public float flySpeed;


    [Header("ü��")]
    public KeyCode minHealth;
    public KeyCode maxHealth;

    [Header("���")]
    public KeyCode dieFromZeroHealth;
    public KeyCode dieFromFall;

    [Header("�ֺ� ������")]
    public KeyCode clearEnemy;
    public float clearRange;
    public LayerMask clearLayer;

    [Header("��ǥ�̵�(�ٵ� ���� ������Ʈ�� �ʿ���, �ƴϸ� �̵��ϱ� ���ϴ� ���� �����ּ���)")]
    public KeyCode showMap;
}


#region Set
public partial class CheatData : ScriptableObject
{
    #region �̱���
    //�������� �̱���: �̱��� ������Ʈ�� �����ϴ� ������ ������Ʈ�� ���ٸ� �������

    private const string SettingFileDirectory = "Assets/Resources";
    private const string SettingFilePath = "Assets/Resources/CheatData.asset";
    //���ҽ��� �ǵ��� ���X, �������� ���Ǵ� ���� ū ���� ���ٰ� ��
    //Resources ������ ������ Ȯ��, ������ ����

    private static CheatData instance;
    public static CheatData Instance
    {
        get
        {
            if (instance != null) //instance�� �����Ѵٸ� ��������
            {
                return instance;
            }

            //���ٸ� 
            instance = Resources.Load<CheatData>("CheatData");

            //������ Ÿ�ӿ��� �ڵ����� �̸� �����ǵ���, ��Ÿ�ӿ� ������ �־����.
#if UNITY_EDITOR

            if (instance == null)
            {
                if (!AssetDatabase.IsValidFolder(SettingFileDirectory))//�ش� ������ ��ȿ����
                {
                    AssetDatabase.CreateFolder("Assets", "Resources"); //�ƴ϶�� Assets �Ʒ��� Resources ���� ����
                }

                //� ������ ������ �Ȱ����Դٸ� �ϵ��ϰ� ��������
                instance = AssetDatabase.LoadAssetAtPath<CheatData>(SettingFilePath);

                if (instance == null) //�׷����� �Ȱ��������ٸ� ���ٴ� ��. ���� ������ֱ�
                {
                    instance = CreateInstance<CheatData>(); //�̷��� �����ϸ� �޸𸮿��� ����. ���� �������� ������ �ȵ�
                    AssetDatabase.CreateAsset(instance, SettingFilePath); //��� ������ ������Ʈ�� ����Ƽ�������� ����, ������ �� �ְ�
                }

            }
#endif
            return instance;
        }
    }
    #endregion
}
#endregion

