using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
public class DynamicCanvasScaler : MonoBehaviour
{
    [Tooltip("基準とする解像度（例: 1920x1080）")]
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    private CanvasScaler canvasScaler;

    void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();

        // UI Scale Modeをスクリーンサイズに依存する設定
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = referenceResolution;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    }

    void Start()
    {
        AdjustScale();
    }

    void Update()
    {
        // ウィンドウリサイズなどで動的にサイズが変わる場合は適宜再計算
        // AdjustScale();
    }

    void AdjustScale()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        float referenceAspect = referenceResolution.x / referenceResolution.y;

        // アスペクト比を比較して動的にmatchWidthOrHeightを決定
        if (Mathf.Approximately(currentAspect, referenceAspect))
        {
            // 基準とほぼ同じアスペクト比なら中間値
            canvasScaler.matchWidthOrHeight = 0.5f;
        }
        else if (currentAspect > referenceAspect)
        {
            // 横に広い => 縦方向に合わせてスケール
            canvasScaler.matchWidthOrHeight = 1.0f;
        }
        else
        {
            // 縦に長い => 横方向に合わせてスケール
            canvasScaler.matchWidthOrHeight = 0.0f;
        }

        // 必要に応じてLayoutGroupの再ビルド
        // LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
}
