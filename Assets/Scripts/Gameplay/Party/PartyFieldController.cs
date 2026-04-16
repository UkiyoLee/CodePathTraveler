using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyFieldController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform followersParent;
    [SerializeField] private GameObject fieldFollowerPrefab;
    [SerializeField] private Transform playerTrans;

    [Header("Settings")]
    [SerializeField] private float followDistance = 1.2f; //相邻两人的距离
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float zOffset = 0.01f;
    [SerializeField] private float sampleDistance = 0.05f;

    private List<Vector3> trail = new();
    private List<FieldFollower> followers = new();

    public void UpdateFollowers(List<CharacterDefinitionSO> partyMembers)
    {
        
    }
}
