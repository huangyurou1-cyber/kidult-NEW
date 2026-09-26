using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIButtonController : MonoBehaviour
{
    // 建立一個結構體，把 Action 與 對應的 Button 組合起來
    [Serializable]
    public struct ButtonInputBinding
    {
        public string buttonName;
        public InputActionReference actionRef;
        public Button targetButton;
    }

    [Header("A, B, X, Y 按鍵 UI 綁定設定")]
    [SerializeField] private ButtonInputBinding[] buttonBindings;

    private void OnEnable()
    {
        foreach (var binding in buttonBindings)
        {
            if (binding.actionRef == null || binding.targetButton == null) continue;

            // 啟用 Input Action
            binding.actionRef.action.Enable();

            // 利用匿名 Lambda 或 捕獲區域變數 來訂閱事件
            Button btn = binding.targetButton;
            string name = binding.buttonName;

            binding.actionRef.action.performed += ctx => HandlePress(btn, name);
            binding.actionRef.action.canceled += ctx => HandleRelease(btn);
        }
    }

    private void OnDisable()
    {
        foreach (var binding in buttonBindings)
        {
            if (binding.actionRef == null) continue;

            // 停用或解綁 Action
            binding.actionRef.action.Disable();
        }
    }

    // 通用的按下邏輯
    private void HandlePress(Button button, string buttonName)
    {
        Debug.Log($"{buttonName} 鍵按下！觸發 Button {buttonName}");

        PointerEventData data = new PointerEventData(EventSystem.current);
        button.OnPointerDown(data);
        button.onClick.Invoke();
    }

    // 通用的放開邏輯
    private void HandleRelease(Button button)
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        button.OnPointerUp(data);
    }
}
