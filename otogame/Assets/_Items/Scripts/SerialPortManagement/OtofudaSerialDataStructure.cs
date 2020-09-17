using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Leap.Unity;
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
    //Header[通知byte(1)][shortデータ総和(2)]
    private byte[] colorBuffer = new byte[1+2+6+1];//(10)
    private byte[] difficultyBuffer = new byte[1+2+2+1];//(6)
    private byte[] hpBuffer = new byte[1+2+2+1];//(6)
    public byte[] MakeColorStructure(int playerId,int r, int g, int b)
    {
        colorBuffer[0] = (byte) OtofudaDataStructure.ColorHeader;
        
        //player1の時は前半だけ、
        if (playerId == 0)
        {
            colorBuffer[3] = (byte) r;
            colorBuffer[4] = (byte) g;
            colorBuffer[5] = (byte) b;
        }//player2の時は後半だけのバッファを上書き
        else if (playerId == 1)
        {
            colorBuffer[6] = (byte) r;
            colorBuffer[7] = (byte) g;
            colorBuffer[8] = (byte) b;
        }

        var sum = (short) 0;
        for (int i = 3; i < 9; i++)
        {
            sum += (short) colorBuffer[i];
        }

        BitConverterNonAlloc.GetBytes(sum, colorBuffer, 1);
        
        //debug
        var builder = new StringBuilder();
        for (int i= 3; i < 9; i++)
        {
            builder.Append(colorBuffer[i]+"/");
        }
//        Debug.Log(builder.ToString());
/*        Debug.Log(BitConverterNonAlloc.ToInt16(colorBuffer, 1));*/
        //
        
        colorBuffer[9] = (byte) OtofudaDataStructure.EndByte;
        return colorBuffer;
    }

    public byte[] MakeDifficultyStructure(int playerId, GameDifficulty gameDifficulty)
    {
        difficultyBuffer[0] = (byte) OtofudaDataStructure.DifficultyHeader;

        if (playerId == 0)
        {
            difficultyBuffer[3] = (byte) (int) gameDifficulty;
        }
        else if (playerId == 1)
        {
            difficultyBuffer[4] = (byte) (int) gameDifficulty;
        }
        
        var sum = (short) 0;
        for (int i = 3; i < 5; i++)
        {
            sum += difficultyBuffer[i];
        }
        
        BitConverterNonAlloc.GetBytes(sum, difficultyBuffer, 1);
        
        difficultyBuffer[5] = (byte) OtofudaDataStructure.EndByte;

        return difficultyBuffer;
    }


    public byte[] MakePlayerHpStructure(int player1Hp, int player2Hp)
    {
        hpBuffer[0] = (byte) OtofudaDataStructure.HpHeader;

        hpBuffer[3] = (byte) player1Hp;
        hpBuffer[4] = (byte) player2Hp;

        
        var sum = (short) 0;
        for (int i = 3; i < 5; i++)
        {
            sum += hpBuffer[i];
        }
        BitConverterNonAlloc.GetBytes(sum, hpBuffer, 1);
        
//        Debug.Log(hpBuffer[1] + "__" + hpBuffer[2]);

        hpBuffer[5] = (byte) OtofudaDataStructure.EndByte;

        return hpBuffer;
    }
}
