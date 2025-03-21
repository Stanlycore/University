__author__ = "Непомнящих Станислав"
import unittest
import numpy as np
from t136o_modul import sum_series_136o
from t178v_modul import count_even_squares
from t334b_modul import double_sum
from t675_modul import insert_column


class TestFunctions(unittest.TestCase):

    # Тесты для задачи 136o
    def test_sum_series_136o_single_element(self):
        self.assertAlmostEqual(sum_series_136o([3]), 4.358898943540674, places=9)

    def test_sum_series_136o_multiple_elements(self):
        self.assertAlmostEqual(sum_series_136o([1, 2, 3]), 11.417181120670016, places=9)

    def test_sum_series_136o_negative_values(self):
        self.assertAlmostEqual(sum_series_136o([-1, -2, -3]), 11.417181120670016, places=9)

    def test_sum_series_136o_zero_values(self):
        self.assertAlmostEqual(sum_series_136o([0, 0, 0]), 9.486832980505138, places=9)

    def test_sum_series_136o_empty_list(self):
        self.assertEqual(sum_series_136o([]), 0)

    # Тесты для задачи 178(в)
    def test_count_even_squares_single_element(self):
        self.assertEqual(count_even_squares([16]), 1)

    def test_count_even_squares_multiple_elements(self):
        self.assertEqual(count_even_squares([16, 4, 9, 36]), 3)

    def test_count_even_squares_no_even_squares(self):
        self.assertEqual(count_even_squares([1, 3, 5, 7]), 0)

    def test_count_even_squares_empty_list(self):
        self.assertEqual(count_even_squares([]), 0)

    def test_count_even_squares_negative_numbers(self):
        self.assertEqual(count_even_squares([-4, -16, 36]), 1)

    # Тесты для задачи 334б
    def test_double_sum_n_1_m_1(self):
        self.assertAlmostEqual(double_sum(1, 1), np.sin(1 ** 3 + 1 ** 4), places=9)

    def test_double_sum_n_2_m_2(self):
        expected_result = np.sin(1 ** 3 + 1 ** 4) + np.sin(1 ** 3 + 2 ** 4) + np.sin(2 ** 3 + 1 ** 4) + np.sin(
            2 ** 3 + 2 ** 4)
        self.assertAlmostEqual(double_sum(2, 2), expected_result, places=9)

    def test_double_sum_n_1_m_3(self):
        expected_result = np.sin(1 ** 3 + 1 ** 4) + np.sin(1 ** 3 + 2 ** 4) + np.sin(1 ** 3 + 3 ** 4)
        self.assertAlmostEqual(double_sum(1, 3), expected_result, places=9)

    def test_double_sum_n_3_m_1(self):
        expected_result = np.sin(1 ** 3 + 1 ** 4) + np.sin(2 ** 3 + 1 ** 4) + np.sin(3 ** 3 + 1 ** 4)
        self.assertAlmostEqual(double_sum(3, 1), expected_result, places=9)

    # Тесты для задачи 675
    def test_insert_column(self):
        n = 6
        identity_matrix = np.eye(n)
        column = np.arange(1, n + 1)
        expected_matrix = np.insert(identity_matrix, 5, column, axis=1)
        result_matrix = insert_column(identity_matrix, column, 5)
        self.assertTrue(np.allclose(result_matrix, expected_matrix))


if __name__ == '__main__':
    unittest.main()
