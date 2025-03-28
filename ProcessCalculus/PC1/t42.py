# Задача 42: Даны действительные числа x, y (x не равно y).
# Меньшее из этих двух чисел заменить их полусуммой, а большее – их удвоенным произведением.
__author__ = "Непомнящих Станислав"
import sys
from t42_modul import modify_numbers


def print_help() -> None:
    """
    Выводит справочную информацию о программе.
    """
    help_text = (
        "Использование: python t42.py [число x] [число y]\n"
        "Если параметры не указаны, программа запросит ввод с клавиатуры.\n"
        "Этот скрипт модифицирует два числа: меньшее заменяется полусуммой, большее — удвоенным произведением."
    )
    print(help_text)


def main() -> None:
    if len(sys.argv) > 1 and sys.argv[1] in ("-h", "--help"):
        print_help()
        return

    if len(sys.argv) > 2:
        try:
            x = float(sys.argv[1])
            y = float(sys.argv[2])
        except ValueError:
            print("Неверный формат аргументов. Ожидаются два числа.")
            sys.exit(1)
    else:
        x = float(input("Введите число x: "))
        y = float(input("Введите число y (не равно x): "))

    if x == y:
        print("Числа должны быть различны.")
        sys.exit(1)

    new_x, new_y = modify_numbers(x, y)
    print("Результат модификации чисел:", new_x, new_y)


# Объяснение:
# Конструкция 'if __name__ == "__main__": main()' гарантирует, что функция main() будет вызвана только тогда,
# когда скрипт запускается напрямую. Если модуль импортирован, код внутри этого блока не выполнится.
if __name__ == '__main__':
    main()
