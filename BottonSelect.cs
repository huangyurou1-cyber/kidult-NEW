using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BottonSelect : MonoBehaviour
{
    [System.Serializable]
    public class ButtonSceneBinding
    {
        public string description;     // 備註名稱（如：開始遊戲、選擇關卡）
        public Button targetButton;    // 拖入對應的 UI 按鈕

#if UNITY_EDITOR
        [Tooltip("可以直接從 Project 視窗拖入 Scene 檔案")]
        public SceneAsset sceneAsset;  // Editor 下可直接拖入 .unity 場景檔案
#endif

        [HideInInspector]
        public string sceneName;       // 實際運作讀取的場景名稱
    }

    [Header("預設選取的按鈕（供手把焦點使用）")]
    public Button firstSelectedButton;

    [Header("按鈕與場景跳轉綁定清單")]
    public List<ButtonSceneBinding> buttonBindings = new List<ButtonSceneBinding>();

    [Header("退出遊戲按鈕")]
    public Button quitButton;

    void OnValidate()
    {
#if UNITY_EDITOR
        // 自動提取場景名稱存入 sceneName
        if (buttonBindings != null)
        {
            foreach (var binding in buttonBindings)
            {
                if (binding.sceneAsset != null)
                {
                    binding.sceneName = binding.sceneAsset.name;
                }
            }
        }
#endif
    }

    void Start()
    {
        // 1. 遊戲開始時給予第一個按鈕手把焦點
        if (firstSelectedButton != null)
        {
            firstSelectedButton.Select();
        }

        // 2. 為跳轉場景按鈕綁定事件
        foreach (var binding in buttonBindings)
        {
            if (binding.targetButton != null && !string.IsNullOrEmpty(binding.sceneName))
            {
                string sceneToLoad = binding.sceneName;
                binding.targetButton.onClick.AddListener(() => LoadScene(sceneToLoad));
            }
        }

        // 3. 為退出按鈕綁定退出事件
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("正在退出遊戲...");

#if UNITY_EDITOR
        // 在 Unity 編輯器測試時停止播放模式
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 實際打包輸出（Build）後關閉遊戲程式
        Application.Quit();
#endif
    }
}