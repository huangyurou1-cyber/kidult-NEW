using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    public enum CommandActionType
    {
        InputA,
        InputB,
        InputX,
        InputY,
        ConfirmRB
    }

    [Serializable]
    public struct ButtonInputBinding
    {
        public string buttonName;
        public CommandActionType actionType;
        public InputActionReference actionRef;
        public Button targetButton;
    }

    [Header("玩家識別 (0: P1 左側, 1: P2 右側)")]
    public int playerId = 0;

    [Header("玩家專屬輸入顯示區容器 (拖入場景上的 P1-Panel 或 P2-Panel)")]
    public Transform slotContainer;

    [Header("場景戰鬥管理器")]
    public TwoPlayerQTEBattleManager battleManager;

    [Header("按鍵輸入與 UI 綁定清單 (A, B, X, Y, RB)")]
    [SerializeField] private ButtonInputBinding[] buttonBindings;

    // 此玩家自己管理的格子陣列
    private Image[] inputSlots;

    public int MaxSlotCount => inputSlots != null ? inputSlots.Length : 0;

    private void Awake()
    {
        if (battleManager == null)
        {
            battleManager = FindObjectOfType<TwoPlayerQTEBattleManager>();
        }

        //自動抓取此玩家 slotContainer 底下的所有 Image 格子
        if (slotContainer != null)
        {
            Image[] allImages = slotContainer.GetComponentsInChildren<Image>(true);
            List<Image> validSlots = new List<Image>();
            foreach (var img in allImages)
            {
                if (img.transform != slotContainer) validSlots.Add(img);
            }
            inputSlots = validSlots.ToArray();
        }

        ClearSlotDisplay();
    }

    private void Start()
    {
        // 向管理器註冊自己
        if (battleManager != null)
        {
            battleManager.RegisterPlayerUI(this);
        }
    }

    private void OnEnable()
    {
        foreach (var binding in buttonBindings)
        {
            if (binding.actionRef == null) continue;

            binding.actionRef.action.Enable();

            Button btn = binding.targetButton;
            string bName = binding.buttonName;
            CommandActionType type = binding.actionType;

            binding.actionRef.action.performed += ctx => HandlePress(btn, bName, type);
            binding.actionRef.action.canceled += ctx => HandleRelease(btn);
        }
    }

    private void OnDisable()
    {
        foreach (var binding in buttonBindings)
        {
            if (binding.actionRef == null) continue;
            binding.actionRef.action.Disable();
        }
    }

    private void HandlePress(Button button, string buttonName, CommandActionType type)
    {
        if (button != null)
        {
            PointerEventData data = new PointerEventData(EventSystem.current);
            button.OnPointerDown(data);
            button.onClick.Invoke();
        }

        if (battleManager == null) return;

        switch (type)
        {
            case CommandActionType.InputA:
                battleManager.ReceivePlayerInput(playerId, CommandType.A);
                break;
            case CommandActionType.InputB:
                battleManager.ReceivePlayerInput(playerId, CommandType.B);
                break;
            case CommandActionType.InputX:
                battleManager.ReceivePlayerInput(playerId, CommandType.X);
                break;
            case CommandActionType.InputY:
                battleManager.ReceivePlayerInput(playerId, CommandType.Y);
                break;
            case CommandActionType.ConfirmRB:
                battleManager.ReceivePlayerConfirm(playerId);
                break;
        }
    }

    private void HandleRelease(Button button)
    {
        if (button != null)
        {
            PointerEventData data = new PointerEventData(EventSystem.current);
            button.OnPointerUp(data);
        }
    }

    // --- UI 渲染接口 (由 BattleManager 呼叫) ---

    /// <summary>
    /// 點亮並設置指定格子的圖示
    /// </summary>
    public void SetSlotIcon(int index, Sprite sprite)
    {
        if (inputSlots != null && index < inputSlots.Length && inputSlots[index] != null)
        {
            inputSlots[index].gameObject.SetActive(true);
            inputSlots[index].sprite = sprite;
        }
    }

    /// <summary>
    /// 清空此玩家所有的 UI 格子
    /// </summary>
    public void ClearSlotDisplay()
    {
        if (inputSlots == null) return;
        for (int i = 0; i < inputSlots.Length; i++)
        {
            if (inputSlots[i] != null)
            {
                inputSlots[i].gameObject.SetActive(false);
            }
        }
    }
}