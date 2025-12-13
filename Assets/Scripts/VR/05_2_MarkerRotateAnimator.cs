// using UnityEngine;
// using System.Collections;

// public class MarkerRotateAnimator : MonoBehaviour
// {
//     public static MarkerRotateAnimator Instance {get; private set;}

//     [Header("Refs")]
//     [SerializeField] private MarkerSelectionController selectionController;

//     [Header("Rotation Settings")]
//     [SerializeField] private float rotateSpeed = 180f;   // degrees per second
//     [SerializeField] private bool loop = true;

//     private Coroutine rotateCo;
//     private MarkerEntity currentTarget;

//     private void Awake()
//     {
//         Instance = this;
//     }

//     /// <summary>
//     /// 회전 애니메이션 시작
//     /// </summary>
//     public void StartRotate()
//     {
//         MarkerEntity selected = selectionController.GetSelectedMarker();
//         if (selected == null)
//             return;

//         // 이미 같은 마커를 회전 중이면 무시
//         if (currentTarget == selected && rotateCo != null)
//             return;

//         StopRotate();

//         currentTarget = selected;
//         rotateCo = StartCoroutine(RotateRoutine());
//     }

//     /// <summary>
//     /// 회전 애니메이션 중지
//     /// </summary>
//     public void StopRotate()
//     {
//         if (rotateCo != null)
//         {
//             StopCoroutine(rotateCo);
//             rotateCo = null;
//         }

//         currentTarget = null;
//     }

//     private IEnumerator RotateRoutine()
//     {
//         while (currentTarget != null)
//         {
//             // Y축 회전만 적용
//             currentTarget.transform.Rotate(
//                 Vector3.up,
//                 rotateSpeed * Time.deltaTime,
//                 Space.World
//             );

//             // 데이터 동기화 (선택: 회전값 저장이 필요할 경우)
//             currentTarget.SyncData();

//             if (!loop)
//                 break;

//             yield return null;
//         }

//         rotateCo = null;
//     }
// }
