using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SerialPortUtility
{
    public class SerialPortReader
    {
        private SynchronizationContext _mainContext;
        public  Action<SerialPort, byte> OnStreamRead;
        private SerialPort _serialStream;
        private bool _isSerialReadRunning;

        public SerialPortReader(SerialPort serialStream)
        {
            _serialStream = serialStream;
            if (!_serialStream.IsOpen)
            {
                Debug.LogError("SerialPortが開いていません");
                return;
            }
            
            _mainContext = SynchronizationContext.Current;
            Debug.Log("Open SerialPort:Read");
        }

        //1byteだけ読む
        private byte ReadByte()
        {
            if (!_serialStream.IsOpen)
            {
                Debug.LogError("SerialPortが開いていません");
            }

            if (_isSerialReadRunning)
            {
                Debug.Log("ReadStreamModeになっています。1byteずつ読みたい場合はStopReadByteTask()を実行してください");
            }
            
            return (byte) _serialStream.ReadByte();
        }

        
        //ReadStreamモードをオンにしてbyteを受信し続けるようにする。
        public void StartReadStream()
        {
            _isSerialReadRunning = true;
            Task.Run(ReadStreamTask);
        }
        
        private void ReadStreamTask()
        {
            while (_serialStream.IsOpen && _serialStream != null && _isSerialReadRunning)
            {
                if (_serialStream.BytesToRead == 0) continue;
                try
                {
                    while (_serialStream.BytesToRead != 0)
                    {
                        var readData = (byte) _serialStream.ReadByte();
                        _mainContext.Post(_ =>
                            {
                                //1byte受け取ったらアクションを発火する
                                OnStreamRead?.Invoke(_serialStream, readData);
                            }
                            , null);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }
        }
        
        public void StopReadStream()
        {
            _isSerialReadRunning = false;
        }
    }
}
