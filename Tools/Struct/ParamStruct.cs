namespace Tools.Struct
{
    public class ParamStruct
    {
        private bool _judotv_actif = false;
        private string _judotv_directory = "";
        private bool _minisite_actif = true;

        private bool _combat_actif = false;

        public bool judotv_actif
        {
            get { return _judotv_actif; }
            set { _judotv_actif = value; }
        }

        public string judotv_directory
        {
            get { return _judotv_directory; }
            set { _judotv_directory = value; }
        }

        public bool minisite_actif
        {
            get { return _minisite_actif; }
            set { _minisite_actif = value; }
        }

        public bool combat_actif
        {
            get { return _combat_actif; }
            set { _combat_actif = value; }
        }
    }
}
