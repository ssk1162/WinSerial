using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using ClassLibrary1;

namespace WindowsSerial2
{
    public partial class Form1 : Form
    {

        bool flag = false;

        public Form1()
        {
            InitializeComponent();
            comboBox1.DataSource = SerialPort.GetPortNames();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            radioButton2.Checked = false;

            timeCommMon.Tick += TimeCommMon_Tick;

            if (chart1.Legends.Count > 0)
            {
                chart1.Legends.RemoveAt(0);
            }

        }

        int count = 0;
        private void ShowValue(string s)
        {
            int v = Int32.Parse(s);

            if (v < 0 || v > 1023)
            {
                return;
            }
            count++;

            chart1.Series["Series1"].Points.Add(v);

            chart1.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart1.ChartAreas["ChartArea1"].AxisX.Maximum = (count >= 100) ? count : 100;

            if (count > 100)
            {
                chart1.ChartAreas["ChartArea1"].AxisX.ScaleView.Zoom(count - 100, count);
            }
            else
            {
                chart1.ChartAreas["ChartArea1"].AxisX.ScaleView.Zoom(0, 100);
            }

        }

        private void TimeCommMon_Tick(object sender, EventArgs e)
        {

            if (!serialPort1.IsOpen) return;

            setCommLamp(true);

            bool[] bits = new bool[8];

            bool success = TComm.AskDigitalInput(serialPort1, bits);

            if (success)
            {
                checkBox5.Checked = bits[0];
                checkBox6.Checked = bits[1];
                checkBox7.Checked = bits[2];
                checkBox8.Checked = bits[3];
            }

            setCommLamp(true);

            int adval;

            success = TComm.AskADDData(serialPort1, out adval);

            if (success)
            {
                label6.Text = Convert.ToString(adval);
                ShowValue(label6.Text);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            bool success = Class1.OpenPorts(serialPort1, comboBox1.Text);

            if (flag == false)
            {
                flag = true;
                button1.BackColor = Color.Green;
                button1.Text = "열림";
                comboBox1.Enabled = false;

            }
            else
            {
                flag = false;
                button1.BackColor = Color.White;
                button1.Text = "연결";
                comboBox1.Enabled = true;
                Class1.ClosePorts(serialPort1);
                label6.Text = "0";
            }

        }

        private void setDOStatus()
        {
            setCommLamp(true);

            bool[] bits = new bool[8];

            bits[0] = checkBox1.Checked;
            bits[1] = checkBox2.Checked;
            bits[2] = checkBox3.Checked;
            bits[3] = checkBox4.Checked;
            bool success = TComm.SetDigitalOutput(serialPort1, bits);

        }

        private void setAOStatus()
        {
            setCommLamp(true);

            int[] davals = new int[4];

            davals[0] = hScrollBar1.Value;
            davals[1] = hScrollBar2.Value;
            davals[2] = hScrollBar3.Value;
            davals[3] = hScrollBar4.Value;
            bool success = TComm.SetAnalogData(serialPort1, davals);
        }

        private void setLEDStatus()
        {
            if (radioButton1.Checked)
            {
                setDOStatus();
            }
            if (radioButton2.Checked)
            {
                setAOStatus();
            }
        }

        private void setCommLamp(bool isOn)
        {
            pictureBox1.Image = (isOn) ? Properties.Resources.LampOn :
                Properties.Resources.LampOff;
        }

        private void timeLampOff_Tick(object sender, EventArgs e)
        {
            setCommLamp(false);
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            setLEDStatus();
        }

        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            label1.Text = hScrollBar1.Value.ToString();
            label2.Text = hScrollBar2.Value.ToString();
            label3.Text = hScrollBar3.Value.ToString();
            label4.Text = hScrollBar4.Value.ToString();
            setLEDStatus();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            setLEDStatus();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            setLEDStatus();
        }


    }
}
