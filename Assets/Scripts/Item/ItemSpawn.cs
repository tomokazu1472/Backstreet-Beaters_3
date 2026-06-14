using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    [SerializeField] CanSide _canSidePrefab;
    [SerializeField] CanUpdown _canUpdownPrefab;
    [SerializeField] Record _recordPrefab;
    [SerializeField] MirrorBall _mirrorBallPrefab;
    [SerializeField] BallField _ballField;
    [SerializeField] BallSwap _ballSwap;
    [SerializeField] SpawnCtrl _spawnCtrl;
    [SerializeField] ItemDeleteCtrl _itemDeleteCtrl;

    public void SpawnItem(ItemId itemId, Vector2Int pos)
    {
        if (_ballField.field[pos.x, pos.y] != null)
        {
            _ballField.field[pos.x, pos.y].DestroyBall();
        }

        ParticleGenerator.Play(ParticleKey.ItemSpawn, pos);
        
        Ball item;
        switch (itemId)
        {
            case ItemId.CanSide:
                item = Instantiate(_canSidePrefab, transform);
                CanSide canSide = item as CanSide;
                item.SetDefault(pos, _ballSwap, _itemDeleteCtrl);
                canSide.SetSprite(ItemId.CanSide, _spawnCtrl.GetBallSprite(ItemId.CanSide));
                break;
            case ItemId.CanUpDown:
                item = Instantiate(_canUpdownPrefab, transform);
                CanUpdown canUpdown = item as CanUpdown;
                item.SetDefault(pos, _ballSwap, _itemDeleteCtrl);
                canUpdown.SetSprite(ItemId.CanUpDown, _spawnCtrl.GetBallSprite(ItemId.CanUpDown));
                break;
            case ItemId.Record: 
                item = Instantiate(_recordPrefab, transform);
                Record record = item as Record;
                item.SetDefault(pos, _ballSwap, _itemDeleteCtrl);
                record.SetSprite(ItemId.Record, _spawnCtrl.GetBallSprite(ItemId.Record));
                break;
            case ItemId.MirrorBall:
                item = Instantiate(_mirrorBallPrefab, transform);
                MirrorBall mirrorBall = item as MirrorBall;
                item.SetDefault(pos, _ballSwap, _itemDeleteCtrl);
                mirrorBall.SetSprite(ItemId.MirrorBall, _spawnCtrl.GetBallSprite(ItemId.MirrorBall));
                break;
            default:
                Debug.LogWarning($"Unknown item ID: {itemId}");
                return;
        }
        _ballField.SetBallPos(item, pos);

    }

    public List<Ball> GetAllItems()
    {
        List<Ball> items = new List<Ball>();
        for (int i = 0; i < _ballField.width; i++)
        {
            for (int j = 0; j < _ballField.height; j++)
            {
                Ball ball = _ballField.field[i, j];
                if (ball != null && ball.spriteId >= ItemId.CanSide)
                {
                    items.Add(ball);
                }
            }
        }
        return items;
    }
}
