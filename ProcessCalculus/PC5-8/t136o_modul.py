__author__ = "Непомнящих Станислав"
import math


def sum_series_136o(a_values):
    """
    Вычисляет сумму ряда sqrt(10 + a_i^2) для заданных значений a1, ..., an.

    :param a_values: список действительных чисел [a1, ..., an]
    :return: сумма ряда
    """
    total = 0  # Инициализация
    for a in a_values:
        total += math.sqrt(10 + a ** 2)
    return total