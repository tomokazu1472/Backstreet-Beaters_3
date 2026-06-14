using UnityEngine;

public enum ItemId
{
    Red,
    Blue,
    Green,
    Yellow,
    CanSide,
    CanUpDown,
    Record,
    MirrorBall,
}

    public static class ItemIdExtensions
    {
        // 色の範囲… enum の定義順に依存（Red～Yellow が色、それ以降がアイテム）
        public static bool IsColor(this ItemId id) => id < ItemId.CanSide;
        public static bool IsItem(this ItemId id) => id >= ItemId.CanSide;

        // 色同士の比較だけを行いたいとき用
        public static bool SameColorAs(this ItemId a, ItemId b)
            => a.IsColor() && b.IsColor() && a == b;

        // スプライト配列（色用）へのインデックス
        public static int ToColorIndex(this ItemId id)
            => id.IsColor() ? (int)id : -1;

    }
public class SpawnCtrl : MonoBehaviour
{
    [SerializeField] BallField _ballField;
    [SerializeField] Ball _ballprefab;
    [SerializeField] BallSwap _ballSwap;
    [SerializeField] BallWeight _ballWeight;
    [SerializeField] DeleteCtrl _deleteCtrl;
    [SerializeField] Sprite[] _ballSprites; // 色用だけを入れる（Red..Yellow）
    public void SpawnBalls()
    {
        for (int x = 0; x < _ballField.width; x++)
        {
            for (int y = 0; y < _ballField.height; y++)
            {
                if (_ballField.field[x, y] == null)
                {
                    Ball ball = Instantiate(_ballprefab, transform);

                    ItemId colorId = _ballWeight.GetWeightedRandomBallId(); //ランダムな色を持ってくる(出現確率に基づく)
                    Sprite sprite = GetBallSprite(colorId);

                    ball.SetDefault(new Vector2Int(x, y + 2), _ballSwap);
                    ball.SetSprite(colorId, sprite);
                    _ballField.SetBallPos(ball, new Vector2Int(x, y));
                }
            }
        }
    }

    public void SpawnBallsStart()
    {
        // 初回生成後、同時に行う処理（空セルを埋める -> マッチを削除 -> 空セルを埋める）を
        // do/while でまとめる。DeleteMatchStart() が true を返す間は再度空セルを埋める。
        do
        {
            for (int x = 0; x < _ballField.width; x++)
            {
                for (int y = 0; y < _ballField.height; y++)
                {
                    if (_ballField.field[x, y] == null)
                    {
                        Ball ball = Instantiate(_ballprefab, transform);

                        ItemId colorId = BallWeight.GetRandomBallId(); //ランダムな色を持ってくる(出現確率に基づく)
                        Sprite sprite = GetBallSprite(colorId);

                        ball.SetDefault(new Vector2Int(x, y), _ballSwap);
                        ball.SetSprite(colorId, sprite);
                        _ballField.SetBallPos(ball, new Vector2Int(x, y));
                    }
                }
            }
        } while (_deleteCtrl.DeleteMatchStart());
    }

    public Sprite GetBallSprite(ItemId id)
    {
        return _ballSprites[(int)id];
    }
}