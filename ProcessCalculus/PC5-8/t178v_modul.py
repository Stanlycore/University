__author__ = "Непомнящих Станислав"
import math


def count_even_squares(sequence):
    """
    Определяет количество элементов последовательности, являющихся квадратами четных чисел.

    :param sequence: список натуральных чисел [a1, ..., an]
    :return: количество элементов, являющихся квадратами четных чисел
    """
    count = 0
    for num in sequence:
        if num < 0:
            continue  # Квадраты не могут быть отрицательными
        root = math.isqrt(num)  # Целочисленный корень
        if root * root == num and root % 2 == 0:
            count += 1
    return count