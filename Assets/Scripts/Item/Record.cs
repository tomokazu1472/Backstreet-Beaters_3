using UnityEngine;

public class Record : Ball
{
    private ItemDeleteCtrl _itemDeleteCtrl;
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

    public override void Activate()
    {
        base.Activate();

        _itemDeleteCtrl.StartDeleteAreaAnimation(position, 5); // 5x5の範囲を削除
        Debug.Log("Record item activated!");
    }
}
