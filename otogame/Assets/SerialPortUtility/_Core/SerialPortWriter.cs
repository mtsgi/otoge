using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace SerialPortUtility
{
    public class SerialPortWriter
    {
        private SerialPort _serialStream;
        private bool _isSerialWriteRunning;

        private readonly object _lockObject　= new object();
        private readonly SynchronizationContext _mainContext;

        public SerialPortWriter(SerialPort serialStream)
        {
            _serialStream = serialStream;
            if (!_serialStream.IsOpen)
            {
                Debug.LogError("SerialPortが開いていません");
                return;
            }
            _mainContext = SynchronizationContext.Current;
            Debug.Log("Open SerialPort:Write");
        }

        
        public void WriteSerialPort(byte[] data)
        {
            var task = Task.Run(() => { WriteTask(data, 0, data.Length); });
        }
        
        public void WriteSerialPort(byte[] data, int offset, int count)
        {
            var task = Task.Run(() => { WriteTask(data, offset, count); });
        }

        //非同期でWriteする
        private void WriteTask(byte[] data, int offset, int count)
        {
            if (count - offset < 0)
            {
                Debug.LogError("byteの始点が終点より小さい値になっています");
                return;
            }
            
            //lockいらない・・・？
            lock (_lockObject)
            {
                _serialStream.Write(data, offset, count);
            }
        }
    }
}