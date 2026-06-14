using UnityEngine;

public class MirrorBall : Ball
{
    private ItemDeleteCtrl _itemDeleteCtrl;
    private Ball _ball;
    //初期化もろもろ。
    public override void SetDefault(Vector2Int pos, BallSwap ballswap, ItemDeleteCtrl itemDeleteCtrl = null)
    {
        base.SetDefault(pos, ballswap, itemDeleteCtrl);

        _itemDeleteCtrl = itemDeleteCtrl;
    }

    public override void SetActive()
    {
        base.SetActive();

        if (_isActivated)
        {
             _selectSprite.gameObject.SetActive(true);
            _selectSprite.color = Color.red;
        }
    }

    public void SetDeleteColorBall(Ball ball)
    {
        if (ball != null) _ball = ball;
    }

    public override void Activate()
    {
        base.Activate();

        if (_ball != null) _itemDeleteCtrl.ActivateItemCombination(this, _ball);
        Debug.Log("MirrorBall item activated!");
        _ball = null; // 使用後は参照をクリア
    }
}
