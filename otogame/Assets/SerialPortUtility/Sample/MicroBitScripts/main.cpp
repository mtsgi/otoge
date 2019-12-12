#include "MicroBit.h"
Serial pc(USBTX, USBRX);
MicroBitDisplay display;

Ticker ticker;
void readSerial();

char signal;
int main()
{
    pc.baud(57600);
    while(1) 
    {
        pc.printf("Test\n");
        wait(0.5);
        while(pc.readable() == 1)
        {
            pc.getc();
            
        }
        if(pc.readable())
        {
           pc.printf("Receive\n");
           //signal=
        }
/*        if(signal=='1')
        {
            display.scroll('A');
        }
        else
        {
            display.scroll('B');
        }*/
        //readSerial();
    }
}