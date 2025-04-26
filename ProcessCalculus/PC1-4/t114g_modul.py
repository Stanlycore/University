__author__ = "Непомнящих Станислав"
from typing import Callable


def sum_series(func: Callable[[int], float], n: int) -> float:
    """
    Вычисляет сумму ряда 1/(2i)^2 для i от 1 до n.

    :param n: количество членов ряда (n >= 1)
    :return: сумма ряда
    """
    total = 0.0
    for i in range(1, n + 1):
        total += func(i)
    return total
