using System;
using System.Collections.Generic;
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
using System.IO;

namespace GazeAnalysis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                using (StreamReader sr = new StreamReader("C:/Users/master/Documents/gazelog/A_03-08_04-02.txt"))
                {
                    String file = sr.ReadToEnd();
                    drawPath(processText(file), System.Windows.Media.Colors.Blue);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            try
            {
                using (StreamReader sr = new StreamReader("C:/Users/master/Documents/gazelog/A_03-08_04-01.txt"))
                {
                    String file = sr.ReadToEnd();
                    drawPath(processText(file), System.Windows.Media.Colors.Red);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        private Point[] processText(String text) {
            Point[] points = new Point[text.Split('\n').Length];
            int ind1 = text.IndexOf("(", 0);
            int ind2 = text.IndexOf(",", 0);
            bool Xcoord = true;
            int pointnumber = 0;
            while (ind1 >= 0 && ind2 >= 0) {
                if (Xcoord)
                {
                    points[pointnumber].X = Convert.ToDouble(text.Substring(ind1 + 1, ind2 - ind1 - 1));
                    ind1 = text.IndexOf(",", ind1);
                    ind2 = text.IndexOf(")", ind2);
                }
                else
                {
                    points[pointnumber].Y = Convert.ToDouble(text.Substring(ind1 + 1, ind2 - ind1 - 1));
                    ind1 = text.IndexOf("(", ind1);
                    ind2 = text.IndexOf(",", ind2);
                    pointnumber++;
                }
                Xcoord = !Xcoord;
            }
            return points;
        }

        private void drawPath(Point[] points, Color color) {
            int i = 0;
            SolidColorBrush brush = new SolidColorBrush(color);
            while (points[i].X != 0 || points[i].Y != 0) {
                Ellipse dot = new Ellipse();
                dot.Height = 5;
                dot.Width = 5;
                dot.Fill = brush;
                Canvas.SetLeft(dot, points[i].X - dot.Width / 2);
                Canvas.SetTop(dot, points[i].Y - dot.Height / 2);
                myCanvas.Children.Add(dot);
                Ellipse shade = new Ellipse();
                shade.Height = 50;
                shade.Width = 50;
                shade.Opacity = .05;
                shade.Fill = brush;
                Canvas.SetLeft(shade, points[i].X - shade.Width / 2);
                Canvas.SetTop(shade, points[i].Y - shade.Height / 2);
                myCanvas.Children.Add(shade);
                if (i > 0) {
                    Line seg = new Line();
                    seg.X1 = points[i].X;
                    seg.Y1 = points[i].Y;
                    seg.X2 = points[i - 1].X;
                    seg.Y2 = points[i - 1].Y;
                    seg.StrokeThickness = 1;
                    seg.Stroke = brush;
                    seg.Opacity = .5;
                    myCanvas.Children.Add(seg);
                }
                i++;
            }
        }
    }
}
