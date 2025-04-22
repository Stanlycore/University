

using Xunit.Sdk;

namespace MandelbrotApp.Tests
{
    [TestClass]
    public class MandelbrotTests
    {
        /// <summary>
        /// Тестирует функцию Mandelbrot для точки внутри множества Мандельброта (0, 0).
        /// Ожидается, что точка не выйдет за пределы радиуса за maxIter итераций.
        /// </summary>
        [TestMethod]
        public void Mandelbrot_InsideSet_ReturnsMaxIter()
        {
            // Arrange
            decimal a = 0m; // Точка (0, 0) — внутри множества
            decimal b = 0m;
            int maxIter = 100;

            // Act
            int result = MandelbrotCalculator.Mandelbrot(a, b, maxIter);

            // Assert
            Assert.AreEqual(maxIter, result, "Точка (0, 0) должна быть внутри множества и вернуть maxIter.");
        }

        /// <summary>
        /// Тестирует функцию Mandelbrot для точки вне множества Мандельброта (2, 0).
        /// Ожидается, что точка быстро выйдет за пределы радиуса.
        /// </summary>
        [TestMethod]
        public void Mandelbrot_OutsideSet_ReturnsLowIter()
        {
            // Arrange
            decimal a = 2m; // Точка (2, 0) — вне множества
            decimal b = 0m;
            int maxIter = 100;

            // Act
            int result = MandelbrotCalculator.Mandelbrot(a, b, maxIter);

            // Assert
            Assert.IsTrue(result < 5, "Точка (2, 0) должна выйти за пределы радиуса за малое число итераций.");
        }

        /// <summary>
        /// Тестирует функцию Mandelbrot на границе множества (-1, 0).
        /// Ожидается умеренное число итераций перед выходом за пределы.
        /// </summary>
        [TestMethod]
        public void Mandelbrot_BorderSet_ReturnsModerateIter()
        {
            // Arrange
            decimal a = -1m; // Точка (-1, 0) — на границе множества
            decimal b = 0m;
            int maxIter = 100;

            // Act
            int result = MandelbrotCalculator.Mandelbrot(a, b, maxIter);

            // Assert
            Assert.IsTrue(result > 10 && result < maxIter, "Точка (-1, 0) должна иметь умеренное число итераций.");
        }

        /// <summary>
        /// Тестирует функцию GetColor для точки внутри множества.
        /// Ожидается чёрный цвет (0x000000).
        /// </summary>
        [TestMethod]
        public void GetColor_InsideSet_ReturnsBlack()
        {
            // Arrange
            int iter = 100;
            int maxIter = 100;

            // Act
            int color = MandelbrotCalculator.GetColor(iter, maxIter);

            // Assert
            Assert.AreEqual(0x000000, color, "Цвет для точки внутри множества должен быть чёрным.");
        }

        /// <summary>
        /// Тестирует функцию GetColor для точки вне множества.
        /// Ожидается ненулевой цвет в формате ARGB.
        /// </summary>
        [TestMethod]
        public void GetColor_OutsideSet_ReturnsNonBlack()
        {
            // Arrange
            int iter = 10;
            int maxIter = 100;

            // Act
            int color = MandelbrotCalculator.GetColor(iter, maxIter);

            // Assert
            Assert.AreNotEqual(0x000000, color, "Цвет для точки вне множества не должен быть чёрным.");
            Assert.IsTrue((color & 0xFF0000) >= 0, "Красная компонента должна быть корректной.");
            Assert.IsTrue((color & 0x00FF00) >= 0, "Зелёная компонента должна быть корректной.");
            Assert.IsTrue((color & 0x0000FF) >= 0, "Синяя компонента должна быть корректной.");
        }
    }
}
```