namespace MandelbrotApp
{
    public static class MandelbrotCalculator
    {
        /// <summary>
        /// Вычисляет количество итераций для точки (a, b) в множестве Мандельброта.
        /// Алгоритм:
        /// 1. Инициализирует комплексное число c = a + bi (a — действительная часть, b — мнимая часть).
        /// 2. Итеративно применяет формулу zn+1 = zn^2 + c, где z0 = 0.
        /// 3. Проверяет, выходит ли |zn| за пределы радиуса 4 (условие выхода из множества).
        /// 4. Возвращает количество итераций до выхода или maxIter, если точка в множестве.
        /// </summary>
        /// <param name="a">Действительная часть комплексного числа</param>
        /// <param name="b">Мнимая часть комплексного числа</param>
        /// <param name="maxIter">Максимальное число итераций</param>
        /// <returns>Число итераций до выхода за пределы или maxIter</returns>
        public static int Mandelbrot(decimal a, decimal b, int maxIter)
        {
            decimal ca = a; // Сохраняем начальные значения для c = a + bi
            decimal cb = b;
            decimal za = 0; // Начальное значение z = 0
            decimal zb = 0;
            int iter = 0;

            while (iter < maxIter)
            {
                // Вычисляем z^2 = (za + zbi)^2 = (za^2 - zb^2) + 2*za*zb*i
                decimal aa = za * za - zb * zb;
                decimal bb = 2 * za * zb;

                // Добавляем c: zn+1 = z^2 + c
                za = aa + ca;
                zb = bb + cb;

                // Проверяем, превышает ли |z| радиус 4
                if (za * za + zb * zb > 16)
                    break;

                iter++;
            }

            return iter;
        }

        /// <summary>
        /// Преобразует число итераций в цвет для визуализации.
        /// Алгоритм:
        /// 1. Если точка принадлежит множеству (iter == maxIter), возвращает чёрный цвет (0x000000).
        /// 2. Для точек вне множества вычисляет RGB-цвет на основе числа итераций.
        /// 3. Использует циклическое отображение итераций в компоненты RGB для создания градиента.
        /// </summary>
        /// <param name="iter">Число итераций</param>
        /// <param name="maxIter">Максимальное число итераций</param>
        /// <returns>Цвет в формате ARGB (int)</returns>
        public static int GetColor(int iter, int maxIter)
        {
            if (iter == maxIter)
                return 0x000000; // Чёрный цвет для точек внутри множества

            // Вычисляем RGB-компоненты с использованием итераций для создания цветового градиента
            int r = (iter * 9) % 256; // Красный: быстрые изменения
            int g = (iter * 7) % 256; // Зелёный: средние изменения
            int b = (iter * 5) % 256; // Синий: медленные изменения

            // Собираем цвет в формате ARGB (A=255, R, G, B)
            return (r << 16) | (g << 8) | b;
        }
    }
}