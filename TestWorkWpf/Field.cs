using System;

namespace TestWorkWpf
{
    public class Field
    {
        //размер поля (матрицы)
        public int count;
        //матрица, хранит положения ручек в виде 0 и 1
        public int[][] mas;
        //промежуточные состояния ручек, нужны при отрисовке
        public float[][] state;

        public Field(int N)
        {
            count = N;
            //заполняем используя рандом
            Random random = new Random();
            //pаполняем все её ячейки одним значением
            int value = random.Next(0, 2);
            mas = new int[N][];
            for (int i = 0; i < N; i++)
            {
                mas[i] = new int[N];
                for (int j = 0; j < N; j++)
                {
                    mas[i][j] = value;
                }
            }
            //теперь вносим путаницу в матрицу
            int x, y;
            // поворачиваем случайные ячейки в матрице половину раз от количества ячеек всего
            for (int i = 0; i < N * N / 2; i++)
            {
                x = random.Next(0, N);
                y = random.Next(0, N);
                //проверяем, не решили ли мы случайно задачу
                if (RotationGen(x, y))
                {
                    //если да, то откатываемся на шаг назад
                    i--;
                }
            }
            //создаем матрицу состояний и обнуляем
            state = new float[N][];
            for (int i = 0; i < N; i++)
            {
                state[i] = new float[N];
                for (int j = 0; j < N; j++)
                {
                    state[i][j] = 0;
                }
            }
        }

        private bool Test()
        {
            //проверяем матрицу, решена ли он сейчас
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    int sum = 0;
                    //находим сумму элементов
                    for (int k = 0; k < count; k++)
                    {
                        sum += mas[i][k] + mas[k][j];
                    }
                    //если в строке или в столбце есть разные элементы, значит она не решена
                    if (sum != count * 2 && sum != 0)
                    {
                        return false;
                    }
                }
            }
            //иначе решена
            return true;
        }

        //метод для генерации матрицы
        private bool RotationGen(int x, int y)
        {
            //поворачиваем ручки с столбце и в строке
            for (int i = 0; i < count; i++)
            {
                mas[y][i] = Inverse(mas[y][i]);
                mas[i][x] = Inverse(mas[i][x]);
            }
            mas[y][x] = Inverse(mas[y][x]);
            //проверяем не решили ли мы задачу
            return Test();
        }

        //метод для поворотов ручек и изменения состояния ручек
        public bool Rotation(int x, int y)
        {
            for (int i = 0; i < count; i++)
            {
                mas[y][i] = Inverse(mas[y][i]);
                //от знака зависит в какую сторону будет поворачиваться ручка
                if (mas[y][i] == 0)
                {
                    state[y][i] = 11;
                }
                else
                {
                    state[y][i] = -11;
                }
                mas[i][x] = Inverse(mas[i][x]);
                if (mas[i][x] == 0)
                {
                    state[i][x] = 11;
                }
                else
                {
                    state[i][x] = -11;
                }
            }
            mas[y][x] = Inverse(mas[y][x]);
            if (mas[y][x] == 0)
            {
                state[y][x] = 11;
            }
            else
            {
                state[y][x] = -11;
            }
            return Test();
        }

        //метод для смены 0 на 1 и обратно
        private int Inverse(int value)
        {
            if (value == 0) return 1;
            else return 0;
        }

    }
}
