using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GeneticMosaicArtist_004
{
    public static class Picture
    {
        //inne
        public static MainWindow mainWindow;

        //obrazek oryginalny
        public static byte[] buffer_imgOrg;
        public static int width = -1;
        public static int height = -1;
        public static WriteableBitmap oryginalPictureBitmap;
        public static uint[] oryginalPicturePixels;
        public static byte[] header;
        public static int lengthOfHeader = -1;
        public static int magic_number = -1;

        //obrazek generowany
        public static WriteableBitmap genPictureBitmap;
        public static uint[] genPicturePixels;
        public static double lineThickness = 1; //0.75

        public static void Picture_init(MainWindow window)
        {
            mainWindow = window;
        }
        public static void PrintHeader(byte[] buffer)
        {
            Console.WriteLine("Type (magic id.): " + (int)(buffer[0] + (buffer[1] * 256)));
            Console.WriteLine("File size: " + (int)(buffer[2] + (buffer[3] * 256) + (buffer[4] * 65536) + (buffer[5] * 16777216)));
            Console.WriteLine("Reserved1: " + (int)(buffer[6] + (buffer[7] * 256)));
            Console.WriteLine("Reserved2: " + (int)(buffer[8] + (buffer[9] * 256)));
            Console.WriteLine("offset (head length): " + (int)(buffer[10] + (buffer[11] * 256) + (buffer[12] * 65536) + (buffer[13] * 16777216)));
            Console.WriteLine("dib_header_size: " + (int)(buffer[14] + (buffer[15] * 256) + (buffer[16] * 65536) + (buffer[17] * 16777216)));
            Console.WriteLine("width: " + (int)(buffer[18] + (buffer[19] * 256) + (buffer[20] * 65536) + (buffer[21] * 16777216)));
            Console.WriteLine("height: " + (int)(buffer[22] + (buffer[23] * 256) + (buffer[24] * 65536) + (buffer[25] * 16777216)));
            Console.WriteLine("Number of color planes: " + (int)(buffer[26] + (buffer[27] * 256)));
            Console.WriteLine("Bits per pixel: " + (int)(buffer[28] + (buffer[29] * 256)));
            Console.WriteLine("Compression type: " + (int)(buffer[30] + (buffer[31] * 256) + (buffer[32] * 65536) + (buffer[33] * 16777216)));
            Console.WriteLine("Image size in bytes: " + (int)(buffer[34] + (buffer[35] * 256) + (buffer[36] * 65536) + (buffer[37] * 16777216)));
            Console.WriteLine("x Pixels per meter: " + (int)(buffer[38] + (buffer[39] * 256) + (buffer[40] * 65536) + (buffer[41] * 16777216)));
            Console.WriteLine("y Pixels per meter: " + (int)(buffer[42] + (buffer[43] * 256) + (buffer[44] * 65536) + (buffer[45] * 16777216)));
            Console.WriteLine("Number of colors: " + (int)(buffer[46] + (buffer[47] * 256) + (buffer[48] * 65536) + (buffer[49] * 16777216)));
            Console.WriteLine("Important colors: " + (int)(buffer[50] + (buffer[51] * 256) + (buffer[52] * 65536) + (buffer[53] * 16777216)));
        }
        public static void Load_Picture()
        {
            // wybranie pliku przez użytkownika
            OpenFileDialog okienko = new OpenFileDialog();
            okienko.ShowDialog();
            okienko.Filter = "|*.bmp";
            string bufor = okienko.FileName;

            //ładowanie danych z pliku
            if (!bufor.Equals(""))
            {
                string path = @bufor;
                byte[] buffer = File.ReadAllBytes(path);
                buffer_imgOrg = new byte[buffer.Length];
                for (int i = 0; i < buffer.Length; i++)
                    buffer_imgOrg[i] = buffer[i];
                lengthOfHeader = buffer[10] + buffer[11] * 256 + buffer[12] * 65536 + buffer[13] * 16777216;
                Console.WriteLine("lengthOfHeader = " + lengthOfHeader);
                width = buffer[18] + buffer[19] * 256 + buffer[20] * 65536 + buffer[21] * 16777216;
                height = buffer[22] + buffer[23] * 256 + buffer[24] * 65536 + buffer[25] * 16777216;
                Console.WriteLine("width/height = " + width + "/" + height);
                int image_size = (int)(buffer[34] + (buffer[35] * 256) + (buffer[36] * 65536) + (buffer[37] * 16777216));
                magic_number = (image_size - (width * height * 3)) / height;
                Console.WriteLine("image_size = " + image_size + ", magic_number = " + magic_number);
                oryginalPictureBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                genPictureBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                oryginalPicturePixels = new uint[width * height];
                genPicturePixels = new uint[width * height];
                header = new byte[lengthOfHeader];
                for (int i = 0; i < lengthOfHeader; i++)
                {
                    header[i] = buffer[i];
                    Console.WriteLine("header[" + i + "] = " + header[i]);
                }
                PrintHeader(header);
                //przepisywanie danych obrazka
                int red, green, blue, alpha;
                /*for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int i = width * y + x;

                        blue = buffer[lengthOfHeader + 3 * (x + (((-1) * (width + 1) * y) + ((width + 1) * (height - 1))))];  //width + 1 bo jest padding po kazdej pionowej linii pikseli !!! -- look link o bmp
                        green = buffer[lengthOfHeader + 3 * (x + (((-1) * (width + 1) * y) + ((width + 1) * (height - 1)))) + 1];
                        red = buffer[lengthOfHeader + 3 * (x + (((-1) * (width + 1) * y) + ((width + 1) * (height - 1)))) + 2];
                        alpha = 255;
                        oryginalPicturePixels[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                        genPicturePixels[i] = oryginalPicturePixels[i];
                    }
                }*/
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int i = width * y + x;

                        blue = buffer[lengthOfHeader + 3 * (width * (height - 1 - y) + x) + (height - 1 - y) * magic_number];
                        green = buffer[lengthOfHeader + 3 * (width * (height - 1 - y) + x) + 1 + (height - 1 - y) * magic_number];
                        red = buffer[lengthOfHeader + 3 * (width * (height - 1 - y) + x) + 2 + (height - 1 - y) * magic_number];
                        alpha = 255;
                        oryginalPicturePixels[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                        genPicturePixels[i] = oryginalPicturePixels[i];
                    }
                }
                oryginalPictureBitmap.WritePixels(new Int32Rect(0, 0, width, height), oryginalPicturePixels, width * 4, 0);
                mainWindow.imageOriginal.Source = oryginalPictureBitmap;
                if (!(width > 1280 || height > 1000))
                {
                    mainWindow.imageGen.Width = width;
                    mainWindow.imageGen.Height = height;
                }
                else if (!(width > 2560 || height > 2000))
                {
                    mainWindow.imageGen.Width = width / 2;
                    mainWindow.imageGen.Height = height / 2;
                }
                else if (!(width > 5120 || height > 4000))
                {
                    mainWindow.imageGen.Width = width / 4;
                    mainWindow.imageGen.Height = height / 4;
                }
                else if (!(width > 10240 || height > 8000))
                {
                    mainWindow.imageGen.Width = width / 8;
                    mainWindow.imageGen.Height = height / 8;
                }
                UpdateGenImage();
            }
        }
        public static void SavePicture()
        {
            // wybranie pliku przez użytkownika
            SaveFileDialog okienko = new SaveFileDialog();
            okienko.ShowDialog();
            string bufor = okienko.FileName;

            //zapisywanie danych do pliku
            if (!bufor.Equals(""))
            {
                string path = @bufor;
                byte[] buffer = new byte[buffer_imgOrg.Length];
                for (int i = 0; i < buffer_imgOrg.Length; i++)
                    buffer[i] = buffer_imgOrg[i];

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int i = width * y + x;
                        buffer[lengthOfHeader + 3 * (width * (height - 1 - y) + x) + (height - 1 - y) * magic_number] = (byte)(genPicturePixels[i] & 0x0000ff);
                        buffer[lengthOfHeader + 3 * (width * (height - 1 - y) + x) + 1 + (height - 1 - y) * magic_number] = (byte)((genPicturePixels[i] & 0x00ff00) >> 8);
                        buffer[lengthOfHeader + 3 * (width * (height - 1 - y) + x) + 2 + (height - 1 - y) * magic_number] = (byte)((genPicturePixels[i] & 0xff0000) >> 16);
                    }
                }
                //string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                File.WriteAllBytes(path, buffer);
            }
        }
        public static void SavePicture(string path)
        {
            //zapisywanie danych do pliku
            byte[] buffer = new byte[buffer_imgOrg.Length];
            for (int i = 0; i < buffer_imgOrg.Length; i++)
                buffer[i] = buffer_imgOrg[i];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int i = width * y + x;
                    buffer[lengthOfHeader + 3 * (width * (height - 1 - y) + x) + (height - 1 - y) * magic_number] = (byte)(genPicturePixels[i] & 0x0000ff);
                    buffer[lengthOfHeader + 3 * (width * (height - 1 - y) + x) + 1 + (height - 1 - y) * magic_number] = (byte)((genPicturePixels[i] & 0x00ff00) >> 8);
                    buffer[lengthOfHeader + 3 * (width * (height - 1 - y) + x) + 2 + (height - 1 - y) * magic_number] = (byte)((genPicturePixels[i] & 0xff0000) >> 16);
                }
            }
            //string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            File.WriteAllBytes(path, buffer);
        }
        public static void UpdateGenImage()
        {
            genPictureBitmap.WritePixels(new Int32Rect(0, 0, width, height), genPicturePixels, width * 4, 0);
            mainWindow.imageGen.Source = genPictureBitmap;
        }
        public static void ResetGenImg()
        {
            for (int i = 0; i < genPicturePixels.Length; i++)
                genPicturePixels[i] = oryginalPicturePixels[i];
            UpdateGenImage();
        }
        public static void DrawFilledTriangle(Point[] points, uint colour)
        {
            if (points.Length != 3)
                throw new Exception("Trojkat musi skladac sie z 3 punktow !!!");

            //Console.WriteLine("\n\nTrojkat: " + points[0].X + " " + points[0].Y + ";" + points[1].X + " " + points[1].Y + ";" + points[2].X + " " + points[2].Y);

            //prostokątne zakresy 3 krawędzi 
            double[][] zakresy = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                zakresy[i] = new double[4];    //xmin, ymin, xmax, ymax (pozycje na obrazku!!!)
                if (i < 2)
                {
                    if (points[i].X < points[i + 1].X)
                    {
                        zakresy[i][0] = points[i].X;
                        zakresy[i][2] = points[i + 1].X;
                    }
                    else
                    {
                        zakresy[i][0] = points[i + 1].X;
                        zakresy[i][2] = points[i].X;
                    }
                    if (points[i].Y < points[i + 1].Y)
                    {
                        zakresy[i][1] = points[i].Y;
                        zakresy[i][3] = points[i + 1].Y;
                    }
                    else
                    {
                        zakresy[i][1] = points[i + 1].Y;
                        zakresy[i][3] = points[i].Y;
                    }
                }
                else
                {
                    if (points[i].X < points[0].X)
                    {
                        zakresy[i][0] = points[i].X;
                        zakresy[i][2] = points[0].X;
                    }
                    else
                    {
                        zakresy[i][0] = points[0].X;
                        zakresy[i][2] = points[i].X;
                    }
                    if (points[i].Y < points[0].Y)
                    {
                        zakresy[i][1] = points[i].Y;
                        zakresy[i][3] = points[0].Y;
                    }
                    else
                    {
                        zakresy[i][1] = points[0].Y;
                        zakresy[i][3] = points[i].Y;
                    }
                }
            }

            /*for (int i = 0; i < 3; i++)
                Console.WriteLine("Zakresy linii: " + zakresy[i][0] + " " + zakresy[i][1] + " " + zakresy[i][2] + " " + zakresy[i][3]);*/

            //prostokątny zakres trójkąta
            double[] zakresT = { points[0].X, points[0].Y, points[0].X, points[0].Y };  //xmin, ymin, xmax, ymax (pozycje na obrazku!!!)
            for (int i = 1; i < 3; i++)
            {
                if (zakresT[0] > points[i].X)
                    zakresT[0] = points[i].X;
                else if (zakresT[2] < points[i].X)
                    zakresT[2] = points[i].X;
                if (zakresT[1] > points[i].Y)
                    zakresT[1] = points[i].Y;
                else if (zakresT[3] < points[i].Y)
                    zakresT[3] = points[i].Y;
            }

            //Console.WriteLine("Zakres trojkata: " + zakresT[0] + " " + zakresT[1] + ";" + zakresT[2] + " " + zakresT[3]);

            //stworzenie bufora z trójkątem
            int buffor_width = (int)((int)zakresT[2] - (int)zakresT[0] + 1);
            int buffor_height = (int)((int)zakresT[3] - (int)zakresT[1] + 1);
            bool[] buf_triangle_img = new bool[(int)(buffor_width * buffor_height)];

            //Wybór grubości linii (W PRZYSZŁOŚCI MOŻNA DOPRACOWAĆ)
            if (buffor_width * buffor_height > 500)
                lineThickness = 1;
            else// if (buffor_width * buffor_height > 200)
                lineThickness = 1.5;

            //Console.WriteLine("Dlugosc bufora z trojkatem img: " + "w(" + buffor_width + ")*h(" + buffor_height + ") = " + buf_triangle_img.Length);

            //rysowanie linii w buforze
            for (int p = 0; p < 3; p++) //iteracja po 3 parach punktow
            {
                int p1, p2;
                if (p < 2)
                {
                    p1 = p;
                    p2 = p + 1;
                }
                else
                {
                    p1 = p;
                    p2 = 0;
                }
                double a, b, yp, xp, bufmin, bufmax;
                /*Console.WriteLine("ma byc > 0    -> " + ((int)zakresy[p][1] - (int)zakresT[1]) + " - " + ((int)zakresy[p][3] - (int)zakresT[1]));
                Console.WriteLine("ma byc > 0    -> " + ((int)zakresy[p][0] - (int)zakresT[0]) + " - " + ((int)zakresy[p][2] - (int)zakresT[0]));*/
                for (int y = (int)zakresy[p][1] - (int)zakresT[1]; y < (int)zakresy[p][3] - (int)zakresT[1] + 1; y++) //iteracja po calej przestrzeni prostokąta trójkąta
                {
                    for (int x = (int)zakresy[p][0] - (int)zakresT[0]; x < (int)zakresy[p][2] - (int)zakresT[0] + 1; x++)
                    {
                        int i = y * buffor_width + x;
                        if (points[p1].X != points[p2].X) // jesli punkty nie są w linii pionowej
                        {
                            a = (points[p2].Y - points[p1].Y) / (points[p2].X - points[p1].X);
                            b = (-1) * a * points[p1].X + points[p1].Y;

                            yp = a * (x + (int)zakresT[0]) + b;
                            if (Math.Abs((y + (int)zakresT[1]) - yp) < lineThickness)
                                buf_triangle_img[i] = true;

                            xp = ((y + (int)zakresT[1]) - b) / a;
                            if (Math.Abs((x + (int)zakresT[0]) - xp) < lineThickness)
                                buf_triangle_img[i] = true;
                        }
                        else
                        {
                            if (points[p1].Y > points[p2].Y)
                            {
                                bufmin = (int)points[p2].Y - zakresT[1];
                                bufmax = (int)points[p1].Y - zakresT[1];
                            }
                            else
                            {
                                bufmin = (int)points[p1].Y - zakresT[1];
                                bufmax = (int)points[p2].Y - zakresT[1];
                            }
                            //Console.WriteLine(">>>>>>>>   x+offset = " + ((int)(x + zakresT[0])) + ", points[p1].X = " + (int)points[p1].X + ", y = " + y + ", bufmin = " + bufmin + ", bufmax = " + bufmax);
                            //Console.WriteLine(((int)(x + zakresT[0]) == (int)points[p1].X) + "   ;   " + (y >= bufmin) + "   ;   " + (y <= bufmax));
                            if ((int)(x + zakresT[0]) == (int)points[p1].X && y >= bufmin && y <= bufmax)
                                buf_triangle_img[i] = true;
                        }
                    }
                }
            }

            /*Console.WriteLine("\n");
            for (int y = 0; y < buffor_height; y++)
            {
                for (int x = 0; x < buffor_width; x++)
                {
                    int i = y * buffor_width + x;
                    if (buf_triangle_img[i] == true)
                        Console.Write("1");
                    else
                        Console.Write("0");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");*/

            //wypełnienie trójkąta w buforze
            List<int> pr; //przedzialy rysowania
            bool previous, firstTrueFound = false;
            for (int y = 0; y < buffor_height; y++)
            {
                previous = false;
                firstTrueFound = false;
                pr = new List<int>();
                for (int x = 0; x < buffor_width; x++)
                {
                    //Console.WriteLine("TEST XXX * XXX");
                    int i = y * buffor_width + x;
                    if (previous == true && buf_triangle_img[i] == false)
                    {
                        pr.Add(x);
                        firstTrueFound = true;
                    }
                    else if (previous == false && buf_triangle_img[i] == true && firstTrueFound == true)
                    {
                        pr.Add(x - 1);
                        break;
                    }
                    previous = buf_triangle_img[i];
                }
                if (pr.Count % 2 == 1)
                {
                    if (pr.Count > 2)
                        Console.WriteLine(pr.Count + "!!!!! ERRRRRRRRRORRRRRRRRR !!!!!");
                    if (pr.Count == 1)
                        pr = new List<int>();
                    else
                        pr.Add(buffor_width - 1);
                }
                for (int j = 0; j < pr.Count; j += 2)
                {
                    //Console.WriteLine("TEST XXX XXX * XXX XXX");
                    for (int x = pr[j]; x < pr[j + 1] + 1; x++)
                    {
                        int i = y * buffor_width + x;
                        //Console.WriteLine("x = " + x + ", y = " + y + ", i = " + i);
                        buf_triangle_img[i] = true;
                    }
                }
            }

            /*Console.WriteLine("\n");
            for (int y = 0; y < buffor_height; y++)
            {
                for (int x = 0; x < buffor_width; x++)
                {
                    int i = y * buffor_width + x;
                    if (buf_triangle_img[i] == true)
                        Console.Write("1");
                    else
                        Console.Write("0");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");*/

            //narysowanie trójkąta na generowanym obrazku
            for (int y = 0; y < buffor_height; y++)
            {
                for (int x = 0; x < buffor_width; x++)
                {
                    if (buf_triangle_img[y * buffor_width + x] == true)
                    {
                        //if (((int)((y + zakresT[1])) >= height || (x + zakresT[0]) >= width))
                        //Console.WriteLine(((int)(y + zakresT[1])) + " ; " + (x + zakresT[0]));
                        genPicturePixels[(y + (int)zakresT[1]) * width + (x + (int)zakresT[0])] = colour;
                    }
                }
            }
        }
        public static void DrawFilledTriangleInit(Point[] points, int triangleIndex)
        {
            if (points.Length != 3)
                throw new Exception("Trojkat musi skladac sie z 3 punktow !!!");

            //Console.WriteLine("\n\nTrojkat: " + points[0].X + " " + points[0].Y + ";" + points[1].X + " " + points[1].Y + ";" + points[2].X + " " + points[2].Y);

            //prostokątne zakresy 3 krawędzi
            double[][] zakresy = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                zakresy[i] = new double[4];    //xmin, ymin, xmax, ymax
                if (i < 2)
                {
                    if (points[i].X < points[i + 1].X)
                    {
                        zakresy[i][0] = points[i].X;
                        zakresy[i][2] = points[i + 1].X;
                    }
                    else
                    {
                        zakresy[i][0] = points[i + 1].X;
                        zakresy[i][2] = points[i].X;
                    }
                    if (points[i].Y < points[i + 1].Y)
                    {
                        zakresy[i][1] = points[i].Y;
                        zakresy[i][3] = points[i + 1].Y;
                    }
                    else
                    {
                        zakresy[i][1] = points[i + 1].Y;
                        zakresy[i][3] = points[i].Y;
                    }
                }
                else
                {
                    if (points[i].X < points[0].X)
                    {
                        zakresy[i][0] = points[i].X;
                        zakresy[i][2] = points[0].X;
                    }
                    else
                    {
                        zakresy[i][0] = points[0].X;
                        zakresy[i][2] = points[i].X;
                    }
                    if (points[i].Y < points[0].Y)
                    {
                        zakresy[i][1] = points[i].Y;
                        zakresy[i][3] = points[0].Y;
                    }
                    else
                    {
                        zakresy[i][1] = points[0].Y;
                        zakresy[i][3] = points[i].Y;
                    }
                }
            }

            /*for (int i = 0; i < 3; i++)
                Console.WriteLine("Zakresy linii: " + zakresy[i][0] + " " + zakresy[i][1] + " " + zakresy[i][2] + " " + zakresy[i][3]);*/

            //prostokątny zakres trójkąta
            double[] zakresT = { points[0].X, points[0].Y, points[0].X, points[0].Y };  //xmin, ymin, xmax, ymax
            for (int i = 1; i < 3; i++)
            {
                if (zakresT[0] > points[i].X)
                    zakresT[0] = points[i].X;
                else if (zakresT[2] < points[i].X)
                    zakresT[2] = points[i].X;
                if (zakresT[1] > points[i].Y)
                    zakresT[1] = points[i].Y;
                else if (zakresT[3] < points[i].Y)
                    zakresT[3] = points[i].Y;
            }

            //Console.WriteLine("Zakres trojkata: " + zakresT[0] + " " + zakresT[1] + ";" + zakresT[2] + " " + zakresT[3]);

            //stworzenie bufora z trójkątem
            int buffor_width = (int)((int)zakresT[2] - (int)zakresT[0] + 1);
            int buffor_height = (int)((int)zakresT[3] - (int)zakresT[1] + 1);
            bool[] buf_triangle_img = new bool[(int)(buffor_width * buffor_height)];

            //Wybór grubości linii (W PRZYSZŁOŚCI MOŻNA DOPRACOWAĆ)
            if (buffor_width * buffor_height > 500)
                lineThickness = 1;
            else// if (buffor_width * buffor_height > 200)
                lineThickness = 1.5;
            /*else
                lineThickness = 3;*/

            //Console.WriteLine("Dlugosc bufora z trojkatem img: " + "w(" + buffor_width + ")*h(" + buffor_height + ") = " + buf_triangle_img.Length);

            //rysowanie linii w buforze
            for (int p = 0; p < 3; p++) //iteracja po 3 parach punktow
            {
                int p1, p2;
                if (p < 2)
                {
                    p1 = p;
                    p2 = p + 1;
                }
                else
                {
                    p1 = p;
                    p2 = 0;
                }
                double a, b, yp, xp, bufmin, bufmax;
                /*Console.WriteLine("ma byc > 0    -> " + ((int)zakresy[p][1] - (int)zakresT[1]) + " - " + ((int)zakresy[p][3] - (int)zakresT[1]));
                Console.WriteLine("ma byc > 0    -> " + ((int)zakresy[p][0] - (int)zakresT[0]) + " - " + ((int)zakresy[p][2] - (int)zakresT[0]));*/
                for (int y = (int)zakresy[p][1] - (int)zakresT[1]; y < (int)zakresy[p][3] - (int)zakresT[1] + 1; y++) //iteracja po calej przestrzeni prostokąta trójkąta
                {
                    for (int x = (int)zakresy[p][0] - (int)zakresT[0]; x < (int)zakresy[p][2] - (int)zakresT[0] + 1; x++)
                    {
                        int i = y * buffor_width + x;
                        if (points[p1].X != points[p2].X) // jesli punkty nie są w linii pionowej
                        {
                            a = (points[p2].Y - points[p1].Y) / (points[p2].X - points[p1].X);
                            b = (-1) * a * points[p1].X + points[p1].Y;

                            yp = a * (x + (int)zakresT[0]) + b;
                            if (Math.Abs((y + (int)zakresT[1]) - yp) < lineThickness)
                                buf_triangle_img[i] = true;

                            xp = ((y + (int)zakresT[1]) - b) / a;
                            if (Math.Abs((x + (int)zakresT[0]) - xp) < lineThickness)
                                buf_triangle_img[i] = true;
                        }
                        else
                        {
                            if (points[p1].Y > points[p2].Y)
                            {
                                bufmin = (int)points[p2].Y - zakresT[1];
                                bufmax = (int)points[p1].Y - zakresT[1];
                            }
                            else
                            {
                                bufmin = (int)points[p1].Y - zakresT[1];
                                bufmax = (int)points[p2].Y - zakresT[1];
                            }
                            //Console.WriteLine(">>>>>>>>   x+offset = " + ((int)(x + zakresT[0])) + ", points[p1].X = " + (int)points[p1].X + ", y = " + y + ", bufmin = " + bufmin + ", bufmax = " + bufmax);
                            //Console.WriteLine(((int)(x + zakresT[0]) == (int)points[p1].X) + "   ;   " + (y >= bufmin) + "   ;   " + (y <= bufmax));
                            if ((int)(x + zakresT[0]) == (int)points[p1].X && y >= bufmin && y <= bufmax)
                                buf_triangle_img[i] = true;
                        }
                    }
                }
            }

            /*for (int y = 0; y < buffor_height; y++)
            {
                for (int x = 0; x < buffor_width; x++)
                {
                    int i = y * buffor_width + x;
                    if (buf_triangle_img[i] == true)
                        Console.Write("1");
                    else
                        Console.Write("0");
                }
                Console.WriteLine();
            }*/

            //wypełnienie trójkąta w buforze
            List<int> pr; //przedzialy rysowania
            bool previous, firstTrueFound = false;
            for (int y = 0; y < buffor_height; y++)
            {
                previous = false;
                firstTrueFound = false;
                pr = new List<int>();
                for (int x = 0; x < buffor_width; x++)
                {
                    //Console.WriteLine("TEST XXX * XXX");
                    int i = y * buffor_width + x;
                    if (previous == true && buf_triangle_img[i] == false)
                    {
                        pr.Add(x);
                        firstTrueFound = true;
                    }
                    else if (previous == false && buf_triangle_img[i] == true && firstTrueFound == true)
                    {
                        pr.Add(x - 1);
                        break;
                    }
                    previous = buf_triangle_img[i];
                }
                if (pr.Count % 2 == 1)
                {
                    if (pr.Count > 2)
                        Console.WriteLine(pr.Count + "!!!!! ERRRRRRRRRORRRRRRRRR !!!!!");
                    if (pr.Count == 1)
                        pr = new List<int>();
                    else
                        pr.Add(buffor_width - 1);
                }
                for (int j = 0; j < pr.Count; j += 2)
                {
                    //Console.WriteLine("TEST XXX XXX * XXX XXX");
                    for (int x = pr[j]; x < pr[j + 1] + 1; x++)
                    {
                        int i = y * buffor_width + x;
                        //Console.WriteLine("x = " + x + ", y = " + y + ", i = " + i);
                        buf_triangle_img[i] = true;
                    }
                }
            }

            /*for (int y = 0; y < buffor_height; y++)
            {
                for (int x = 0; x < buffor_width; x++)
                {
                    int i = y * buffor_width + x;
                    if (buf_triangle_img[i] == true)
                        Console.Write("1");
                    else
                        Console.Write("0");
                }
                Console.WriteLine();
            }*/

            //RÓŻNICA: START
            uint red = 0;
            uint green = 0;
            uint blue = 0;
            uint ii = 0;

            for (int y = 0; y < buffor_height; y++) //iteracja po calej przestrzeni
            {
                for (int x = 0; x < buffor_width; x++)
                {
                    int i = y * buffor_width + x;
                    if (buf_triangle_img[i] == true)
                    {
                        ii++;
                        int i2 = (y + (int)zakresT[1]) * width + (x + (int)zakresT[0]);
                        red += (byte)((oryginalPicturePixels[i2] & 0xff0000) >> 16);
                        green += (byte)((oryginalPicturePixels[i2] & 0x00ff00) >> 8);
                        blue += (byte)((oryginalPicturePixels[i2] & 0x0000ff));
                    }
                    //Console.Write((x + (int)zakresT[0]) + ",");
                }
                //Console.Write('\n' + (y + (int)zakresT[1]) + "|");
            }
            red /= ii;
            green /= ii;
            blue /= ii;
            uint colour = (((uint)255 << 24) + (red << 16) + (green << 8) + blue);
            GMA.mosaicTriangsColors[triangleIndex] = (red << 16) + (green << 8) + blue;
            //RÓŻNICA: STOP

            //narysowanie trójkąta na generowanym obrazku
            for (int y = 0; y < buffor_height; y++)
            {
                for (int x = 0; x < buffor_width; x++)
                {
                    if (buf_triangle_img[y * buffor_width + x] == true)
                    {
                        //if (((int)((y + zakresT[1])) >= height || (x + zakresT[0]) >= width))
                        //Console.WriteLine(((int)(y + zakresT[1])) + " ; " + (x + zakresT[0]));
                        genPicturePixels[(y + (int)zakresT[1]) * width + (x + (int)zakresT[0])] = colour;
                    }
                }
            }
        }
        public static double evaluationSimilarity()
        {
            double evaluation = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int i = width * y + x;
                    uint blue1 = (genPicturePixels[i] & 0x0000ff);
                    uint green1 = ((genPicturePixels[i] & 0x00ff00) >> 8);
                    uint red1 = ((genPicturePixels[i] & 0xff0000) >> 16);
                    uint blue2 = (oryginalPicturePixels[i] & 0x0000ff);
                    uint green2 = ((oryginalPicturePixels[i] & 0x00ff00) >> 8);
                    uint red2 = ((oryginalPicturePixels[i] & 0xff0000) >> 16);
                    evaluation += ((red1 - red2) * (red1 - red2)) + ((green1 - green2) * (green1 - green2)) + ((blue1 - blue2) * (blue1 - blue2));
                }
            }
            if (evaluation < 0)
                throw new Exception("STOP ITS ILLEGAL !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return evaluation;
        }
    }
}
