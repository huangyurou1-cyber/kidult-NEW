using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwoPlayerQTEBattleManager : MonoBehaviour
{
    [Header("怪物設定")]
    public MonsterController currentMonster;
    public GameObject monsterPrefab;
    public Transform monsterSpawnPoint;

    [Header("指令隨機長度範圍 (例如 2 ~ 4)")]
    [Range(1, 6)] public int minCommandLength = 2;
    [Range(1, 6)] public int maxCommandLength = 4;

    [Header("手把按鍵 Sprite (A, B, X, Y)")]
    public Sprite iconA;
    public Sprite iconB;
    public Sprite iconX;
    public Sprite iconY;

    // 儲存已註冊的玩家 UI 控制器 (P1 = index 0, P2 = index 1)
    private PlayerUIController p1Controller;
    private PlayerUIController p2Controller;

    private List<CommandType> p1Buffer = new List<CommandType>();
    private List<CommandType> p2Buffer = new List<CommandType>();

    void Start()
    {
        ClearAllDisplayUI();
        SpawnRandomMonster();
    }

    /// <summary>
    /// 由 PlayerUIController 自動向管理器註冊
    /// </summary>
    public void RegisterPlayerUI(PlayerUIController controller)
    {
        if (controller.playerId == 0) p1Controller = controller;
        else if (controller.playerId == 1) p2Controller = controller;
    }

    public void ReceivePlayerInput(int playerId, CommandType cmd)
    {
        if (currentMonster == null) return;

        List<CommandType> targetBuffer = (playerId == 0) ? p1Buffer : p2Buffer;
        PlayerUIController targetController = (playerId == 0) ? p1Controller : p2Controller;

        if (targetController == null) return;

        // 依據玩家自己的 UI 格子數做上限限制
        if (targetBuffer.Count < targetController.MaxSlotCount)
        {
            int slotIndex = targetBuffer.Count;
            targetBuffer.Add(cmd);

            // 通知該玩家的 UI 點亮對應格子
            targetController.SetSlotIcon(slotIndex, GetSprite(cmd));
        }
    }

    public void ReceivePlayerConfirm(int playerId)
    {
        if (currentMonster == null) return;

        List<CommandType> targetBuffer = (playerId == 0) ? p1Buffer : p2Buffer;
        bool isMatch = true;

        if (targetBuffer.Count != currentMonster.targetCommands.Count)
        {
            isMatch = false;
        }
        else
        {
            for (int i = 0; i < targetBuffer.Count; i++)
            {
                if (targetBuffer[i] != currentMonster.targetCommands[i])
                {
                    isMatch = false;
                    break;
                }
            }
        }

        if (isMatch)
        {
            Debug.Log($"玩家 P{playerId + 1} 成功消滅怪物！");
            currentMonster.OnDefeated();
            ClearAllDisplayUI();
            SpawnRandomMonster();
        }
        else
        {
            Debug.Log($"玩家 P{playerId + 1} 指令錯誤，清空該玩家輸入！");
            ClearSinglePlayerUI(playerId);
        }
    }

    private void ClearSinglePlayerUI(int playerId)
    {
        if (playerId == 0)
        {
            p1Buffer.Clear();
            if (p1Controller != null) p1Controller.ClearSlotDisplay();
        }
        else if (playerId == 1)
        {
            p2Buffer.Clear();
            if (p2Controller != null) p2Controller.ClearSlotDisplay();
        }
    }

    private void ClearAllDisplayUI()
    {
        ClearSinglePlayerUI(0);
        ClearSinglePlayerUI(1);
    }

    private void SpawnRandomMonster()
    {
        if (monsterPrefab == null || monsterSpawnPoint == null) return;

        int randomLength = Random.Range(minCommandLength, maxCommandLength + 1);
        GameObject newMonsterObj = Instantiate(monsterPrefab, monsterSpawnPoint.position, Quaternion.identity);
        currentMonster = newMonsterObj.GetComponent<MonsterController>();
        currentMonster.SetupRandomCommands(randomLength);
    }

    private Sprite GetSprite(CommandType cmd) => cmd switch
    {
        CommandType.A => iconA,
        CommandType.B => iconB,
        CommandType.X => iconX,
        CommandType.Y => iconY,
        _ => null
    };
}