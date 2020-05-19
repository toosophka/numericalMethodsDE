using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace vchmat6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        double x0 = 0, y0 = 10, xn;
        int n;

        double[] x;
        double[] y;

        //проверка на пустоту
        private bool empty(TextBox box)
        {
            if (box.Text == "")
                return true;
            else
                return false;
        }

        private bool check()
        {
            if(empty(textBox1))
            {
                MessageBox.Show("Введите координату х конечной точки.");
                return false;
            }
            if (empty(textBox2))
            {
                MessageBox.Show("Введите шаг.");
                return false;
            }
            if((double.Parse(textBox2.Text)<0)||(double.Parse(textBox2.Text)> double.Parse(textBox1.Text)))
            {
                MessageBox.Show("Шаг введен некорректно.");
                textBox2.Clear();
                return false;
            }
            return true;
        }

        //функция, подготавливающая данные для использования методов, задаются значения переменных
        private void data(double h)
        {
            xn = double.Parse(textBox1.Text);       //координата х конечной точки
            n = (int)Math.Ceiling((xn - x0) / h);    //количество интервалов, округляется в бОльшую
            x = new double[n + 1];                       //сторону, если (x(n)-x(0)) не делится на h
            y = new double[n + 1];
            
            //заполняем массив х в соответствии с шагом
            for (int i = 0; i < n; i++)
                x[i] = x0 + h * i;
            x[n] = xn;
        }

        //построение графика, настраиваются оси
        private void graph(double[] x, double[] y, double h, int i)
        {
            chart1.ChartAreas[0].AxisX.Minimum = x0-3; //вычитаем произвольное число, чтобы на графике было лучше видно
            chart1.ChartAreas[0].AxisX.Maximum = double.Parse(textBox1.Text)+3; //аналогично мин
            chart1.ChartAreas[0].AxisX.MajorGrid.Interval = h;
            chart1.Series[i].Points.DataBindXY(x, y);
        }

        //вычисляет функцию f(x,y)
        double function(double x, double y)
        {
            return (Math.Sin(x) - y);
        }

        //стереть график метод Эйлера
        private void button3_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
        }

        //стереть график модифицированный
        private void button4_Click(object sender, EventArgs e)
        {
            chart1.Series[2].Points.Clear();
        }

        //метод Рунге-Кутты Мерсона
        private void button5_Click(object sender, EventArgs e)
        {
            if (!check())
                return;
            if(empty(textBox5))
            {
                MessageBox.Show("Введите точность.");
                return;
            }
            chart1.Series[3].Points.Clear();
            double h = double.Parse(textBox2.Text);
            xn = double.Parse(textBox1.Text);       //координата х конечной точки
            double eps= double.Parse(textBox5.Text);
            double[] k = new double[5];
            double delta;
            double x = x0, y = y0;

            chart1.Series[3].Points.AddXY(x, y);
            while (x<xn)
            {
                k[0] = h * function(x, y);
                k[1] = h * function(x + (double)1 / 3 * h, y + (double)1 / 3 * k[0]);
                k[2] = h * function(x + (double)1 / 3 * h, y + (double)1 / 6 * k[0] + (double)1 / 6 * k[1]);
                k[3] = h * function(x + (double)1 / 2 * h, y + (double)1 / 8 * k[0] + (double)3 / 8 * k[2]);
                k[4] = h * function(x + h, y + (double)1 / 2 * k[0] - (double)3 / 2 * k[2] + 2 * k[3]);

                delta = (double)1 / 30 * (2 * k[0] - 9 * k[2] + 8 * k[3] - k[4]);
                if (Math.Abs(delta) >= eps*Math.Abs(y))
                {
                    h = (double)h / 2;
                    continue;
                }
                chart1.Series[3].Points.AddXY(x, y);
                y += (double)1 / 6 * k[0] + (double)2 / 3 * k[3] + (double)1 / 6 * k[4];
                x += h;
                if (Math.Abs(delta) <= (double)eps * Math.Abs(y) / 32)
                    h = (double)h * 2;
            }
            k[0] = h * function(xn, y);
            k[1] = h * function(xn + (double)1 / 3 * h, y + (double)1 / 3 * k[0]);
            k[2] = h * function(xn + (double)1 / 3 * h, y + (double)1 / 6 * k[0] + (double)1 / 6 * k[1]);
            k[3] = h * function(xn + (double)1 / 2 * h, y + (double)1 / 8 * k[0] + (double)3 / 8 * k[2]);
            k[4] = h * function(xn + h, y + (double)1 / 2 * k[0] - (double)3 / 2 * k[2] + 2 * k[3]);
            y += (double)1 / 6 * k[0] + (double)2 / 3 * k[3] + (double)1 / 6 * k[4];
            chart1.Series[3].Points.AddXY(xn, y);
        }

        //стереть график Рунге-Кутты Мерсона
        private void button6_Click(object sender, EventArgs e)
        {
            chart1.Series[3].Points.Clear();
        }

        //точное решение
        private void button9_Click(object sender, EventArgs e)
        {
            if (!check())
                return;
            double h = double.Parse(textBox2.Text);
            data(h);
            y[0] = y0;
            for (int i = 1; i < n + 1; i++)
                y[i] = -0.5 * Math.Cos(x[i]) + 0.5 * Math.Sin(x[i]) + (double)(21 / 2) * Math.Exp(-x[i]);

            graph(x, y, h, 1);
        }

        //стереть точное решение
        private void button10_Click(object sender, EventArgs e)
        {
            chart1.Series[1].Points.Clear();
        }

        //метод Адамса 3го порядка
        private void button7_Click(object sender, EventArgs e)
        {
            if (!check())
                return;
            double h = double.Parse(textBox2.Text);
            data(h);
            double[] k = new double[3];
            double delta;

            y[0] = y0;
            y[1] = eiler(h, x[0], y[0]);
            y[2] = eiler(h, x[1], y[1]);

            for(int i=3;i<n;i++)
            {
                for (int j = 0; j < 3; j++)
                    k[j] = function(x[i - 1], y[i - 1])*h;
                delta = (double)1 / 12 * (23 * k[0] - 16 * k[1] + 5 * k[2]);
                y[i] = y[i - 1] + delta;
            }
            graph(x, y, h, 4);
        }

        double eiler(double h, double x, double y)
        {
            return ( y + h * function(x + h / 2, y + h / 2 * function(x, y)));
        }

        //стереть график Адамса
        private void button8_Click(object sender, EventArgs e)
        {
            chart1.Series[4].Points.Clear();
        }

        //метод Эйлера модифицированный
        private void button2_Click(object sender, EventArgs e)
        {
            if (!check())
                return;
            double h = double.Parse(textBox2.Text);
            data(h);

            y[0] = y0;
            for (int i = 1; i < n; i++)
                y[i] = eiler(h, x[i - 1], y[i - 1]); 
            y[n] = eiler(h, x[n - 1], y[n - 1]);

            graph(x, y, h, 2);
        }
        
        //метод Эйлера
        private void button1_Click(object sender, EventArgs e)
        {
            if (!check())
                return;
            double h = double.Parse(textBox2.Text);
            data(h);
            double h1 = x[n] - x[n - 1];

            y[0] = y0;
            for (int i = 1; i < n; i++)
                y[i] = y[i - 1] + h * function(x[i - 1], y[i - 1]);
            y[n]= y[n - 1] + h1 * function(x[n - 1], y[n - 1]);
            graph(x, y, h, 0);
        }
    }
}
