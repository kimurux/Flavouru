import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import ConfirmReset from './confirm-reset';
import '@testing-library/jest-dom';

vi.mock('react-router-dom', async () => {
  const actual: any = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useLocation: () => ({ state: { method: 'email' } })
  };
});

describe('ConfirmReset: email и код подтверждения', () => {
  let alertMock: ReturnType<typeof vi.spyOn>;

  beforeEach(() => {
    alertMock = vi.spyOn(window, 'alert').mockImplementation(() => {});
  });

  it('валидирует email и код', async () => {
    render(
      <MemoryRouter>
        <ConfirmReset />
      </MemoryRouter>
    );

    fireEvent.change(screen.getByPlaceholderText('Введите почту'), {
      target: { value: 'test@test.test' }
    });
    fireEvent.click(screen.getByText('Проверить'));

    expect(await screen.findByText('Почта подтверждена')).toBeInTheDocument();

    const codeInput = screen.getByPlaceholderText('6-значный код');

    // Неверный код
    fireEvent.change(codeInput, { target: { value: '000000' } });
    fireEvent.click(screen.getByText('Продолжить'));
    expect(alertMock).toHaveBeenCalledWith('Неверный код');

    // Верный код
    fireEvent.change(codeInput, { target: { value: '123456' } });
    fireEvent.click(screen.getByText('Продолжить'));
    expect(alertMock).toHaveBeenCalledWith('Код подтверждён!');
  });
});
