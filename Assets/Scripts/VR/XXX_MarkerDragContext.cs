// using UnityEngine;

// public enum MarkerDragKind
// {
//     None,
//     New,
//     Move
// }

// public static class MarkerDragContext
// {
//     public static MarkerDragKind Kind { get; private set; } = MarkerDragKind.None;

//     // New payload
//     public static T6MarkerData NewData { get; private set; }

//     // Move payload
//     public static MarkerInstance MovingMarker { get; private set; }
//     public static Vector3 MoveOriginalPos { get; private set; }
//     public static Quaternion MoveOriginalRot { get; private set; }
//     public static bool MoveOriginalActive { get; private set; }

//     public static bool IsDragging => Kind != MarkerDragKind.None;

//     // 변경 감지(고스트가 새로 Sync해야 할 때)
//     public static int Version { get; private set; } = 0;

//     public static void BeginNew(T6MarkerData data)
//     {
//         if (data == null) return;

//         ResetInternal();
//         Kind = MarkerDragKind.New;
//         NewData = data;

//         Version++;
//         CameraInputGate.Lock();
//     }

//     public static void BeginMove(MarkerInstance marker)
//     {
//         if (marker == null) return;

//         ResetInternal();
//         Kind = MarkerDragKind.Move;
//         MovingMarker = marker;

//         MoveOriginalPos = marker.transform.position;
//         MoveOriginalRot = marker.transform.rotation;
//         MoveOriginalActive = marker.gameObject.activeSelf;

//         // 이동 중엔 실체 숨김(권장 UX)
//         marker.gameObject.SetActive(false);
//         // marker.SetAlpha(0f);

//         Version++;
//         CameraInputGate.Lock();
//     }

//     public static T6MarkerData GetActiveData()
//     {
//         return Kind switch
//         {
//             MarkerDragKind.New => NewData,
//             MarkerDragKind.Move => MovingMarker != null ? MovingMarker.Data : null,
//             _ => null
//         };
//     }

//     public static void CancelMoveRollback()
//     {
//         if (Kind != MarkerDragKind.Move || MovingMarker == null) return;

//         MovingMarker.transform.SetPositionAndRotation(MoveOriginalPos, MoveOriginalRot);
//         MovingMarker.gameObject.SetActive(MoveOriginalActive);
//     }

//     public static void End()
//     {
//         // End는 “정상 종료/실패 종료” 공통 정리만 담당
//         ResetInternal();
//         CameraInputGate.Unlock();
//         Version++;
//     }

//     private static void ResetInternal()
//     {
//         Kind = MarkerDragKind.None;
//         NewData = null;
//         MovingMarker = null;
//         MoveOriginalPos = default;
//         MoveOriginalRot = default;
//         MoveOriginalActive = false;
//     }
// }
