using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ball : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _ballSprite, _selectSprite;
    [SerializeField] private BallColor _ballColor;
    [SerializeField] private float _vibeSize;
    private Color _color;
    private BallSwap _ballSwap;
    private Vector2Int _position; // 現在の位置
    public Vector2Int position => _position;
    private Vector2Int _Previousposition; // 以前の位置
    private ItemId _spriteId; // ボールのテクスチャー番号
    public ItemId spriteId => _spriteId;
    private bool _isMoving;
    public bool isMoving => _isMoving;
    private float _animatonTime;
    protected bool _isActivated = false;
    public bool isActivated => _isActivated;
    private Sequence _sequence;
    private Camera _mainCamera;

    // 初期化もろもろ
    public virtual void SetDefault(Vector2Int pos, BallSwap ballswap, ItemDeleteCtrl _itemDeleteCtrl = null)
    {
        _mainCamera = Camera.main; // カメラをキャッシュ
        _ballSwap = ballswap;
        _position = pos;
        _Previousposition = pos;
        transform.localPosition = (Vector2)pos;
        _ballSprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        _ballSprite.sprite = null;
        _animatonTime = _ballColor.animationTime * BeatCtrl.GetOneBeatTime();
    }

    public void SetSprite(ItemId id, Sprite sprite)
    {
        _spriteId = id;
        _color = _ballColor.GetBallColor(id);
        _ballSprite.sprite = sprite;
        _selectSprite.color = _color;
        _selectSprite.gameObject.SetActive(false);
    }

    public void SetPosition(Vector2Int pos)
    {
        _position = pos;
        transform.localPosition = new Vector2(pos.x, pos.y);
    }

    public virtual void SetActive()
    {
        _isActivated = true;
    }

    public virtual void Activate()
    {
        // アイテムのアクティベーション処理をここに実装
    }

    public virtual void VibeBall()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Join(transform.DOPunchScale(new Vector3(_vibeSize, _vibeSize, 1f), _animatonTime * 0.75f))
                .SetLink(gameObject);
    }

    public void MovetoPosition(Vector2Int pos, Ease easemode = Ease.InOutQuad)
    {
        _position = pos;
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOMove(new Vector2(pos.x, pos.y), _animatonTime).SetEase(easemode))
                .SetLink(gameObject);
    }

    public void MovetoPositionVisually(Vector2Int pos, float duration, Ease easemode = Ease.InQuad, System.Action onComplete = null)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOMove(new Vector2(pos.x, pos.y), duration).SetEase(easemode))
                .AppendCallback(() =>
                {
                    onComplete?.Invoke();
                })
                .SetLink(gameObject);
    }

    public void MoveandGoback(Vector2Int pos)
    {
        _position = pos;
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOMove(new Vector2(pos.x, pos.y), _animatonTime))
                .AppendCallback(() =>
                {
                    MoveBackPos();
                })
                .Append(transform.DOMove(new Vector2(_position.x, _position.y), _animatonTime))
                .SetLink(gameObject);
    }

    public void DeleteAnimation()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOScale(Vector3.zero, _animatonTime).SetEase(Ease.InQuad))
                .OnComplete(() =>
                {
                    DestroyBall();
                })
                .SetLink(gameObject);
        ParticleGenerator.Play(ParticleKey.DeleteBall, transform.position);
    }

    public void DestroyBall()
    {
        _sequence?.Kill();
        DOTween.Kill(transform);
        Destroy(gameObject);
    }

    public void RememberPos()
    {
        _Previousposition = _position;
    }

    public void MoveBackPos()
    {
        _position = _Previousposition;
    }

    void Update()
    {
        // 移動中フラグの更新
        _isMoving = (Vector2)transform.localPosition != new Vector2(_position.x, _position.y);

        // 入力情報の取得（タッチ・マウス共通）
        Vector2 screenPosition = Vector2.zero;
        bool isPressed = false;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            isPressed = Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        }
        else if (Mouse.current != null)
        {
            screenPosition = Mouse.current.position.ReadValue();
            isPressed = Mouse.current.leftButton.wasPressedThisFrame;
        }
        else
        {
            return;
        }

        Vector2 worldPosition = (Vector2)_mainCamera.ScreenToWorldPoint(screenPosition);

        // 接触判定
        if (Vector2.Distance(worldPosition, (Vector2)transform.position) <= 0.5f)
        {
            if (_ballSwap.swapLock) return;

            _selectSprite.gameObject.SetActive(true);

            if (isPressed)
            {
                _ballSwap.SetBallPressed(this);
            }
        }
        else
        {
            if (_isActivated && (GetType() != typeof(Ball))) return;
            _selectSprite.gameObject.SetActive(false);
        }
    }
}