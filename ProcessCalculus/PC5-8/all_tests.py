__author__ = "Непомнящих Станислав"
from t136o_modul import sum_series_136o
from t178v_modul import count_even_squares
from t334b_modul import double_sum
from t675_modul import insert_column
import numpy as np


def run_tests():
    """
    Запускает блок тестов для проверки функций.
    """
    print("Запуск тестов...")

    # Тесты для задачи 136o
    assert abs(sum_series_136o([3]) - 4.358898943540674) < 1e-9, "Ошибка в тесте с одним элементом"
    assert abs(sum_series_136o([1, 2, 3]) - 11.417181120670016) < 1e-9, "Ошибка в тесте с несколькими элементами"
    assert abs(sum_series_136o([-1, -2, -3]) - 11.417181120670016) < 1e-9, "Ошибка в тесте с отрицательными значениями"
    assert abs(sum_series_136o([0, 0, 0]) - 9.486832980505138) < 1e-9, "Ошибка в тесте с нулевыми значениями"
    assert sum_series_136o([]) == 0, "Ошибка в тесте с пустым списком"

    # Тесты для задачи 178(в)
    assert count_even_squares([16]) == 1, "Ошибка в тесте с одним элементом (16)"
    assert count_even_squares([16, 4, 9, 36]) == 3, "Ошибка в тесте с несколькими элементами"
    assert count_even_squares([1, 3, 5, 7]) == 0, "Ошибка в тесте без квадратов четных чисел"
    assert count_even_squares([]) == 0, "Ошибка в тесте с пустым списком"
    assert count_even_squares([-4, -16, 36]) == 1, "Ошибка в тесте с отрицательными числами"

    # Тесты для задачи 334б
    assert abs(double_sum(1, 1) - np.sin(1**3 + 1**4)) < 1e-9, "Ошибка в тесте с n=1, m=1"
    expected_result = np.sin(1**3 + 1**4) + np.sin(1**3 + 2**4) + np.sin(2**3 + 1**4) + np.sin(2**3 + 2**4)
    assert abs(double_sum(2, 2) - expected_result) < 1e-9, "Ошибка в тесте с n=2, m=2"
    expected_result = np.sin(1**3 + 1**4) + np.sin(1**3 + 2**4) + np.sin(1**3 + 3**4)
    assert abs(double_sum(1, 3) - expected_result) < 1e-9, "Ошибка в тесте с n=1, m=3"
    expected_result = np.sin(1**3 + 1**4) + np.sin(2**3 + 1**4) + np.sin(3**3 + 1**4)
    assert abs(double_sum(3, 1) - expected_result) < 1e-9, "Ошибка в тесте с n=3, m=1"

    # Тесты для задачи 675
    # Тест 1: n = 6, вставка столбца
    n = 6
    identity_matrix = np.eye(n)
    column = np.arange(1, n + 1)
    expected_matrix = np.insert(identity_matrix, 5, column, axis=1)
    result_matrix = insert_column(identity_matrix, column, 5)
    assert np.allclose(result_matrix, expected_matrix), "Ошибка в тесте с n=6"

    print("Все тесты пройдены успешно!")


if __name__ == '__main__':
    run_tests()