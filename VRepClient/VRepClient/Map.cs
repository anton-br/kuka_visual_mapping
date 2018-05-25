using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
namespace VRepClient
{
    public class ObstaclesPoint
    {
        public float X;
        public float Y;
        public float weight = 2f;
    }
    public class Map
    {
        public List<ObstaclesPoint> GlobalMapList = new List<ObstaclesPoint>();// x, y
        public Dictionary<List<int>, float> GlobalOdomDict;// словарь с реальными значениями проходимости
        public float[] RobOdData; //только для вывода на форму картинки с роботом
        public bool invalidateform = false;
        public float[,,] Val; // локальные расстояния до каждого пикселя камеры
        public float[,,] GlobalVal; // глобальные расстояния до каждого пикслея камеры
        public string GlobalValString; // те же координаты, только в строке, для создания пост запроста
        public string GlobalOdomString; // реальные значения проходимости в строке для пост запроса
        public Map()
        {
            GlobalOdomDict = new Dictionary<List<int>, float>();
            GlobalOdomString = "";

        }
        public void ImgDistance()
        {
            //string progToRun = "C:\\Users\\Anton\\Desktop\\diplom\\python\\module.py";// path to file with calculated coordinates of every pixel on the image

            //Process proc = new Process();
            //proc.StartInfo.FileName = "C:\\Users\\Anton\\Anaconda3\\python.exe";
            //proc.StartInfo.UseShellExecute = false;
            //proc.StartInfo.RedirectStandardError = false;
            //proc.StartInfo.RedirectStandardOutput = false;
            //proc.StartInfo.CreateNoWindow = true;

            //string startParams = "{'dist_y':21.5,'dist_x':27,'pixx':0.0,'pixy':0.0}";
            //string endParams = "{'dist_y':153.5,'dist_x':86.75,'pixx':0.0}";
            string shape = "28,28,3";
            string SavePath = "C:\\Users\\Anton\\Desktop\\diplom\\python";
            //proc.StartInfo.Arguments = string.Concat(progToRun + " -s " + shape + " -n " + startParams + " -f " + endParams + " -p " + SavePath);
            //proc.Start();
            //proc.WaitForExit();

            string text = System.IO.File.ReadAllText(SavePath + "\\dist_map.txt");
            string[] mass = text.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int x = Convert.ToInt32(shape.Split(',')[0]);
            int y = Convert.ToInt32(shape.Split(',')[1]);
            Val = new float[x, y, 3];

            int m = 0;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                    for (int k = 0; k < 2; k++)
                        Val[i, j, k] = float.Parse(mass[m++], CultureInfo.InvariantCulture);
            }
        }

        //public void FillNewPerm()
        //{
        //    //string progToRun = "C:\\Users\\Anton\\Desktop\\diplom\\python\\trees.py";// path to file with calculated coordinates of every pixel on the image

        //    //Process proc = new Process();
        //    //proc.StartInfo.FileName = "C:\\Users\\Anton\\Anaconda3\\python.exe";
        //    //proc.StartInfo.UseShellExecute = false;
        //    //proc.StartInfo.RedirectStandardOutput = true;
        //    //proc.StartInfo.CreateNoWindow = true;

        //    //string train = " -t False", robot_num=" -r 25", width=" -w 224", height=" -m 224", quality=" -q 150";
        //    //string path = " -i C:\\Users\\Anton\\Desktop\\diplom\\python\\dots_224.png";
        //    ////proc.StartInfo.Arguments = string.Concat(" -W ignore " + progToRun + train + robot_num + width + height + quality);

        //    //proc.StartInfo.Arguments = string.Concat(" -W ignore " + progToRun + train + path);
        //    //proc.Start();
        //    //string perm = proc.StandardOutput.ReadToEnd();
        //    //proc.WaitForExit();

        //    //string pattern = @"[^0-9 ]";

        //    //Regex regex = new Regex(pattern);
        //    //Match match = regex.Match(perm);


        //    //string replacement1 = "";    

        //    //string[] perm_mass = Regex.Replace(perm, pattern, replacement1).Split(' ');

        //}

        public void FillNewVal(string[] perm_mass)
        {
            if (perm_mass.Length == 0)
            {
                for (int i = 0; i < Val.GetLength(0); i++)
                {
                    for (int j = 0; j < Val.GetLength(1); j++)
                        Val[i, j, 2] = 1;
                }
                return;
            }
            int m = 0;
            for (int i = 0; i < Val.GetLength(0); i++)
            {
                for (int j = 0; j < Val.GetLength(1); j++)
                    Val[i, j, 2] = int.Parse(perm_mass[m++], CultureInfo.InvariantCulture);
            }
        }

