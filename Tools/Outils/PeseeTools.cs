using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tools.Outils
{
    public class PeseeTools : INotifyPropertyChanged
    {
        private double _poids = 0.0F;
        private bool? _stable = null;

        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void OnDataReceivedHandler(object sender);
        public event OnDataReceivedHandler OnDataReceived;

        SerialPort _sp = null;
        string result = "";
        
        /// <summary>
        /// Initialise une instance
        /// </summary>
        /// <param name="port_communication">le port COM utilisé</param>

        public PeseeTools(SerialPort port_communication)
        {
            _sp = port_communication;
        }

        /// <summary>
        /// Détermine si le port COM est ouvert
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            try
            {
                if (_sp != null)
                {
                    return _sp.IsOpen;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Commande de lecture des poids
        /// </summary>

        public void ReadPesee()
        {            
            if (_sp != null)
            {
                //LogTools.Trace("InitPesee ReadPesee ", LogTools.Level.DEBUG);

                string command1 = "@" + Environment.NewLine;
                _sp.Write(command1);

                _sp.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                string command2 = "SIR" + Environment.NewLine;
                _sp.Write(command2);

                //LogTools.Trace("InitPesee ReadPesee COMMAND SEND", LogTools.Level.DEBUG);
            }
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (!_sp.IsOpen) return;
                // Obtenir le nombre d'octets en attente dans le tampon du port
                int bytes = _sp.BytesToRead;
                // Créez une zone tampon (tableau d'octets) pour stocker les données entrantes
                byte[] buffer = new byte[bytes];
                // Lire les données du port et de le stocker dans la mémoire tampon
                _sp.Read(buffer, 0, bytes);

                string strReceiveData = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                strReceiveData = strReceiveData.Replace(" ", "_");

                result += strReceiveData;
                if (result.Contains(Environment.NewLine))
                {
                    //LogTools.Log("port_DataReceived : " + result);

                    if (result.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Count() > 1)
                    {


                        if (!result.EndsWith(System.Environment.NewLine))
                        {
                            result = result.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Last();
                            return;
                        }
                        else
                        {
                            result = result.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Last();
                        }
                    }
                    string[] res = result.Trim().Split(new string[] { "_" }, StringSplitOptions.None);

                    _poids = 0.0F;

                    try
                    {
                        if (res.Length >= 3)
                        {
                            double.TryParse(res.ElementAt(res.Length - 2), NumberStyles.Float, CultureInfo.InvariantCulture, out _poids);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogTools.Trace(ex, LogTools.Level.WARN);
                    }


                    if (res.Last() == "kg")
                    {
                        //return poids;
                    }
                    else if (res.Last() == "g")
                    {
                        _poids = _poids / 1000F;
                    }

                    Poids = Math.Truncate(_poids * 10.0) / 10.0;
                    Stable = res.FirstOrDefault() == "S" && res.ElementAt(1) == "S";
                }

                if (OnDataReceived != null)
                {
                    OnDataReceived(this);
                }
            }
            catch (Exception ex)
            {
                LogTools.Trace(ex, LogTools.Level.ERROR);
            }
        }

        /// <summary>
        /// Ferme le port COM
        /// </summary>

        public void Close()
        {
            try
            {
                if (_sp != null)
                {
                    string command1 = "@" + System.Environment.NewLine;
                    _sp.Write(command1);

                    _sp.Close();
                }
            }
            catch (Exception ex)
            {
                LogTools.Trace(ex, LogTools.Level.WARN);
            }
        }

        /// <summary>
        /// Le dernier Poids lus
        /// </summary>

        public double Poids
        {
            get
            {
                return
                    _poids;
            }
            set
            {
                _poids = value;
                OnPropertyChanged("Poids");
            }
        }

        /// <summary>
        /// Détermine si le dernier poids lu est stable
        /// </summary>

        public bool? Stable
        {
            get { return _stable; }
            set
            {
                _stable = value;
                OnPropertyChanged("Stable");
            }
        }

        /// <summary>
        /// Le port COM
        /// </summary>

        public SerialPort Port
        {
            get { return _sp; }
        }


        /// <summary>
        /// GetSerialPort : renvoie un port paramétré comme il faut
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SerialPort GetSerialPort(string name)
        {
            SerialPort port = new SerialPort(name);
            port.Handshake = Handshake.None;
            port.BaudRate = 9600;
            port.Parity = Parity.None;
            port.DataBits = 8;

            port.ReadTimeout = 2000;
            port.WriteTimeout = 1000;

            return port;
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public static class TestPesee
    {
        private static ManualResetEvent _wait = null;

        public static bool Tester(PeseeTools pesee)
        {
            _wait = new ManualResetEvent(false);
            Thread work = new Thread(new ThreadStart(() =>
            {
                pesee.OnDataReceived += Pesee_DataReceived;
            }));
            work.Start();
            Boolean signal = _wait.WaitOne(3000);
            if (!signal)
            {
                work.Abort();
            }
            return signal;
        }

        private static void Pesee_DataReceived(object sender)
        {
            _wait.Set();
            ((PeseeTools)sender).OnDataReceived -= Pesee_DataReceived;
        }
    }
}
