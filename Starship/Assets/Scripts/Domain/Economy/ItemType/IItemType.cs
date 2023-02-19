using Services.Reources;
using UnityEngine;

namespace Economy.ItemType
{
    public enum ItemQuality
    {
        Low5,
        Low4,
        Low3,
        Low2,
        Low1,
        Common,
        Medium,
        High,
        Perfect,
        Epic,
        Legend,
    }

    public interface IItemType
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        Sprite GetIcon(IResourceLocator resourceLocator);
        Color Color { get; }
        Price Price { get; }
        ItemQuality Quality { get; }

        void Consume(int amount);
        void Withdraw(int amount);
        int MaxItemsToConsume { get; }
        int MaxItemsToWithdraw { get; }
    }
}
