using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : Singleton<PartyManager>
{
    [Header("Initial Party")]
    [SerializeField] private CharacterDefinitionSO PlayerDefinition;
    [SerializeField] private List<CharacterRuntimeData> partyMembers = new();

    public List<CharacterRuntimeData> PartyMembers => partyMembers;

    protected override void Awake()
    {
        base.Awake();
        InitParty();
    }

    private void InitParty()
    {
        if (partyMembers.Count == 0)
        {
            partyMembers.Add(new CharacterRuntimeData(PlayerDefinition));
        }
    }

    private void AddMember(CharacterDefinitionSO characterDefinition)
    {
        partyMembers.Add(new CharacterRuntimeData(characterDefinition));
    }

    public void RecruitMember(CharacterDefinitionSO newCharacter)
    {
        AddMember(newCharacter);
        GameModeManager.Instance.RequestChangeGameMode(GameMode.Explore);
    }
}
