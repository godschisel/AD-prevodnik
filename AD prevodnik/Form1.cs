using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Timers;
using NationalInstruments;
using NationalInstruments.DAQmx;
using System.Threading;

namespace AD_prevodnik
{
    public partial class Form1 : Form
    {

        //DAQmx premenne

        public static Task UlohaCounter;
        private CIChannel CICh;
        private CounterReader Counter;
        private int[] ttlSignal;


        private void button3_Click(object sender, EventArgs e)   // Digital Read
        {

            DigitalSingleChannelReader myDigitalReader;
            Task digitalInTask = new Task();

            DIChannel myDIChannel;

            myDIChannel = digitalInTask.DIChannels.CreateChannel(
                "Dev2/port0/line0:7",    //what????  potrebujem pre PFI 0
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

        private void button4_Click(object sender, EventArgs e) // NOPE!!!! 
        {
            /*
            UlohaCounter = new Task("Counter");
            CICh = UlohaCounter.CIChannels.CreateCountEdgesChannel("dev1/ctr0", "Counter", CICountEdgesActiveEdge.Rising, 0, CICountEdgesCountDirection.Up);
            UlohaCounter.Control(TaskAction.Verify);

            /*
            krokDyn = Convert.ToDouble(krokDynParamMaskedTextBox.Text);
                krokCas = Convert.ToDouble(casKrokMaskedTextBox.Text);
                aktualnyKrok = 0;
                aktualneTlaky = 0;
                currentDataIndex = 0;


            Counter = new CounterReader(UlohaCounter.Stream);

             if (scanSpectrumRadioButton.Checked)
                    {
                        Pisac.WriteSingleSample(true, Convert.ToDouble(statickyParamMaskedTextBox.Text) / 10);
                        Thread.Sleep(500);
                    }

                    UlohaCounter.Start();
                                        
                    Casovac.Enabled = true;
            */
        }


        public Form1()
        {
            InitializeComponent();
            timer = new System.Timers.Timer(500);
            timer.Elapsed += Timer_Elapsed;
        }

       

        private void button1_Click(object sender, EventArgs e)  //Read Data
        {
            textBox1.Text = analogReadData();
        }

        private void button2_Click(object sender, EventArgs e)        // Write Data
        {
            analogWriteData(textBox2.Text);
          //  button3_Click(sender, e);
        }

        string analogReadData()
        {
            Task analogInTask = new Task();
            AIChannel myAIChannel;


            myAIChannel = analogInTask.AIChannels.CreateVoltageChannel(
                "dev2/ai1",
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
                "dev2/ao1",
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


        System.Timers.Timer timer;
        int aktualnyKrok;
        private int last;

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (aktualnyKrok < 20)
                {
                    int hodnota = Counter.ReadSingleSampleInt32();
                    ttlSignal[aktualnyKrok++] = hodnota;
                    MessageBox.Show(UlohaCounter.CIChannels.ToString());
                    //UlohaCounter.Stop(); 
                    //UlohaCounter.Start();

                }
                else
                {
                    Casovac.Enabled = false;
                    string s = "";
                    foreach (int i in ttlSignal)
                    {
                        s += i.ToString() + "\n";
                    }
                    MessageBox.Show(s);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //UlohaCounter.Dispose();

            }
        }


        private void Casovac_Tick(object sender, EventArgs e)
        {

            // couDataStorage[aktualnyKrok] = Counter.ReadSingleSampleUInt32();
            try
            {
                if (aktualnyKrok < 20)
                {
                    int hodnota = Counter.ReadSingleSampleInt32();
                    
                    ttlSignal[aktualnyKrok++] = hodnota- last;
                    last = hodnota;
                    // MessageBox.Show(UlohaCounter.CIChannels.ToString());
                    //UlohaCounter.Stop(); 
                    //UlohaCounter.Start();

                }
                else {
                    Casovac.Enabled = false;
                    string s = "";
                    foreach (int i in ttlSignal) {
                        s += i.ToString() + "\n";
                    }
                    MessageBox.Show(s);
                 //   last = ttlSignal[ttlSignal.Length - 1];
                }


            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                //UlohaCounter.Dispose();
                             
            }
            // navrat do povodneho stavu             
            //    Casovac.Enabled = false;               
            //      UlohaCounter.Dispose();     



        //    textBox4.Text = ttlSignal[aktualnyKrok].ToString();//Counter.ReadSingleSampleInt32().ToString();

        }

        private void button5_Click(object sender, EventArgs e) //Trigger start
        {
            last = 0;
            Task UlohaCounter = new Task("Counter");
            ttlSignal = new int[20];
       
            CICh = UlohaCounter.CIChannels.CreateCountEdgesChannel(
                "Dev2/ctr0",
                "Dev2ctr0",
                CICountEdgesActiveEdge.Falling,
                0, 
                CICountEdgesCountDirection.Up
            );
            UlohaCounter.Control(TaskAction.Verify);
            MessageBox.Show(UlohaCounter.CIChannels.ToString());
            Counter = new CounterReader(UlohaCounter.Stream);
            UlohaCounter.Start();
         
            aktualnyKrok = 0;
             Casovac.Enabled = true;
            //timer.Enabled = true;
//            textBox4.Text = Convert.ToString(n.ReadMultiSampleDouble(2)    );








           // DigitalSingleChannelReader citaj = new DigitalSingleChannelReader(UlohaCounter.Stream);
        //    textBox4.Text = Convert.ToString( citaj.ReadSingleSampleSingleLine()  );


            //------------------


            /*

           Task psik = DaqSystem.Local.CreateWatchdogTimerTask(
                "Timer", 
                "Dev1",
                4,
                new string[] { },
                new WatchdogDOExpirationState[] { WatchdogDOExpirationState.NoChange}
            );

            psik.Watchdog.Timeout = 100;
   
            psik.CounterOutput += Psik_CounterOutput;
//            psik.Triggers.StartTrigger.DigitalEdge.Edge=

            UlohaCounter.Control(TaskAction.Verify);

            Counter = new CounterReader(UlohaCounter.Stream);

            


            UlohaCounter.Start();

            UlohaCounter.Stop();
            UlohaCounter.Dispose();
            */
        }

        private void Psik_CounterOutput(object sender, CounterOutputEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void button6_Click(object sender, EventArgs e) // Trigger close
        {
            UlohaCounter.Stop();
            UlohaCounter.Dispose();

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

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
                "dev2/ai1",
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
                "dev2/ao1",
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