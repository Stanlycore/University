__author__ = "Непомнящих Станислав"


def half_sum(x: float, y: float) -> float:
    """
    Вычисляет полусумму двух чисел.

    :param x: первое число
    :param y: второе число
    :return: (x + y) / 2
    """
    return (x + y) / 2


def double_product(x: float, y: float) -> float:
    """
    Вычисляет удвоенное произведение двух чисел.

    :param x: первое число
    :param y: второе число
    :return: 2 * x * y
    """
    return 2 * x * y


def modify_numbers(x: float, y: float) -> tuple[float, float]:
    """
    Модифицирует два числа:
      - Меньшее заменяется на их полусумму.
      - Большее заменяется на их удвоенное произведение.

    :param x: первое число (x ≠ y)
    :param y: второе число
    :return: кортеж (новое_x, новое_y) с модифицированными значениями
    """
    if x < y:
        return half_sum(x, y), double_product(x, y)
    else:
        return double_product(x, y), half_sum(x, y)

