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
        Masculin = 0
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
            _enum = (s == "M") ? EpreuveSexeEnum.Masculin : EpreuveSexeEnum.Feminine;
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
            return (_enum == EpreuveSexeEnum.Masculin) ? "M" : "F";
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
