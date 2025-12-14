// using System.Collections.Generic;
// using UnityEngine;

// /// <summary>
// /// 마커 필터 조건 관리자
// /// - 검색 문자열
// /// - 즐겨찾기만 보기 여부
// /// 판단만 담당 (표현/연출 ❌)
// /// </summary>
// public class MarkerFilterController : MonoBehaviour
// {
//     public static MarkerFilterController Instance { get; private set; }

//     // =========================
//     // Filter State
//     // =========================
//     private string searchText = string.Empty;
//     private bool favoriteOnly = false;

//     // =========================
//     // Refs
//     // =========================
//     [Header("Refs")]
//     [SerializeField] private MarkerSlotSpawner slotSpawner;
//     [SerializeField] private MarkerRotateAnimator rotateAnimator;

//     // =========================
//     // Unity Lifecycle
//     // =========================
//     private void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;
//     }

//     // =========================
//     // Public API (Input)
//     // =========================

//     /// <summary>
//     /// 검색 문자열 설정
//     /// </summary>
//     public void SetSearchText(string text)
//     {
//         searchText = string.IsNullOrEmpty(text)
//             ? string.Empty
//             : text.Trim().ToLowerInvariant();

//         Apply();
//     }

//     /// <summary>
//     /// 즐겨찾기만 보기 토글
//     /// </summary>
//     public void SetFavoriteOnly(bool value)
//     {
//         favoriteOnly = value;
//         Apply();
//     }

//     /// <summary>
//     /// 즐겨찾기만 보기 토글 (버튼용)
//     /// </summary>
//     public void ToggleFavoriteOnly()
//     {
//         favoriteOnly = !favoriteOnly;
//         Apply();
//     }

//     // =========================
//     // Core Logic
//     // =========================
//     private void Apply()
//     {
//         HashSet<string> visibleDefinitionIds = CollectVisibleDefinitionIds();

//         // 슬롯 필터링
//         slotSpawner.ApplyFilterByDefinitionIds(visibleDefinitionIds);

//         // 회전 대상 갱신
//         ApplyRotation(visibleDefinitionIds);
//     }

//     private HashSet<string> CollectVisibleDefinitionIds()
//     {
//         HashSet<string> result = new();

//         foreach (var def in MarkerDefinitionRepository.Instance.GetAll())
//         {
//             if (favoriteOnly && !def.IsFavorite)
//                 continue;

//             if (!string.IsNullOrEmpty(searchText))
//             {
//                 if (string.IsNullOrEmpty(def.DisplayName) ||
//                     !def.DisplayName.ToLowerInvariant().Contains(searchText))
//                     continue;
//             }

//             result.Add(def.DefinitionId);
//         }

//         return result;
//     }

//     private void ApplyRotation(HashSet<string> visibleDefinitionIds)
//     {
//         if (visibleDefinitionIds.Count == 0)
//         {
//             rotateAnimator.StopRotate();
//             return;
//         }

//         List<MarkerInstance> targets =
//             MarkerInstanceRegistry.Instance
//                 .GetInstancesByDefinitionIds(visibleDefinitionIds);

//         if (targets.Count == 0)
//         {
//             rotateAnimator.StopRotate();
//             return;
//         }

//         rotateAnimator.StopRotate();
//         rotateAnimator.SetMultipleTargets(targets);
//     }
// }
