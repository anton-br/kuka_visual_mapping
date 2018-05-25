using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using VRepAdapter;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace VRepClient
{
    public partial class Form1 : Form

    {
        public Form1()
        {
            InitializeComponent();
            f1 = this;

            

        }

        public RobotAdapter ra; //экземпляр класса ra - robot adapter
        public Drive RobDrive;
        public SequencePoints SQ;//объект класса sequencePoints
        public Map map;//объект класса Map
        public SearchInGraph SiG;//объект класса поиска по графу
        //все что в абзаце ниже, удалить
        public tacticalLevel TactLevel = new tacticalLevel();
        //  public PotField PotFiel = new PotField();
        public KukaPotField KukaPotField = new KukaPotField();
        public int PotfieldButtonA = 0;//если кнопка нажате то методм PotField доступен
        public int KukaPotButtonB = 0;//если кнопка нажата то работает метод кука.
                                      //  public Bitmap Rob = new Bitmap(@"C:\Users\Илан\Pictures\Robot.jpg");
        public List<Point> ListPoints = new List<Point>();

        public Network net; //объект класса предназначенного для связи с сервером 
        /*enum ErrorCodes
        {
            simx_error_noerror = 0x000000,
            simx_error_novalue_flag = 0x000001,		// input buffer doesn't contain the specified command 
            simx_error_timeout_flag = 0x000002,		//command reply not received in time for simx_opmode_oneshot_wait operation mode 
            simx_error_illegal_opmode_flag = 0x000004,		//command doesn't support the specified operation mode
            simx_error_remote_error_flag = 0x000008,		// command caused an error on the server side 
            simx_error_split_progress_flag = 0x000010,		// previous similar command not yet fully processed (applies to simx_opmode_oneshot_split operation modes) 
            simx_error_local_error_flag = 0x000020,		// command caused an error on the client side //
            simx_error_initialize_error_flag = 0x000040		// simxStart was not yet called //
        };*/

        private void button1_Click(object sender, EventArgs e)
        {
            ra.Init();
        }
        
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        public int counter = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (net != null)
                net.GetNewImage("False");
            PaintEventArgs p = new PaintEventArgs(pictureBox1.CreateGraphics(), pictureBox1.Bounds); //Компонент на котором нужно рисовать и область на которой нужно рисовать
            pictureBox1_Paint(sender, p);
            if (ra is VrepAdapter)
            {
                var vrep = ra as VrepAdapter;

                string Lidar = VRepFunctions.GetStringSignal(vrep.clientID, "Lidar");//str-данные с ледара
                string RobPos = VRepFunctions.GetStringSignal(vrep.clientID, "RobPos");//получение координат робота на сцене Врепа
                vrep.ReceiveLedData(Lidar);
                vrep.ReceiveOdomData(RobPos);
            }


            if (ra != null)
            {
                ra.Send(RobDrive);
            }

            if (RobDrive != null & ra != null && SQ != null)//отправка одометрии в экземпляр класса drive
            {
                if (map.Val == null)
                {
                    map.ImgDistance();
                    //    map.FillNewPerm();
                }

                map.GlobListToGraph(map.GlobalMapList, map.RobOdData);
                float GoalPointX = Convert.ToSingle(textBox8.Text);
                float GoalPointY = Convert.ToSingle(textBox9.Text);

                Point start = new Point((int)(ra.RobotOdomData[0] * 10 + map.Xmax / 2), (int)(ra.RobotOdomData[1] * 10 + map.Ymax / 2));
                Point goal = new Point((int)GoalPointX * 10 + map.Xmax / 2, (int)GoalPointY * 10 + map.Ymax / 2);
                
                ListPoints = SiG.FindPath(map.graph, start, goal);


                if (ListPoints != null)
                {
                    SQ.GetNextPoint(ListPoints, ra.RobotOdomData[0], ra.RobotOdomData[1], ra.RobotOdomData[2], map.Xmax, map.Ymax);
                    RobDrive.GetDrive(ra.RobotOdomData[0], ra.RobotOdomData[1], ra.RobotOdomData[2], SQ.CurrentPointX, SQ.CurrentPointY, map.Xmax, map.Ymax);
                    ra.Send(RobDrive);
                }

                if (net.PythonIsReady())
                {
                    net.GetNewImage("True", map.GlobalValString, map.GlobalOdomString);// отправляем глобальные координаты для всех точек
                    map.GlobalOdomString = "";
                    map.FillNewVal(net.GetNewVal());// обновляем массив Val новыми значениями проходимости
                    net.GetNewImage("False");
                }

                map.LedDataToList(map.Val, ra.RobotOdomData); // обновление массива GlobalMapList 
            }
            if (ra != null & RobDrive != null)//вывод переменных из Робот Адаптера на форму
            {
                string OutOdomData = "";
                for (int i = 0; i < ra.RobotOdomData.Length; i++)
                {
                    OutOdomData = OutOdomData + ra.RobotOdomData[i] + "; ";
                }
                richTextBox2.Text = OutOdomData;
                if (RobDrive != null)
                {
                    textBox2.Text = RobDrive.Phi.ToString();
                    textBox2.Invalidate();
                    textBox3.Text = RobDrive.RobotDirection.ToString();
                    textBox3.Invalidate();
                    textBox4.Text = RobDrive.TargetDirection.ToString();
                    textBox4.Invalidate();
                    textBox5.Text = RobDrive.DistToTarget.ToString();
                    textBox5.Invalidate();
                }
            }



        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ra != null)
            {
                ra.Deactivate();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            PotfieldButtonA = 1;

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public static Form1 f1;

        private void bt_tcp_test_Click(object sender, EventArgs e)
        {
            if (ra is YoubotAdapter)
            {
                var ya = ra as YoubotAdapter;
                ya.TCPconnect(tb_ip.Text);
            }
            else { MessageBox.Show("вы работаете с Врепом, а не с реальным роботом!"); }
        }

        public void ShowLedData(string s)
        {
            rtb_tcp.Invoke(new Action(() => rtb_tcp.Text = s));
        }
        public void ShowOdomData(string s)
        {
            rtb_tcp2.Invoke(new Action(() => rtb_tcp2.Text = s));
        }

        private void btsend_Click(object sender, EventArgs e)
        {
            //     if (tc == null) 
            //    {

            //        return;
            //     }
            //     tc.Send(rtb_send.Text); 
        }

        private void rtb_send_TextChanged(object sender, EventArgs e)
        {

        }

        private void KukaPotButton_Click(object sender, EventArgs e)
        {

        }

        private void VrepAdapter_Click(object sender, EventArgs e)
        {
            ra = new VrepAdapter();

        }

        private void YoubotAdapter_Click(object sender, EventArgs e)
        {
            ra = new YoubotAdapter();
            net = new Network();
        }

        private void Drive_Click(object sender, EventArgs e)
        {
            SQ = new SequencePoints();
            RobDrive = new Drive();
            map = new Map();
            SiG = new SearchInGraph();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void rtb_tcp_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (ra != null && SQ != null && Drive != null && map != null && map.graph != null)
            {

                int yy = 0;
                int xx = 0;
                //  this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
                //  if (10==timer1.Tick) { }               
                /*

                     for (int l = 0; l < map.Ymax + 1; l++)//отрисовываем сетку
                     {
                         e.Graphics.DrawLine(new Pen(Color.Black), 0, yy, pictureBox1.Width, yy);
                         // xx = xx + 50;
                         yy = yy + pictureBox1.Height / map.Ymax;
                     }
                     for (int l = 0; l < map.Xmax + 1; l++)
                     {
                         e.Graphics.DrawLine(new Pen(Color.Black), xx, 0, xx, pictureBox1.Height);
                         xx = xx + pictureBox1.Width / map.Xmax;
                     }
                     System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);

            */

                int MapWidth = map.Xmax;
                int MapHeight = map.Ymax;
                int CellSize = 4;
                // Размер карты в пискелях.
                int mapWidthPxls = MapWidth * (CellSize + 1) + 1,
                    mapHeightPxls = MapHeight * (CellSize + 1) + 1;
                Bitmap mapImg = new Bitmap(mapWidthPxls, mapHeightPxls);
                Graphics g = Graphics.FromImage(mapImg);

                // Заливаем весь битмап:
                g.Clear(Color.White);

                // Рисуем сетку:
                for (int x = 0; x <= MapWidth; x++)
                    g.DrawLine(Pens.LightGray, x * (CellSize + 1), 0, x * (CellSize + 1), mapHeightPxls);
                for (int y = 0; y <= MapHeight; y++)
                    g.DrawLine(Pens.LightGray, 0, y * (CellSize + 1), mapWidthPxls, y * (CellSize + 1));
                PictureBox p = pictureBox1;
                if (p.Image != null)
                    p.Image.Dispose();

                pictureBox1.Image = mapImg;
                g.Dispose();


                for (int i = 0; i < map.Xmax; i++) //закрашиваем ячейки с препядствиями
                {
                    for (int k = 0; k < map.Ymax; k++)
                    {
                        Color col = Color.Blue;
                        if (map.graph[i, k] == 2)
                            col = Color.Blue;
                        //else if (map.graph[i, k] == 3)
                        //    col = Color.GreenYellow;
                        //else if (map.graph[i, k] == 4)
                        //    col = Color.Yellow;
                        //else if (map.graph[i, k] == 5)
                        //    col = Color.Tomato;
                        if (map.graph[i, k] != 1)
                        {
                            int H = CellSize + 1;//(int)(pictureBox1.Height / map.Ymax);
                            int W = CellSize + 1;//(int)(pictureBox1.Width / map.Xmax);

                            SolidBrush blueBrush = new SolidBrush(col);
                            Rectangle rect = new Rectangle((i) * W, pictureBox1.Height + ((-1) * k * H), W, H);
                            e.Graphics.FillRectangle(blueBrush, rect);
                        }
                        /*
                        if (map.graph[i, k] ==0 )//закрашиваем пустые ячейки прозрачным цветом
                        {
                            int H = (int)(pictureBox1.Height / map.Ymax);
                            int W = (int)(pictureBox1.Width / map.Xmax);

                            Color brushColor = Color.FromArgb(250 / 100 * 0, 255, 0, 0);
                            SolidBrush blueBrush = new SolidBrush(brushColor);
                            // Create rectangle.//ниже путаница со знаками, по Иксу двигается а по У нет
                            Rectangle rect = new Rectangle((i) * W, pictureBox1.Height + ((-1) * k * H), W, H);

                            // Fill rectangle to screen.
                            e.Graphics.FillRectangle(blueBrush, rect);

                        }*/

                    }
                }
                if (ListPoints != null)//ресуем получившийся маршрут
                {
                    for (int i = 0; i < ListPoints.Count; i++)
                    {
                        int H = CellSize + 1; //(int)(pictureBox1.Height / map.Ymax);
                        int W = CellSize + 1;//(int)(pictureBox1.Width / map.Xmax);

                        SolidBrush blueBrush = new SolidBrush(Color.Red);
                        SolidBrush greenBrush = new SolidBrush(Color.Green);
                        Rectangle rect = new Rectangle((ListPoints[i].X) * W, pictureBox1.Height + ((-1) * ListPoints[i].Y * H), W, H);
                        Rectangle rectCurrentPoint = new Rectangle(((int)SQ.CurrentPointX) * W, pictureBox1.Height + ((-1) * (int)SQ.CurrentPointY * H), W, H);

                        e.Graphics.FillRectangle(blueBrush, rect);
                        e.Graphics.FillRectangle(greenBrush, rectCurrentPoint);
                    }

                }
                int H2 = CellSize + 1;// (int)(pictureBox1.Height / map.Ymax);
                int W2 = CellSize + 1; //(int)(pictureBox1.Width / map.Xmax);
                Point start = new Point((int)(ra.RobotOdomData[0] * 10 + map.Xmax / 2), (int)(ra.RobotOdomData[1] * 10 + map.Ymax / 2));
                e.Graphics.DrawEllipse(Pens.Chocolate, (int)start.X * W2 - 2 * W2, pictureBox1.Height + ((-1) * (int)start.Y) * H2 - 2 * W2, 20, 20);

            }
            //  e.Graphics.Clear(Color.Teal);
            // e.Graphics.Clear();
            if (map != null && map.invalidateform == true)//обновляем форму
            {
                pictureBox1.Invalidate();//вызов отрисовки на пикчербоксе перенести в более логичное мето
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }



        public Image Rob { get; set; }

        private void tb_ip_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
