namespace DeliverySystem.Domain.Entities;

public abstract class EntityBase
{
    private int _id;

    public int Id
    {
        get => _id;
        protected set => _id = value;
    }
}
