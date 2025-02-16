from typing import Dict, List, Optional
import enum

class PromptType(enum.Enum):
    """Типы промптов."""
    FIXED_START = 1  # Фиксированный в начале
    FLOATING_SYSTEM = 2  # Плавающий системный
    CONTEXT_TEMPORARY = 3  # Временный контекстный

class PromptPart:
    """Класс для представления части промпта."""

    def __init__(self, part_type: PromptType, content: str, parameters: Optional[Dict] = None):
        """
        Инициализация части промпта.

        :param part_type: Тип промпта (из PromptType).
        :param content: Содержимое промпта.
        :param parameters: Параметры для форматирования (опционально).
        """
        self.type = part_type
        self.original_content = content
        self.parameters = parameters or {}

    def format(self, **kwargs) -> str:
        """Форматирует содержимое с параметрами как f-строка."""
        try:
            return self.original_content.format(**{**self.parameters, **kwargs})
        except KeyError as e:
            raise ValueError(f"Отсутствует параметр {e} в шаблоне промпта")

    @property
    def is_fixed_start(self) -> bool:
        """Проверяет, является ли промпт фиксированным в начале."""
        return self.type == PromptType.FIXED_START

    @property
    def is_floating_system(self) -> bool:
        """Проверяет, является ли промпт плавающим системным."""
        return self.type == PromptType.FLOATING_SYSTEM

    @property
    def is_context_temporary(self) -> bool:
        """Проверяет, является ли промпт временным контекстным."""
        return self.type == PromptType.CONTEXT_TEMPORARY

    def __str__(self) -> str:
        """Строковое представление объекта для удобного вывода."""
        return f"PromptPart(type={self.type.name}, content={self.original_content}, parameters={self.parameters})"