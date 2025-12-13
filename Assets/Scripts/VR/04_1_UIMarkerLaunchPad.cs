// using UnityEngine;
// using UnityEngine.EventSystems;
// using System.Collections;

// public class UIMarkerLaunchPad : MonoBehaviour,
//     IPointerDownHandler, IPointerUpHandler
// {
//     [Header("Marker")]
//     [SerializeField] private T6MarkerData markerTemplate;

//     [Header("Long Press")]
//     [SerializeField] private float longPressTime = 0.4f;

//     [Header("Refs")]
//     [SerializeField] private MarkerSpawnController spawnController;

//     private Coroutine longPressCo;
//     private bool isPointerDown;

//     // 1 발사대 = 1 마커
//     private MarkerEntity spawnedMarker;

//     private void Awake()
//     {
//         spawnController = FindAnyObjectByType<MarkerSpawnController>();
//     }

//     public void OnPointerDown(PointerEventData eventData)
//     {
//         if (spawnedMarker != null)
//             return;

//         isPointerDown = true;
//         longPressCo = StartCoroutine(LongPressRoutine());
//     }

//     public void OnPointerUp(PointerEventData eventData)
//     {
//         isPointerDown = false;

//         if (longPressCo != null)
//         {
//             StopCoroutine(longPressCo);
//             longPressCo = null;
//         }
//     }

//     private IEnumerator LongPressRoutine()
//     {
//         float t = 0f;

//         while (t < longPressTime)
//         {
//             if (!isPointerDown)
//                 yield break;

//             t += Time.deltaTime;
//             yield return null;
//         }

//         // 롱프레스 성립
//         spawnedMarker = spawnController.SpawnFromLaunchPad(
//             this,
//             markerTemplate
//         );
//         InventoryScroll.Instance.SetScroll(false);
//     }

//     public void ClearSpawnedMarker()
//     {
//         spawnedMarker = null;
//     }
// }
