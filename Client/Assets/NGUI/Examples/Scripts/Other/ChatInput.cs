using UnityEngine;

[RequireComponent(typeof(UIInput))]
[AddComponentMenu("NGUI/Examples/Chat Input")]
public class ChatInput : MonoBehaviour
{
    public UITextList textList;
    public UIInput m_IptUsername;

    private UIInput mInput;

    private void Start()
    {
        mInput = GetComponent<UIInput>();
        mInput.label.maxLineCount = 1;
    }

    public void OnSubmit()
    {
        if (textList != null)
        {
            // It's a good idea to strip out all symbols as we don't want user input to alter colors, add new lines, etc
            string text = NGUIText.StripSymbols(mInput.value);

            if (!string.IsNullOrEmpty(text))
            {
                textList.Add(text);
                mInput.value = "";
                mInput.isSelected = false;
            }
        }
    }

    public void OnConnect()
    {
    }
}