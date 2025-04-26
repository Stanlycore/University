# Задача 64: Дано натуральное число n (n > 99). Определить число сотен в нем.
__author__ = "Непомнящих Станислав"
import sys
from t64_modul import count_hundreds


def print_help() -> None:
    """
    Выводит справочную информацию о программе.
    """
    help_text = (
        "Использование: python t64.py [натуральное число n]\n"
        "Если параметр не указан, программа запросит ввод с клавиатуры.\n"
        "Этот скрипт определяет число сотен в заданном натуральном числе (n > 99)."
    )
    print(help_text)


def main() -> None:
    if len(sys.argv) > 1 and sys.argv[1] in ("-h", "--help"):
        print_help()
        return

    if len(sys.argv) > 1:
        try:
            n = int(sys.argv[1])
        except ValueError:
            print("Неверный формат аргумента. Ожидается натуральное число.")
            sys.exit(1)
    else:
        n = int(input("Введите натуральное число (n > 99): "))

    try:
        hundreds = count_hundreds(n)
        print("Число сотен в числе:", hundreds)
    except ValueError as e:
        print(e)
        sys.exit(1)


# Объяснение:
# Конструкция 'if __name__ == "__main__": main()' гарантирует, что функция main() будет вызвана только тогда,
# когда скрипт запускается напрямую. Если модуль импортирован, код внутри этого блока не выполнится.
if __name__ == '__main__':
    main()
