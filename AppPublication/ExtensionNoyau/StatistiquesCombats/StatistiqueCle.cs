using System;
using FranceJudo.Metier.Noyau.Organisation;

namespace AppPublication.ExtensionNoyau.StatistiquesCombats
{
    public readonly struct StatistiqueCle : IEquatable<StatistiqueCle>
    {
        public TypeEntiteStatistique TypeEntite { get; }
        public string IdEntite { get; }
        public EpreuveSexe Sexe { get; } // Strictement M ou F

        public StatistiqueCle(TypeEntiteStatistique typeEntite, string idEntite, EpreuveSexe sexe)
        {
            TypeEntite = typeEntite;
            IdEntite = idEntite ?? string.Empty;
            Sexe = sexe;
        }

        public bool Equals(StatistiqueCle other) =>
            TypeEntite == other.TypeEntite &&
            Sexe.Equals(other.Sexe) && // Remplacement de '==' par '.Equals()'
            IdEntite == other.IdEntite;

        public override bool Equals(object obj) => obj is StatistiqueCle other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + TypeEntite.GetHashCode();
                hash = hash * 31 + StringComparer.OrdinalIgnoreCase.GetHashCode(IdEntite);
                hash = hash * 31 + Sexe.GetHashCode();
                return hash;
            }
        }

        public override string ToString() => $"{TypeEntite}-{IdEntite}-{Sexe}";
    }
}