using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

namespace SerialPortUtility
{
    public class SerialPortManager : MonoBehaviour
    {
        [SerializeField] private SerialPortSetting _serialPortSetting = new SerialPortSetting();
        
        public SerialPort serialStream = new SerialPort();
        
        public SerialPortReader _serialPortReader;
        public SerialPortWriter _serialPortWriter;
        
        public virtual void Start()
        {
            serialStream = new SerialPort(_serialPortSetting.targetPortName, (int) _serialPortSetting.baudRate);
            
            serialStream.Open();
            serialStream.DiscardInBuffer();
            serialStream.DiscardOutBuffer();
            
            _serialPortReader = new SerialPortReader(serialStream);
            _serialPortWriter = new SerialPortWriter(serialStream);
        }

        public virtual void StartSerialRead()
        {
            _serialPortReader.StartReadStream();
        }

        public virtual void OnDestroy()
        {
            _serialPortReader.StopReadStream();
            CloseSerialStream();
        }

        private void CloseSerialStream()
        {
            serialStream.Close();
        }
    }
}