        public void LedDataToList(float[,,] LedData, float[] OdomData)
        // LegData - массив в котором приходят расстояния в метрах , !!!OdomData! (координаты робота по оси х,y и угол поворота в радианах, по дефолту 0)
        //тут должен изменять координаты каждой точки с локальных на глобальные, потом из получившегося листа будет формироваться граф
        {
            RobOdData = new float[OdomData.Length];
            RobOdData[0] = OdomData[0]; RobOdData[1] = OdomData[1]; RobOdData[2] = OdomData[2];//строчка только для вывода перемеще
            GlobalVal = new float[LedData.GetLength(0), LedData.GetLength(1), 3];//массив с координатами точек относительно робота//убрать коменты
            //float A = 180f / LedData.Length;//угол между данными с ледара
            //float x; // угол между катетом и гипотенузой
            //float c; //длина гипотенузы
            //float u = 1;//для умножения X на -1 когда x становится больше 90 градусов
            //OdomData[2] = (float)0.5;
            GlobalValString = "";
            for (int i = 0; i < LedData.GetLength(0); i++)
            {
                for (int j = 0; j < LedData.GetLength(1); j++)
                {
                    float LedRX = (float)Math.Cos(OdomData[2]);
                    float LedRY = (float)Math.Sin(OdomData[2]);
                    //ledData[x,y] = [y,x,w]!
                    //ledDataMass[x,y] = [x,y,w]!!
                    float x = LedData[i, j, 0] / 100;
                    float y = LedData[i, j, 1] / 100;
                    float calc_x = (x * LedRX + y * LedRY) + OdomData[0];
                    float calc_y = (-x * LedRY + y * LedRX) + OdomData[1] + 0.1f;
                    GlobalValString += (int)Math.Round(calc_x * 10) + " " + (int)Math.Round(calc_y * 10) + "; ";
                    GlobalVal[i, j, 0] = calc_x;
                    GlobalVal[i, j, 1] = calc_y;
                    GlobalVal[i, j, 2] = LedData[i, j, 2];
                }
            }
            List<int> Coordinates = new List<int>();
            Coordinates.Add((int)Math.Round(OdomData[0] * 10));
            Coordinates.Add((int)Math.Round(OdomData[1] * 10));
            if (!GlobalOdomDict.ContainsKey(Coordinates))
            {
                GlobalOdomDict[Coordinates] = OdomData[3];
                GlobalOdomString += Coordinates[0] + " " + Coordinates[1] + " " + OdomData[3] + ";";
            }
            //using (StreamWriter sw = File.AppendText("C:\\Users\\Anton\\Desktop\\diplom\\python\\OdomData.txt"))
            //{
            //    sw.WriteLine(GlobalOdomDict.Last().Key[0] + " " + GlobalOdomDict.Last().Key[1] + " " + GlobalOdomDict.Last().Value);
            //}
            GlobalMapList.Add(new ObstaclesPoint { X = 0f, Y = 0f, weight = 1 });//задаю две точки по умолчанию с обеих сторон от робота



            float Xpel;
            float Ypel;
            float DistBetweenPoints = 0;
            //Random r = new Random(1);
            //int[] rand = new int[LedData.GetLength(0) * LedData.GetLength(1)];
            //for (int i = 0; i < LedData.GetLength(0) * LedData.GetLength(1); i++)
            //{
            //    rand[i] = r.Next(1, 6);
            //}
            int s = 0;
            for (int g = 0; g < LedData.GetLength(0); g++)
            {
                for (int k = 0; k < LedData.GetLength(1); k++, s++)
                {
                    //float radius = 0;
                    // что происходит тут? зачем тебе нужны кратчайшие точки?
                    float h = 4;//для запомнания кратчайшей точки в цикле
                                //for (int i = 0; i < GlobalMapList.Count; i++)
                                //{
                                //    Xpel = LedDataMass[g, 0] - GlobalMapList[i].X;
                                //    Ypel = LedDataMass[g, 1] - GlobalMapList[i].Y;
                                //    DistBetweenPoints = (float)Math.Abs(Math.Sqrt(Xpel * Xpel + Ypel * Ypel));
                                //    // radius = (float)(Math.Sqrt(LedDataMass[g, 0] * LedDataMass[g, 0] + LedDataMass[g, 1] * LedDataMass[g, 1]));
                                //    //radius = LedData[g];// для отсеивания точек дальше 4-ех метров
                                //    if (DistBetweenPoints < h)
                                //    {
                                //        h = DistBetweenPoints;
                                //    }

                    //if (h > 0.01 && radius < 2 && i == GlobalMapList.Count - 1 && radius > 0.2)//было 0.01
                    //{
                    // добовлять только те вершины, в которых вес не равен 1, потому что в графе все веса по дефолту равны 1

                    GlobalMapList.Add(new ObstaclesPoint { X = GlobalVal[k, g, 0], Y = GlobalVal[k, g, 1], weight = GlobalVal[k, g, 2]});//LedDataMass[g, k, 2] });
                                                                                                                                          //ЗАКОМЕНЧЕНО ЧТОБЫ РОБОТ НЕ ЗАПОМИНАЛ ПРЕПЯТСТВИЯ 25.10.2016
                                                                                                                                          //}



                }
            }
            //filterGlobalMapList(GlobalMapList, LedData, OdomData, LedDataMass);//отправка листа в функцию на отфильтровывание точек    

        }

