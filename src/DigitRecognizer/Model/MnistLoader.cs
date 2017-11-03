using System;
using System.IO;
using System.Linq;
using System.Windows;
using SimplePerceptron.Trains;

namespace DigitRecognizer.Model
{
    class MnistLoader
    {
        private readonly string _trainsFilePath;
        private readonly string _trainsLabelsFilePath;

        private readonly string _testFilePath;
        private readonly string _testTrainsLabelsFilePath;
        private readonly ITrainSetConstructor _trainConstructor;

        private readonly DigitImage[] _trainImages;
        private readonly DigitImage[] _testImages;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int InputCount => Width * Height;
        public readonly int CategoryCount = 10;

        public MnistLoader(string trainsFilePath, string labelsFilePath, string testFilePath, string testTrainsLabelsFilePath, ITrainSetConstructor trainConstructor)
        {
            _trainsFilePath = trainsFilePath;
            _trainsLabelsFilePath = labelsFilePath;
            _testTrainsLabelsFilePath = testTrainsLabelsFilePath;
            _trainConstructor = trainConstructor;
            _testFilePath = testFilePath;

            _trainImages = ReadData(_trainsFilePath, _trainsLabelsFilePath);
            _testImages = ReadData(_testFilePath, _testTrainsLabelsFilePath);
        }

        public TrainSet[] GetTrainSets()
        {
            return _trainImages.Select(x => _trainConstructor.CreateSet(x.Bytes, x.Label, CategoryCount)).ToArray();
        }

        public TrainSet[] GetTestSets()
        {
            return _testImages.Select(x => _trainConstructor.CreateSet(x.Bytes, x.Label, CategoryCount)).ToArray();
        }

        public DigitImage GetTestImage(int index) => _testImages[index];
        public int TestCount => _testImages.Length;
        public int TrainCount => _trainImages.Length;

        private DigitImage[] ReadData(string imagesPath, string labelsPath)
        {
            var datasetStream = Application.GetResourceStream(new Uri(imagesPath)).Stream;
            var labelsStream = Application.GetResourceStream(new Uri(labelsPath)).Stream;
            BinaryReader brImages = new BinaryReader(datasetStream);
            BinaryReader brLabels = new BinaryReader(labelsStream);

            int magic1 = brImages.ReadInt32(); 
            magic1 = ReverseBytes(magic1);
            int imageCount = brImages.ReadInt32();
            imageCount = ReverseBytes(imageCount);
            int numRows = brImages.ReadInt32();
            numRows = ReverseBytes(numRows);
            int numCols = brImages.ReadInt32();
            numCols = ReverseBytes(numCols);
            int magic2 = brLabels.ReadInt32();
            magic2 = ReverseBytes(magic2);
            int numLabels = brLabels.ReadInt32();
            numLabels = ReverseBytes(numLabels);

            Width = numCols;
            Height = numRows;
            
            byte[] image = new byte[imageCount];
            var digitImages = new DigitImage[imageCount];
            
            for (int imageNum = 0; imageNum < imageCount; ++imageNum)
            {
                image = brImages.ReadBytes(numCols * numRows);
                var label = brLabels.ReadByte();
                digitImages[imageNum] = new DigitImage(numCols, numRows, image, label);
            }
            datasetStream.Close();
            brImages.Close();
            labelsStream.Close();
            brLabels.Close();

            return digitImages;
        }

        private int ReverseBytes(int v)
        {
            byte[] intAsBytes = BitConverter.GetBytes(v);
            Array.Reverse(intAsBytes);
            return BitConverter.ToInt32(intAsBytes, 0);
        }
    }
}
