using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace TestWorkWpf
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        //вертикальная картинка
        BitmapSource BSTop;
        //горизонтальная картинка
        BitmapSource BSLeft;
        //поле
        Field F;
        //счетчик ходов пользователя
        int step;
        //флаг победы
        bool victory = false;
        //таймер для отрисовки
        System.Windows.Threading.DispatcherTimer T;
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //преобразуем картинку из Resources к source
            System.Drawing.Bitmap top = Properties.Resources.top;
            BSTop = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                top.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            System.Drawing.Bitmap left = Properties.Resources.left;
            BSLeft = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                left.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            T = new System.Windows.Threading.DispatcherTimer();
            T.Interval = new TimeSpan(0, 0, 0, 0, 50);
            T.Tick += new EventHandler(T_Tick);
        }

        void T_Tick(object sender, EventArgs e)
        {
            DrawField();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //инициализация
            victory = false;
            //ограничение размера поля
            uint size;
            try
            {
                size = Convert.ToUInt32(TextBoxSizeField.Text);
            }
            catch {
                size = 2;
                TextBoxSizeField.Text = size.ToString();
            }
            if (size > 20)
            {
                size = 20;
                TextBoxSizeField.Text = size.ToString();
            }
            else if (size < 2)
            {
                size = 2;
                TextBoxSizeField.Text = size.ToString();
            }
            F = new Field((int)size);
            TextBlockVictory.Text = "";
            //запускаем игру
            Init();
        }

        public void Init()
        {
            GridFieldImage.Children.Clear();
            GridFieldImage.RowDefinitions.Clear();
            GridFieldImage.ColumnDefinitions.Clear();
            //генерируем поле для картинок
            for (int i = 0; i < F.count; i++)
            {
                GridFieldImage.ColumnDefinitions.Add(new ColumnDefinition());
                GridFieldImage.RowDefinitions.Add(new RowDefinition());
                for (int j = 0; j < F.count; j++)
                {

                    Image I = new Image();
                    if (F.mas[i][j] == 0)
                    {
                        I.Source = BSTop;
                    }
                    else
                    {
                        I.Source = BSLeft;
                    }
                    I.MouseDown += new MouseButtonEventHandler(I_MouseDown);
                    GridFieldImage.Children.Add(I);
                    Grid.SetRow(I, i);
                    Grid.SetColumn(I, j);
                }
            }
            step = 0;
            TextBoxCountStep.Text = step.ToString();
            if (!T.IsEnabled)
            {
                T.Start();
            }
        }

        void I_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!victory)
            {
                //смотрим индекс у картинки
                var I = (Image)sender;
                int y = Grid.GetRow(I);
                int x = Grid.GetColumn(I);
                //делаем поворот
                bool rez = F.Rotation(x, y);
                //увеличиваем ход
                step++;
                TextBoxCountStep.Text = step.ToString();
                DrawField();
                //проверяем поле на решение
                if (rez)
                {
                    victory = true;
                    TextBlockVictory.Text = "Победа!";
                }
                else
                {
                    TextBlockVictory.Text = "Мимо";
                }
            }
        }

        //метод который рисует поле
        public void DrawField()
        {
            int i = 0, j = -1;
            //просматриваем картинки, которые находятся в гриде
            foreach (var im in GridFieldImage.Children)
            {
                //считаем индексы
                j++;
                if (j == F.count)
                {
                    j = 0;
                    i++;
                }
                if (i == F.count) i = 0;
                //если состояние изменилось
                if (F.state[i][j] != 0)
                {
                    //поворачиваем картинки
                    if (F.mas[i][j] == 0)
                    {
                        F.state[i][j]--;
                        ((Image)im).Source = BSTop;
                        ((Image)im).RenderTransformOrigin = new Point(0.5,0.5);
                        RotateTransform rt = new RotateTransform()
                        {
                            Angle = F.state[i][j] * 9
                        };
                        ((Image)im).RenderTransform = rt;
                    }
                    else
                    {
                        F.state[i][j]++;
                        ((Image)im).Source = BSLeft;
                        ((Image)im).RenderTransformOrigin = new Point(0.5, 0.5);
                        RotateTransform rt = new RotateTransform()
                        {
                            Angle = F.state[i][j] * 9
                        };
                        ((Image)im).RenderTransform = rt;
                    }
                }
            }
        }

    }
}
