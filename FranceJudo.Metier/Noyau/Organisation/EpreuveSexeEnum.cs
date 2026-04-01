using System;

namespace FranceJudo.Metier.Noyau.Organisation
{
    /// <summary>
    /// Enumération des type de sexe
    /// </summary>
    public enum EpreuveSexeEnum
    {
        Feminine = 1,
        Masculin = 0,
        Mixte = 2
    }

    public struct EpreuveSexe
    {
        private EpreuveSexeEnum _enum;

        public EpreuveSexe(int s)
        {
            _enum = (EpreuveSexeEnum)s;
        }

        public EpreuveSexe(EpreuveSexeEnum s)
        {
            _enum = s;
        }

        public EpreuveSexe(string s)
        {
            _enum = s switch
            {
                "M" => EpreuveSexeEnum.Masculin,
                "F" => EpreuveSexeEnum.Feminine,
                "X" => EpreuveSexeEnum.Mixte,
                _ => throw new ArgumentOutOfRangeException("Invalid value for EpreuveSexe: " + s),
            };
        }

        public EpreuveSexe(bool s)
        {
            _enum = s ? EpreuveSexeEnum.Feminine : EpreuveSexeEnum.Masculin;
        }

        public EpreuveSexeEnum Enum
        {
            readonly get
            {
                return _enum;
            }
            set
            {
                _enum = value;
            }

        }

        public override readonly string ToString()
        {
            return _enum switch
            {
                EpreuveSexeEnum.Masculin => "M",
                EpreuveSexeEnum.Feminine => "F",
                EpreuveSexeEnum.Mixte => "X",
                _ => throw new ArgumentOutOfRangeException("Invalid value for EpreuveSexeEnum: " + _enum),
            };
        }

        public static implicit operator int(EpreuveSexe s)
        {
            return (int)s._enum;
        }

        public static implicit operator bool(EpreuveSexe s)
        {
            return s._enum == EpreuveSexeEnum.Feminine;
        }
    }
}
