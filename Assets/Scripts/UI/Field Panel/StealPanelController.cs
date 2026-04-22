using UnityEngine.UI;
using TMPro;
using System;

public class StealPanelController : PanelController
{
    [Header("Steal Panel")]

    [SerializeField] private StealItemIcon stealItemButtonPrefab;
    [SerializeField] private RectTransform contentRoot;

    [Header("Confirm Popup")]
    [SerializeField] private RectTransform confirmPopup;
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public override Type PanelActionType => typeof(StealAction);
}