        void filterGlobalMapList(List<ObstaclesPoint> GlobalMapList, float[] LedData, float[] OdomData, float[,] LedDataMass)
        {
            for (int i = 0; i < GlobalMapList.Count; i++)
            {
                for (int g = 0; g < LedData.Length; g++)
                {
                    float Tg = (float)Math.Atan(GlobalMapList[i].X / GlobalMapList[i].Y);
                    float Tg2 = (float)Math.Atan(LedDataMass[g, 0] / LedDataMass[g, 1]);
                    float dx = Math.Abs(GlobalMapList[i].X) - Math.Abs(LedDataMass[g, 0]);
                    float dy = Math.Abs(GlobalMapList[i].Y) - Math.Abs(LedDataMass[g, 1]);
                    float da = (float)Math.Abs(Tg - Tg2);

                    float Xpel = GlobalMapList[i].X - OdomData[0];
                    float Ypel = GlobalMapList[i].Y - OdomData[1];
                    float TargetonPoint = (float)Math.Atan2(Xpel, Ypel);
                    float TT = (float)Math.Abs(TargetonPoint - OdomData[2]);

                    float XpelP = LedDataMass[g, 0] - GlobalMapList[i].X;//для рсчета расстояния между точками
                    float YpelP = LedDataMass[g, 1] - GlobalMapList[i].Y;
                    float DistBetweenPoints = (float)Math.Abs(Math.Sqrt(XpelP * XpelP + YpelP * YpelP));

                    if (LedData[g] < 2 && DistBetweenPoints > 0.1 && da < 0.005 && TT < 1.5) //проверка если старая точка заслоняется новой то ее удалить
                    {
                        // GlobalMapList.RemoveAt(i);//закоментированно удаление несуществующих точек, временно
                        //   break;
                    }

                    //вся эта функция неправльная, она должна быть привязана к точке листа i а не к g
                    if (LedData[g] > 2.1 && da < 0.002 && TT < 1.2) //если в данном направвлении точек нет то существующие удалить
                    {
                        //  GlobalMapList.RemoveAt(i);//эта функция почемуто удаляет точки при вращении
                        //  break;
                    }
                }
            }

        }

        // public float[,] graph;
        public float[,] graph;
        public int Ymax = 180;
        public int Xmax = 180;
        int oldXmax = 0;
        int oldX = 0; int oldY = 0;

        public void GlobListToGraph(List<ObstaclesPoint> GlobalMapList, float[] OdomData)//метод для перевода листа в матрицу
        {

            graph = new float[Xmax, Ymax];//матрица которая является взвешенным графом
            int ymatrix = 0;
            int xmatrix = 0;
            bool key1 = false;
            bool key2 = false;
            for (int k = 0; k < Xmax; k++)
            {
                for (int k2 = 0; k2 < Ymax; k2++)
                {
                    graph[k, k2] = 1;
                }
            }

            for (int i = 0; i < GlobalMapList.Count; i++)
            {
                if (GlobalMapList[i].weight != 1)
                {
                    float Tx = GlobalMapList[i].X * 10;
                    float Ty = GlobalMapList[i].Y * 10;
                    xmatrix = (int)Math.Floor(Tx);
                    ymatrix = (int)Math.Floor(Ty);

                    graph[xmatrix + Xmax / 2, ymatrix + Ymax / 2] = GlobalMapList[i].weight;
                    //if (GlobalMapList[i].weight == 2)
                    //{
                    //    graph[xmatrix + 1 + Xmax / 2, ymatrix + Ymax / 2] = GlobalMapList[i].weight;
                    //    graph[xmatrix + Xmax / 2, ymatrix + 1 + Ymax / 2] = GlobalMapList[i].weight;
                    //    graph[xmatrix - 1 + Xmax / 2, ymatrix + Ymax / 2] = GlobalMapList[i].weight;
                    //    graph[xmatrix + Xmax / 2, ymatrix - 1 + Ymax / 2] = GlobalMapList[i].weight;
                    //    graph[xmatrix + 1 + Xmax / 2, ymatrix + 1 + Ymax / 2] = GlobalMapList[i].weight;
                    //    graph[xmatrix - 1 + Xmax / 2, ymatrix - 1 + Ymax / 2] = GlobalMapList[i].weight;
                    //    graph[xmatrix - 1 + Xmax / 2, ymatrix + 1 + Ymax / 2] = GlobalMapList[i].weight;
                    //    graph[xmatrix + 1 + Xmax / 2, ymatrix - 1 + Ymax / 2] = GlobalMapList[i].weight;
                    //}
                }
            }

        }

    }
}
