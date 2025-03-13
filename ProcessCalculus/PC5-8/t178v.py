# Задача 178(в): Определить количество членов ak последовательности a1,...,an,
# являющихся квадратами четных чисел.
__author__ = "Непомнящих Станислав"
import sys
from t178v_modul import count_even_squares


def print_help():
    """
    Выводит справочную информацию о программе.
    """
    help_text = (
        "Использование: python t178v.py [количество элементов n] [a1 a2 ... an]\n"
        "Если параметры не указаны, программа запросит ввод с клавиатуры.\n"
        "Этот скрипт определяет количество элементов последовательности,\n"
        "являющихся квадратами четных чисел."
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
            sequence = list(map(int, sys.argv[2:]))
            if len(sequence) != n:
                raise ValueError("Количество элементов не соответствует n.")
        except ValueError as e:
            print(f"Ошибка ввода: {e}")
            sys.exit(1)
    else:
        try:
            n = int(input("Введите количество элементов (n >= 1): "))
            sequence = []
            for i in range(n):
                a = int(input(f"Введите a{i + 1}: "))
                sequence.append(a)
        except ValueError:
            print("Неверный формат ввода. Ожидается целое число для n и элементов последовательности.")
            sys.exit(1)

    # Вычисление результата
    result = count_even_squares(sequence)
    print("Количество элементов, являющихся квадратами четных чисел:", result)


if __name__ == '__main__':
    main()