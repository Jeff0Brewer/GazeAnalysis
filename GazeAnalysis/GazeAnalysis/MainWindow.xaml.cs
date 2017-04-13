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
        String gazePathA = "C:/Users/master/Documents/gazelog/Pair14/A_04-07_12-04.txt";
        String gamePathA = "C:/Users/master/Documents/gamelog/Pair14/A_04-07_12-04_Img1.txt";
        String gazePathB = "C:/Users/master/Documents/gazelog/Pair14/B_04-07_12-04.txt";
        String gamePathB = "C:/Users/master/Documents/gamelog/Pair14/B_04-07_12-04_Img1.txt";
        int imageNumber = 0;
        int AInd = 0;
        System.Windows.Media.Color AColor;
        DataPoint[] AData;
        double ATmax;
        double ATmin;
        int BInd = 0;
        System.Windows.Media.Color BColor;
        DataPoint[] BData;
        double BTmax;
        double BTmin;
        bool adone = true;
        bool bdone = true;

        System.Windows.Threading.DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(update);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 15);
        }

        void update(object sender, EventArgs e)
        {
            double pTime = 0;
            SolidColorBrush brush = new SolidColorBrush(AColor);
            if ((AData[AInd].X != 0 || AData[AInd].Y != 0) && (pTime = AData[AInd].time - AData[0].time) < ATmax && !adone)
            {
                if (pTime >= ATmin)
                {
                    Acurr.Visibility = Visibility.Visible;
                    Canvas.SetLeft(Acurr, AData[AInd].X - Acurr.Width);
                    Canvas.SetTop(Acurr, AData[AInd].Y - Acurr.Height);
                    Ellipse dot = new Ellipse();
                    dot.Height = 5;
                    dot.Width = 5;
                    dot.Fill = brush;
                    Canvas.SetLeft(dot, AData[AInd].X - dot.Width / 2);
                    Canvas.SetTop(dot, AData[AInd].Y - dot.Height / 2);
                    Panel.SetZIndex(dot, AInd);
                    myCanvas.Children.Add(dot);
                    Ellipse shade = new Ellipse();
                    shade.Height = 50;
                    shade.Width = 50;
                    shade.Opacity = .05;
                    shade.Fill = brush;
                    Canvas.SetLeft(shade, AData[AInd].X - shade.Width / 2);
                    Canvas.SetTop(shade, AData[AInd].Y - shade.Height / 2);
                    Panel.SetZIndex(shade, AInd);
                    myCanvas.Children.Add(shade);
                    if (AInd > 0)
                    {
                        Line seg = new Line();
                        seg.X1 = AData[AInd].X;
                        seg.Y1 = AData[AInd].Y;
                        seg.X2 = AData[AInd - 1].X;
                        seg.Y2 = AData[AInd - 1].Y;
                        seg.StrokeThickness = 1;
                        seg.Stroke = brush;
                        seg.Opacity = .5;
                        Panel.SetZIndex(seg, AInd);
                        myCanvas.Children.Add(seg);
                    }
                }
                AInd++;
            }
            else
            {
                adone = true;
                Acurr.Visibility = Visibility.Hidden;
            }
            brush = new SolidColorBrush(BColor);
            if ((BData[BInd].X != 0 || BData[BInd].Y != 0) && (pTime = BData[BInd].time - BData[0].time) < BTmax && !bdone)
            {
                if (pTime >= BTmin)
                {
                    Bcurr.Visibility = Visibility.Visible;
                    Canvas.SetLeft(Bcurr, BData[BInd].X - Bcurr.Width/2);
                    Canvas.SetTop(Bcurr, BData[BInd].Y - Bcurr.Height/2);
                    Ellipse dot = new Ellipse();
                    dot.Height = 5;
                    dot.Width = 5;
                    dot.Fill = brush;
                    Canvas.SetLeft(dot, BData[BInd].X - dot.Width / 2);
                    Canvas.SetTop(dot, BData[BInd].Y - dot.Height / 2);
                    Panel.SetZIndex(dot, BInd);
                    myCanvas.Children.Add(dot);
                    Ellipse shade = new Ellipse();
                    shade.Height = 50;
                    shade.Width = 50;
                    shade.Opacity = .05;
                    shade.Fill = brush;
                    Canvas.SetLeft(shade, BData[BInd].X - shade.Width / 2);
                    Canvas.SetTop(shade, BData[BInd].Y - shade.Height / 2);
                    Panel.SetZIndex(shade, BInd);
                    myCanvas.Children.Add(shade);
                    if (BInd > 0)
                    {
                        Line seg = new Line();
                        seg.X1 = BData[BInd].X;
                        seg.Y1 = BData[BInd].Y;
                        seg.X2 = BData[BInd - 1].X;
                        seg.Y2 = BData[BInd - 1].Y;
                        seg.StrokeThickness = 1;
                        seg.Stroke = brush;
                        seg.Opacity = .5;
                        Panel.SetZIndex(seg, BInd);
                        myCanvas.Children.Add(seg);
                    }
                }
                BInd++;
            }
            else
            {
                bdone = true;
                Bcurr.Visibility = Visibility.Hidden;
            }
            if (adone && bdone)
                timer.Stop();
        }

        private int findStartInd(int ind, DataPoint[] data, double tma, double tmi)
        {
            double pTime = 0;
            while ((data[ind].X != 0 || data[ind].Y != 0) && (pTime = data[ind].time - data[0].time) < tma && !(pTime >= tmi))
            {
                ind++;
            }
            return ind;
        }

        private DataPoint[] processText(String text)
        {
            DataPoint[] points = new DataPoint[text.Split('\n').Length];
            int ind1 = text.IndexOf("(", 0);
            int ind2 = text.IndexOf(",", 0);
            int pState = 0;
            int pointnumber = 0;
            points[0] = new DataPoint();
            while (ind1 >= 0)
            {
                if (pState == 0)
                {
                    points[pointnumber].X = Convert.ToDouble(text.Substring(ind1 + 1, ind2 - ind1 - 1));
                    ind1 = text.IndexOf(",", ind1);
                    ind2 = text.IndexOf(")", ind2);
                    pState = 1;
                }
                else if (pState == 1)
                {
                    points[pointnumber].Y = Convert.ToDouble(text.Substring(ind1 + 1, ind2 - ind1 - 1));
                    ind1 = text.IndexOf("@", ind1) + 1;
                    ind2 = text.IndexOf("o", ind2) - 1;
                    pState = 2;
                }
                else
                {
                    points[pointnumber].time = parseTime(text.Substring(ind1 + 1, ind2 - ind1 - 1));
                    ind1 = text.IndexOf("(", ind1);
                    ind2 = text.IndexOf(",", ind2);
                    pointnumber++;
                    points[pointnumber] = new DataPoint();
                    pState = 0;
                }
            }
            return points;
        }

        private double[] getTimes(String path)
        {
            double[] times = new double[7];
            times[0] = 0;
            using (StreamReader sr = new StreamReader(path))
            {
                string gameData = sr.ReadToEnd();
                int ind1 = gameData.IndexOf("-") + 2,
                    ind2 = gameData.IndexOf("s", ind1) - 2;
                int timeCount = 1;
                double totalTime = 0;
                while (ind1 > 1) {
                    totalTime += Convert.ToDouble(gameData.Substring(ind1, ind2 - ind1));
                    times[timeCount] = totalTime;
                    ind1 = gameData.IndexOf("-", ind2) + 2;
                    ind2 = gameData.IndexOf("s", ind1) - 2;
                    timeCount++;
                }
            }
            return times;
        }

        private double[] getTimes(int imNum, String path)
        {
            double[] times = { 0, 0 };
            using (StreamReader sr = new StreamReader(path))
            {
                string gameData = sr.ReadToEnd();
                int ind1 = gameData.IndexOf("-") + 2,
                    ind2 = gameData.IndexOf("s", ind1) - 2;
                double totalTime = 0;
                int imageCount = 0;
                bool fromStart = false;
                bool fromEnd = false;
                if (imNum == 10)
                {
                    fromStart = true;
                    imNum = 2;
                }
                else if (imNum == 11) {
                    fromEnd = true;
                    imNum = 3;
                }
                while (ind1 > 1 && imageCount <= imNum)
                {
                    totalTime += Convert.ToDouble(gameData.Substring(ind1, ind2 - ind1));
                    times[0] = times[1];
                    times[1] = totalTime;
                    ind1 = gameData.IndexOf("-", ind2) + 2;
                    ind2 = gameData.IndexOf("s", ind1) - 2;
                    imageCount++;
                }
                if (fromStart)
                    times[0] = 0;
                if (fromEnd)
                    times[1] = 999999;
            }
            return times;
        }

        private double[] drawPath(DataPoint[] points, Color color, double tmin, double tmax) {
            int i = 0;
            double pTime = 0;
            double aDist = 0;
            double topTime = 0;
            double leftTime = 0;
            double topLeftTime = 0;
            double topRightTime = 0;
            double bottomLeftTime = 0;
            double bottomRightTime = 0;
            double inTime = 0;
            SolidColorBrush brush = new SolidColorBrush(color);
            while ((points[i].X != 0 || points[i].Y != 0) && (pTime = points[i].time - points[0].time) < tmax) {
                if (pTime >= tmin)
                {
                    Ellipse dot = new Ellipse();
                    dot.Height = 5;
                    dot.Width = 5;
                    dot.Fill = brush;
                    Canvas.SetLeft(dot, points[i].X - dot.Width / 2);
                    Canvas.SetTop(dot, points[i].Y - dot.Height / 2);
                    Panel.SetZIndex(dot, i);
                    myCanvas.Children.Add(dot);
                    Ellipse shade = new Ellipse();
                    shade.Height = 50;
                    shade.Width = 50;
                    shade.Opacity = .05;
                    shade.Fill = brush;
                    Canvas.SetLeft(shade, points[i].X - shade.Width / 2);
                    Canvas.SetTop(shade, points[i].Y - shade.Height / 2);
                    Panel.SetZIndex(shade, i);
                    myCanvas.Children.Add(shade);
                    if (i > 0)
                    {
                        aDist += distance(points[i - 1], points[i])*.5;
                        Line seg = new Line();
                        seg.X1 = points[i].X;
                        seg.Y1 = points[i].Y;
                        seg.X2 = points[i - 1].X;
                        seg.Y2 = points[i - 1].Y;
                        seg.StrokeThickness = 1;
                        seg.Stroke = brush;
                        seg.Opacity = .5;
                        Panel.SetZIndex(seg, i);
                        myCanvas.Children.Add(seg);
                    }
                    //if (points[i].Y < 350 && points[i].X < 600)
                    //    topTime++;
                    //if (points[i].X < 300)
                    //    leftTime++;
                    //if (points[i].X < 600)
                    //    inTime++;
                    DataPoint p = points[i];
                    if (p.X < 600)
                        inTime++;
                    if (p.Y < 350 && p.X < 300)
                        topLeftTime++;
                    else if (p.Y < 350 && p.X < 600)
                        topRightTime++;
                    else if (p.Y > 350 && p.X < 300)
                        bottomLeftTime++;
                    else if (p.Y > 350 && p.X < 600)
                        bottomRightTime++;
                }
                i++;
            }
            //double[] ret = {aDist / i, topTime / inTime, leftTime/inTime};
            double[] ret = { aDist / i, topLeftTime / inTime, topRightTime / inTime, bottomLeftTime / inTime, bottomRightTime / inTime };
            return ret;
        }

        private double distance(DataPoint p1, DataPoint p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private double parseTime(String timeStr) {
            double hours, minutes, seconds, mseconds;
            int i1 = timeStr.IndexOf(":"),
                i2 = timeStr.IndexOf(":",i1 + 1),
                i3 = timeStr.IndexOf(".");
            hours = Convert.ToDouble(timeStr.Substring(0, i1));
            minutes = Convert.ToDouble(timeStr.Substring(i1 + 1,i2 - i1 - 1));
            seconds = Convert.ToDouble(timeStr.Substring(i2 + 1, 2));
            mseconds = Convert.ToDouble(timeStr.Substring(i3 + 1, 2));
            return hours * 60 * 60 + minutes * 60 + seconds + mseconds / 100;
        }

        private bool timeCheck(double t, double tmin, double tmax) {
            return t <= tmax && t >= tmin;
        }

        private class DataPoint {
            public double X;
            public double Y;
            public double time;

            public DataPoint(double ix,double iy, double it) {
                X = ix;
                Y = iy;
                time = it;
            }

            public DataPoint() {
                X = 0;
                Y = 0;
                time = 0;
            }
        }

        private void imgChange(object sender, MouseButtonEventArgs e)
        {
            List<UIElement> itemstoremove = new List<UIElement>();
            foreach (UIElement child in myCanvas.Children) {
                if (!(child.GetType().Equals(btn.GetType()) | child.GetType().Equals(Atxt.GetType()) | child.GetType().Equals(Acurr.GetType()))) {
                    itemstoremove.Add(child);
                }
            }
            foreach (UIElement child in itemstoremove) {
                myCanvas.Children.Remove(child);
            }
            using (StreamReader sr = new StreamReader(gazePathA))
            {
                imageNumber = Convert.ToInt32((sender as Button).Content) - 1;
                String gazeData = sr.ReadToEnd();
                double[] times = getTimes(imageNumber, gamePathA);
                AColor = System.Windows.Media.Colors.Blue;
                AData = processText(gazeData);
                ATmin = times[0];
                ATmax = times[1];
                AInd = 0;
                AInd = findStartInd(AInd, AData, ATmax, ATmin);
                double[] stats = drawPath(processText(gazeData), System.Windows.Media.Colors.Blue, times[0], times[1]);
                //Atxt.Text = "Adist: " + stats[0].ToString() + "\n"
                //            + "Atop: " + stats[1].ToString() + "\n"
                //            + "Aleft: " + stats[2].ToString();
                Atxt.Text = "Adist: " + stats[0].ToString() + "\n"
                            + "Atopleft: " + stats[1].ToString() + "\n"
                            + "Atopright: " + stats[2].ToString() + "\n"
                          + "Abottomleft: " + stats[3].ToString() + "\n"
                            + "Abottomright: " + stats[4].ToString();

            }
            using (StreamReader sr = new StreamReader(gazePathB))
            {
                String gazeData = sr.ReadToEnd();
                double[] times = getTimes(imageNumber, gamePathB);
                BColor = System.Windows.Media.Colors.Red;
                BData = processText(gazeData);
                BTmin = times[0];
                BTmax = times[1];
                BInd = 0;
                BInd = findStartInd(BInd, BData, BTmax, BTmin);
                double[] stats = drawPath(processText(gazeData), System.Windows.Media.Colors.Red, times[0], times[1]);
                //Btxt.Text = "Bdist: " + stats[0].ToString() + "\n"
                //            + "Btop: " + stats[1].ToString() + "\n"
                //            + "Bleft: " + stats[2].ToString();
                Btxt.Text = "Bdist: " + stats[0].ToString() + "\n"
                       + "Btopleft: " + stats[1].ToString() + "\n"
                      + "Btopright: " + stats[2].ToString() + "\n"
                      + "Bbottomleft: " + stats[3].ToString() + "\n"
                      + "Bbottomright: " + stats[4].ToString();
            }
            adone = false;
            bdone = false;
            timer.Start();
        }
    }
}
