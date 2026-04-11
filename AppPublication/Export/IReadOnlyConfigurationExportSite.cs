namespace AppPublication.Export
{
    /// <summary>
    /// Vue en lecture seule de la configuration d'export. 
    /// Empêche toute modification directe en dehors des mécanismes sécurisés.
    /// </summary>
    public interface IReadOnlyConfigurationExportSite
    {
        bool PublierProchainsCombats { get; }
        bool PublierAffectationTapis { get; }
        bool PublierEngagements { get; }
        bool EngagementsAbsents { get; }
        bool EngagementsTousCombats { get; }
        bool EngagementsScoreGP { get; }
        bool AfficherPositionCombat { get; }
        long DelaiActualisationClientSec { get; }
        int NbProchainsCombats { get; }
        string MsgProchainsCombats { get; }
        string Logo { get; }
        bool PouleEnColonnes { get; }
        bool PouleToujoursEnColonnes { get; }
        int TailleMaxPouleColonnes { get; }
        bool UseIntituleCommun { get; }
        string IntituleCommun { get; }

        // Ajoute ici toute autre propriété publique dont GenerateurSite a besoin en lecture
    }
}