import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import React from "react";
import { MemoryRouter } from "react-router-dom";
import GameBoard from "../../components/game/GameBoard/GameBoard";
import { AuthProvider } from "../../contexts/AuthContext";
import { GameProvider } from "../../contexts/GameContext";

// Mock the API calls
jest.mock("../../api/gameApi", () => ({
  gameApi: {
    getGame: jest.fn().mockResolvedValue({
      id: "game1",
      name: "Test Game",
      players: ["player1", "player2"],
      status: "in_progress",
      currentPlayerId: "player1",
      turnNumber: 1,
    }),
    getGameState: jest.fn().mockResolvedValue({
      id: "gameState1",
      gameId: "game1",
      drawPileCount: 40,
      discardPile: [],
      playerHands: {
        player1: [
          {
            id: "card1",
            type: "attack",
            name: "Attack",
            effect: "End your turn without drawing. Next player takes 2 turns.",
            imageUrl: "/images/attack.png",
          },
          {
            id: "card2",
            type: "skip",
            name: "Skip",
            effect: "End your turn without drawing a card.",
            imageUrl: "/images/skip.png",
          },
        ],
        player2: [
          // Player 2 cards (not visible to player 1)
        ],
      },
      explodedPlayers: [],
      attackCount: 0,
      lastAction: "Game started",
      updatedAt: new Date().toISOString(),
      currentPlayerId: "player1",
      turnNumber: 1,
    }),
    playCard: jest.fn().mockResolvedValue({}),
    drawCard: jest.fn().mockResolvedValue({}),
    playCombo: jest.fn().mockResolvedValue({}),
    seeFuture: jest.fn().mockResolvedValue({
      topCards: [
        {
          id: "card3",
          type: "defuse",
          name: "Defuse",
          effect: "Save yourself from an exploding kitten.",
          imageUrl: "/images/defuse.png",
        },
        {
          id: "card4",
          type: "favor",
          name: "Favor",
          effect: "Force another player to give you a card.",
          imageUrl: "/images/favor.png",
        },
        {
          id: "card5",
          type: "exploding_kitten",
          name: "Exploding Kitten",
          effect: "You explode! Unless you have a defuse card.",
          imageUrl: "/images/exploding.png",
        },
      ],
    }),
  },
}));

// Mock localStorage
beforeEach(() => {
  const mockUser = {
    id: "player1",
    username: "Player 1",
  };
  jest.spyOn(Storage.prototype, "getItem").mockImplementation((key) => {
    if (key === "user") {
      return JSON.stringify(mockUser);
    }
    if (key === "token") {
      return "mock-token";
    }
    return null;
  });
});

afterEach(() => {
  jest.restoreAllMocks();
});

describe("GameBoard Integration Tests", () => {
  test("renders game board and player hand", async () => {
    render(
      <MemoryRouter>
        <AuthProvider>
          <GameProvider>
            <GameBoard gameId="game1" />
          </GameProvider>
        </AuthProvider>
      </MemoryRouter>
    );

    // Wait for the game to load
    await waitFor(() => {
      expect(screen.getByText("Test Game")).toBeInTheDocument();
    });

    // Check game info is displayed
    expect(screen.getByText("Status: in_progress")).toBeInTheDocument();
    expect(screen.getByText("Turn: 1")).toBeInTheDocument();
    expect(screen.getByText("Current Player: You")).toBeInTheDocument();

    // Check player hand is displayed
    expect(screen.getByText("Your Hand")).toBeInTheDocument();
    expect(screen.getByText("Attack")).toBeInTheDocument();
    expect(screen.getByText("Skip")).toBeInTheDocument();

    // Check draw pile and discard pile are displayed
    expect(screen.getByText("Draw Pile (40)")).toBeInTheDocument();
    expect(screen.getByText("Discard Pile (0)")).toBeInTheDocument();
  });

  test("allows playing a card", async () => {
    render(
      <MemoryRouter>
        <AuthProvider>
          <GameProvider>
            <GameBoard gameId="game1" />
          </GameProvider>
        </AuthProvider>
      </MemoryRouter>
    );

    // Wait for the game to load
    await waitFor(() => {
      expect(screen.getByText("Test Game")).toBeInTheDocument();
    });

    // Find the Attack card and play it
    const attackCard = screen.getByText("Attack");
    fireEvent.click(attackCard);

    // Should call the playCard function
    await waitFor(() => {
      expect(
        require("../../api/gameApi").gameApi.playCard
      ).toHaveBeenCalledWith("game1", {
        playerId: "player1",
        cardId: "card1",
        targetPlayerId: null,
      });
    });
  });

  test("allows drawing a card", async () => {
    render(
      <MemoryRouter>
        <AuthProvider>
          <GameProvider>
            <GameBoard gameId="game1" />
          </GameProvider>
        </AuthProvider>
      </MemoryRouter>
    );

    // Wait for the game to load
    await waitFor(() => {
      expect(screen.getByText("Test Game")).toBeInTheDocument();
    });

    // Find the draw button and click it
    const drawButton = screen.getByText("Draw a Card");
    fireEvent.click(drawButton);

    // Should call the drawCard function
    await waitFor(() => {
      expect(
        require("../../api/gameApi").gameApi.drawCard
      ).toHaveBeenCalledWith("game1", {
        playerId: "player1",
      });
    });
  });
});
