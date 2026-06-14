using System.Collections.Generic;
using UnityEngine;

public class MatchShape : MonoBehaviour
{
    [SerializeField] BallField _ballField;

    public List<Ball> GetItemMatchGroup(Ball ball, out ItemId itemId)
    {
        itemId = default;
        if (ball == null) return null;

        // 片方向の同色本数（中心を含めない）
        int left  = CountSame(ball, Vector2Int.left);
        int right = CountSame(ball, Vector2Int.right);
        int down  = CountSame(ball, Vector2Int.down);
        int up    = CountSame(ball, Vector2Int.up);

        int horizLen = left + 1 + right; // 横の連続（中心含む）
        int vertLen  = down + 1 + up;    // 縦の連続（中心含む）

        bool hasH = horizLen >= 3;
        bool hasV = vertLen  >= 3;

        // 返却用（必要に応じて中心を含める／含めない）
        var result = new List<Ball>(8);

        // ---- 4本直線：中央2マス「横は左側」「縦は上側」だけ中心 ----
        // 横4：left == 1（n/2-1）かつ縦は絡まない
        if (hasH && horizLen == 4 && !hasV && left == (horizLen / 2 - 1))
        {
            itemId = ItemId.CanSide; // 横一列アイテム
            // 中心は含めない（中心はアイテムになる）
            result.AddRange(GetLine(ball, Vector2Int.left));
            result.Add(ball);
            result.AddRange(GetLine(ball, Vector2Int.right));
            return result;
        }

        // 縦4：down == 2（n/2）かつ横は絡まない
        if (hasV && vertLen == 4 && !hasH && down == (vertLen / 2))
        {
            itemId = ItemId.CanUpDown; // 縦一列アイテム
            result.AddRange(GetLine(ball, Vector2Int.up));
            result.Add(ball);
            result.AddRange(GetLine(ball, Vector2Int.down));
            return result;
        }
        // ---- 直線5個以上（色消し） ----
        // 横：奇数長→真ん中(left==right)、偶数長→中央2のうち左側(left==n/2-1)
        if (hasH && horizLen >= 5 && !hasV)
        {
            bool isCenter =
                (horizLen % 2 == 1 && left == right) ||            // 5,7,9...
                (horizLen % 2 == 0 && left == (horizLen / 2 - 1)); // 6,8,10... の左側

            if (isCenter)
            {
                itemId = ItemId.MirrorBall; // 同色全消し
                result.AddRange(GetLine(ball, Vector2Int.left));   // 中心は含めない設計
                result.AddRange(GetLine(ball, Vector2Int.right));
                return result;
            }
        }

        // 縦：奇数長→真ん中(down==up)、偶数長→中央2のうち上側(down==n/2)
        if (hasV && vertLen >= 5 && !hasH)
        {
            bool isCenter =
                (vertLen % 2 == 1 && down == up) ||                // 5,7,9...
                (vertLen % 2 == 0 && down == (vertLen / 2));       // 6,8,10... の上側

            if (isCenter)
            {
                itemId = ItemId.MirrorBall;
                result.AddRange(GetLine(ball, Vector2Int.up));     // 中心は含めない設計
                result.AddRange(GetLine(ball, Vector2Int.down));
                return result;
            }
        }

        // ---- T / L（交差）：横3以上かつ縦3以上 → この ball が交差点で中心 ----
        if (hasH && hasV && horizLen >= 3 && vertLen >= 3)
        {
            itemId = ItemId.Record; // 範囲爆発
            result.AddRange(GetLine(ball, Vector2Int.left));
            result.AddRange(GetLine(ball, Vector2Int.right));
            result.AddRange(GetLine(ball, Vector2Int.up));
            result.AddRange(GetLine(ball, Vector2Int.down));
            result.Add(ball);
            return result;
        }

        // ---- 通常3マッチ（アイテムなし）：中心を含めて返す ----
        if (hasH && horizLen == 3 && !hasV && left == right) // 1,ball,1
        {
            result.AddRange(GetLine(ball, Vector2Int.left));
            result.Add(ball);
            result.AddRange(GetLine(ball, Vector2Int.right));
            return result;
        }
        if (hasV && vertLen == 3 && !hasH && down == up)     // 1,ball,1
        {
            result.AddRange(GetLine(ball, Vector2Int.up));
            result.Add(ball);
            result.AddRange(GetLine(ball, Vector2Int.down));
            return result;
        }
        //Debug.Log("No match");
        // 該当なし
        return null;
    }

    // ————— helpers —————

    // 色IDか（enumの並び：色 < CanSide を前提）
    private bool IsColor(ItemId id) => id < ItemId.CanSide;

    // 色同士の一致判定（アイテムは一致しない）
    private bool SameColor(ItemId a, ItemId b) => IsColor(a) && IsColor(b) && a == b;

    // 同色を1方向に取得（中心を含めない）
    private IEnumerable<Ball> GetLine(Ball start, Vector2Int dir)
    {
        ItemId colorId = start.spriteId;
        Vector2Int p = start.position + dir;

        while (IsInside(p))
        {
            Ball b = _ballField.field[p.x, p.y];
            if (b == null || !SameColor(b.spriteId, colorId)) break;
            yield return b;
            p += dir;
        }
    }

    // 片方向に何本続くか（中心を含めない）
    private int CountSame(Ball start, Vector2Int dir)
    {
        int count = 0;
        ItemId colorId = start.spriteId;
        Vector2Int p = start.position + dir;

        while (IsInside(p))
        {
            Ball b = _ballField.field[p.x, p.y];
            if (b == null || !SameColor(b.spriteId, colorId)) break;
            count++;
            p += dir;
        }
        return count;
    }

    private bool IsInside(Vector2Int p)
        => p.x >= 0 && p.x < _ballField.width && p.y >= 0 && p.y < _ballField.height;
}
