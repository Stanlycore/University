__author__ = "Nepomnyaschikh S. I."
from math import factorial
from typing import Callable

# Лямбда-функции для формул
a: Callable[[int], int] = lambda i: i
b: Callable[[int], int] = lambda i: i ** 2
c: Callable[[int], int] = lambda i: 2 ** (i + 1)
d: Callable[[int], int] = lambda i: 2 ** i + 3 ** (i + 1)
e: Callable[[int], float] = lambda i: (2 ** i) / factorial(i)
f: Callable[[int], float] = lambda i: sum(1 / k for k in range(1, i + 1))
g: Callable[[int], float] = lambda i: sum((-1) ** (k + 1) / k for k in range(1, i + 1))
h: Callable[[int], float] = lambda i: i * sum(1 / factorial(k) for k in range(1, i + 1))

def generate_sequence(n: int, formula: Callable[[int], float]) -> list[float]:
    """
    Генерация последовательности на основе заданной формулы.
    :param n: Количество элементов в последовательности.
    :param formula: Функция, описывающая формулу последовательности.
    :return: Список значений последовательности.
    """
    return [formula(i) for i in range(1, n + 1)]