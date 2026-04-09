using UnityEngine;
using UnityEngine.UI;
using TMPro;

// [ExecuteAlways] 是关键：它让脚本在编辑模式下也运行
[ExecuteAlways]
[RequireComponent(typeof(LayoutElement))]
public class LayoutMaxWidth : MonoBehaviour
{
    public float maxWidth = 400f; // 在 Inspector 面板随时调
    private TextMeshProUGUI textComponent;
    private LayoutElement layoutElement;

    void OnEnable()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        layoutElement = GetComponent<LayoutElement>();
    }

    // 在 Update 里实时监听，编辑器里打字就能看到变化
    void Update()
    {
        if (textComponent == null || layoutElement == null) return;

        // 获取文字如果不换行时的自然宽度
        float preferredWidth = textComponent.preferredWidth;

        // 核心逻辑：
        // 如果文字宽度没到最大值，LayoutElement 不限制宽度（-1）
        // 如果超过了，就强制锁定为 maxWidth，从而触发 TMP 的换行机制
        layoutElement.preferredWidth = (preferredWidth >= maxWidth) ? maxWidth : -1;
    }
}