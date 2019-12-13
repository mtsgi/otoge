using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public enum OtofudaDataStructure : byte
{
    ColorHeader = 220,
    DifficultyHeader = 219,
    HpHeader = 213,
        
    EndByte = 255
}

public class OtofudaSerialDataStructure
{
    private byte[] colorBuffer = new byte[8];
    private byte[] difficultyBuffer = new byte[4];
    private byte[] hpBuffer = new byte[4];
    public byte[] MakeColorStructure(int playerId,int r, int g, int b)
    {
        colorBuffer[0] = (byte) OtofudaDataStructure.ColorHeader;
        
        //player1の時は前半だけ、
        if (playerId == 0)
        {
            colorBuffer[1] = (byte) r;
            colorBuffer[2] = (byte) g;
            colorBuffer[3] = (byte) b;
        }//player2の時は後半だけのバッファを上書き
        else if (playerId == 1)
        {
            colorBuffer[4] = (byte) r;
            colorBuffer[5] = (byte) g;
            colorBuffer[6] = (byte) b;
        }
        
        //debug
/*        var builder = new StringBuilder();
        for (int i = 1; i < 7; i++)
        {
            builder.Append(colorBuffer[i]+"/");
        }

        Debug.Log(builder.ToString());*/
        
        colorBuffer[7] = (byte) OtofudaDataStructure.EndByte;
        return colorBuffer;
    }

    public byte[] MakeDifficultyStructure(int playerId, JsonReadManager.DIFFICULTY difficulty)
    {
        difficultyBuffer[0] = (byte) OtofudaDataStructure.DifficultyHeader;

        if (playerId == 0)
        {
            difficultyBuffer[1] = (byte) (int) difficulty;
        }
        else if (playerId == 1)
        {
            difficultyBuffer[2] = (byte) (int) difficulty;
        }
        difficultyBuffer[3] = (byte) OtofudaDataStructure.EndByte;

        return difficultyBuffer;
    }


    public byte[] MakePlayerHpStructure(int player1Hp, int player2Hp)
    {
        hpBuffer[0] = (byte) OtofudaDataStructure.HpHeader;

        hpBuffer[1] = (byte) player1Hp;
        hpBuffer[2] = (byte) player2Hp;

//        Debug.Log(hpBuffer[1] + "__" + hpBuffer[2]);

        hpBuffer[3] = (byte) OtofudaDataStructure.EndByte;

        return hpBuffer;
    }
}
