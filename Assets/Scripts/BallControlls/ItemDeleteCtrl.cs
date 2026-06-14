using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ItemDeleteCtrl : MonoBehaviour
{
    [SerializeField] BallField _ballField;
    [SerializeField] DeleteCtrl _deleteCtrl;
    [SerializeField] ItemSpawn _itemSpawn;
    [SerializeField] DeleteCounter _deleteCounter;
    [SerializeField] AudioClip _popEffectSound;

    //ミラーボールとアイテムの組み合わせで死ぬほど効果音が鳴っているとき
    //可読性さよならクソ実装
    bool _isManyItemsActivating;

    ItemKind GetKind(ItemId id)
    {
        if (!DeleteCtrl.IsItem(id)) return ItemKind.NormalColor;

        switch (id)
        {
            case ItemId.CanSide: return ItemKind.LineH;
            case ItemId.CanUpDown: return ItemKind.LineV;
            case ItemId.Record: return ItemKind.Area;
            case ItemId.MirrorBall: return ItemKind.ColorBomb;
            default: return ItemKind.None;
        }
    }

    // 左右の順序を「強い方が左」になるように正規化（ColorBomb > Area > Line > Color）
    (Ball A, Ball B, ItemKind KA, ItemKind KB) NormalizePair(Ball a, Ball b)
    {
        ItemKind ka = GetKind(a.spriteId);
        ItemKind kb = GetKind(b.spriteId);

        int Rank(ItemKind k) => k switch
        {
            ItemKind.ColorBomb => 5,
            ItemKind.Area => 4,
            ItemKind.LineH => 3,
            ItemKind.LineV => 3,
            ItemKind.NormalColor => 2,
            _ => 0
        };

        if (Rank(kb) > Rank(ka)) return (b, a, kb, ka); // 強い方を左に
        return (a, b, ka, kb);
    }

    public void ActivateItemCombination(Ball ball, Ball neighborBall)
    {
        if (ball == null || neighborBall == null) return;
        Debug.Log("Activating item combination for " + ball.spriteId + " and " + neighborBall.spriteId);

        (Ball A, Ball B, ItemKind KindA, ItemKind KindB) = NormalizePair(ball, neighborBall);

        switch (KindA, KindB)
        {
            case (ItemKind.ColorBomb, ItemKind.ColorBomb):
                // ★ 盤面全消し
                _deleteCtrl.DeleteBallAnimatedVisually(A);
                _deleteCtrl.DeleteBallAnimatedVisually(B);
                StartDeleteAllAnimation(); // ★ Added
                Debug.Log("Double Color Bomb -> DeleteAll");
                break;

            case (ItemKind.ColorBomb, ItemKind.Area):
            case (ItemKind.ColorBomb, ItemKind.LineH):
            case (ItemKind.ColorBomb, ItemKind.LineV):
                // 色消し（既存）
                _deleteCtrl.DeleteBallAnimatedVisually(A);
                _deleteCtrl.DeleteBallAnimatedVisually(B);
                StartDeleteColorItemAnimation(B.spriteId);
                Debug.Log("Color Bomb activated with " + KindB);
                break;

            case (ItemKind.Area, ItemKind.ColorBomb):
            case (ItemKind.LineH, ItemKind.ColorBomb):
            case (ItemKind.LineV, ItemKind.ColorBomb):
                // 色消し（既存）
                _deleteCtrl.DeleteBallAnimatedVisually(A);
                _deleteCtrl.DeleteBallAnimatedVisually(B);
                StartDeleteColorItemAnimation(A.spriteId);
                Debug.Log("Color Bomb activated with " + KindA);
                break;

            case (ItemKind.ColorBomb, _):
                // 色消し（既存）
                _deleteCtrl.DeleteBallAnimatedVisually(A);
                _deleteCtrl.DeleteBallAnimatedVisually(B);
                StartDeleteColorAnimation(B.spriteId);
                Debug.Log("Color Bomb activated with " + KindB);
                break;

            case (_, ItemKind.ColorBomb):
                // 色消し（既存）
                _deleteCtrl.DeleteBallAnimatedVisually(A);
                _deleteCtrl.DeleteBallAnimatedVisually(B);
                StartDeleteColorAnimation(A.spriteId);
                Debug.Log("Color Bomb activated with " + KindA);
                break;

            // ★ Area × Line：基準列(行)と左右(上下)の3列(3行)消し
            case (ItemKind.Area, ItemKind.LineH):
                _deleteCtrl.DeleteBallAnimatedVisually(A);
                _deleteCtrl.DeleteBallAnimatedVisually(B);
                StartDeleteTripleLines(A.position, vertical: false); // 横3行
                Debug.Log("Area + LineH -> triple horizontal rows");
                break;

            case (ItemKind.Area, ItemKind.LineV):
                _deleteCtrl.DeleteBallAnimatedVisually(A);
                _deleteCtrl.DeleteBallAnimatedVisually(B);
                StartDeleteTripleLines(A.position, vertical: true);  // 縦3列
                Debug.Log("Area + LineV -> triple vertical columns");
                break;

            // ★ Line × Line（向き不問）：十字消し
            case (ItemKind.LineH, ItemKind.LineH):
            case (ItemKind.LineH, ItemKind.LineV):
            case (ItemKind.LineV, ItemKind.LineH):
            case (ItemKind.LineV, ItemKind.LineV):
                _deleteCtrl.DeleteBallAnimatedVisually(A);
                _deleteCtrl.DeleteBallAnimatedVisually(B);
                // 横1行＋縦1列（中心A）
                StartDeleteLineAnimation(A.position, new Vector2Int(1, 0)); // 横
                StartDeleteLineAnimation(A.position, new Vector2Int(0, 1)); // 縦
                Debug.Log("Line + Line -> cross");
                break;

            // ★ Area × Area：7x7
            case (ItemKind.Area, ItemKind.Area):
                _deleteCtrl.DeleteBallAnimatedVisually(A);
                _deleteCtrl.DeleteBallAnimatedVisually(B);
                StartDeleteAreaAnimation(A.position, 7);
                Debug.Log("Area + Area -> 7x7");
                break;

            // 既存：その他は通常の削除
            default:
                _deleteCtrl.DeleteBallAnimatedVisually(A);
                _deleteCtrl.DeleteBallAnimatedVisually(B);
                Debug.Log("Normal item combination");
                break;
        }
    }

    public void StartDeleteLineAnimation(Vector2Int center, Vector2Int dir)
    {
        List<Vector2Int> line = BuildLinePositions(center, dir);
        StartCoroutine(DeleteAnimation(line));
    }

    // 中心 center を基準に n×n の範囲を削除するアニメーションを開始
    public void StartDeleteAreaAnimation(Vector2Int center, int n)
    {
        if (n <= 0) return;
        List<Vector2Int> targets = BuildAreaPositions(center, n);

        if (targets == null || targets.Count == 0) return;

        //スコアを追加する
        _deleteCounter.AddBallDelete(targets, ScoreBonus.Record);

        SoundManager.Instance.PlaySFX(_popEffectSound);
        
        foreach (Vector2Int p in targets)
        {
            Ball ball = _ballField.field[p.x, p.y];

            if (ball == null) continue;

            // アイテムは発動キューへ回す
            if (DeleteCtrl.IsItem(ball.spriteId)) // ←色ボールのみ対象
            {
                //ミラーボールの時は適当なボールを持ってくる
                if (ball.spriteId == ItemId.MirrorBall)
                {
                    Ball RandomBall = _ballField.GetRandomNormalBall();
                    MirrorBall mirrorBall = ball as MirrorBall;
                    mirrorBall.SetDeleteColorBall(RandomBall);
                }
                ball.SetActive();
            }
            else
            {
                _deleteCtrl.DeleteBallAnimated(ball);
            }
        }
        
    }

    public void StartDeleteColorAnimation(ItemId itemId)
    {
        List<Vector2Int> deleteTargets = BuildColorPositions(itemId);

        if (deleteTargets == null || deleteTargets.Count == 0) return;

        StartCoroutine(PlayDeleteAnimationSound(10));
        StartCoroutine(DeleteAnimation(deleteTargets));
    }

    public void StartDeleteColorItemAnimation(ItemId itemId)
    {
        ItemId randomID = _ballField.GetRandomBall().spriteId;

        List<Vector2Int> deleteTargets = BuildColorPositions(randomID);
        if (deleteTargets == null || deleteTargets.Count == 0) return;

        StartCoroutine(PlayDeleteAnimationSound(10));

        if (DeleteCtrl.IsItem(itemId))
        {
            // アイテムの色消しアニメーション
            StartCoroutine(ItemChangeActiveAnimation(deleteTargets, itemId));
            return;
        }

        StartCoroutine(DeleteAnimation(deleteTargets, 0.1f));
    }

    // ★ 盤面全消し（アニメ＋スコア加算の既存フローを利用）
    public void StartDeleteAllAnimation()
    {
        List<Vector2Int> all = new List<Vector2Int>();
        for (int x = 0; x < _ballField.width; x++)
        {
            for (int y = 0; y < _ballField.height; y++)
            {
                all.Add(new Vector2Int(x, y));
            }
        }
        if (all.Count > 0)
        {
            StartCoroutine(DeleteAnimation(all));
        }
    }

    private IEnumerator PlayDeleteAnimationSound(int loopcount, float perSoundDelay = 0.03f)
    {
        for (int i = 0; i < loopcount; i++)
        {
            SoundManager.Instance.PlaySFX(_popEffectSound);
            yield return new WaitForSeconds(perSoundDelay);
        }
    }

    // ★ 基準座標を中心に、縦/横の「3本ライン」をまとめて消す
    public void StartDeleteTripleLines(Vector2Int center, bool vertical)
    {
        List<List<Vector2Int>> lines = new();

        if (vertical)
        {
            // center.x の列と x±1 の列（縦スイープは dir=(0,1) で統一）
            for (int dx = -1; dx <= 1; dx++)
            {
                int x = center.x + dx;
                lines.Add(BuildLinePositions(new Vector2Int(x, center.y), new Vector2Int(0, 1)));
            }
        }
        else
        {
            // center.y の行と y±1 の行（横スイープは dir=(1,0) で統一）
            for (int dy = -1; dy <= 1; dy++)
            {
                int y = center.y + dy;
                lines.Add(BuildLinePositions(new Vector2Int(center.x, y), new Vector2Int(1, 0)));
            }
        }

        // 3本を「同じステップで同時に進める」
        StartCoroutine(DeleteAnimationSynchronized(lines));
    }
    private IEnumerator DeleteAnimation(List<Vector2Int> DeleteTargets, float perCellDelay = 0.03f)
    {
        _deleteCtrl.isAnimating = true;

        //消す前にスコア加算
        _deleteCounter.AddBallDelete(DeleteTargets, ScoreBonus.Item);

        // 2) 重複防止のためHashSetで対象を集める
        HashSet<Ball> toDelete = new();

        // 3) 見た目だけ消す
        for (int i = 0; i < DeleteTargets.Count; i++)
        {
            Vector2Int p = DeleteTargets[i];
            if (!_deleteCtrl.IsInside(p)) continue;

            Ball ball = _ballField.field[p.x, p.y];
            if (ball == null || ball.isMoving) continue;

            if (!_isManyItemsActivating)
                SoundManager.Instance.PlaySFX(_popEffectSound);
            
            // アイテムは発動キューへ回す
            if (DeleteCtrl.IsItem(ball.spriteId))
            {
                //ミラーボールの時は適当なボールを持ってくる
                if (ball.spriteId == ItemId.MirrorBall)
                {
                    Ball RandomBall = _ballField.GetRandomNormalBall();
                    MirrorBall mirrorBall = ball as MirrorBall;
                    mirrorBall.SetDeleteColorBall(RandomBall);
                }
                ball.SetActive();
            }
            else
            {
                _deleteCtrl.DeleteBallAnimatedVisually(ball);
                toDelete.Add(ball);
            }
            yield return new WaitForSeconds(perCellDelay);
        }

        // 4) 全演出が終わるまで待つ
        yield return new WaitForSeconds(0.5f); // 少し余裕

        // 5) 実際に配列から削除
        foreach (Ball ball in toDelete)
        {
            _deleteCtrl.DeleteBall(ball);
        }

        //Debug.Log(toDelete.Count + "個が削除");

        _deleteCtrl.isAnimating = false; // アニメーションが終わったらフラグを下ろす
    }

    //複数のボールを1アニメーション時に消すことがある場合
    private IEnumerator DeleteAnimationSynchronized(List<List<Vector2Int>> lines)
    {
        _deleteCtrl.isAnimating = true;
        float perStepDelay = 0.03f;

        // 盤内に限定＆重複除去（各行は順序維持）
        var sanitized = lines
            .Select(line => line.Where(_deleteCtrl.IsInside).ToList())
            .ToList();

        int maxLen = sanitized.Max(l => l.Count);
        HashSet<Ball> toDelete = new();

        for (int step = 0; step < maxLen; step++)
        {
            // このステップで各ラインの「同じインデックス」を同時に処理
            for (int li = 0; li < sanitized.Count; li++)
            {
                if (step >= sanitized[li].Count) continue;

                Vector2Int p = sanitized[li][step];
                Ball ball = _ballField.field[p.x, p.y];
                if (ball == null || ball.isMoving) continue;

                SoundManager.Instance.PlaySFX(_popEffectSound);

                if (DeleteCtrl.IsItem(ball.spriteId))
                {
                    // MirrorBallは既存仕様を踏襲
                    if (ball.spriteId == ItemId.MirrorBall)
                    {
                        Ball random = _ballField.GetRandomNormalBall();
                        (ball as MirrorBall)?.SetDeleteColorBall(random);
                    }
                    ball.SetActive();
                }
                else
                {
                    _deleteCtrl.DeleteBallAnimatedVisually(ball);
                    toDelete.Add(ball);
                }
            }

            yield return new WaitForSeconds(perStepDelay);
        }
        //消す前にスコア加算
        _deleteCounter.AddBallDelete(toDelete.ToList(), ScoreBonus.Item);

        // 余韻
        yield return new WaitForSeconds(0.5f);


        foreach (var b in toDelete) _deleteCtrl.DeleteBall(b);

        _deleteCtrl.isAnimating = false;
    }

    private IEnumerator ItemChangeActiveAnimation(List<Vector2Int> DeleteTargets, ItemId itemId)
    {
        _deleteCtrl.isAnimating = true;
        _isManyItemsActivating = true;
        float perCellDelay = 0.08f;

        //アイテムに変える
        foreach (Vector2Int pos in DeleteTargets)
        {
            SoundManager.Instance.PlaySFX(_popEffectSound);
            _itemSpawn.SpawnItem(itemId, pos);
            yield return new WaitForSeconds(perCellDelay);
        }

        //変えたアイテムを発動
        foreach (Vector2Int pos in DeleteTargets)
        {
            SoundManager.Instance.PlaySFX(_popEffectSound);
            Ball ball = _ballField.field[pos.x, pos.y];
            if (ball == null || ball.isMoving) continue;
            ball.Activate();
            _deleteCtrl.DeleteBallAnimated(ball);
            yield return new WaitForSeconds(perCellDelay);
        }
        _deleteCtrl.isAnimating = false; // アニメーションが終わったらフラグを下ろす
        
        yield return new WaitForSeconds(BeatCtrl.GetOneBeatTime());
        _isManyItemsActivating = false;
    }

    // 掃く順の座標を作る（左右上下に対応）
    private List<Vector2Int> BuildLinePositions(Vector2Int pos, Vector2Int dir)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        if (dir.x > 0)                 // 右へ掃く：左→右
            for (int x = 0; x < _ballField.width; x++) list.Add(new Vector2Int(x, pos.y));
        else if (dir.x < 0)            // 左へ掃く：右→左
            for (int x = _ballField.width - 1; x >= 0; x--) list.Add(new Vector2Int(x, pos.y));
        else if (dir.y > 0)            // 上へ掃く：下→上
            for (int y = 0; y < _ballField.height; y++) list.Add(new Vector2Int(pos.x, y));
        else if (dir.y < 0)            // 下へ掃く：上→下
            for (int y = _ballField.height - 1; y >= 0; y--) list.Add(new Vector2Int(pos.x, y));
        return list;
    }

    // 中心 center から n×n の座標リストを作る
    private List<Vector2Int> BuildAreaPositions(Vector2Int center, int n)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        int half = (n - 1) / 2; // n=3 -> 1, n=4 -> 1（偶数は右/上側に1マス広がる）

        int startX = center.x - half;
        int startY = center.y - half;
        int endX = startX + n - 1;
        int endY = startY + n - 1;

        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                var p = new Vector2Int(x, y);
                if (_deleteCtrl.IsInside(p))
                {
                    list.Add(p);
                }
            }
        }

        // 見た目の演出にばらつきが欲しければシャッフル
        if (list.Count > 0) list.Shuffle();
        return list;
    }
    private List<Vector2Int> BuildColorPositions(ItemId itemId)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        for (int i = 0; i < _ballField.width; i++)
        {
            for (int j = 0; j < _ballField.height; j++)
            {
                Ball ball = _ballField.field[i, j];
                if (ball != null && ball.spriteId == itemId)
                {
                    // Check if the position is inside the bounds of the ball field
                    if (_deleteCtrl.IsInside(ball.position))
                    {
                        list.Add(ball.position);
                    }
                }
            }
        }

        if (list != null) list.Shuffle();
        return list;
    }

}
