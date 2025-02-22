__author__ = "Непомнящих Станислав"
import math

# Задача 3
from t3_modul import volume, side_surface_area

assert volume(2) == 8
assert volume(3) == 27
assert side_surface_area(2) == 16
assert side_surface_area(3) == 36

print("Задача 3 оттестирована")

# Задача 42
from t42_modul import half_sum, double_product, modify_numbers

assert half_sum(2, 4) == 3
assert double_product(2, 4) == 16
result = modify_numbers(2, 4)
assert result == (3, 16)
result = modify_numbers(5, 3)
assert result == (30, 4)
result = modify_numbers(-3, 1)
expected = (half_sum(-3, 1), double_product(-3, 1)) if -3 < 1 else (double_product(-3, 1), half_sum(-3, 1))
assert result == expected

print("Задача 42 оттестирована")

# Задача 64
from t64_modul import count_hundreds

assert count_hundreds(100) == 1
assert count_hundreds(999) == 9
assert count_hundreds(1234) == 12
try:
    count_hundreds(99)
    assert False
except ValueError:
    pass

print("Задача 64 оттестирована")

# Задача 114г
from t114g_modul import sum_series

term = lambda i: 1 / ((2 * i) ** 2)
assert math.isclose(sum_series(term, 1), 0.25, rel_tol=1e-9)
assert math.isclose(sum_series(term, 2), 0.3125, rel_tol=1e-9)
expected = 0.25 + 0.0625 + 1 / ((2 * 3) ** 2)
assert math.isclose(sum_series(term, 3), expected, rel_tol=1e-9)

print("Задача 114г оттестирована")
