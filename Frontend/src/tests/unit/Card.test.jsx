// Card.test.jsx - Example Jest test
import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import Card from '../../components/game/Card/Card';

describe('Card Component', () => {
  test('renders card with correct name', () => {
    const mockOnPlay = jest.fn();
    render(
      <Card 
        id='card-1'
        type='defuse'
        name='Defuse Card'
        imageUrl='/images/defuse.png'
        onPlay={mockOnPlay}
      />
    );
    
    expect(screen.getByText('Defuse Card')).toBeInTheDocument();
  });

  test('calls onPlay when clicked', () => {
    const mockOnPlay = jest.fn();
    render(
      <Card 
        id='card-1'
        type='defuse'
        name='Defuse Card'
        imageUrl='/images/defuse.png'
        onPlay={mockOnPlay}
      />
    );
    
    fireEvent.click(screen.getByText('Defuse Card'));
    expect(mockOnPlay).toHaveBeenCalledWith('card-1');
  });
});
