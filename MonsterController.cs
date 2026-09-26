using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{
    [Header("預先放置的 4 個指令 Image (Slot)")]
    [Tooltip("請依序拖入 Command Container 底下的 Image_1 ~ Image_4")]
    public Image[] commandSlots = new Image[4];

    [Header("指令對應 Sprite (A:綠, B:紅, X:藍, Y:黃)")]
    public Sprite iconA;
    public Sprite iconB;
    public Sprite iconX;
    public Sprite iconY;

    // 儲存當前怪物的目標指令序列
    public List<CommandType> targetCommands { get; private set; } = new List<CommandType>();

    /// <summary>
    /// 初始化隨機指令 (長度為 1 ~ 4)
    /// </summary>
    public void SetupRandomCommands(int length)
    {
        targetCommands.Clear();
        // 限制長度不超過現有格子數 (4)
        length = Mathf.Clamp(length, 1, commandSlots.Length);

        // 1. 先將 4 個格子全部隱藏
        for (int i = 0; i < commandSlots.Length; i++)
        {
            if (commandSlots[i] != null)
            {
                commandSlots[i].gameObject.SetActive(false);
            }
        }

        // 2. 依長度隨機指派指令並換上對應的 Sprite
        for (int i = 0; i < length; i++)
        {
            CommandType randomCmd = (CommandType)Random.Range(0, 4);
            targetCommands.Add(randomCmd);

            if (commandSlots[i] != null)
            {
                commandSlots[i].gameObject.SetActive(true);
                commandSlots[i].sprite = GetSprite(randomCmd);
            }
        }
    }

    private Sprite GetSprite(CommandType cmd)
    {
        return cmd switch
        {
            CommandType.A => iconA,
            CommandType.B => iconB,
            CommandType.X => iconX,
            CommandType.Y => iconY,
            _ => null
        };
    }

    public void OnDefeated()
    {
        Destroy(gameObject);
    }
}