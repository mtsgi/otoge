using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using SerialPortUtility;
using UnityEngine;

public class SerialPortTestManager : SerialPortManager
{
    private StringBuilder builder = new StringBuilder();
    public override void Start()
    {
        base.Start();
        _serialPortReader.StartReadStream();
        _serialPortReader.OnStreamRead += OnGetData;
    }

    public void OnGetData(SerialPort serialPort, byte readData)
    {
        char readChar = (char) readData;
/*
        Debug.Log(readChar);
*/
        if (readChar == '\n')
        {
            Debug.Log(builder.ToString());
            builder.Clear();
        }
        else
        {
            builder.Append(readChar);
        }
    }
    
    private void Update()
    {
        //WriteColor
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            serialStream.Write("1");
        }
        
        //WriteColor
        if (Input.GetKeyUp(KeyCode.A))
        {
            var testColor = new byte[4] {(byte) 255, (byte) 255, (byte) 0, (byte) 0};
            _serialPortWriter.WriteSerialPort(testColor);
        }
        
        //WriteDifficulty
        if (Input.GetKeyUp(KeyCode.B))
        {
            var testDiff = new byte[2] {(byte) 254, (byte) 3};
            _serialPortWriter.WriteSerialPort(testDiff);
        }
    }
}
