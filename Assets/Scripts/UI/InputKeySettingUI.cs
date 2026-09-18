using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class InputKeySettingUI : MonoBehaviour
{
    private SaveManager saveManager;

    //��ư���� 
    [Header("��ư�̶� �ؽ�Ʈ ����������")]
    [SerializeField] private Button[] inputKeyButton;
    private TextMeshProUGUI[] inputKeyButtonText = new TextMeshProUGUI[(int)KeyAction.KEYCOUNT];

    public bool isEditingKey { get; private set; }
    public bool completeEditingKey { get; private set; }
    private int currentKeyIndex = -1; 

    private void Awake()
    {
        for (int i = 0; i < inputKeyButton.Length; i++)
        {
            //Closure �����߻�
            // �ݺ������� �����Լ�, delegate �Լ� ���� �߻��ϴ� ����
            // i���� ���簪�� ����ϴ� ���� �ƴ϶� i���� �� ��ü�� ����, �ݺ����� �� ���� �� i���� ������
            // �ذ��� ���ؼ� i�� ���簪�� ���� ������ֱ�
            
            int index = i;
            inputKeyButton[index].onClick.AddListener(()=> EditInputKey(index));

            inputKeyButtonText[i] = inputKeyButton[i].gameObject.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        saveManager = SaveManager.instance;

        for (int i = 0; i < (int)KeyAction.KEYCOUNT; i++)
        {
            ChangeInputKeyButtonText(i);
        }
    }

    private void Update()
    {
        if (isEditingKey)
        {
            //�����߿� �Է��� Ű�ڵ� ������ None �� �ƴ� ��� ��ȯ
            if (DetectPressedKeyCode() != KeyCode.None)
            {
                //�޴���ư�� ���콺 ��Ŭ���̸� �ſ� ��������
                if (currentKeyIndex == (int)KeyAction.MENU)
                {
                    if (DetectPressedKeyCode() == KeyCode.Mouse0)
                    {
                        return;
                    }
                }

                //����Ű ����
                saveManager.ChangeKeySetting(currentKeyIndex, DetectPressedKeyCode());
                saveManager.SaveInputKeyData();

                //UI ����
                ChangeInputKeyButtonText(currentKeyIndex);

                currentKeyIndex = -1;
                isEditingKey = false;

                //�޴� ������� �ߺ����� �ʵ���
                completeEditingKey = true;
            }
        }
    }

    public void CompleteEditingKey(bool complete)
    {
        completeEditingKey = complete;
    }

    private KeyCode DetectPressedKeyCode()
    {//���� �Էµ� Ű�ڵ� ���� ��ȯ
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                return kcode;
            }
        }
        return KeyCode.None;
    }

    private void EditInputKey(int index)
    {//��ư�� ������ ����Ǵ� �Լ�
        isEditingKey = true;
        currentKeyIndex = index;
    }

    private void ChangeInputKeyButtonText(int index)
    {//��ư �ؽ�Ʈ ����

        if (saveManager.InputKeys.ContainsKey((KeyAction)index))
        {
            string text = saveManager.InputKeys[(KeyAction)index].ToString();

            inputKeyButtonText[index].text = text;
        }
    }
}
