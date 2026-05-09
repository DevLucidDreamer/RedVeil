using UnityEngine;
using UnityEngine.UI;

namespace RedVeil.Core
{
    /// <summary>
    /// RawImage가 RenderTexture를 정수 배율로 업스케일해서 표시하도록 보장.
    /// 화면 크기가 바뀔 때마다 RawImage 크기를 재계산해서 도트 픽셀이 뭉개지지 않게 한다.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class PixelDisplayFitter : MonoBehaviour
    {
        [SerializeField] private RenderTexture sourceTexture;
        [Tooltip("정수 배율만 사용할지 여부. 끄면 가능한 한 크게 채움(레터박스).")]
        [SerializeField] private bool useIntegerScale = true;

        private RawImage _rawImage;
        private RectTransform _rectTransform;
        private Vector2Int _lastScreenSize;

        private void Awake()
        {
            _rawImage = GetComponent<RawImage>();
            _rectTransform = GetComponent<RectTransform>();

            if (sourceTexture != null)
                _rawImage.texture = sourceTexture;
        }

        private void OnEnable() => Fit();

        private void Update()
        {
            Vector2Int current = new Vector2Int(Screen.width, Screen.height);
            if (current != _lastScreenSize)
            {
                _lastScreenSize = current;
                Fit();
            }
        }

        private void Fit()
        {
            if (sourceTexture == null) return;

            float screenW = Screen.width;
            float screenH = Screen.height;
            float texW = sourceTexture.width;
            float texH = sourceTexture.height;

            float scale;
            if (useIntegerScale)
            {
                // 화면에 들어가는 가장 큰 정수 배율
                int scaleX = Mathf.FloorToInt(screenW / texW);
                int scaleY = Mathf.FloorToInt(screenH / texH);
                scale = Mathf.Max(1, Mathf.Min(scaleX, scaleY));
            }
            else
            {
                // 비율 유지하면서 화면 채우기
                scale = Mathf.Min(screenW / texW, screenH / texH);
            }

            // RectTransform을 stretch에서 fixed size로 전환
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _rectTransform.pivot = new Vector2(0.5f, 0.5f);
            _rectTransform.anchoredPosition = Vector2.zero;
            _rectTransform.sizeDelta = new Vector2(texW * scale, texH * scale);
        }
    }
}