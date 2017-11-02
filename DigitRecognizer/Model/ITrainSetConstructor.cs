using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplePerceptron.Trains;

namespace DigitRecognizer.Model
{
    public interface ITrainSetConstructor
    {
        TrainSet CreateSet(byte[] data);
        TrainSet CreateSet(byte[] imageColors, byte label, int categoryCount);
        TrainSet CreateSetInverted(byte[] data);
        TrainSet CreateSetInverted(byte[] data, byte label, int categoryCount);
    }
}
