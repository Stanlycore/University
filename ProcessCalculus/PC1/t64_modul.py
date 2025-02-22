__author__ = "Непомнящих Станислав"


def count_hundreds(n: int) -> int:
    """
    Определяет число сотен в натуральном числе n.

    :param n: натуральное число (n > 99)
    :return: число сотен в числе n
    :raises ValueError: если n меньше 100
    """
    if n < 100:
        raise ValueError("n должно быть больше 99")
    return n // 100
