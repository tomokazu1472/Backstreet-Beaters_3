using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}

//アイテムの種類を強さ順に並び替えたもの
public enum ItemKind
{
    NormalColor,
    LineH,
    LineV,
    Area,
    ColorBomb,
    None
}

public class DeleteCtrl : MonoBehaviour
{
    [SerializeField] BallField _ballField;
    [SerializeField] ItemDeleteCtrl _itemDeleteCtrl;
    [SerializeField] MatchCheck _matchCheck;
    [SerializeField] MatchShape _matchShape;
    [SerializeField] ItemSpawn _itemSpawn;
    [SerializeField] ComboCountCtrl _comboCountCtrl;
    [SerializeField] DeleteCounter _deleteCounter;
    [SerializeField] AudioClip _popEffectSound;
    public bool isAnimating;

    public void DeleteBall(Ball deleteBall)
    {
        if (deleteBall != null)
        {
            _ballField.SetPositonToNull(deleteBall.position);
            deleteBall.DestroyBall();
        }
    }

    public void DeleteBallAnimated(Ball deleteBall)
    {
        if (deleteBall != null)
        {
            deleteBall.DeleteAnimation();
        }
    }

    public void DeleteBallAnimatedVisually(Ball deleteBall)
    {
        if (deleteBall != null)
        {
            deleteBall.DeleteAnimation();
        }
    }

    public void DeleteAllBalls()
    {
        // フィールド内のすべてのBallを削除
        for (int x = 0; x < _ballField.width; x++)
        {
            for (int y = 0; y < _ballField.height; y++)
            {
                if (_ballField.field[x, y] != null)
                {
                    Destroy(_ballField.field[x, y].gameObject); // Ballオブジェクトを削除
                    _ballField.field[x, y] = null; // 配列の参照を切る
                }
            }
        }
    }

    public bool IsInside(Vector2Int p)
    {
        // Check if the position is within the bounds of the ball field
        return p.x >= 0 && p.x < _ballField.width && p.y >= 0 && p.y < _ballField.height;
    }

    public static bool IsItem(ItemId itemId)
    {
        // Check if the itemId corresponds to an item
        return itemId >= ItemId.CanSide;
    }

    //初回だけの処理
    //最初にマッチ可能なものを消す
    public bool DeleteMatchStart()
    {
        List<Ball> matchedBalls = _matchCheck.GetMatchedBalls();
        
        // 正しくは "matchedBalls が null でなく、かつ Count が 0 より大きい" をチェックする
        if (matchedBalls != null && matchedBalls.Count > 0)
        {
            foreach (Ball ball in matchedBalls)
            {
                _ballField.SetPositonToNull(ball.position);
                ball.DestroyBall();
            }
            return true;
        }
        // マッチできるものがなかった場合
        return false;
    }


    /// <summary>
    ///マッチするボールの塊１つを削除する。
    ///アイテムがマッチした場合はアイテムを生成する
    /// </summary>
    public bool DeleteMatchGroup()
    {

        List<Ball> matchedBalls = _matchCheck.GetMatchedBalls();
        
        for (int i = 0; i < _ballField.width; i++)
        {
            for (int j = 0; j < _ballField.height; j++)
            {
                Ball ball = _ballField.field[i, j];
                if (ball != null && matchedBalls.Contains(ball))
                {
                    if (TryDeleteMatchItemGroup(ball))
                    {
                        // アイテムがマッチした場合
                        //Debug.Log("Item matched");
                        SoundManager.Instance.PlaySFX(_popEffectSound);
                        _comboCountCtrl.CountUpCombo(ball);

                        _deleteCounter.AddBallDelete(matchedBalls, ScoreBonus.Item);
                        return true; // 一つのマッチで処理を終える
                    }
                }
            }
        }

        // アイテム化できないボールのマッチを削除
        for (int i = 0; i < _ballField.width; i++)
        {
            for (int j = 0; j < _ballField.height; j++)
            {
                Ball ball = _ballField.field[i, j];
                if (ball != null && matchedBalls.Contains(ball))
                {
                    if (TryDeleteMatchBallGroup(ball))
                    {
                        // 3つの塊がマッチした場合
                        //Debug.Log("ball matched");
                        SoundManager.Instance.PlaySFX(_popEffectSound);
                        _comboCountCtrl.CountUpCombo(ball);

                        _deleteCounter.AddBallDelete(matchedBalls, ScoreBonus.Normal);
                        return true; // 一つのマッチで処理を終える
                    }
                }
            }
        }
        // マッチできるものがなかった場合
        return false;
    }

    /// <summary>
    /// ボールのマッチを削除する。
    /// </summary>
    private bool TryDeleteMatchBallGroup(Ball ball)
    {
        List<Ball> matchGroup = _matchShape.GetItemMatchGroup(ball, out ItemId itemId);

        if (matchGroup == null || matchGroup.Count == 0) return false;

        if (matchGroup.Count == 3)
        {
            foreach (Ball deleteBall in matchGroup)
            {
                //ボールは削除する
                if (deleteBall != null && IsItem(deleteBall.spriteId))
                {
                    // アイテムは発動キューへ回す
                    deleteBall.SetActive();
                }
                else
                {
                    DeleteBallAnimated(deleteBall);
                }
            }
            return true; // 一つのマッチで処理を終える
        }
        return false; // マッチボールではなかった場合
    }

    /// <summary>
    /// アイテムのマッチを削除する。
    /// 引数のBallがアイテム生成の中心の場合はアイテムを生成する。
    /// 同時に同じ塊のボールをアイテム中心に集めて消す
    /// </summary>
    private bool TryDeleteMatchItemGroup(Ball center)
    {
        List<Ball> matchGroup = _matchShape.GetItemMatchGroup(center, out ItemId newItemId);
        if (matchGroup == null || matchGroup.Count == 0) return false;
        if (!IsItem(newItemId)) return false;

        StartCoroutine(AnimateItemCreation(center, matchGroup, newItemId));
        return true;
    }

    private IEnumerator AnimateItemCreation(Ball center, List<Ball> matchGroup, ItemId newItemId)
    {
        isAnimating = true;

        var others = matchGroup.Where(b => b != null && b != center).ToList();

        // 先に盤面から取り除く
        foreach (var b in others)
            _ballField.SetPositonToNull(b.position);

        // 吸い込み演出＋削除
        foreach (var b in others)
        {
            if (IsItem(b.spriteId))
            {
                if (b.spriteId == ItemId.MirrorBall)
                {
                    Ball random = _ballField.GetRandomNormalBall();
                    (b as MirrorBall)?.SetDeleteColorBall(random);
                }
                b.SetActive();
            }

            b.MovetoPositionVisually(center.position, 0.3f, DG.Tweening.Ease.InQuad,
                onComplete: () => DeleteBall(b));
        }

        yield return new WaitForSeconds(0.4f);

        // 中心にアイテム生成
        _itemSpawn.SpawnItem(newItemId, center.position);



        isAnimating = false;
    }

}
