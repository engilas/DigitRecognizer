using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimplePerceptron.Network;
using SimplePerceptron.Parameters;
using SimplePerceptron.Parameters.Activation;
using SimplePerceptron.Serialization;
using SimplePerceptron.Trains;

namespace DigitRecognizer.Model
{
    class NetworkCreator
    {
        public INetwork CreateDefaultNetwork(double learnSpeed, double moment, int inputCount, int outputCount)
        {
            var learnParameters = new LearnParameters
            {
                LearnSpeed = learnSpeed,
                Moment = moment
            };

            var networkParameters = new NetworkParameters
            {
                InputCount = inputCount,
                LearnParameters = learnParameters,
                HiddenLayers = new[]
                {
                    new HiddenLayerParameters
                    {
                        ActivationMethod = ActivationMethod.Sigmoid,
                        HasBias = true,
                        Count = 16
                    },
                    //new HiddenLayerParameters
                    //{
                    //    ActivationMethod = ActivationMethod.Sigmoid,
                    //    //HasBias = true,
                    //    Count = 8
                    //}

                },

                OutputActivationMethod = ActivationMethod.Sigmoid,
                OutputCount = outputCount
            };

            return new Network(networkParameters);
        }
        

        public INetwork DeserializeNetwork(string jsonString)
        {
            var serializedNetwork = JsonConvert.DeserializeObject<SerializedNetwork>(jsonString);
            return DeserializeNetwork(serializedNetwork);
        }

        public INetwork DeserializeNetwork(SerializedNetwork parameters)
        {
            return new Network(parameters.Parameters, parameters.Weights);
        }

        public NetworkTeacher CreateDefaultTeacher(INetwork network, TrainSet[] sets, double minErrorEpsilon = 0.001)
        {
            return new NetworkTeacher(network, sets, new TeacherParameters { MinErrorEpsilon = minErrorEpsilon });
        } 
    }
}
