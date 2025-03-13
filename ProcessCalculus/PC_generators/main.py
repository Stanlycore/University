__author__ = "Nepomnyaschikh S. I."
import argparse
from generators import *

def main():
    # Создаем парсер для аргументов командной строки
    parser = argparse.ArgumentParser(description="Генерация последовательностей на основе формул.")
    parser.add_argument("n", type=int, nargs='?', help="Количество элементов в последовательности")
    args = parser.parse_args()

    # Если аргумент n не был передан через командную строку, запрашиваем его у пользователя
    if args.n is None:
        try:
            n = int(input("Введите количество элементов в последовательности (n): "))
        except ValueError:
            print("Ошибка: Введите целое число.")
            return
    else:
        n = args.n

    # Генерация и вывод последовательностей
    print("Результат а:", generate_sequence(n, a))
    print("Результат б:", generate_sequence(n, b))
    print("Результат в:", generate_sequence(n, c))
    print("Результат г:", generate_sequence(n, d))
    print("Результат д:", [f"{x:.3f}" for x in generate_sequence(n, e)])
    print("Результат е:", [f"{x:.3f}" for x in generate_sequence(n, f)])
    print("Результат ж:", [f"{x:.3f}" for x in generate_sequence(n, g)])
    print("Результат з:", [f"{x:.3f}" for x in generate_sequence(n, h)])


if __name__ == "__main__":
    main()