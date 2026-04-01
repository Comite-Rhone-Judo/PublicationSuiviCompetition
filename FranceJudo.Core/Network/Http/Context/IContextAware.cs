namespace FranceJudo.Core.Network.Http.Context
{
    /// <summary>
    /// Interface à implémenter par les modules qui ont besoin d'accéder aux données de l'application.
    /// </summary>
    public interface IContextAware
    {
        void SetContext(IContextProvider container);
    }
}
