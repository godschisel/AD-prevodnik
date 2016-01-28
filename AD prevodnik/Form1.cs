using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using NationalInstruments;
using NationalInstruments.DAQmx;

namespace AD_prevodnik
{
    public partial class Form1 : Form
    {

        private Task UlohaVstup = null;            //DAQmx premenne
        private Task UlohaVystup = null;
        private Task UlohaCounter = null;
        private AIChannel AICh;
        private AOChannel AOCh;
        private CIChannel CICh;
        private AnalogSingleChannelReader Citac;
        private AnalogSingleChannelWriter Pisac;
        private CounterReader Counter;

        private void button3_Click(object sender, EventArgs e)   // Digital Read
        {



            // DigitalSingleChannelReader myDigitalReader;

            // myTask.DIChannels.CreateChannel();

            DigitalSingleChannelReader myDigitalReader;
            Task digitalInTask = new Task();

            DIChannel myDIChannel;

            myDIChannel = digitalInTask.DIChannels.CreateChannel(
                "Dev1/port0/line0:7",    //what????  potrebujem pre PFI 0
                 "myChannel",
                 ChannelLineGrouping.OneChannelForAllLines
                );

            myDigitalReader = new DigitalSingleChannelReader(digitalInTask.Stream);

            bool[] vysl = myDigitalReader.ReadSingleSampleMultiLine();


            int val = 0;
            for (int i = 0; i < vysl.Length; i++)
            {
                if (vysl[i])
                {   //if bit is true
                    //add decimal value of bit
                    val += 1 << i;
                }
            }

            //display read value in hex
            textBox3.Text = String.Format("0x{0:X}", val);


            //textBox3.Text = Convert.ToString(vysl[1]);
        }

        private void button4_Click(object sender, EventArgs e) // Digital Write
        {

        }


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)  //Read Data
        {
            textBox1.Text = analogReadData();
        }



        private void button2_Click(object sender, EventArgs e)        // Write Data
        {
            analogWriteData(textBox2.Text);
        }



        string analogReadData()
        {
            Task analogInTask = new Task();
            AIChannel myAIChannel;


            myAIChannel = analogInTask.AIChannels.CreateVoltageChannel(
                "dev1/ai1",
                "myAIChannel",
                AITerminalConfiguration.Differential,
                0,
                5,
                AIVoltageUnits.Volts
                );

            AnalogSingleChannelReader reader = new AnalogSingleChannelReader(analogInTask.Stream);

            double analogDataIn = reader.ReadSingleSample();

            return analogDataIn.ToString();
        }




        void analogWriteData(string vstup)
        {
            Task analogOutTask = new Task();

            AOChannel myAOChannel;

            myAOChannel = analogOutTask.AOChannels.CreateVoltageChannel(
                "dev1/ao1",
                "myAOChannel",
                0,
                5,
                AOVoltageUnits.Volts
                );

            AnalogSingleChannelWriter writer = new AnalogSingleChannelWriter(analogOutTask.Stream);

            double analogDataOut = 0 ;

            if (vstup != "")
            {
                try {
                    analogDataOut = Convert.ToDouble(vstup);
                }
                catch (System.FormatException e) {
                    MessageBox.Show("Vstup " + Convert.ToString( vstup) + " nie je v spravnom tvare.");
                }
            }
            else
            {
                textBox2.Text = "0";
                analogDataOut = Convert.ToDouble(textBox2.Text);
            }
            writer.WriteSingleSample(true, analogDataOut);
        }

        
    }
}

























//-----------------------------------------------------------------------------------------------------------------------



/*


namespace AD_prevodnik
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)  //Read Data
        {
            Task analogInTask = new Task();   
            AIChannel myAIChannel;
            

            myAIChannel = analogInTask.AIChannels.CreateVoltageChannel(
                "dev1/ai1",
                "myAIChannel",
                AITerminalConfiguration.Differential,
                0,
                5,
                AIVoltageUnits.Volts
                );

            AnalogSingleChannelReader reader = new AnalogSingleChannelReader(analogInTask.Stream);

            double analogDataIn = reader.ReadSingleSample();

            textBox1.Text = analogDataIn.ToString();
        }

        private void button2_Click(object sender, EventArgs e)        // Write Data
        {
            Task analogOutTask = new Task();

            AOChannel myAOChannel;

            myAOChannel = analogOutTask.AOChannels.CreateVoltageChannel(
                "dev1/ao1",
                "myAOChannel",
                0,
                5,
                AOVoltageUnits.Volts
                );

            AnalogSingleChannelWriter writer = new AnalogSingleChannelWriter(analogOutTask.Stream);

            double analogDataOut;

            if (textBox2.Text != "")
            {
                analogDataOut = Convert.ToDouble(textBox2.Text);
            }
            else {
                textBox2.Text = "0";
                analogDataOut = Convert.ToDouble(textBox2.Text);
            }

            writer.WriteSingleSample(true, analogDataOut);
        }

        
    }
}
*/