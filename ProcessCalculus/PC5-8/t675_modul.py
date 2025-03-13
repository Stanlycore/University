import numpy as np

def insert_column(matrix, column, position):
    """
    Вставляет столбец в матрицу в указанную позицию.

    :param matrix: исходная матрица (ndarray размера n x n)
    :param column: столбец для вставки (ndarray размера n)
    :param position: позиция для вставки (индекс столбца)
    :return: новая матрица с вставленным столбцом (ndarray размера n x (n+1))
    """
    return np.insert(matrix, position, column, axis=1)
