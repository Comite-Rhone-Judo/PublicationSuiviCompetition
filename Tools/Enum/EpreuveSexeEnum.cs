using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Tools.Enum
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
            switch(s)
            {
                case "M":
                    _enum = EpreuveSexeEnum.Masculin;
                    break;
                case "F":
                    _enum = EpreuveSexeEnum.Feminine;
                    break;
                case "X":
                    _enum = EpreuveSexeEnum.Mixte;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid value for EpreuveSexe: " + s);
            }
        }

        public EpreuveSexe(bool s)
        {
            _enum = s ? EpreuveSexeEnum.Feminine : EpreuveSexeEnum.Masculin;
        }

        public EpreuveSexeEnum Enum
        {
            get
            {
                return _enum;
            }
            set
            {
                _enum = value;
            }

        }

        public override string ToString()
        {
            switch(_enum)
            {
                case EpreuveSexeEnum.Masculin:
                    return "M";
                case EpreuveSexeEnum.Feminine:
                    return "F";
                case EpreuveSexeEnum.Mixte:
                    return "X";
                default:
                    throw new ArgumentOutOfRangeException("Invalid value for EpreuveSexeEnum: " + _enum);
            }
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
