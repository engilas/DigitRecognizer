using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplePerceptron.Trains;

namespace DigitRecognizer.Model
{
    class TrainSetConstructor : ITrainSetConstructor
    {
        public TrainSet CreateSet(byte[] imageColors)
        {
            return CreateTrainSet(imageColors, 0, 0);
        }

        public TrainSet CreateSet(byte[] imageColors, byte label, int categoryCount)
        {
            return CreateTrainSet(imageColors, label, categoryCount);
        }

        public TrainSet CreateSetInverted(byte[] imageColors)
        {
            return CreateTrainSet(InverseBytes(imageColors), 0, 0);
        }

        public TrainSet CreateSetInverted(byte[] imageColors, byte label, int categoryCount)
        {
            return CreateTrainSet(InverseBytes(imageColors), label, categoryCount);
        }

        private TrainSet CreateTrainSet(byte[] pixels, byte label, int categoryCount)
        {
            int len = pixels.Length;
            double[] input = new double[len];
            

            for (int i = 0; i < len; ++i)
            {
                input[i] = NormalizeByte(pixels[i]);
            }

            double[] output = null;
            if (categoryCount > 0)
            {
                output = new double[categoryCount];
                output[label] = 1;
            }

            return new TrainSet(input, output);
        }

        private double NormalizeByte(byte b)
        {
            return 1 / (double)255 * b;
        }

        private byte[] InverseBytes(byte[] arr)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = (byte)(255 - arr[i]);
            }
            return arr;
        }
    }
}
