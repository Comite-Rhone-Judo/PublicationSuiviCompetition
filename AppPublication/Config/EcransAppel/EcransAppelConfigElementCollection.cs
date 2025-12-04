using System.Configuration;
using System.Linq;
using Tools.Configuration;

namespace AppPublication.Config.EcransAppel
{
    /// <summary>
    /// Collection d'éléments de configuration pour les écrans.
    /// </summary>
    [ConfigurationCollection(typeof(EcransAppelConfigElement), AddItemName = "ecran", CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class EcransAppelConfigElementCollection : ConfigCollectionBase<EcransAppelConfigSection, EcransAppelConfigElement>
    {
        /// <summary>
        /// Seule méthode obligatoire à implémenter : définir la clé unique de l'élément.
        /// </summary>
        protected override object GetElementKey(EcransAppelConfigElement element)
        {
            return element.Id;
        }

        public EcransAppelConfigElement GetElementById(int id)
        {
            return (EcransAppelConfigElement) this.Cast<EcransAppelConfigElement>().FirstOrDefault(e => e.Id == id);
        }
    }
}