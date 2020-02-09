using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
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
            var testData = new byte[10];
            
            var testColor1 = new byte[3] {200, 151, 220};
            var testColor2 = new byte[3] {90, 102, 167};
            
            var sum = (short)0;
            for (int i = 0; i < 3; i++)
            {
                sum += (short) (testColor1[i] + testColor2[i]);
            }
            var sumByte = BitConverter.GetBytes(sum);

            testData[0] = (byte) 220;
            Buffer.BlockCopy(sumByte, 0, testData, 1, 2);
            Buffer.BlockCopy(testColor1, 0, testData, 3, 3);
            Buffer.BlockCopy(testColor2, 0, testData, 6, 3);
            
            testData[9] = (byte) 255;

            for (int i = 0; i < testData.Length; i++)
            {
                Debug.Log(testData[i]);
            }
            
            _serialPortWriter.WriteSerialPort(testData);
        }
        
        //WriteDifficulty
        if (Input.GetKeyUp(KeyCode.B))
        {
            var testDiff = new byte[2] {(byte) 254, (byte) 3};
            _serialPortWriter.WriteSerialPort(testDiff);
        }
    }
}
