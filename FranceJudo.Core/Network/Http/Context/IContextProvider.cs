namespace FranceJudo.Core.Network.Http.Context
{
    /// <summary>
    /// Interface permettant de récupérer un contexte par son type.
    /// </summary>
    public interface IContextProvider
    {
        T GetContext<T>() where T : class;
    }
}
