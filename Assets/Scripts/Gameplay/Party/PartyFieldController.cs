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
    [SerializeField] private float followDistance = 1.2f;
    [SerializeField] private float followSpeed = 5f;
}
