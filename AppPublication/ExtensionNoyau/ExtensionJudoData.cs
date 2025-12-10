using KernelImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.ExtensionNoyau
{
    public class ExtensionJudoData
    {
        #region MEMBRES
        // TODO travailler a partir de l'interface
        private JudoData _serverData = null;
        #endregion

        #region CONSTRUCTEURS
        // TODO a voir pour que la donnees soit reprise a chaque fois par rapport au snapshot
        public ExtensionJudoData(JudoData serverData)
        {
            _serverData = serverData;
            _deroulement = new ExtensionNoyau.Deroulement.DataDeroulement();
        }

        #endregion

        // TODO Voir pour renommer cela en engagement

        #region PROPERTIES
        private ExtensionNoyau.Deroulement.DataDeroulement _deroulement = null;
        public ExtensionNoyau.Deroulement.DataDeroulement Deroulement
        {
            get { return _deroulement; }
            set { _deroulement = value; }
        }
        #endregion

        #region METHODES
        public void SyncAll()
        {
            Deroulement.SyncAll(_serverData);
        }

        #endregion
    }
}
