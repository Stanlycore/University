

using Xunit.Sdk;

namespace MandelbrotApp.Tests
{
    [TestClass]
    public class MandelbrotTests
    {
        /// <summary>
        /// ��������� ������� Mandelbrot ��� ����� ������ ��������� ������������ (0, 0).
        /// ���������, ��� ����� �� ������ �� ������� ������� �� maxIter ��������.
        /// </summary>
        [TestMethod]
        public void Mandelbrot_InsideSet_ReturnsMaxIter()
        {
            // Arrange
            decimal a = 0m; // ����� (0, 0) � ������ ���������
            decimal b = 0m;
            int maxIter = 100;

            // Act
            int result = MandelbrotCalculator.Mandelbrot(a, b, maxIter);

            // Assert
            Assert.AreEqual(maxIter, result, "����� (0, 0) ������ ���� ������ ��������� � ������� maxIter.");
        }

        /// <summary>
        /// ��������� ������� Mandelbrot ��� ����� ��� ��������� ������������ (2, 0).
        /// ���������, ��� ����� ������ ������ �� ������� �������.
        /// </summary>
        [TestMethod]
        public void Mandelbrot_OutsideSet_ReturnsLowIter()
        {
            // Arrange
            decimal a = 2m; // ����� (2, 0) � ��� ���������
            decimal b = 0m;
            int maxIter = 100;

            // Act
            int result = MandelbrotCalculator.Mandelbrot(a, b, maxIter);

            // Assert
            Assert.IsTrue(result < 5, "����� (2, 0) ������ ����� �� ������� ������� �� ����� ����� ��������.");
        }

        /// <summary>
        /// ��������� ������� Mandelbrot �� ������� ��������� (-1, 0).
        /// ��������� ��������� ����� �������� ����� ������� �� �������.
        /// </summary>
        [TestMethod]
        public void Mandelbrot_BorderSet_ReturnsModerateIter()
        {
            // Arrange
            decimal a = -1m; // ����� (-1, 0) � �� ������� ���������
            decimal b = 0m;
            int maxIter = 100;

            // Act
            int result = MandelbrotCalculator.Mandelbrot(a, b, maxIter);

            // Assert
            Assert.IsTrue(result > 10 && result < maxIter, "����� (-1, 0) ������ ����� ��������� ����� ��������.");
        }

        /// <summary>
        /// ��������� ������� GetColor ��� ����� ������ ���������.
        /// ��������� ������ ���� (0x000000).
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
            Assert.AreEqual(0x000000, color, "���� ��� ����� ������ ��������� ������ ���� ������.");
        }

        /// <summary>
        /// ��������� ������� GetColor ��� ����� ��� ���������.
        /// ��������� ��������� ���� � ������� ARGB.
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
            Assert.AreNotEqual(0x000000, color, "���� ��� ����� ��� ��������� �� ������ ���� ������.");
            Assert.IsTrue((color & 0xFF0000) >= 0, "������� ���������� ������ ���� ����������.");
            Assert.IsTrue((color & 0x00FF00) >= 0, "������ ���������� ������ ���� ����������.");
            Assert.IsTrue((color & 0x0000FF) >= 0, "����� ���������� ������ ���� ����������.");
        }
    }
}
```