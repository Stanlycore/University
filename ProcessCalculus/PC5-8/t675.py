__author__ = "Непомнящих Станислав"
import sys
import numpy as np
from t675_modul import insert_column


def print_help():
    """
    Выводит справочную информацию о программе.
    """
    help_text = (
        "Использование: python t675.py [размер матрицы n] [элементы столбца a1 a2 ... an]\n"
        "Если параметры не указаны, программа запросит ввод с клавиатуры.\n"
        "Этот скрипт вставляет столбец между пятым и шестым столбцами единичной матрицы."
    )
    print(help_text)


def main():
    if len(sys.argv) > 1 and sys.argv[1] in ("-h", "--help"):
        print_help()
        return

    # Чтение входных данных
    if len(sys.argv) > 1:
        try:
            n = int(sys.argv[1])
            if n < 6:
                raise ValueError("Размер матрицы должен быть >= 6.")
            column = list(map(float, sys.argv[2:]))
            if len(column) != n:
                raise ValueError("Количество элементов столбца не соответствует размеру матрицы.")
        except ValueError as e:
            print(f"Ошибка ввода: {e}")
            sys.exit(1)
    else:
        try:
            n = int(input("Введите размер матрицы (n >= 6): "))
            if n < 6:
                raise ValueError("Размер матрицы должен быть >= 6.")
            column = []
            for i in range(n):
                a = float(input(f"Введите a{i + 1}: "))
                column.append(a)
        except ValueError:
            print("Неверный формат ввода. Ожидается целое число для n и действительные числа для a1, ..., an.")
            sys.exit(1)

    # Создание исходной единичной матрицы
    identity_matrix = np.eye(n)

    # Вставка столбца между пятым и шестым столбцами
    new_matrix = insert_column(identity_matrix, column, 5)

    # Вывод результата
    print("Новая матрица:")
    print(new_matrix)


if __name__ == '__main__':
    main()