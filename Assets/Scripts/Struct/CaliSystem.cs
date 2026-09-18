using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enemy;

public class CalliSystem : MonoBehaviour
{
    private List<char> paint = new List<char>();
    [SerializeField] private int maxPaintOver = 10;
    public int MaxPaintOver {  get { return maxPaintOver; } }
    private char lastColor = ' '; 

    public float paintWhite { get; private set; }
    public float paintBlack { get; private set; }
    public float paintOver { get; private set; }

    private void Awake()
    {
        // ���� ������ ���� maxPaintOver�� ����. ��ǻ� EnemyData���� ������
        Enemy enemy = GetComponent<Enemy>();
        maxPaintOver = 3;
    }

    public void Painting(char color, float value)
    {
        // ���� ���� ���� ��� paint�� �߰��� ��
        if (paint.Count > 0 && paint[paint.Count - 1] == color)
        {
            for (var i = 0; i < value; i++)
            {
                paint.Add(color);
            }
        }
        else
        {
            // �ٸ� ���� ���� ��� ��ĥ ������ �����
            float previousValue = paint.Count > 0 ? paint[paint.Count - 1] : 0;

            // ���� ���� �ٸ� ���� �������� ���� ���� paintOver ����
            if (paint.Count > 0 && lastColor != ' ' && lastColor != color)
            {
                paintOver += value;
                paintOver = Mathf.Min(paintOver, maxPaintOver);
            }

            for (var i = 0; i < value; i++)
            {
                paint.Add(color);
                if (paint.Count > maxPaintOver)
                {
                    paint.RemoveAt(0); // ťó�� FIFO ������� ����
                }
            }
        }

        lastColor = color;
        UpdatePaint();
    }

    private void UpdatePaint()
    {
        paintWhite = 0;
        paintBlack = 0;

        foreach (var type in paint)
        {
            if (type == 'W')
            {
                paintWhite++;
            }
            if (type == 'B')
            {
                paintBlack++;
            }
        }
    }

    public float StackRatio() // (���� ���� / �ִ� ����)
    {
        return (float)paintOver / maxPaintOver;
    }

    public void ResetPaint()
    {
        paint.Clear();
        paintOver = 0;
        lastColor = ' ';
    }

    public bool IsPaintOverMax()
    {
        return paintOver >= maxPaintOver;
    }
}
