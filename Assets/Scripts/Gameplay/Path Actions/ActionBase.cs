public abstract class ActionBase : MonoBehaviour
{
    public PlayerJob MatchJob = PlayerJob.Any;

    public ActionCommandInfo CommandInfo;

    /// <summary>
    /// 判断是否可以显示的虚方法
    /// </summary>
    /// <param name="interactor">交互者的定义数据对象</param>
    /// <returns>总是返回true，表示默认情况下总是可以显示</returns>
    public virtual bool CanShow(AllyDefinitionSO interactor)
    {
        return IsJobMatch(interactor);
    }

    public virtual bool CanExecute(AllyDefinitionSO interactor) => true;

    public virtual void TriggerAction(AllyDefinitionSO interactor)
    {
        // 默认行为，如果需要打开面板操作，请重写这个方法
        Execute(interactor);
    }

    public virtual void Execute(object contextData = null)
    {

    }

    protected virtual bool IsJobMatch(AllyDefinitionSO interactor)
    {
        return MatchJob == PlayerJob.Any || MatchJob == interactor.Job;
    }
}

[System.Serializable]
public struct ActionCommandInfo
{
    public string DisplayName;
    public Sprite Icon;
    public int Order;
}