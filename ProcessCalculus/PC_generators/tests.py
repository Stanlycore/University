__author__ = "Nepomnyaschikh S. I."
from generators import *

def run_tests():
    # Тесты для функции a
    assert generate_sequence(4, a) == [1, 2, 3, 4], "Тест a не пройден"
    assert generate_sequence(5, a) == [1, 2, 3, 4, 5], "Тест a не пройден"
    assert generate_sequence(1, a) == [1], "Тест a не пройден"

    # Тесты для функции b
    assert generate_sequence(4, b) == [1, 4, 9, 16], "Тест b не пройден"
    assert generate_sequence(5, b) == [1, 4, 9, 16, 25], "Тест b не пройден"
    assert generate_sequence(1, b) == [1], "Тест b не пройден"

    # Тесты для функции c
    assert generate_sequence(4, c) == [4, 8, 16, 32], "Тест c не пройден"
    assert generate_sequence(5, c) == [4, 8, 16, 32, 64], "Тест c не пройден"
    assert generate_sequence(1, c) == [4], "Тест c не пройден"

    # Тесты для функции d
    assert generate_sequence(4, d) == [11, 31, 89, 259], "Тест d не пройден"
    assert generate_sequence(5, d) == [11, 31, 89, 259, 761], "Тест d не пройден"
    assert generate_sequence(1, d) == [11], "Тест d не пройден"

    # Тесты для функции e
    assert [round(x, 10) for x in generate_sequence(4, e)] == [2.0, 2.0, 1.3333333333, 0.6666666667], "Тест e не пройден"
    assert [round(x, 10) for x in generate_sequence(5, e)] == [2.0, 2.0, 1.3333333333, 0.6666666667, 0.2666666667], "Тест e не пройден"
    assert [round(x, 10) for x in generate_sequence(1, e)] == [2.0], "Тест e не пройден"

    # Тесты для функции f
    assert [round(x, 10) for x in generate_sequence(4, f)] == [1.0, 1.5, 1.8333333333, 2.0833333333], "Тест f не пройден"
    assert [round(x, 10) for x in generate_sequence(5, f)] == [1.0, 1.5, 1.8333333333, 2.0833333333, 2.2833333333], "Тест f не пройден"
    assert [round(x, 10) for x in generate_sequence(1, f)] == [1.0], "Тест f не пройден"

    # Тесты для функции g
    assert [round(x, 10) for x in generate_sequence(4, g)] == [1.0, 0.5, 0.8333333333, 0.5833333333], "Тест g не пройден"
    assert [round(x, 10) for x in generate_sequence(5, g)] == [1.0, 0.5, 0.8333333333, 0.5833333333, 0.7833333333], "Тест g не пройден"
    assert [round(x, 10) for x in generate_sequence(1, g)] == [1.0], "Тест g не пройден"

    # Тесты для функции h
    assert [round(x, 10) for x in generate_sequence(4, h)] == [1.0, 3.0, 5.0, 6.8333333333], "Тест h не пройден"
    assert [round(x, 10) for x in generate_sequence(5, h)] == [1.0, 3.0, 5.0, 6.8333333333, 8.5833333333], "Тест h не пройден"
    assert [round(x, 10) for x in generate_sequence(1, h)] == [1.0], "Тест h не пройден"

    print("Все тесты успешно пройдены!")


if __name__ == "__main__":
    run_tests()
