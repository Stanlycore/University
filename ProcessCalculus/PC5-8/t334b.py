# Задача 334б: Вычислить двойную сумму Sum[1->i](Sum[1->j](sin(i^3 + j^4))).
__author__ = "Непомнящих Станислав"

import sys
from t334b_modul import double_sum


def print_help():
    """
    Выводит справочную информацию о программе.
    """
    help_text = (
        "Использование: python t334b.py [количество итераций внешней суммы n] [количество итераций внутренней суммы m]\n"
        "Если параметры не указаны, программа запросит ввод с клавиатуры.\n"
        "Этот скрипт вычисляет двойную сумму Sum[1->i](Sum[1->j](sin(i^3 + j^4)))."
    )
    print(help_text)


def main():
    if len(sys.argv) > 1 and sys.argv[1] in ("-h", "--help"):
        print_help()
        return

    # Чтение входных данных
    if len(sys.argv) > 2:
        try:
            n = int(sys.argv[1])
            m = int(sys.argv[2])
            if n <= 0 or m <= 0:
                raise ValueError("Количество итераций должно быть положительным числом.")
        except ValueError as e:
            print(f"Ошибка ввода: {e}")
            sys.exit(1)
    else:
        try:
            n = int(input("Введите количество итераций внешней суммы (n >= 1): "))
            m = int(input("Введите количество итераций внутренней суммы (m >= 1): "))
            if n <= 0 or m <= 0:
                raise ValueError("Количество итераций должно быть положительным числом.")
        except ValueError:
            print("Неверный формат ввода. Ожидается целое число для n и m.")
            sys.exit(1)

    # Вычисление результата
    result = double_sum(n, m)
    print("Результат двойной суммы:", result)


if __name__ == '__main__':
    main()