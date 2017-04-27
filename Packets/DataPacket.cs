using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanglionReader
{
    public class DataPacket
    {

        /// <summary>
        /// Sample index of the packet
        /// </summary>
        public int SampleIndex { get; set; }

        /// <summary>
        /// Voltage brain values
        /// </summary>
        public int[] Values { get; set; }

        /// <summary>
        /// Acceleration values
        /// </summary>
        public int[] AuxValues { get; set; }

        /// <summary>
        /// Raw voltage brain values
        /// </summary>
        public byte[][] RawValues { get; set; }

        /// <summary>
        /// Raw Acceleration values
        /// </summary>
        public byte[][] RawAuxValues { get; set; }

        /// <summary>
        /// StreamWriter as a resource
        /// TODO: Close writer on application exit
        /// </summary>
        public static StreamWriter Writer = null;

        /// <summary>
        /// Writes the packet to a file 
        /// Yet to be tested
        /// </summary>
        public void WriteDataPacket()
        {
            //If the writer is null then initialize writer
            if(Writer == null)
            {
                if (!Directory.Exists(Constants.DATA_DIR)) //Check if DIR exists, create DIR
                    Directory.CreateDirectory(Constants.DATA_DIR);

                FileStream fs = File.Create(Constants.DATA_DIR + DateTime.Now); //Create sample file
                Writer = new StreamWriter(fs);
            }

            if (Writer != null)
            {
                Writer.Write(SampleIndex); //Write sample index
                WriteValues(Values, Constants.SCALE_FAC_UVOLTS_PER_COUNT); //Write voltage scaled voltage values
                WriteValues(AuxValues, Constants.SCALE_FAC_ACCEL_G_PER_COUNT); //Write accleration scaled values
                Writer.WriteLine();
            }
        }

        /// <summary>
        /// Write value with scaling
        /// </summary>
        /// <param name="values"></param>
        /// <param name="scale_fac"></param>
        private void WriteValues(int[] values, float scale_fac)
        {
            int nVal = values.Length;
            for (int Ival = 0; Ival < nVal; Ival++)
            {
                Writer.Write(", ");
                Writer.Write(String.Format("%.2f", scale_fac * values[Ival]));
            }
        }


        //constructor, give it "nValues", which should match the number of values in the
        //data payload in each data packet from the Arduino.  This is likely to be at least
        //the number of EEG channels in the OpenBCI system (ie, 8 channels if a single OpenBCI
        //board) plus whatever auxiliary data the Arduino is sending.
        public DataPacket(int nValues, int nAuxValues)
        {
            Values = new int[nValues];
            AuxValues = new int[nAuxValues];
            RawValues = new byte[nValues][];
            for(int i = 0; i < nValues; i++)
            {
                RawValues[i] = new byte[Constants.RAW_ADS_SIZE];
            }
            RawAuxValues = new byte[nAuxValues][];
            for (int i = 0; i < nAuxValues; i++)
            {
                RawAuxValues[i] = new byte[Constants.RAW_ADS_SIZE];
            }
        }

        /// <summary>
        /// Initialize empty arrays
        /// </summary>
        public void Initialize()
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = 0;
            }
            for (int i = 0; i < AuxValues.Length; i++)
            {
                AuxValues[i] = 0;
            }
        }

        /// <summary>
        /// Copy packet to target
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public int CopyTo(DataPacket target) { return CopyTo(target, 0, 0); }

        /// <summary>
        /// Copy values and aux to target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="target_startInd_values"></param>
        /// <param name="target_startInd_aux"></param>
        /// <returns></returns>
        int CopyTo(DataPacket target, int target_startInd_values, int target_startInd_aux)
        {
            target.SampleIndex = SampleIndex;
            return CopyValuesAndAuxTo(target, target_startInd_values, target_startInd_aux);
        }

        /// <summary>
        /// Copy values and aux to target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="target_startInd_values"></param>
        /// <param name="target_startInd_aux"></param>
        /// <returns></returns>
        int CopyValuesAndAuxTo(DataPacket target, int target_startInd_values, int target_startInd_aux)
        {
            int nvalues = Values.Length;
            for (int i = 0; i < nvalues; i++)
            {
                target.Values[target_startInd_values + i] = Values[i];
                target.RawValues[target_startInd_values + i] = RawValues[i];
            }
            nvalues = AuxValues.Length;
            for (int i = 0; i < nvalues; i++)
            {
                target.AuxValues[target_startInd_aux + i] = AuxValues[i];
                target.RawAuxValues[target_startInd_aux + i] = RawAuxValues[i];
            }
            return 0;
        }
    };
}
