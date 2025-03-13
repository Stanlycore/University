import math


def double_sum(n, m):
    """
    Вычисляет двойную сумму Sum[1->i](Sum[1->j](sin(i^3 + j^4))).

    :param n: количество итераций внешней суммы (по i)
    :param m: количество итераций внутренней суммы (по j)
    :return: результат вычисления двойной суммы
    """
    total_sum = 0
    for i in range(1, n + 1):
        for j in range(1, m + 1):
            total_sum += math.sin(i**3 + j**4)
    return total_sum