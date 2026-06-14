
/*
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UIPieChart : Graphic
{
    [Tooltip("各セクションの値（合計に対する割合として使われます）")]
    public List<float> values = new List<float> { 40, 30, 20, 10 };

    [Tooltip("各セクションの色。足りない分はGraphicのcolorが使われます")]
    public List <Color> sliceColors = new List<Color> 
    { 
        new Color(0.95f, 0.30f, 0.30f),
        new Color(0.30f, 0.70f, 0.95f),
        new Color(0.30f, 0.85f, 0.50f),
        new Color(0.95f, 0.85f, 0.30f)
    };

    [Header("形状")]
    [Tooltip("0 だとRectTransformから半径を自動決定")]
    public float radius = 0f;
    [Tooltip("0で円。>0でドーナツ")]
    public float innerRadius = 0f;
    [Range(3, 512), Tooltip("円周の分割数（見た目の滑らかさ）")]
    public int segmentsPerFullCircle = 128;

    [Header("向き")]
    [Tooltip("開始角度 (度)")]
    public float startAngle = 0f;
    [Tooltip("時計回りならtrue")]
    public bool clockwise = true;

    public void SetData(IList<float> vals, IList<Color> cols = null)
    {
        values = new List<float>(vals);
        if (cols != null) sliceColors = new List<Color>(cols);
        SetVerticesDirty();
        SetMaterialDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (values == null || values.Count == 0) return;

        // 合計
        float sum = 0f;
        for (int i = 0; i < values.Count; i++) if (values[i] > 0) sum += values[i];
        if (sum <= 0f) return;

        // 半径の自動決定
        float outerR = radius > 0f ? radius : Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;
        float innerR = Mathf.Clamp(innerRadius, 0f, outerR - 0.001f);

        // 角度方向
        float dir = clockwise ? -1f : 1f;
        float a = startAngle * Mathf.Deg2Rad;

        int colorCount = sliceColors != null ? sliceColors.Count : 0;
        int fullSeg = Mathf.Max(3, segmentsPerFullCircle);

        for (int s = 0; s < values.Count; s++)
        {
            float v = Mathf.Max(0f, values[s]);
            if (v <= 0f) continue;

            float portion = v / sum;
            float arcRad = Mathf.PI * 2f * portion;
            int segs = Mathf.Max(1, Mathf.CeilToInt(fullSeg * portion));

            Color32 col = (colorCount > 0) ? (Color32)sliceColors[Mathf.Min(s, colorCount - 1)]
                                           : (Color32)color;

            float a0 = a;
            float a1 = a + dir * arcRad;
            float step = (a1 - a0) / segs;

            int baseIndex = vh.currentVertCount;

            // 扇形の内外リングをストリップで追加
            for (int i = 0; i <= segs; i++)
            {
                float ang = a0 + step * i;
                float cos = Mathf.Cos(ang);
                float sin = Mathf.Sin(ang);

                Vector2 pInner = new Vector2(cos, sin) * innerR;
                Vector2 pOuter = new Vector2(cos, sin) * outerR;

                UIVertex vert = UIVertex.simpleVert;
                vert.color = col;

                vert.position = pInner; vh.AddVert(vert);
                vert.position = pOuter; vh.AddVert(vert);
            }

            for (int i = 0; i < segs; i++)
            {
                int i0 = baseIndex + i * 2;
                int i1 = baseIndex + i * 2 + 1;
                int i2 = baseIndex + i * 2 + 2;
                int i3 = baseIndex + i * 2 + 3;

                if (clockwise)
                {
                    vh.AddTriangle(i0, i1, i3);
                    vh.AddTriangle(i0, i3, i2);
                }
                else
                {
                    vh.AddTriangle(i0, i3, i1);
                    vh.AddTriangle(i0, i2, i3);
                }
            }

            a = a1; // 次のセクション開始角
        }
    }

    // インスペクタ変更時も再描画
    protected override void OnValidate()
    {
        base.OnValidate();
        SetVerticesDirty();
    }
}
*/