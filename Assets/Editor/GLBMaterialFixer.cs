using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

public class GLBMaterialFixer : AssetPostprocessor
{
    void OnPostprocessModel(GameObject go)
    {
        // GLB 또는 GLTF만 처리
        if (!assetPath.EndsWith(".glb") && !assetPath.EndsWith(".gltf"))
            return;

        // 모든 MeshRenderer에 안전한 머티리얼 적용
        var renderers = go.GetComponentsInChildren<MeshRenderer>();
        foreach (var r in renderers)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            // ★ 반투명 방지: Opaque 모드 강제 적용
            mat.SetFloat("_Surface", 0);  // 0 = Opaque
            mat.SetFloat("_AlphaClip", 0);
            mat.renderQueue = 2000;

            r.sharedMaterial = mat;
        }
    }
}
