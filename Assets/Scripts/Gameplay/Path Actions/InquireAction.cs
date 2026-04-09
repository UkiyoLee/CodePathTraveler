public class InquireAction : ActionBase
{
    [Header("打听消息配置")]
    [SerializeField] private List<InquireActionData> inquireActionDatas = new();

    public int PickRandomMessageIndex() => Random.Range(0, inquireActionDatas.Count);
    public void GetInquireActionData(int index, out InquireActionData data) => data = inquireActionDatas[index];

    public override void TriggerAction(AllyDefinitionSO interactor)
    {
        EventBus.Publish(new PanelRequestEvent(this));
    }
}

[System.Serializable]
public class InquireActionData
{
    [Header("消息显示信息")]
    public string title;
    public string personName;

    [TextArea(2, 6)]
    public string message; 
    public Sprite portraitOverride;
}