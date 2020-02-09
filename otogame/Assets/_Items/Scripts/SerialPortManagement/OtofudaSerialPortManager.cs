using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using SerialPortUtility;
using UnityEngine;

public class OtofudaSerialPortManager : SingletonMonoBehaviour<OtofudaSerialPortManager>
{
    private OtofudaSerialDataStructure otofudaDataMaker = new OtofudaSerialDataStructure();
    private SerialPort serialStream = new SerialPort();
    [SerializeField] private SerialPortSetting _serialPortSetting;
    
    public SerialPortReader serialPortReader;
    public SerialPortWriter serialPortWriter;

    private StringBuilder _builder = new StringBuilder();
    
    private void Start()
    {
        serialStream = new SerialPort(_serialPortSetting.targetPortName, (int) _serialPortSetting.baudRate);
        
        serialStream.Open();
        serialStream.DiscardInBuffer();
        serialStream.DiscardOutBuffer();
            
        serialPortReader = new SerialPortReader(serialStream);
        serialPortWriter = new SerialPortWriter(serialStream);
        
        otofudaDataMaker = new OtofudaSerialDataStructure();
        serialPortReader.StartReadStream();
        serialPortReader.OnStreamRead += OnGetData;
    }

    public void OnGetData(SerialPort serialPort, byte readData)
    {
        char readChar = (char) readData;
//        Debug.Log(readChar);
        if (readChar == '\n')
        {
            Debug.Log(_builder.ToString());
            _builder.Clear();
        }
        else
        {
            _builder.Append(readChar);
        }
    }

    private void OnEnable()
    {
        DontDestroyOnLoad(this);
    }

    private void OnDestroy()
    {
        serialPortReader.StopReadStream();
        CloseSerialStream();
    }

    public void CloseSerialStream()
    {
        if (serialStream.IsOpen)
        {
            serialStream.Close();
        }
    }

    public void SendFumenColor(int playerId,int[] color)
    {
        if (color.Length < 3)
        {
            Debug.Log("Colorの配列として与えられたデータ数が足りません");
        }
        
        var data = otofudaDataMaker.MakeColorStructure(playerId, color[0], color[1], color[2]);
        serialPortWriter.WriteSerialPort(data);
    }

    public void SendDifficultyColor(int playerId, JsonReadManager.DIFFICULTY difficulty)
    {
        var data = otofudaDataMaker.MakeDifficultyStructure(playerId, difficulty);
        serialPortWriter.WriteSerialPort(data);
    }
    
    public void SendPlayerHp(int[] playersHp)
    {
        if (playersHp.Length < 2)
        {
            Debug.Log("Hpの配列として与えられたデータ数が足りません");
        }
        var data = otofudaDataMaker.MakePlayerHpStructure(playersHp[0], playersHp[1]);
        serialPortWriter.WriteSerialPort(data);
    }

}
