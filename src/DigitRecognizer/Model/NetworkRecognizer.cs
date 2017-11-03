using System.Linq;
using SimplePerceptron.Network;
using SimplePerceptron.Trains;

namespace DigitRecognizer.Model
{
    class NetworkRecognizer
    {
        private readonly INetwork _network;

        public NetworkRecognizer(INetwork network)
        {
            _network = network;
        }

        public RecognizedData[] RecognizeSet(TrainSet set)
        {

            var results = _network.Iterate(set.Input).Select(x => new RecognizedData
            {
                Value = 0,
                Percent = x * 100
            }).ToArray();

            for (int i = 0; i < results.Length; ++i)
            {
                results[i].Value = i;
            }
            return results.OrderByDescending(x => x.Percent).ToArray();
        }

        public double TrainSetRecognizeError(TrainSet[] sets)
        {
            int totalCount = sets.Length;
            if (totalCount == 0) return 0;
            int errorCount = 0;
            foreach (var trainSet in sets)
            {
                var networkAnswer = RecognizeSet(trainSet).OrderByDescending(x => x.Percent).First().Value;
                var setAnswer = TrainSetAnswer(trainSet);
                if (networkAnswer != setAnswer)
                    errorCount++;
            }

            return errorCount / (double) totalCount;
        }


        private int TrainSetAnswer(TrainSet set)
        {
            double max = 0;
            int answer = -1;
            for (var i = 0; i < set.Output.Length; i++)
            {
                if (set.Output[i] > max)
                {
                    max = set.Output[i];
                    answer = i;
                }
            }
            return answer;
        }
    }
}
