using UnityEngine;

namespace RedVeil.World
{
    /// <summary>
    /// 에디터에서 빌보드 위치 파악을 돕는 보조 컴포넌트.
    /// 런타임에는 셰이더가 빌보드 회전을 처리하므로 이 스크립트는 동작하지 않는다.
    /// 에디터의 Scene 뷰에서만 카메라를 향하도록 회전한다.
    /// </summary>
    [ExecuteInEditMode]
    public class BillboardEditorPreview : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying) return;

            UnityEditor.SceneView sceneView = UnityEditor.SceneView.lastActiveSceneView;
            if (sceneView == null || sceneView.camera == null) return;

            Vector3 toCamera = sceneView.camera.transform.position - transform.position;
            toCamera.y = 0f;

            if (toCamera.sqrMagnitude < 0.0001f) return;

            transform.rotation = Quaternion.LookRotation(-toCamera, Vector3.up);
        }
#endif
    }
}