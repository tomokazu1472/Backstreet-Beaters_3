using System.Collections.Generic;
using UnityEngine;

public class DeleteCounter : MonoBehaviour
{
    [SerializeField] BallField _ballField;
    [SerializeField] GameInfo _gameInfo;

    public void AddBallDelete(List<Ball> balls, ScoreBonus bonusType)
    {
        if (balls == null || balls.Count == 0) return;

        foreach (Ball ball in balls)
        {
            if (ball == null) continue;

            //_deletedBalls.Add(ball);
            _gameInfo.DecreaseMissionAmount(ball.spriteId);
        }
        //Debug.Log(id + " : " + deletedBalls[id]);
        _gameInfo.AddScore(balls.Count, bonusType);
    }
    
    public void AddBallDelete(List<Vector2Int> ballpos, ScoreBonus bonusType)
    {
        int ballCount = 0;
        for (int x = 0; x < _ballField.width; x++)
        {
            for (int y = 0; y < _ballField.height; y++)
            {
                Ball ball = _ballField.field[x, y];
                if (ball == null) continue;

                if (ballpos.Contains(ball.position))
                {
                    _gameInfo.DecreaseMissionAmount(ball.spriteId);
                    ballCount++;
                }
            }
        }
        //Debug.Log(id + " : " + deletedBalls[id]);
        _gameInfo.AddScore(ballCount, bonusType);
    }
}
