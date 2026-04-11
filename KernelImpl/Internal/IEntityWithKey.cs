namespace KernelImpl.Internal
{
    internal interface IEntityWithKey<IDType>
    {
        IDType EntityKey { get; }
    }
}
