using System;
using System.IO.Ports;
using ClassLibrary1;

namespace WindowsSerial2
{
    internal class TComm
    {

        public static bool SetDigitalOutput(SerialPort serialPort, bool[] bits)
        {

            // 수신버퍼청소
            string dum = Class1.Read(serialPort);

            // 데이터 만들기
            int bitval = 0;

            if (bits[0]) bitval += 0x1;
            if (bits[1]) bitval += 0x2;
            if (bits[2]) bitval += 0x4;
            if (bits[3]) bitval += 0x8;
            if (bits[4]) bitval += 0x10;
            if (bits[5]) bitval += 0x20;
            if (bits[6]) bitval += 0x40;
            if (bits[7]) bitval += 0x80;

            // 명령송신
            string hexnum = String.Format("{0:X}", bitval);
            if (hexnum.Length == 1) hexnum = "0" + hexnum;
            string st = Class1.sSTX() + "DO" + hexnum + Class1.sETX();
            Class1.Send(serialPort, st);

            // 송신시간기록
            DateTime stime = DateTime.Now;

            // 수신대기
            int idx1, idx2;
            bool success = false;

            string rbuff = "";
            while (true)
            {
                TimeSpan dsec = DateTime.Now - stime;
                // timeout 검사
                double dtime = (double)(dsec.Ticks / 10000000.0);
                if (dtime > 0.5) return false;

                // 수신버퍼검사
                rbuff += Class1.Read(serialPort);

                idx1 = rbuff.IndexOf(Class1.sACK());
                idx2 = rbuff.IndexOf(Class1.sETX());

                if (idx1 >= 0 && idx2 - idx1 == 3)
                {
                    if (rbuff.Substring(idx1 + 1, 2) == "DO")
                    {
                        success = true;
                        break;
                    }
                }

            }

            return success;
        }

        public static bool AskDigitalInput(SerialPort serialPort, bool[] bits)
        {

            // 수신버퍼청소
            string dum = Class1.Read(serialPort);
            string st = Class1.sSTX() + "RI" + Class1.sETX();
            Class1.Send(serialPort, st);

            // 송신시간기록
            DateTime stime = DateTime.Now;

            // 수신대기
            int idx1, idx2, indata;
            bool success = false;

            string rbuff = "";
            while (true)
            {
                TimeSpan dsec = DateTime.Now - stime;
                // timeout 검사
                double dtime = (double)(dsec.Ticks / 10000000.0);
                if (dtime > 0.5) return false;

                // 수신버퍼검사
                rbuff += Class1.Read(serialPort);

                idx1 = rbuff.IndexOf(Class1.sACK());
                idx2 = rbuff.IndexOf(Class1.sETX());

                if (idx1 >= 0 && idx2 - idx1 == 5)
                {
                    if (rbuff.Substring(idx1 + 1, 2) == "RI")
                    {
                        // 한개 찾음
                        string dd = rbuff.Substring(idx1 + 3, 2);
                        indata = Convert.ToInt32(dd, 16); // 16진수 10진수로 변환
                        success = true;
                        break;
                    }
                }

            }

            if (success)
            {
                bits[0] = ((indata & 0x1) > 0) ? true : false;
                bits[1] = ((indata & 0x2) > 0) ? true : false;
                bits[2] = ((indata & 0x4) > 0) ? true : false;
                bits[3] = ((indata & 0x8) > 0) ? true : false;
                bits[4] = ((indata & 0x10) > 0) ? true : false;
                bits[5] = ((indata & 0x20) > 0) ? true : false;
                bits[6] = ((indata & 0x40) > 0) ? true : false;
                bits[7] = ((indata & 0x80) > 0) ? true : false;
            }

            return success;

        }

        public static bool SetAnalogData(SerialPort serialPort, int[] davals)
        {

            // 수신버퍼청소
            string dum = Class1.Read(serialPort);
            string[] hexnum = new string[4];
            for (int i = 0; i < 4; i++)
            {
                hexnum[i] = String.Format("{0:x}", davals[i]);
                if (hexnum[i].Length == 1) hexnum[i] = "0" + hexnum[i];
            }

            // 명령송신
            string st = Class1.sSTX() + "AO" + hexnum[0] + "," + hexnum[1] + "," +
                        hexnum[2] + "," + hexnum[3] + Class1.sETX();
            Class1.Send(serialPort, st);

            // 송신시간기록
            DateTime stime = DateTime.Now;

            // 수신대기
            int idx1, idx2;
            bool success = false;

            string rbuff = "";
            while (true)
            {
                TimeSpan dsec = DateTime.Now - stime;
                // timeout 검사
                double dtime = (double)(dsec.Ticks / 10000000.0);
                if (dtime > 0.5) return false;

                // 수신버퍼검사
                rbuff += Class1.Read(serialPort);

                idx1 = rbuff.IndexOf(Class1.sACK());
                idx2 = rbuff.IndexOf(Class1.sETX());

                if (idx1 >= 0 && idx2 - idx1 == 3)
                {
                    if (rbuff.Substring(idx1 + 1, 2) == "AO")
                    {
                        // 한개 찾음
                        string dd = rbuff.Substring(idx1 + 1, 2);
                        success = true;
                        break;
                    }
                }

            }

            return success;

        }


        public static bool AskADDData(SerialPort serialPort, out int adval)
        {

            adval = 0;

            // 수신버퍼청소
            string dum = Class1.Read(serialPort);

            // 명령송신
            string st = Class1.sSTX() + "RA" + Class1.sETX();
            Class1.Send(serialPort, st);

            // 송신시간기록
            DateTime stime = DateTime.Now;

            // 수신대기
            int idx1, idx2;
            bool success = false;

            string rbuff = "";
            while (true)
            {
                TimeSpan dsec = DateTime.Now - stime;
                // timeout 검사
                double dtime = (double)(dsec.Ticks / 10000000.0);
                if (dtime > 0.5) return false;

                // 수신버퍼검사
                rbuff += Class1.Read(serialPort);

                idx1 = rbuff.IndexOf(Class1.sACK());
                idx2 = rbuff.IndexOf(Class1.sETX());

                if (idx1 >= 0 && idx2 - idx1 == 6)
                {
                    if (rbuff.Substring(idx1 + 1, 2) == "RA")
                    {
                        // 한개 찾음
                        string dds = rbuff.Substring(idx1 + 3, 3);
                        adval = Convert.ToInt32(dds, 16); // 16진수 10진수로 변환
                        success = true;
                        break;
                    }
                }

            }

            return success;
        }

    }
}
