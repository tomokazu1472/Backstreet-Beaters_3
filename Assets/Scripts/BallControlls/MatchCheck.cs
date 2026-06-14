using System.Collections.Generic;
using UnityEngine;

public class MatchCheck : MonoBehaviour
{
    [SerializeField] BallField _ballField;

    /*
    public bool CheckMatch(Ball ball)
    {
        //横に揃ったやつら
        List<Ball> matchedRowBalls = new List<Ball>();
        //縦に揃ったやつら
        List<Ball> matchedColumnBalls = new List<Ball>();
        //いくつ隣に移動したか
        int shiftCount = 0;

        //右に順番にチェック
        while (_ballField.field[ball.position.x + shiftCount, ball.position.y].ballcolor == ball.ballcolor)
        {
            //揃ったやつを横リストに入れる。自分含む
            matchedRowBalls.Add(_ballField.field[ball.position.x + shiftCount, ball.position.y]);

            shiftCount++;

            //次の参照位置が配列外だったらbreak
            if (ball.position.x + shiftCount >= _ballField.width)
            {
                break;
            }
            //次の参照位置が動いているかnullだったらbreak
            if (_ballField.field[ball.position.x + shiftCount, ball.position.y] == null ||
                _ballField.field[ball.position.x + shiftCount, ball.position.y].isMoving)
            {
                break;
            }
        }

        //
        shiftCount = 0;
        //左に順番にチェック
        while (_ballField.field[ball.position.x - shiftCount, ball.position.y].ballcolor == ball.ballcolor)
        {
            //揃ったやつを横リストに入れる。自分含めない
            if (shiftCount != 0)
            {
                matchedRowBalls.Add(_ballField.field[ball.position.x - shiftCount, ball.position.y]);
            }
            shiftCount++;

            //次の参照位置が配列外だったらbreak
            if (ball.position.x - shiftCount >= _ballField.width)
            {
                break;
            }
            //次の参照位置が動いているかnullだったらbreak
            if (_ballField.field[ball.position.x - shiftCount, ball.position.y] == null ||
                _ballField.field[ball.position.x - shiftCount, ball.position.y].isMoving)
            {
                break;
            }
        }

        shiftCount = 0;
        //上に順番にチェック
        while (_ballField.field[ball.position.x, ball.position.y + shiftCount].ballcolor == ball.ballcolor)
        {
            //揃ったやつをリストに入れる。自分含む
            matchedColumnBalls.Add(_ballField.field[ball.position.x, ball.position.y + shiftCount]);

            shiftCount++;

            //次の参照位置が配列外だったらbreak
            if (ball.position.y + shiftCount >= _ballField.width)
            {
                break;
            }
            //次の参照位置が動いているかnullだったらbreak
            if (_ballField.field[ball.position.x, ball.position.y + shiftCount] == null ||
                _ballField.field[ball.position.x, ball.position.y + shiftCount].isMoving)
            {
                break;
            }
        }

        shiftCount = 0;
        //上に順番にチェック
        while (_ballField.field[ball.position.x, ball.position.y - shiftCount].ballcolor == ball.ballcolor)
        {
            //揃ったやつをリストに入れる。自分含めない
            if (shiftCount != 0)
            {
                matchedColumnBalls.Add(_ballField.field[ball.position.x, ball.position.y + shiftCount]);
            }
            shiftCount++;

            //次の参照位置が配列外だったらbreak
            if (ball.position.y - shiftCount >= _ballField.width)
            {
                break;
            }
            //次の参照位置が動いているかnullだったらbreak
            if (_ballField.field[ball.position.x, ball.position.y - shiftCount] == null ||
                _ballField.field[ball.position.x, ball.position.y - shiftCount].isMoving)
            {
                break;
            }
        }

        //揃っているのがあれば
        if (matchedRowBalls.Count + matchedColumnBalls.Count <= 0)
        {
            //削除させる
            _deleteCtrl.CheckDeletable(ball, matchedRowBalls, matchedColumnBalls);
            return true;
        }
        else
        {
            return false;
        }

    }
    */
    public List<Ball> GetMatchedBalls()
    {
        HashSet<Ball> matchedBalls = new HashSet<Ball>();

        // 横方向チェック
        for (int y = 0; y < _ballField.height; y++)
        {
            List<Ball> currentLine = new List<Ball>();
            Ball lastBall = null;

            for (int x = 0; x < _ballField.width; x++)
            {
                Ball current = _ballField.field[x, y];

                if (current == null)
                {
                    currentLine.Clear();
                    lastBall = null;
                    continue;
                }

                if (lastBall != null && current.spriteId == lastBall.spriteId && !current.isMoving)
                {
                    currentLine.Add(current);
                }
                else
                {
                    if (currentLine.Count >= 3)
                    {
                        foreach (var b in currentLine)
                            matchedBalls.Add(b);
                    }
                    currentLine.Clear();
                    currentLine.Add(current);
                }

                lastBall = current;
            }

            if (currentLine.Count >= 3)
            {
                foreach (var b in currentLine)
                    matchedBalls.Add(b);
            }
        }

        // 縦方向チェック
        for (int x = 0; x < _ballField.width; x++)
        {
            List<Ball> currentLine = new List<Ball>();
            Ball lastBall = null;

            for (int y = 0; y < _ballField.height; y++)
            {
                Ball current = _ballField.field[x, y];

                if (current == null)
                {
                    currentLine.Clear();
                    lastBall = null;
                    continue;
                }

                if (lastBall != null && current.spriteId == lastBall.spriteId && !current.isMoving)
                {
                    currentLine.Add(current);
                }
                else
                {
                    if (currentLine.Count >= 3)
                    {
                        foreach (var b in currentLine)
                            matchedBalls.Add(b);
                    }
                    currentLine.Clear();
                    currentLine.Add(current);
                }

                lastBall = current;
            }

            if (currentLine.Count >= 3)
            {
                foreach (var b in currentLine)
                    matchedBalls.Add(b);
            }
        }

        return new List<Ball>(matchedBalls);
    }

}
