// using UnityEngine;

// public class MarkerGhostVisual : MonoBehaviour
// {
//     [Header("Ghost Target")]
//     [SerializeField] private Renderer markRenderer;  // 실제 보이는 메쉬
//     [SerializeField] private Material ghostMaterial; // 고스트 전용 머티리얼(투명)

//     private Material originalSharedMaterial;

//     private void Awake()
//     {
//         if (markRenderer == null || ghostMaterial == null)
//         {
//             Debug.LogError("[MarkerGhostVisual] Renderer or GhostMaterial missing");
//             enabled = false;
//             return;
//         }

//         originalSharedMaterial = markRenderer.sharedMaterial;
//     }

//     public void EnableGhost()
//     {
//         if (markRenderer == null) return;
//         markRenderer.sharedMaterial = ghostMaterial;
//     }

//     public void DisableGhost()
//     {
//         if (markRenderer == null) return;
//         markRenderer.sharedMaterial = originalSharedMaterial;
//     }
// }
