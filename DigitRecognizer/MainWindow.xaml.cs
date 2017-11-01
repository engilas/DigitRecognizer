using DigitRecognizer.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DigitRecognizer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Test(object sender, EventArgs e)
        {
            //// Отображаем следующее изображение
            //int nextIndex = int.Parse(textBox4.Text);
            //DigitImage currImage = trainImages[nextIndex];
            //int mag = int.Parse(comboBox1.SelectedItem.ToString());
            //Bitmap bitMap = MakeBitmap(currImage, mag);
            ////pictureBox1.Image = bitMap;
            //string pixelVals = PixelValues(currImage);
            ////textBox5.Text = pixelVals;
            ////textBox3.Text = textBox4.Text; // обновляем текущий индекс
            ////textBox4.Text = (nextIndex + 1).ToString();
            ////listBox1.Items.Add("Curr image index = " +
            //                   //textBox3.Text + " label = " + currImage.label);
        }
    }
}
