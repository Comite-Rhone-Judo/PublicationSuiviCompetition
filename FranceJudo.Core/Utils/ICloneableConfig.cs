namespace FranceJudo.Core.Utils
{
    /// <summary>
    /// Contrat pour les configurations capables de fournir une copie conforme d'elles-mêmes.
    /// </summary>
    public interface ICloneableObject<T>
    {
        T Clone();
    }
}