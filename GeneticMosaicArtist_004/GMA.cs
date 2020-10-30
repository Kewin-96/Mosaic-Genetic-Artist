using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GeneticMosaicArtist_004
{
    public static class GMA
    {
        //inne
        public static MainWindow mainWindow;
        public static long time = -1;
        public static int sideOfSquare = -1;

        //parametry PSO
        public static int populationParticles = 20;
        public static double PSO_c1 = 0.3;
        public static double PSO_c2 = 0.3;
        public static double PSO_w = 0.729;

        //parametry mozaiki
        public static double picRatio;
        public static double bestRatio = 0.577350269;  //"najlepsze ratio"
        public static int recs = 100;
        public static int triangs = -1;
        public static int yRecsCount = -1;
        public static int xRecsCount = -1;
        public static double xRec = -1;
        public static double yRec = -1;

        //dane mozaiki
        public static List<List<Point>> mosaicPoints;
        public static int mosaicPointsCount = 0;
        public static List<Point[]> mosaicTriangs;  //pamiętać że tutaj powtarzają się punkty z mosaicPoints nawet po 6 razy
        public static List<uint> mosaicTriangsColors; //moze bedzie trzeba jednak inta dac
        public static ChromosomeK mosaicChromosome;
        public static List<ChromosomeK> mosaicChromosomes;

        public static void GMA_init(MainWindow window)
        {
            mainWindow = window;
        }
        public static void CalcMosaicNDraw()
        {
            //reset wygenerowanego obrazka
            Picture.ResetGenImg();

            //obliczanie parametrów
            CalcParameters();

            //obliczanie punktów pierwszej mozaiki
            CalcFirstMosaic();

            //obliczanie trójkątów
            CalcTriangs();
            CalcTriangsStartColours();

            //rysowanie trójkątów mozaiki
            CalcTriangsPixelsInit();
            Picture.UpdateGenImage();

            Console.WriteLine(CheckTriangsAngles());
        }
        public static void CalcParameters()
        {
            picRatio = (double)Picture.width / (double)Picture.height; //np pinowy obrazek 500x1000 ma ratio 0.5. "Najlepsze ratio" = sqrt(3)/3 = 0.577350269
            recs = (int)mainWindow.triangleCount_slider.Value;
            double ratioX = picRatio / bestRatio; // stosunek liczby prostokątów X w poziomie do liczby prostokątów X w pionie
            double yRecs_doubleBuf = Math.Sqrt(recs / ratioX);
            yRecsCount = (int)Math.Round(yRecs_doubleBuf); //liczba prostokatow X w pionie
            xRecsCount = (int)Math.Round(recs / yRecs_doubleBuf);   //liczba prostokatow X w poziomie
            recs = yRecsCount * xRecsCount;
            triangs = (2 * xRecsCount + 1) * (2 * yRecsCount);
            mainWindow.triangs_textBlock.Text = "Triangles = " + triangs;
            xRec = (double)Picture.width / xRecsCount;
            yRec = (double)Picture.height / yRecsCount;
            Console.WriteLine("picRatio = " + picRatio);
            Console.WriteLine("ratioX = " + ratioX);
            Console.WriteLine("yRecs_doubleBuf = " + yRecs_doubleBuf);
            Console.WriteLine("yRecsCount = " + yRecsCount);
            Console.WriteLine("xRecsCount = " + xRecsCount);
            Console.WriteLine("recs = " + recs);
            Console.WriteLine("xRec = " + xRec);
            Console.WriteLine("yRec = " + yRec);
            Console.WriteLine("triangs = " + triangs);
        }
        public static void CalcFirstMosaic()
        {
            mosaicPoints = new List<List<Point>>();
            for (int y = 0; y < 2 * yRecsCount + 1; y++)
            {
                mosaicPoints.Add(new List<Point>());
                if (y % 2 == 0)
                    for (int x = 0; x < xRecsCount + 2; x++)
                    {
                        if (x == 0)
                        {
                            mosaicPoints[y].Add(new Point(0, y * (yRec / 2)));
                        }
                        else if (x == xRecsCount + 1)
                        {
                            mosaicPoints[y].Add(new Point(Picture.width - 1, y * (yRec / 2)));
                        }
                        else
                        {
                            mosaicPoints[y].Add(new Point(xRec * (x - 0.5), y * (yRec / 2)));
                        }
                    }
                else //if (y % 2 == 1)
                    for (int x = 0; x < xRecsCount + 1; x++)
                    {
                        mosaicPoints[y].Add(new Point(x * xRec, y * (yRec / 2)));
                    }
            }

            for (int i = 0; i < mosaicPoints.Count; i++)
                for (int j = 0; j < mosaicPoints[i].Count; j++)
                {
                    mosaicPoints[i][j] = new Point((int)mosaicPoints[i][j].X, (int)mosaicPoints[i][j].Y);
                }

            /*Console.WriteLine('\n');
            for (int i = 0; i < mosaicPoints.Count; i++)
                for (int j = 0; j < mosaicPoints[i].Count; j++)
                    Console.WriteLine("mosaicPoints[" + i + "][" + j + "] = " + mosaicPoints[i][j].X + ", " + mosaicPoints[i][j].Y);
            Console.WriteLine('\n');*/

            for (int i = 0; i < mosaicPoints.Count; i++)
                for (int j = 0; j < mosaicPoints[i].Count; j++)
                {
                    if (mosaicPoints[i][j].X >= Picture.width)
                        mosaicPoints[i][j] = new Point(mosaicPoints[i][j].X - 1, mosaicPoints[i][j].Y);
                    if (mosaicPoints[i][j].Y >= Picture.height)
                        mosaicPoints[i][j] = new Point(mosaicPoints[i][j].X, mosaicPoints[i][j].Y - 1);
                }

            for (int i = 0; i < mosaicPoints.Count; i++)
                for (int j = 0; j < mosaicPoints[i].Count; j++)
                    if (mosaicPoints[i][j].X >= Picture.width || mosaicPoints[i][j].Y >= Picture.height)
                        throw new Exception("!!!!! ERRRRRORRRR !!!!! mosaicPoints[" + i + "][" + j + "] = " + mosaicPoints[i][j].X + ", " + mosaicPoints[i][j].Y);

            Console.WriteLine("!!!!! !!!!! !!!!! mosaicPoints.Count = " + mosaicPoints.Count);
            Console.WriteLine("!!!!! !!!!! !!!!! mosaicPoints[0].Count = " + mosaicPoints[0].Count);
            int buf1 = 0;
            for (int i = 0; i < mosaicPoints.Count; i++)
                buf1 += mosaicPoints[i].Count;
            Console.WriteLine("!!!!! !!!!! !!!!! mosaicPoints.Count.Count = " + buf1);
            for (int i = 0; i < mosaicPoints[0].Count; i++)
            {
                Console.Write(mosaicPoints[0][i] + ", ");
            }
            Console.WriteLine();
        }
        public static void CalcTriangs()
        {
            mosaicTriangs = new List<Point[]>();
            for (int y = 0; y < mosaicPoints.Count - 1; y++)
            {
                if (y % 2 == 0)
                {
                    for (int x = 0; x < mosaicPoints[y].Count - 1; x++)
                    {
                        mosaicTriangs.Add(new Point[3] { mosaicPoints[y][x], mosaicPoints[y][x + 1], mosaicPoints[y + 1][x] });
                    }
                    for (int x = 1; x < mosaicPoints[y].Count - 1; x++)
                    {
                        mosaicTriangs.Add(new Point[3] { mosaicPoints[y][x], mosaicPoints[y + 1][x], mosaicPoints[y + 1][x - 1] });
                    }
                }
                else
                {
                    for (int x = 0; x < mosaicPoints[y].Count - 1; x++)
                    {
                        mosaicTriangs.Add(new Point[3] { mosaicPoints[y][x], mosaicPoints[y][x + 1], mosaicPoints[y + 1][x + 1] });
                    }
                    for (int x = 0; x < mosaicPoints[y].Count; x++)
                    {
                        mosaicTriangs.Add(new Point[3] { mosaicPoints[y][x], mosaicPoints[y + 1][x + 1], mosaicPoints[y + 1][x] });
                    }
                }
            }

            /*List<List<Point>> buf = new List<List<Point>>();
            buf.Add(new List<Point>());
            for(int i = 0; i < mosaicTriangs.Count; i++)
            {
                for (int j = 0; j < mosaicTriangs[i].Length; j++)
                {
                    buf[0].Add(mosaicTriangs[i][j]);
                    if (mosaicTriangs[i][j].X > Picture.width || mosaicTriangs[i][j].Y > Picture.height)
                        throw new Exception("nastepny wyjatek");
                }
            }
            DrawX.drawLines(buf);*/
        }
        public static void CalcTriangsStartColours()
        {
            mosaicTriangsColors = new List<uint>();
            for (int i = 0; i < mosaicTriangs.Count; i++)
            {
                mosaicTriangsColors.Add((uint)((((int)(i * (200 / (double)triangs))) << 16) + (((int)(i * (200 / (double)triangs))) << 8) + (((int)(i * (200 / (double)triangs))))));
            }
        }
        public static void CalcTriangsPixels()
        {
            for (int i = 0; i < mosaicTriangs.Count; i++)//TUTUTUTU - przywrocic po sprawdzeniu !!!
            {
                Picture.DrawFilledTriangle(mosaicTriangs[i], (((uint)255 << 24) + mosaicTriangsColors[i]));
            }
        }
        public static void CalcTriangsPixelsInit()
        {
            for (int i = 0; i < mosaicTriangs.Count; i++)//TUTUTUTU - przywrocic po sprawdzeniu !!!
            {
                Picture.DrawFilledTriangleInit(mosaicTriangs[i], i);
            }
        }
        public static void SetTriangsBlackColours()
        {
            mosaicTriangsColors = new List<uint>();
            for (int i = 0; i < mosaicTriangs.Count; i++)
            {
                mosaicTriangsColors.Add(0);
            }
        }
        public static bool CheckTriangsAngles() //jesli kąty trójkątów są w przedziale (0,180) to TRUE
        {
            for (int i = 0; i < mosaicTriangs.Count; i++)
            {
                double buf = CountAngle(mosaicTriangs[i]);
                if (buf <= 0 || buf >= 180)
                {
                    //Console.WriteLine("Triangle: (" + mosaicTriangs[i][0].X + "," + mosaicTriangs[i][0].Y + "),(" + mosaicTriangs[i][1].X + "," + mosaicTriangs[i][1].Y + "),(" + mosaicTriangs[i][2].X + "," + mosaicTriangs[i][2].Y + "), angle = " + buf + ", FALSE");
                    return false;
                }
                //Console.WriteLine("Triangle: (" + mosaicTriangs[i][0].X + "," + mosaicTriangs[i][0].Y + "),(" + mosaicTriangs[i][1].X + "," + mosaicTriangs[i][1].Y + "),(" + mosaicTriangs[i][2].X + "," + mosaicTriangs[i][2].Y + "), angle = " + buf + ", TRUE");
            }
            return true;
        }
        public static double CountAngle(Point[] triangle)
        {
            double angle1, angle2;
            double x = (triangle[0].X - triangle[1].X);
            double y = (-1) * (triangle[0].Y - triangle[1].Y);    //(-1) BO Y ROSNIE W DÓŁ NA OBRAZIE TO TRZEBA PRZEKONWERTOWAĆ !!!
            if (y >= 0 && x >= 0)
                angle1 = Math.Atan(y / x) * (180 / Math.PI);
            else if (y >= 0 && x <= 0)
                angle1 = Math.Atan(y / x) * (180 / Math.PI) + 180;
            else if (y <= 0 && x < 0)   //TAK, x<0, sprawdzone !!!
                angle1 = Math.Atan(y / x) * (180 / Math.PI) + 180;
            else
                angle1 = Math.Atan(y / x) * (180 / Math.PI) + 360;
            x = (triangle[2].X - triangle[1].X);
            y = (-1) * (triangle[2].Y - triangle[1].Y);
            if (y >= 0 && x >= 0)
                angle2 = Math.Atan(y / x) * (180 / Math.PI);
            else if (y >= 0 && x <= 0)
                angle2 = Math.Atan(y / x) * (180 / Math.PI) + 180;
            else if (y <= 0 && x < 0)   //TAK, x<0, sprawdzone !!!
                angle2 = Math.Atan(y / x) * (180 / Math.PI) + 180;
            else
                angle2 = Math.Atan(y / x) * (180 / Math.PI) + 360;
            if (angle1 > angle2)
            {
                angle1 -= 360;
            }
            return angle2 - angle1;
        }
        public static void calcSideOfRandSquare()
        {
            if (xRec > yRec / 2)
                sideOfSquare = (int)Math.Round((yRec / 4) - 4);
            else
                sideOfSquare = (int)Math.Round((xRec / 2) - 4);
            if (sideOfSquare < 0)
                sideOfSquare = 0;
        }
        public static void randomizeMosaicPointsPositions()
        {
            Random rnd = new Random();
            //Console.WriteLine("xRec = " + xRec + "yRec = " + yRec + "sideOfSquare = " + sideOfSquare);
            for (int y = 0; y < mosaicPoints.Count; y++) //y
            {
                for (int x = 0; x < mosaicPoints[y].Count; x++) //x
                {
                    //Console.WriteLine("y = " + y + ", x = " + x + ", Przed: X = " + mosaicPoints[y][x].X + ", Y = " + mosaicPoints[y][x].Y);
                    if ((y == 0 || y == mosaicPoints.Count - 1))
                    {
                        if (x > 0 && x < mosaicPoints[y].Count - 1)
                            mosaicPoints[y][x] = new Point(mosaicPoints[y][x].X + rnd.Next((-1) * sideOfSquare, sideOfSquare), mosaicPoints[y][x].Y);
                    }
                    else
                    {
                        if (x == 0 || x == mosaicPoints[y].Count - 1)
                            mosaicPoints[y][x] = new Point(mosaicPoints[y][x].X, mosaicPoints[y][x].Y + rnd.Next((-1) * sideOfSquare, sideOfSquare));
                        else if (x > 0)
                            mosaicPoints[y][x] = new Point(mosaicPoints[y][x].X + rnd.Next((-1) * sideOfSquare, sideOfSquare), mosaicPoints[y][x].Y + rnd.Next((-1) * sideOfSquare, sideOfSquare));
                    }
                    //Console.WriteLine("Po: X = " + mosaicPoints[y][x].X + ", Y = " + mosaicPoints[y][x].Y);
                }
            }
        }
        public static void randomizeMosaicColours()
        {
            Random rnd = new Random();
            for (int i = 0; i < mosaicTriangsColors.Count; i++)
            {
                uint blue = (uint)((mosaicTriangsColors[i] & 0x0000ff) + rnd.Next(-10, 11));
                uint green = (uint)(((mosaicTriangsColors[i] & 0x00ff00) >> 8) + rnd.Next(-10, 11));
                uint red = (uint)(((mosaicTriangsColors[i] & 0xff0000) >> 16) + rnd.Next(-10, 11));
                if (blue > 255)
                    blue = 255;
                else if (blue < 0)
                    blue = 0;
                if (green > 255)
                    green = 255;
                else if (green < 0)
                    green = 0;
                if (red > 255)
                    red = 255;
                else if (red < 0)
                    red = 0;
                mosaicTriangsColors[i] = ((red << 16) + (green << 8) + blue);
            }
        }
        public static void calcMosaicPointCount()
        {
            //*obliczanie ilosci punktow mozaiki
            mosaicPointsCount = 0;
            for (int i = 0; i < mosaicPoints.Count; i++)
                for (int j = 0; j < mosaicPoints[i].Count; j++)
                    mosaicPointsCount++;
        }
        public static void ConvertDataIntoChromosome()
        {
            //konwersja punktów

            //*przepisywanie punktow mozaiki
            double[] mosaicPointsD = new double[mosaicPointsCount * 2];
            int ii = 0;
            for (int i = 0; i < mosaicPoints.Count; i++)
                for (int j = 0; j < mosaicPoints[i].Count; j++)
                {
                    mosaicPointsD[ii] = mosaicPoints[i][j].X;
                    mosaicPointsD[ii + 1] = mosaicPoints[i][j].Y;
                    ii += 2;
                }

            //konwersja kolorów
            double[] mosaicTriangsColorsD = new double[mosaicTriangsColors.Count * 3];
            for (int i = 0; i < mosaicTriangsColorsD.Length; i += 3)
            {
                mosaicTriangsColorsD[i] = mosaicTriangsColors[i / 3] & 0x0000ff;
                mosaicTriangsColorsD[i + 1] = ((mosaicTriangsColors[i / 3] & 0x00ff00) >> 8);
                mosaicTriangsColorsD[i + 2] = ((mosaicTriangsColors[i / 3] & 0xff0000) >> 16);
            }

            //inicjalizacja zmiennych
            double[] geneValues = new double[mosaicPointsD.Length + mosaicTriangsColorsD.Length];
            double[] mins = new double[mosaicPointsD.Length + mosaicTriangsColorsD.Length];
            double[] maxs = new double[mosaicPointsD.Length + mosaicTriangsColorsD.Length];

            //wygenerowanie min, max wartosci, liczby bitow, liczby cyfr po przecinku
            //*PUNKTY
            for (int i = 0; i < mosaicPointsD.Length; i++)
            {
                geneValues[i] = mosaicPointsD[i];

                //min, max wartosci
                if (Math.Round(mosaicPointsD[i]) == 0)   //punkt z krawędzi - punkt ma pozostac na krawędzi !!!
                {
                    mins[i] = 0;
                    maxs[i] = 0;
                }
                else if (i % 2 == 0 && Math.Round(mosaicPointsD[i]) == Picture.width - 1)   //współrzędna punktu z krawędzi - punkt ma pozostac na krawędzi !!!
                {
                    mins[i] = Picture.width - 1;
                    maxs[i] = Picture.width - 1;
                }
                else if (i % 2 == 1 && Math.Round(mosaicPointsD[i]) == Picture.height - 1)   //współrzędna punktu z krawędzi - punkt ma pozostac na krawędzi !!!
                {
                    mins[i] = Picture.height - 1;
                    maxs[i] = Picture.height - 1;
                }
                else
                {
                    mins[i] = 0;
                    if (i % 2 == 0) maxs[i] = Picture.width - 1; else maxs[i] = Picture.height - 1;
                }
            }
            //KOLORY
            for (int i = mosaicPointsD.Length; i < geneValues.Length; i++)
            {
                geneValues[i] = mosaicTriangsColorsD[i - mosaicPointsD.Length];
                mins[i] = 0;
                maxs[i] = 255;
                //Console.WriteLine("maxs[+" + i + "] = " + maxs[i]);
            }

            //stworzenie chromosomu
            mosaicChromosome = new ChromosomeK(mins, maxs, geneValues);
        }
        public static void ConvertChromosomeIntoData(double[] geneValues)
        {
            int ii = 0;
            for (int i = 0; i < mosaicPoints.Count; i++)
                for (int j = 0; j < mosaicPoints[i].Count; j++)
                {
                    mosaicPoints[i][j] = new Point(geneValues[ii], geneValues[ii + 1]);
                    ii += 2;
                }
            int ii_buf = ii;
            for (int i = ii_buf; i < geneValues.Length; i += 3)
            {
                mosaicTriangsColors[(i - ii_buf) / 3] = (((uint)255 << 24) + (uint)geneValues[ii + 2] << 16) + ((uint)geneValues[ii + 1] << 8) + (uint)geneValues[ii];
                //Console.WriteLine(((uint)geneValues[ii + 2] << 16) + ", " + ((uint)geneValues[ii + 1] << 8) + ", " + (uint)geneValues[ii]);
                if (geneValues[ii] < 0 || geneValues[ii] > 16777216)
                    throw new Exception("Cos nie tak");
                ii += 3;
            }
        }
        public static string secsIntoHMS(long timeInSeconds)
        {
            string result = "";
            result += timeInSeconds / 3600;
            result += "h";
            timeInSeconds %= 3600;
            result += timeInSeconds / 60;
            result += "m";
            timeInSeconds %= 60;
            result += timeInSeconds;
            result += "s";
            return result;
        }
        public static void Start()
        {
            calcSideOfRandSquare();
            calcMosaicPointCount();
            string path_buf = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!Directory.Exists(System.IO.Path.Combine(path_buf, "results")))
                Directory.CreateDirectory(System.IO.Path.Combine(path_buf, "results"));
            path_buf = System.IO.Path.Combine(path_buf, "results\\Iteration_0_Time_" + DateTime.Now.Ticks + ".bmp");
            Picture.SavePicture(path_buf);
            mosaicChromosomes = new List<ChromosomeK>();

            bool randPoints = true, randColours = false;
            //mainWindow.mainGrid.Dispatcher.Invoke(() => { randPoints = (bool)mainWindow.randPoints_checkBox.IsChecked; randColours = (bool)mainWindow.randColours_checkBox.IsChecked; });
            if (randPoints || randColours)
            {
                //bufory
                List<List<Point>> mosaicPointsBuf = new List<List<Point>>();
                if (randPoints)
                {
                    for (int i = 0; i < mosaicPoints.Count; i++)
                    {
                        mosaicPointsBuf.Add(new List<Point>());
                        for (int j = 0; j < mosaicPoints[i].Count; j++)
                        {
                            mosaicPointsBuf[i].Add(new Point(mosaicPoints[i][j].X, mosaicPoints[i][j].Y));
                        }
                    }
                }
                List<uint> mosaicTriangsColorsBuf = new List<uint>();
                if (randColours)
                {
                    for (int i = 0; i < mosaicTriangsColors.Count; i++)
                    {
                        mosaicTriangsColorsBuf.Add(mosaicTriangsColors[i]);
                    }
                }

                //tworzenie lekko zrandomizowanych chromosomow w liczbie <minPop>
                for (int i = 0; i < populationParticles; i++)
                {
                    //przepisanie z bufora
                    if (randPoints)
                    {
                        for (int j = 0; j < mosaicPoints.Count; j++)
                        {
                            for (int k = 0; k < mosaicPoints[j].Count; k++)
                            {
                                mosaicPoints[j][k] = new Point(mosaicPointsBuf[j][k].X, mosaicPointsBuf[j][k].Y);
                            }
                        }
                        while (true)
                        {
                            randomizeMosaicPointsPositions();
                            CalcTriangs();
                            CalcTriangsPixelsInit();
                            if (CheckTriangsAngles() == true)
                                break;
                        }
                    }
                    if (randColours)
                    {
                        for (int j = 0; j < mosaicTriangsColors.Count; j++)
                        {
                            mosaicTriangsColors[j] = mosaicTriangsColorsBuf[j];
                        }
                        randomizeMosaicColours();
                    }
                    if (CheckTriangsAngles() == false)      //DEBUG 001
                        Console.WriteLine("X1[" + i + "] RANDOMIZACJA BŁĘDNA !!!");
                    ConvertDataIntoChromosome();
                    mosaicChromosomes.Add(mosaicChromosome);
                }
            }
            else
            {
                for (int i = 0; i < populationParticles; i++)
                    mosaicChromosomes.Add(mosaicChromosome);
            }
            for (int i = 0; i < mosaicChromosomes.Count; i++) //DEBUG 001
            {
                ConvertChromosomeIntoData(mosaicChromosomes[i].geneValues);
                if (CheckTriangsAngles() == false)
                    Console.WriteLine("X2[" + i + "] RANDOMIZACJA BŁĘDNA !!!");
                else
                {
                    Console.WriteLine("i = " + i);
                    CalcTriangs();
                    CalcTriangsPixelsInit();
                    if (CheckTriangsAngles() == false)
                        Console.WriteLine((double)Picture.width * (double)Picture.height * (double)195075);
                    else
                        Console.WriteLine(Picture.evaluationSimilarity());
                }
            }

            //wyswietlenie (ostatniego) chromosomu (obrazka)
            CalcTriangs();
            CalcTriangsPixelsInit();
            mainWindow.mainGrid.Dispatcher.Invoke(() => { Picture.UpdateGenImage(); });

            //algorytm PSO
            Console.WriteLine("\n***** PSO running *****");
            algorytmPSO(populationParticles, PSO_c1, PSO_c2, PSO_w);
            Console.WriteLine("\n***** PSO finished *****");

            Console.WriteLine("5 sec");
            Thread.Sleep(5000);

            mainWindow.mainGrid.Dispatcher.Invoke(() => { mainWindow.start_button.IsEnabled = true; });
            //mainWindow.mainGrid.Dispatcher.Invoke(() => { mainWindow.start_button.Content = "Start"; });
            mainWindow.mainGrid.Dispatcher.Invoke(() => { mainWindow.save_button.IsEnabled = true; });
            mainWindow.mainGrid.Dispatcher.Invoke(() => { mainWindow.load_button.IsEnabled = true; });
            mainWindow.mainGrid.Dispatcher.Invoke(() => { mainWindow.triangleCount_slider.IsEnabled = true; });
        }

        public static void algorytmPSO(int numParticles, double c1, double c2, double w)
        {
            int maxEpochs = 1000000;   //max liczba epok

            ChromosomeK bestChromosome = Solve(numParticles, maxEpochs, c1, c2, w);
            double bestError = Error(bestChromosome.geneValues);

            Console.WriteLine("Best solution found. Showing image ...");
            ConvertChromosomeIntoData(bestChromosome.geneValues);
            CalcTriangs();
            CalcTriangsPixelsInit();
            mainWindow.mainGrid.Dispatcher.Invoke(() => { Picture.UpdateGenImage(); });
            Console.WriteLine("\nFinal best error = " + bestError.ToString("F5"));
        }
        public static double Error(double[] geneValues)
        {
            ConvertChromosomeIntoData(geneValues);
            CalcTriangs();
            if (CheckTriangsAngles() == false)
                return (double)Picture.width * (double)Picture.height * (double)195075;    //teoretycznie najgorsza mozliwa wartosc funkcji przystosowania
            CalcTriangsPixelsInit();
            return Picture.evaluationSimilarity();
        }

        public static ChromosomeK Solve(int numParticles, int maxEpochs, double c1, double c2, double w)
        {
            // assumes existence of an accessible Error function and a Particle class
            Random rnd = new Random(0);
            Particle[] swarm = new Particle[numParticles];
            //double[] bestGlobalPosition = new double[dim]; // best solution found by any particle in the swarm
            ChromosomeK bestChromosome = null;
            double bestGlobalError = double.MaxValue; // smaller values better

            // swarm initialization
            for (int i = 0; i < swarm.Length; ++i)
            {
                double error = Error(mosaicChromosomes[i].geneValues);
                double[] initTableVelocity = new double[mosaicChromosomes[0].geneValues.Length];
                swarm[i] = new Particle(mosaicChromosomes[i].geneValues, error, initTableVelocity, mosaicChromosomes[i].geneValues, error);

                // does current Particle have global best position/solution?
                if (swarm[i].error < bestGlobalError)
                {
                    bestGlobalError = swarm[i].error;
                    bestChromosome = mosaicChromosomes[i];
                }
            } // initialization

            // prepare
            //double w = 0.729; // inertia weight. see http://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=00870279
            //double c1 = 1.49445; // cognitive/local weight
            //double c2 = 1.49445; // social/global weight
            double r1, r2; // cognitive and social randomizations
            double probDeath = 0.01;
            int epoch = 0;

            double[] newVelocity = new double[mosaicChromosomes[0].geneValues.Length];
            ChromosomeK newPosition = new ChromosomeK(new double[0], new double[0], new double[mosaicChromosomes[0].geneValues.Length]);
            double newError;

            //using (var writer = new StreamWriter(@"D:\PŁ\Zajęcia\X sem (inf)\Obliczenia Ewolucyjne\Laboratoria\plik" + ii + ".csv")) { }
            // main loop
            time = DateTime.Now.Ticks;
            while (epoch < maxEpochs)
            {
                double bufBestGlobalErr = double.MaxValue;
                for (int i = 0; i < swarm.Length; ++i) // each Particle/chromosome
                {
                    Particle currP = swarm[i]; // for clarity

                    int jj = 0;
                    while (true)
                    {
                        for (int j = 0; j < currP.velocity.Length; ++j) // each component of the velocity
                        {
                            r1 = rnd.NextDouble();
                            r2 = rnd.NextDouble();

                            // new velocity
                            newVelocity[j] = (w * currP.velocity[j]) +
                              (c1 * r1 * (currP.bestPosition[j] - currP.geneValue[j])) +
                              (c2 * r2 * (bestChromosome.geneValues[j] - currP.geneValue[j]));
                            /*Console.WriteLine("[" + j + "]Velocity change = " + (newVelocity[j] - currP.velocity[j]));
                            Console.WriteLine("[" + j + "]Gene value before = " + currP.geneValue[j]);*/

                            // new position
                            newPosition.geneValues[j] = currP.geneValue[j] + newVelocity[j];
                            if (newPosition.geneValues[j] < mosaicChromosomes[i].mins[j])
                                newPosition.geneValues[j] = mosaicChromosomes[i].mins[j];
                            else if (newPosition.geneValues[j] > mosaicChromosomes[i].maxs[j])
                                newPosition.geneValues[j] = mosaicChromosomes[i].maxs[j];
                        }
                        ConvertChromosomeIntoData(newPosition.geneValues);
                        CalcTriangs();
                        CalcTriangsPixelsInit();
                        if (CheckTriangsAngles() == true || jj == 10)
                        {
                            if (jj == 10)
                                Console.WriteLine("ojojoj, za dużo losowanych F=0");
                            break;
                        }
                        jj++;
                    }
                    newVelocity.CopyTo(currP.velocity, 0);
                    newPosition.geneValues.CopyTo(currP.geneValue, 0);

                    newError = Error(newPosition.geneValues);
                    currP.error = newError;

                    if (newError < currP.bestError)
                    {
                        newPosition.geneValues.CopyTo(currP.bestPosition, 0); 
                        currP.bestError = newError;
                    }

                    if (newError < bestGlobalError)
                    {
                        //bestChromosome = newPosition; tututu1 = "poprawa";
                        bestChromosome = new ChromosomeK(new double[0], new double[0], newPosition.geneValues);
                        bestGlobalError = newError;
                        Console.WriteLine("New best error for " + epoch + " iteration = " + bestGlobalError);
                        /*using (var writer = new StreamWriter(@"D:\PŁ\Zajęcia\X sem (inf)\Obliczenia Ewolucyjne\Laboratoria\plik" + ii + ".csv", true))
                        {
                            writer.WriteLine("Generation:;" + epoch + ";Error:;" + bestGlobalError);
                        }*/
                    }

                    // death?           -> operator śmierci - można potem spróbować ...
                    /*double die = rnd.NextDouble();
                    if (die < probDeath)
                    {
                        // new position, leave velocity, update error
                        for (int j = 0; j < currP.geneValue.Length; ++j)
                            currP.geneValue[j] = (maxX - minX) * rnd.NextDouble() + minX;
                        currP.error = Error(mosaicChromosomes[i]);
                        currP.geneValue.CopyTo(currP.bestPosition, 0);
                        currP.bestError = currP.error;

                        if (currP.error < bestGlobalError) // global best by chance?
                        {
                            bestGlobalError = currP.error;
                            bestChromosome = mosaicChromosomes[i];
                            Console.WriteLine("Best error for " + epoch + " epoch = " + bestGlobalError);
                            //using (var writer = new StreamWriter(@"D:\PŁ\Zajęcia\X sem (inf)\Obliczenia Ewolucyjne\Laboratoria\plik" + ii + ".csv", true))
                            //{
                            //    writer.WriteLine("Generation:;" + epoch + ";Error:;" + bestGlobalError);
                            //}
                        }
                    }*/

                } // each Particle
                //test1(swarm);
                ++epoch;
                if(bufBestGlobalErr > bestGlobalError)
                {
                    ConvertChromosomeIntoData(bestChromosome.geneValues);
                    CalcTriangs();
                    CalcTriangsPixelsInit();
                    mainWindow.mainGrid.Dispatcher.Invoke(() => { Picture.UpdateGenImage(); });
                    Console.WriteLine("Iteration = " + epoch + ", bestFitness = " + bestGlobalError + ", time = " + secsIntoHMS((DateTime.Now.Ticks - time) / 10000000));
                    string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    path = System.IO.Path.Combine(path, "results\\Iteration_" + epoch + "_Time_" + secsIntoHMS((DateTime.Now.Ticks - time) / 10000000) + "_Fit_" + bestGlobalError + ".bmp");
                    Picture.SavePicture(path);
                }
            } // while

            // show final swarm
            Console.WriteLine("\nProcessing complete");
            Console.WriteLine("\nFinal swarm:\n");
            /*for (int i = 0; i < swarm.Length; ++i)
                Console.WriteLine(swarm[i].ToString());*/

            ChromosomeK result = bestChromosome;
            return result;
        }
        public static void test1(Particle[] p)
        {
            for (int i = 0; i < p.Length; i++)
                Console.WriteLine("p[" + i + "].currF = " + p[i].error);
        }
    }
}
