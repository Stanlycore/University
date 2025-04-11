__author__ = "Nepomnyaschikh S. I."
from typing import Callable, Generator, Dict

# Кэш для факториалов
_factorial_cache: Dict[int, int] = {0: 1, 1: 1}

def cached_factorial(n: int) -> int:
    """Вычисляет факториал с кэшированием."""
    if n in _factorial_cache:
        return _factorial_cache[n]
    result = _factorial_cache[max(_factorial_cache)]
    for i in range(max(_factorial_cache) + 1, n + 1):
        result *= i
        _factorial_cache[i] = result
    return result


# Вспомогательные лямбда для сумм формата:
# x - конечный индекс
# acc - накопление суммы
# k - текущий индекс
_f_sum: Callable[[int, float, int], float] = lambda x, acc, k: acc if k > x else _f_sum(x, acc + 1/k, k + 1)
_g_sum: Callable[[int, float, int], float] = lambda x, acc, k: acc if k > x else _g_sum(x, acc + (-1)**(k+1)/k, k + 1)
_h_sum: Callable[[int, float, int], float] = lambda x, acc, k: acc if k > x else _h_sum(x, acc + 1/cached_factorial(k), k + 1)

# Лямбда-функции для формул
a: Callable[[int], int] = lambda i: i
b: Callable[[int], int] = lambda i: i ** 2
c: Callable[[int], int] = lambda i: 2 ** (i + 1)
d: Callable[[int], int] = lambda i: 2 ** i + 3 ** (i + 1)
e: Callable[[int], float] = lambda i: (2 ** i) / cached_factorial(i)
f: Callable[[int], float] = lambda i: _f_sum(i, 0.0, 1)
g: Callable[[int], float] = lambda i: _g_sum(i, 0.0, 1)
h: Callable[[int], float] = lambda i: i * _h_sum(i, 0.0, 1)

def generate_sequence(n: int, formula: Callable[[int], float]) -> Generator[float, None, None]:
    """Генерирует последовательность по формуле с использованием yield."""
    for i in range(1, n + 1):
        yield formula(i)