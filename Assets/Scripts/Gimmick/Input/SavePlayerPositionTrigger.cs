using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePlayerPositionTrigger : EventData
{
    private SaveManager saveManager;

    [SerializeField] AreaName areaName;

    private void Start()
    {
        saveManager = SaveManager.instance;
    }

    public override void Execute()
    {
        //����1:
        //currentIndex�� �ֱٰ�θ� ����Ű�� �ְ�, ���� UI���� ���� �ֱ� ��θ� �ҷ��� ��,
        //ĳ���Ͱ� Ʈ���� ��Ƽ� ����Ǵ� �Լ�(Execute())�� �����͸� �о���̴� �Լ����� ���� ����Ǹ鼭 ���� �����θ� �ҷ����� ���� �߻�
        //-> NotMoveNextIndex ��� bool ������ ����� ���Ѿ���� �ӽ���ġ

        saveManager.CurrentAreaIndex = (int)areaName;
        saveManager.CurrentPosition = this.transform.position;
        saveManager.SaveSloatData();

        saveManager.MoveToNextIndex();
    }
}
