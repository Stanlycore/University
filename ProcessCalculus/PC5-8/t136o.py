#Задача 136о: Вычислить сумму sqrt(10 + a1^2) + ... + sqrt(10 + an^2).
__author__ = "Непомнящих Станислав"

import sys
from t136o_modul import sum_series_136o


def print_help():
    """
    Выводит справочную информацию о программе.
    """
    help_text = (
        "Использование: python t136o.py [количество элементов n] [a1 a2 ... an]\n"
        "Если параметры не указаны, программа запросит ввод с клавиатуры.\n"
        "Этот скрипт вычисляет сумму ряда sqrt(10 + a_i^2), используя переданную функцию."
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
            a_values = list(map(float, sys.argv[2:]))
            if len(a_values) != n:
                raise ValueError("Количество элементов не соответствует n.")
        except ValueError as e:
            print(f"Ошибка ввода: {e}")
            sys.exit(1)
    else:
        try:
            n = int(input("Введите количество элементов (n >= 1): "))
            a_values = []
            for i in range(n):
                a = float(input(f"Введите a{i + 1}: "))
                a_values.append(a)
        except ValueError:
            print("Неверный формат ввода. Ожидается целое число для n и действительные числа для a1, ..., an.")
            sys.exit(1)

    # Вычисление результата
    result = sum_series_136o(a_values)
    print("Сумма ряда:", result)


if __name__ == '__main__':
    main()