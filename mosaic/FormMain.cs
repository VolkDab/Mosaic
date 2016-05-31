//by Vlasov Andrey (andrew.vlasof@yandex.ru)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mosaic
{
    // Класс главной формы, размещает на себе элементы
    // управления - панель, прямоугольники PictureBox и возможно другие.
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            
        }

        // Массив объектов прямоугольников для хранения сегментов картинки.
        PictureBox[] PB = null;
        bool canMove = false;
        // Длина стороны в прямоугольниках.
        int LengthSides = 3;
        // Объект хранения картинки.
        Bitmap Picture = null;
        Point p;
        PictureBox pic;
        

        // Создание области рисования картинки, для удобства определения ее размеров
        // прямоугольники массива PB размещаются на панели panel1.
        void CreatePictureRegion()
        {
            // Удалим предыдущий массив, чтобы создать новый.
            if (PB != null)
            {
                for (int i = 0; i < PB.Length; i++)
                {
                    PB[i].Dispose();
                }
                PB = null;
            }
            
            
            int num = LengthSides;
            // Создаем массив прямоугольников размером LengthSides Х LengthSides.
            PB = new PictureBox[num * num];
            
            // Вычислим габаритные размеры прямоугольников.
            int w = panel1.Width / num;
            int h = panel1.Height / num;

            int countX = 0; // счетчик прямоугольников по координате Х в одном ряду
            int countY = 0; // счетчик прямоуголников по координате Y в одном столбце
            for (int i = 0; i < PB.Length; i++)
            {
                PB[i] = new PictureBox(); // непосредственное создание прямоугольника, элемента массива

                // Размеры и координаты размещения созданного прямоугольника.
                PB[i].Width = w;
                PB[i].Height = h;
                PB[i].Left = 0 + countX * PB[i].Width;
                PB[i].Top = 0 + countY * PB[i].Height;

                // Запомним начальные координаты прямоугольника для
                // восстановления перемешанной картинки,
                // определения полной сборки картинки.
                Point pt = new Point();
                pt.X = PB[i].Left;
                pt.Y = PB[i].Top;
                PB[i].Tag = pt; // сохраним координаты в свойстве Tag каждого прямоугольника

                // Считаем прямоугольники по рядам и столбцам.
                countX++;
                if (countX == num)
                {
                    countX = 0;
                    countY++;
                }


                PB[i].Parent = panel1; // разместим прямоугольники на панели
                PB[i].BorderStyle = BorderStyle.None;
                PB[i].SizeMode = PictureBoxSizeMode.StretchImage; // размеры картинки будут подгоняться под размеры прямоугольника
                PB[i].Show(); // гарантия видимости прямоугольника

                // Для всех прямоугольников массива событие клика мыши
                // будет обрабатываться в одной и той же функции, для удобства
                // вычисления координат прямоугольников в одном месте.
                PB[i].MouseMove += new MouseEventHandler(PB_Move);
                PB[i].MouseDown += new MouseEventHandler(PB_MouseDown);
                PB[i].MouseUp += new MouseEventHandler(PB_MouseUp);
            }

            // Раскидываем картинку на сегменты и рисуем каждый сегмент
            // на своем прямоугольнике.
            DrawPicture();
            
        }

        void DrawPicture()
        {
            if (Picture == null) return;
            int countX = 0;
            int countY = 0;
            int num = LengthSides;
            for (int i = 0; i < PB.Length; i++)
            {
                int w = Picture.Width / num;
                int h = Picture.Height / num;
                PB[i].Image = Picture.Clone(new RectangleF(countX * w, countY * h, w, h), Picture.PixelFormat);
                countX++;
                if (countX == LengthSides)
                {
                    countX = 0;
                    countY++;
                }

            }
        }
        //Нажата кнопка мыши
        void PB_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
          
    

            if (pb.Name != "Static" )
            {
                canMove = true;
                pb.BringToFront();
                p = pb.Location;
            }

      
        }
        //Отпущена кнопка мыши
        void PB_MouseUp(object sender, MouseEventArgs e)
        {
            canMove = false;
            for (int j = 0; j < PB.Length; j++)
            {
                Point point = (Point)PB[j].Tag;
                if (PB[j].Location == point & PB[j].Name!="Static")
                {
                    PB[j].Name = "Static";
                    
                    PB[j].BorderStyle = BorderStyle.None;
                    
                }
            }

            for (int m = 0; m < PB.Length; m++)
            {
                if (PB[m].Name != "Static") return;
            }

               MessageBox.Show("Готово!!");
               
               
            
        }
        //Перемещение фрагмента
        void PB_Move(object sender, MouseEventArgs e)
        {
            if (canMove)
            {
                pic = (PictureBox)sender;
                pic.Top += e.Y ;
                pic.Left += e.X;
                Point point = (Point)pic.Tag;
                if(pic.Location == point)
                {
                    pic.Name = "Static";
                    pic.BorderStyle = BorderStyle.None;
                    
                    
                    canMove = false;
                }
            }
        }

        // Открытие диалогового окна выбора файла и создание новой области прорисовки картинки.
        private void toolStripButtonLoadPicture_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofDlg = new OpenFileDialog();
            // Фильтр показа только файлов с определенным расширением.
            ofDlg.Filter = "файлы картинок (*.bmp;*.jpg;*.jpeg;)|*.bmp;*.jpg;.jpeg|All files (*.*)|*.*";
            ofDlg.FilterIndex = 1;
            ofDlg.RestoreDirectory = true;

            if (ofDlg.ShowDialog() == DialogResult.OK)
            {
                // Загружаем выбранную картинку.
                Picture = new Bitmap(ofDlg.FileName);
                // Создаем новую область прорисовки.
                CreatePictureRegion();
            }
        }

        // Перемешивание прямоугольников, хаотично меняем их координаты.
        private void toolStripButtonMixed_Click(object sender, EventArgs e)
        {
            if (Picture == null) return;

            // Создаем объект генерирования псевослучайных чисел,
            // для различного набора случайных чисел инициализацию
            // объекта Random производим от счетчика количества
            // миллисекунд прошедших со времени запуска операционной системы.
            Random rand = new Random(Environment.TickCount);
            int r = 0;
            for (int i = 0; i < PB.Length; i++)
            {
                PB[i].Visible = true;
                r = rand.Next(0, PB.Length);
                Point ptR = PB[r].Location;
                Point ptI = PB[i].Location;
                PB[i].Location = ptR;
                PB[r].Location = ptI;
                PB[i].BorderStyle = BorderStyle.FixedSingle;
            }

            // Случайным образом выбираем пустой прямоугольник,
            // делаем его невидимым.
            r = rand.Next(0, PB.Length);
           
           
        }


        // В каждом прямоугольнике будет хранится соответствующий
        // сегмент картинки.
        

        // Восстанавливаем картинку соответсвенно первичным координатам.
        private void toolStripButtonRestore_Click(object sender, EventArgs e)
        {
            if (Picture == null) return;
            for (int i = 0; i < PB.Length; i++)
            {
                Point pt = (Point)PB[i].Tag;
                PB[i].Location = pt;
                PB[i].Visible = true;
            }
        }

        // Открываем диалоговое окно настроек приложения.
        private void toolStripButtonSetting_Click(object sender, EventArgs e)
        {
            SetDlg setDlg = new SetDlg();
            setDlg.LengthSides = LengthSides;
            if (setDlg.ShowDialog() == DialogResult.OK)
            {
                LengthSides = setDlg.LengthSides;

                
                CreatePictureRegion();
            }
        }

        // Открываем диалоговое окно с нормальным видо картинки,
        // для освежения памяти пользователя.
        private void toolStripButtonHelp_Click(object sender, EventArgs e)
        {
            HelpDlg helpDlg = new HelpDlg();
            helpDlg.ImageDuplicate = Picture;
            helpDlg.ShowDialog();
        }



        
    }
}
