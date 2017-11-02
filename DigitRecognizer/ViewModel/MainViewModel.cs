using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DigitRecognizer.Model;
using DigitRecognizer.Properties;
using Microsoft.Win32;
using SimplePerceptron.Network;
using SimplePerceptron.Trains;

namespace DigitRecognizer.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private List<string> _logs = new List<string>(50);
        private INetwork _network;
        private NetworkTeacher _teacher;
        private readonly NetworkCreator _networkCreator;
        private readonly MnistLoader _loader;
        private int _selectedImageIndex;
        private string _recognizeResult;
        private readonly BitmapPainter _painter;
        private readonly ITrainSetConstructor _trainCounstructor;
        private double _learnSpeed;
        private double _moment;

        public double LearnSpeed
        {
            get => _learnSpeed;
            set
            {
                _learnSpeed = value;
                OnPropertyChanged();
            }
        }

        public double Moment
        {
            get => _moment;
            set
            {
                _moment = value;
                OnPropertyChanged();
            }
        }

        public double ReluCoeff { get; set; }

        public int TrainProgressValue => _teacher?.CurrentDataSet ?? 0;

        public string RecognizeResult
        {
            get { return _recognizeResult; }
            set
            {
                if (value == _recognizeResult) return;
                _recognizeResult = value;
                OnPropertyChanged();
            }
        }

        public int TestCount { get; }
        public int TrainSetCount { get; }

        public string ErrorLog => string.Join("\r\n", _logs);

        public int SelectedImageIndex
        {
            get { return _selectedImageIndex; }
            set
            {
                _selectedImageIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Image));
            }
        }

        public ImageSource Image => _loader.GetTestImage(SelectedImageIndex).ToBitmap(1);

        public ImageSource PaintImage => _painter.GetImage();

        public MainViewModel()
        {
            var path = $"pack://application:,,,/{this.GetType().Assembly.GetName().Name};component/DataSets/";
            _trainCounstructor = new TrainSetConstructor();
            _loader = new MnistLoader(
                path + "train-images.idx3-ubyte",
                path + "train-labels.idx1-ubyte",
                path + "t10k-images.idx3-ubyte",
                path + "t10k-labels.idx1-ubyte",
                _trainCounstructor);
            
            TestCount = _loader.TestCount;
            TrainSetCount = _loader.TrainCount;

            _painter = new BitmapPainter(_loader.Width, _loader.Height);
            OnPropertyChanged(nameof(PaintImage));
            _networkCreator = new NetworkCreator();
        }

        public void CreateDefaultNetwork()
        {
            var trainSets = _loader.GetTrainSets();
            var inputCount = trainSets[0].InputCount;
            var outputCount = trainSets[0].OutputCount;

            _network = _networkCreator.CreateDefaultNetwork(LearnSpeed, Moment, inputCount, outputCount);
            _teacher = _networkCreator.CreateDefaultTeacher(_network, trainSets);
            SubscribeTeacherEpochResults();
        }

        public void LoadNetwork()
        {
            string jsonResult = null;
            var dialog = new OpenFileDialog();
            dialog.Filter = "JSON file (*.json)|*.json";
            if (dialog.ShowDialog() == true)
            {
                jsonResult = File.ReadAllText(dialog.FileName);
            }

            var trainSets = _loader.GetTrainSets();
            _network = _networkCreator.DeserializeNetwork(jsonResult);
            _teacher = _networkCreator.CreateDefaultTeacher(_network, trainSets);
            SubscribeTeacherEpochResults();

            LearnSpeed = _network.LearnSpeed;
            Moment = _network.Moment;
        }

        private void SubscribeTeacherEpochResults()
        {
            _teacher.EpochResults += (sender, args) =>
            {
                _logs.Add($"error: {args.Error}\r\ndelta: {args.Delta}\r\nepochCount: {args.EpochCount}\r\n");
                OnPropertyChanged(nameof(ErrorLog));
                if (_logs.Count > 50)
                {
                    _logs = _logs.GetRange(_logs.Count - 50, 50);
                }
            };
        }

        public void SaveNetwork()
        {
            if (!CheckInitialization()) return;

            var json = _network.Serialize();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON file (*.json)|*.json";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, json);
        }

        public void StartLearning()
        {
            if (!CheckInitialization()) return;

            UpdateParameters();
            Task.Run(() => _teacher.Learn());

            Task.Run(() =>
            {
                while (_teacher.IsLearning)
                {
                    OnPropertyChanged(nameof(TrainProgressValue));
                    Thread.Sleep(200);
                }
            });
        }

        public void StopLearning()
        {
            if (!CheckInitialization()) return;

            _teacher.StopLearn();
            OnPropertyChanged(nameof(TrainSetCount));
        }

        public void UpdateParameters()
        {
            if (!CheckInitialization()) return;

            _network.LearnSpeed = LearnSpeed;
            _network.Moment = Moment;
        }

        private bool CheckInitialization()
        {
            if (_network == null || _teacher == null)
            {
                MessageBox.Show("Network does not initialized");
                return false;
            }
            return true;
        }

        public void NextImage()
        {
            if (SelectedImageIndex + 1 >= TestCount)
            {
                SelectedImageIndex = 0;
                return;
            }

            SelectedImageIndex++;
        }

        public void PrevImage()
        {
            if (SelectedImageIndex - 1 < 0)
            {
                SelectedImageIndex = TestCount - 1;
                return;
            }

            SelectedImageIndex--;
        }

        private struct RecognizeData
        {
            public int Value;
            public double Percent;
        }

        public void Recognize()
        {
            var image = _loader.GetTestImage(SelectedImageIndex);
            var set = _trainCounstructor.CreateSet(image.Bytes);

            RecognizeSet(set);
        }

        public void RecognizePaint()
        {
            var bytes = _painter.GetBytes();
            var set = _trainCounstructor.CreateSetInverted(bytes);

            RecognizeSet(set);
        }

        private void RecognizeSet(TrainSet set)
        {
            if (!CheckInitialization()) return;

            if (_teacher.IsLearning)
            {
                MessageBox.Show("Network is learning now");
                return;
            }

            var results = _network.Iterate(set.Input).Select(x => new RecognizeData
            {
                Value = 0,
                Percent = x * 100
            }).ToArray();

            string answer = "";

            for (int i = 0; i < results.Length; ++i)
            {
                results[i].Value = i;
            }

            var top3 = results.OrderByDescending(x => x.Percent).Take(3);

            foreach (var result in top3)
            {
                answer += $"{result.Value}: {result.Percent: 0.##} %\r\n";
            }

            RecognizeResult = answer;
        }

        public void ResetWeights()
        {
            if (!CheckInitialization()) return;
            _teacher.ReinitWeights();
        }

        public void ImageMouseMove(bool leftPressed, Point pos, Size size)
        {
            var width = _loader.Width;
            var height = _loader.Height;
            

            var xScaleFactor = width / size.Width;
            var yScaleFactor = height / size.Height;

            var scaledPos = new Point(pos.X * xScaleFactor, pos.Y * yScaleFactor);

            if (leftPressed)
            {
                _painter.DrawPixel(scaledPos);
            }
        }

        public void ClearPaintImage()
        {
            _painter.ClearImage();
            OnPropertyChanged(nameof(PaintImage));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
