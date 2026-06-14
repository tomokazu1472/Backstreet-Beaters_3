using System.Collections.Generic;
using UnityEngine;

public class BallField : MonoBehaviour
{
    //ボールの入る配列の縦、横
    [SerializeField, Header("ボールの入る配列縦、横")] int _width;
    [SerializeField] int _height;

    public int width => _width;
    public int height => _height;

    //ボールの入る配列
    public Ball[,] field;

    void Awake()
    {
        ClearField();
    }

    public void ClearField()
    {
        field = new Ball[_width, _height];
    }

    public void SetBallPos(Ball ball, Vector2Int newPos)
    {
        if (ball == null) return;

        Vector2Int old = ball.position;

        // 旧セルを空に（同じ参照のときだけ）
        if (old.x >= 0 && old.x < _width && old.y >= 0 && old.y < _height &&
            field[old.x, old.y] == ball)
        {
            SetPositonToNull(old);
        }

        // 新セルへ
        field[newPos.x, newPos.y] = ball;

        // Ball 側も更新（内部座標＋見た目アニメ）
        ball.MovetoPosition(newPos); // ここで DOTween.DOMove + SetLink する実装にしておく
    }

    public void SetPositonToNull(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < _width && pos.y >= 0 && pos.y < _height)
        {
            field[pos.x, pos.y] = null;
        }
    }

    public Ball GetRandomNormalBall()
    {
        List<Ball> balls = new List<Ball>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Ball b = field[x, y];
                if (b != null && !DeleteCtrl.IsItem(b.spriteId)) // CanSide 以上のアイテムは除外
                {
                    balls.Add(b);
                }
            }
        }

        if (balls.Count > 0)
        {
            return balls[Random.Range(0, balls.Count)];
        }
        return null;
    }

    public Ball GetRandomBall()
    {
        List<Ball> balls = new List<Ball>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Ball b = field[x, y];
                if (b != null)
                {
                    balls.Add(b);
                }
            }
        }

        if (balls.Count > 0)
        {
            return balls[Random.Range(0, balls.Count)];
        }
        return null;
    }

    public bool IsMoving()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Ball b = field[x, y];
                if (b != null && b.isMoving)
                {
                    return true; // 動いているボールがあれば true
                }
            }
        }
        return false; // 全てのボールが静止している
    }

    public bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _width && pos.y >= 0 && pos.y < _height;
    }

    public bool FallBalls()
    {
        bool falled = false;
        for (int x = 0; x < _width; x++)
        {
            int gapsBelow = 0; // ここまでに見つけた「下側の空き数」

            // 下(0)→上(_height-1)へ走査
            for (int y = 0; y < _height; y++)
            {
                Ball b = field[x, y];

                if (b == null)
                {
                    //空きがあればカウント
                    gapsBelow++;
                    continue;
                }
                // ボールがあり、かつ下に空きがあるなら落とす
                if (gapsBelow > 0)
                {
                    Vector2Int newPos = new Vector2Int(x, y - gapsBelow);
                    SetBallPos(b, newPos); // 旧セルnull→新セル代入→見た目更新までここでやる
                    falled = true; // 少なくとも一つは落ちた
                }
            }

            // 最後に、上側の余りセルを明示的にnull（保険）
            for (int y = _height - gapsBelow; y < _height; y++)
            {
                SetPositonToNull(new Vector2Int(x, y));
            }
        }
        return falled; // 何も落ちなかったらfalse
    }

    public void VibeBalls()
    {
        foreach (Ball ball in field)
        {
            if (ball == null) continue;
            if (DeleteCtrl.IsItem(ball.spriteId))
            {
                ball.VibeBall();
            }
        }
    }

    public void ActivateAllItems()
    {
        foreach (Ball ball in field)
        {
            if (ball == null) continue;
            
            if (DeleteCtrl.IsItem(ball.spriteId))
            {
                if (ball.spriteId == ItemId.MirrorBall)
                {
                    Ball random = GetRandomNormalBall();
                    (ball as MirrorBall)?.SetDeleteColorBall(random);
                }
                ball.SetActive();
            }
        }
    }
}
