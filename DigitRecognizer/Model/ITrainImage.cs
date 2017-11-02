using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SimplePerceptron.Trains;

namespace DigitRecognizer.Model
{
    interface ITrainImage
    {
        ImageSource Image { get; }
        TrainSet GeTrainSet();
    }
}
