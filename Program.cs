using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace JacobiSeidelLab
{
    public class Program
    {
        private static List<double> jacobiResiduals = new List<double>();
        private static List<double> seidelResiduals = new List<double>();

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form form = new Form
            {
                Text = "4. Методы Якоби и Гаусса-Зейделя",
                Width = 1180,
                Height = 720,
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.White
            };

            // МАТРИЦА A - поля ввода
            TextBox[,] AFields = new TextBox[3, 3];
            int yPos = 80;
            for (int i = 0; i < 3; i++)
            {
                new Label
                {
                    Text = $"Строка {i + 1}:",
                    Location = new Point(40, yPos + i * 70),
                    Parent = form,
                    Font = new Font("Arial", 11)
                };

                for (int j = 0; j < 3; j++)
                {
                    AFields[i, j] = new TextBox
                    {
                        Text = (i == j) ? "4" : "1", 
                        Location = new Point(150 + j * 85, yPos + i * 70 - 8),
                        Size = new Size(70, 35),
                        Font = new Font("Arial", 12),
                        TextAlign = HorizontalAlignment.Center,
                        Parent = form
                    };
                }
            }

            // ВЕКТОР b - поля ввода
            new Label
            {
                Text = "b =",
                Location = new Point(450, yPos - 30),
                Parent = form,
                Font = new Font("Arial", 11, FontStyle.Bold)
            };

            TextBox[] bFields = new TextBox[3];
            for (int i = 0; i < 3; i++)
            {
                bFields[i] = new TextBox
                {
                    Text = "1",
                    Location = new Point(450, yPos + i * 70 - 8),
                    Size = new Size(70, 35),
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    TextAlign = HorizontalAlignment.Center,
                    BackColor = Color.LightYellow,
                    Parent = form
                };
            }

            // ТОЧНОСТЬ
            new Label
            {
                Text = "Точность ε:",
                Location = new Point(40, yPos + 230),
                Parent = form,
                Font = new Font("Arial", 11)
            };

            TextBox precisionField = new TextBox
            {
                Text = "0.000001",
                Location = new Point(150, yPos + 225),
                Size = new Size(120, 35),
                Font = new Font("Arial", 12),
                Parent = form
            };

            Button solveButton = new Button
            {
                Text = "Решить",
                Location = new Point(40, yPos + 290),
                Size = new Size(140, 45),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.LightSkyBlue,
                Parent = form
            };

            Label resultLabel = new Label
            {
                Location = new Point(40, yPos + 350),
                Size = new Size(1120, 160),
                Font = new Font("Consolas", 12),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Parent = form
            };

          
            solveButton.Click += (s, e) =>
            {
                try
                {
                    // Чтение матрицы A из полей ввода
                    double[,] A = new double[3, 3];
                    double[] b = new double[3];

                    for (int i = 0; i < 3; i++)
                    {
                        b[i] = double.Parse(bFields[i].Text);
                        for (int j = 0; j < 3; j++)
                            A[i, j] = double.Parse(AFields[i, j].Text);
                    }

                    double eps = double.Parse(precisionField.Text);

                    // Вызов методов решения
                    var jac = Jacobi(A, b, eps);
                    var gs = Seidel(A, b, eps);

                    //  построения графиков
                    jacobiResiduals = jac.Item2;   
                    seidelResiduals = gs.Item2;    

                
                    resultLabel.Text =
                        $"Решение Якоби:      x ≈ ({jac.Item1[0]:F9}, {jac.Item1[1]:F9}, {jac.Item1[2]:F9})\n" +
                        $"Решение Зейделя:    x ≈ ({gs.Item1[0]:F9},  {gs.Item1[1]:F9},  {gs.Item1[2]:F9})\n\n" +
                        $"Якоби:   {jac.Item3} итераций\n" +
                        $"Зейдель: {gs.Item3} итераций\n\n" +
                        (gs.Item3 < jac.Item3
                            ? $"Зейдель быстрее на {jac.Item3 - gs.Item3} итераций"
                            : $"Якоби быстрее на {gs.Item3 - jac.Item3} итераций");

                    
                    form.Invalidate();
                }
                catch
                {
                    MessageBox.Show("Проверь числа, брат!");
                }
            };

       
            form.Paint += (s, e) =>
            {
          
                if (jacobiResiduals.Count < 2) return;

                Graphics g = e.Graphics; 

               
                int x0 = 580, y0 = 80, w = 570, h = 420;

          
                g.FillRectangle(Brushes.White, x0 - 10, y0 - 10, w + 20, h + 60);
                g.DrawRectangle(Pens.Black, x0, y0, w, h);

          
                g.DrawString("Сходимость методов", new Font("Arial", 14, FontStyle.Bold), Brushes.Black, x0 + 190, y0 - 45);

                // Оси координат
                g.DrawLine(Pens.Black, x0, y0 + h, x0 + w, y0 + h); 
                g.DrawLine(Pens.Black, x0, y0 + h, x0, y0);         
                g.DrawString("Итерация", new Font("Arial", 10), Brushes.Black, x0 + w / 2 - 30, y0 + h + 25);
                g.DrawString("Невязка (лог. шкала)", new Font("Arial", 10), Brushes.Black, x0 - 80, y0 + h / 2 - 70);

                double minY = 1e-8, maxY = 2.0; 
                double logMin = Math.Log10(minY), logMax = Math.Log10(maxY);

                for (int p = -8; p <= 0; p++) 
                {
                    double val = Math.Pow(10, p);
                    int py = y0 + h - (int)((Math.Log10(val) - logMin) / (logMax - logMin) * h);
                    g.DrawLine(Pens.LightGray, x0, py, x0 + w, py); 
                    g.DrawString($"10^{p}", new Font("Arial", 9), Brushes.Gray, x0 - 45, py - 8);
                }

            
                void DrawCurve(List<double> data, Color color)
                {
                    for (int i = 1; i < data.Count; i++)
                    {
                   
                        double y1 = Math.Max(data[i - 1], minY);
                        double y2 = Math.Max(data[i], minY);

                   
                        int px1 = x0 + (i - 1) * w / (data.Count - 1);
                        int px2 = x0 + i * w / (data.Count - 1);

                  
                        int py1 = y0 + h - (int)((Math.Log10(y1) - logMin) / (logMax - logMin) * h);
                        int py2 = y0 + h - (int)((Math.Log10(y2) - logMin) / (logMax - logMin) * h);

                    
                        if (py1 < y0) py1 = y0;
                        if (py1 > y0 + h) py1 = y0 + h;
                        if (py2 < y0) py2 = y0;
                        if (py2 > y0 + h) py2 = y0 + h;
                        g.DrawLine(new Pen(color, 3), px1, py1, px2, py2);
                        g.FillEllipse(new SolidBrush(color), px2 - 6, py2 - 6, 12, 12);
                    }
                }

                // графики обоих методов
                DrawCurve(jacobiResiduals, Color.Blue);    // синий - метод Якоби
                DrawCurve(seidelResiduals, Color.Red);     // красный - метод Зейделя

                g.FillEllipse(Brushes.Blue, x0 + 40, y0 + 20, 18, 18);
                g.DrawString("Якоби", new Font("Arial", 11, FontStyle.Bold), Brushes.Blue, x0 + 65, y0 + 17);
                g.FillEllipse(Brushes.Red, x0 + 160, y0 + 20, 18, 18);
                g.DrawString("Зейдель", new Font("Arial", 11, FontStyle.Bold), Brushes.Red, x0 + 185, y0 + 17);
            };

            Application.Run(form);
        }

        // МЕТОД ЯКОБИ
        private static Tuple<double[], List<double>, int> Jacobi(double[,] A, double[] b, double eps)
        {
            int n = b.Length;
            double[] x = new double[n];     
            double[] xNew = new double[n];   
            var residuals = new List<double> { 10.0 }; 
            int iter = 0; 

            while (true)
            {
                iter++;

                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < n; j++)
                        if (j != i)
                            sum += A[i, j] * x[j];

                    xNew[i] = (b[i] - sum) / A[i, i];
                }

                double res = 0;
                for (int i = 0; i < n; i++)
                {
                    double t = b[i];
                    for (int j = 0; j < n; j++) t -= A[i, j] * xNew[j];
                    res = Math.Max(res, Math.Abs(t));
                }

                residuals.Add(res);
                if (res < eps || iter > 1000) break;
                Array.Copy(xNew, x, n);
            }
            return Tuple.Create(xNew, residuals, iter);
        }

        // МЕТОД ЗЕЙДЕЛЯ
        private static Tuple<double[], List<double>, int> Seidel(double[,] A, double[] b, double eps)
        {
            int n = b.Length;
            double[] x = new double[n];
            var residuals = new List<double> { 10.0 }; 
            int iter = 0; 

            while (true)
            {
                iter++;
                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < n; j++)
                        if (j != i)
                            sum += A[i, j] * x[j]; 
                    x[i] = (b[i] - sum) / A[i, i];
                }

                // Вычисление невязки
                double res = 0;
                for (int i = 0; i < n; i++)
                {
                    double t = b[i];
                    for (int j = 0; j < n; j++) t -= A[i, j] * x[j];
                    res = Math.Max(res, Math.Abs(t));
                }
                residuals.Add(res);

                if (res < eps || iter > 1000) break;
            }
            return Tuple.Create(x, residuals, iter);
        }
    }
}
