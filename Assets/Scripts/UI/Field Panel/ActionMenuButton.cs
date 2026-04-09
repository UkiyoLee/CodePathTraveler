using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ActionMenuButton : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void SetButton(ActionCommandInfo commandInfo, UnityAction onClick)
    {
        _icon.sprite = commandInfo.Icon;
        _buttonText.text = commandInfo.DisplayName;

        if (_button is not null)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(onClick);
        }

    }
}
