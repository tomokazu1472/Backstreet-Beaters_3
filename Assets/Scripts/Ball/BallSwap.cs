using UnityEngine;
using UnityEngine.InputSystem;

public class BallSwap : MonoBehaviour
{
    [SerializeField] private BallField _ballField;
    [SerializeField] private DeleteCtrl _deleteCtrl;
    [SerializeField] private ItemDeleteCtrl _itemDeleteCtrl;
    private Ball _ball;
    private bool _isSwapping;
    public bool isSwapping => _isSwapping;
    private Vector2 _mouseDownPos; // クリック/タッチされたときの座標
    private Vector2 _mouseUpPos; // 離されたときの座標
    public bool swapLock;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    /// <summary>
    /// ボール側でクリック/タッチされたときに呼び出される。
    /// </summary>
    public void SetBallPressed(Ball clickedball)
    {
        if (!swapLock)
        {
            Vector2 screenPosition = Vector2.zero;
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            }
            else if (Mouse.current != null)
            {
                screenPosition = Mouse.current.position.ReadValue();
            }

            _mouseDownPos = (Vector2)_mainCamera.ScreenToWorldPoint(screenPosition);
            _ball = clickedball;
            _isSwapping = true;
        }
    }

    /// <summary>
    /// 離されたときに座標計算をさせる
    /// </summary>
    private void SetBallReleased()
    {
        Vector2 screenPosition = Vector2.zero;
        if (Touchscreen.current != null)
        {
            // タッチの場合は直前の座標を取得
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            screenPosition = Mouse.current.position.ReadValue();
        }

        _mouseUpPos = (Vector2)_mainCamera.ScreenToWorldPoint(screenPosition);
        _isSwapping = false;

        if (_ball != null)
        {
            // 戻すときのために記憶する
            _ball.RememberPos();

            Vector2 _distance = _mouseUpPos - _mouseDownPos;

            // 右にスワイプされたとき
            if (_distance.x >= 0.1f && Mathf.Abs(_distance.x) > Mathf.Abs(_distance.y))
            {
                OnSwap(new Vector2Int(1, 0));
                return;
            }

            // 左にスワイプされたとき
            if (_distance.x < -0.1f && Mathf.Abs(_distance.x) > Mathf.Abs(_distance.y))
            {
                OnSwap(new Vector2Int(-1, 0));
                return;
            }

            // 上にスワイプされたとき
            if (_distance.y >= 0.1f && Mathf.Abs(_distance.x) < Mathf.Abs(_distance.y))
            {
                OnSwap(new Vector2Int(0, 1));
                return;
            }

            // 下にスワイプされたとき
            if (_distance.y < -0.1f && Mathf.Abs(_distance.x) < Mathf.Abs(_distance.y))
            {
                OnSwap(new Vector2Int(0, -1));
                return;
            }

            // スワイプがなかったとき
            if (DeleteCtrl.IsItem(_ball.spriteId))
            {
                // ミラーボールの時は単押しで発動させない
                if (_ball.spriteId != ItemId.MirrorBall)
                {
                    _ball.SetActive();
                }
                return;
            }
            _ball = null;
        }
    }

    private void OnSwap(Vector2Int swapDirection)
    {
        Vector2Int targetPos = new Vector2Int(_ball.position.x + swapDirection.x, _ball.position.y + swapDirection.y);
        
        if (!_ballField.IsInside(targetPos)) return;
        if (_ballField.field[targetPos.x, targetPos.y] == null) return;

        // 座標の値を持ってくる
        Vector2Int ballpos = _ball.position;

        // 交換するボールを指定
        Ball neighborBall = _ballField.field[targetPos.x, targetPos.y];
        Vector2Int neighborpos = neighborBall.position;

        // 位置を交換する
        ballpos += swapDirection;
        neighborpos -= swapDirection;

        // 移動させる
        _ballField.SetBallPos(_ball, ballpos);
        _ballField.SetBallPos(neighborBall, neighborpos);
        
        // アイテムを発動させる
        if ((DeleteCtrl.IsItem(_ball.spriteId) && DeleteCtrl.IsItem(neighborBall.spriteId))
            || _ball.spriteId == ItemId.MirrorBall || neighborBall.spriteId == ItemId.MirrorBall)
        {
            _itemDeleteCtrl.ActivateItemCombination(_ball, neighborBall);
        }
        else
        {
            _ball.SetActive();
            neighborBall.SetActive();
        }
        _ball = null;
    }

    void Update()
    {
        // マウスまたはタッチが離された判定
        bool isReleased = false;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
        {
            isReleased = true;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isReleased = true;
        }

        if (isReleased)
        {
            SetBallReleased();
        }
    }
}