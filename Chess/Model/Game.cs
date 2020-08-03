﻿using System;
using System.Collections.Generic;
 using Newtonsoft.Json.Linq;

 namespace Chess.Model {
    public class Game {
        private Player _p1;
        private Player _p2;
        private Player _currentTurn;
        private Board _board;
        private GameState _state;
        private List<Move> _moves = new List<Move>();

        public JObject toJSON() {
            dynamic game = new JObject();
            game.player1 = this._p1.toJSON();
            game.player2 = this._p2.toJSON();
            game.currentTurn = this._currentTurn.isWhite;
            game.state = this._state.ToString();
            game.board = this._board.boardToJson();
            return game;
        }
       
        
        public Game(Player p1, Player p2) {
            this.board = new Board();
            this.resetGame(p1, p2, false);
        }
        
        public bool move(Player player, int originX, int originY, int destX, int destY) {
            Square origin = this.board.getSquare(originX, originY);
            Square dest = this.board.getSquare(destX, destY);
            Move move = new Move(player, origin, dest);
            return this.performMove(move);
        }

        private bool performMove(Move move) {
            Player player = move.player;
            
            // Returns false if not piece selected, not the right player moving or moving the opposite players piece
            if (move.piece == null || this.currentTurn != player || move.piece.isWhite != player.isWhite) {
                return false;
            }

            // Returns false if illegal move
            if (!move.piece.canMove(this.board, move.origin, move.dest)) {
                return false;
            }
            
            // If a piece is killed it is added to the dead list and players score is updated
            if (move.destPiece != null && move.destPiece.isWhite != player.isWhite) {
                move.destPiece.isKilled = true;
                this.board.deadPieces.Add(move.destPiece);
                this.currentTurn.score += move.destPiece.value;
            }
            
            // If its a castling move
            if (move.isCastlingMove) {
                var king = (King) move.piece;
                if (king.canCastle(move.destPiece)) {
                    // Finds the new column for the king and rook
                    int xForKing = move.dest.x == 0 ? 2 : 6;
                    int xForRook = xForKing == 2 ? 3 : 5;

                    // Moves the king and rook to their new squares and remove them from the [origin] and [dest] square
                    this.board.squares[xForKing][move.origin.y].piece = move.piece;
                    this.board.squares[xForRook][move.origin.y].piece = move.destPiece;
                    move.origin.piece = null;
                    move.dest.piece = null;
                    
                    // Updates the pieces
                    move.piece.hasMoved = true;
                    move.destPiece.hasMoved = true;
                    
                } else return false;
            }
            else {
                // Performs the move
                move.piece.hasMoved = true;
                move.dest.piece = move.piece;
                move.origin.piece = null;

                // Move creates checkmate
                if (this._board.isCheckMate(this._currentTurn == this._p1 ? this._p2 : this._p1)) {
                    this._state = (this._currentTurn.isWhite ? GameState.WhiteWin : GameState.BlackWin);
                }

                // Changes the state of the game if the king is killed
                if (move.destPiece != null && move.destPiece.GetType() == typeof(King)) {
                    this._state = player.isWhite ? GameState.WhiteWin : GameState.BlackWin;
                }
                
                // If a pawn have made it to the last rank, then it becomes a queen
                int enemyKingRank = player.isWhite ? 7 : 0;
                if (move.piece.GetType() == typeof(Pawn) && move.dest.y == enemyKingRank) {
                    move.dest.piece = new Queen(player.isWhite);
                }
            }
            
            // Change of turn
            this.moves.Add(move);
            this.currentTurn = this.currentTurn == this.p1 ? this.p2 : this.p1;
            return true;
        }

        //TODO FIX
        public void startGame() {

            while (this.state == GameState.Active) {
                
            }
        }
        

        public void resetGame(Player p1, Player p2, bool changeColor) {
            if (p1 == null || p2 == null) {
                throw new Exception("Two players are needed");
            }
            
            if (p1.isWhite == p2.isWhite) {
                throw new Exception("Players cannot have the same color: " + (p1.isWhite ? "White" : "Black"));
            }

            // If the players want to change side
            if (changeColor) {
                p1.isWhite = !p1.isWhite;
                p2.isWhite = !p2.isWhite;
            }
            
            this.p1 = p1;
            this.p2 = p2;

            this.p1.score = 0;
            this.p2.score = 0;

            this.state = GameState.Active;
            this.moves.Clear();
            this.board.resetBoard();
            
            this.currentTurn = this.p1.isWhite ? this.p1 : this.p2;
        }

        public Player currentTurn {
            get => this._currentTurn;
            set => this._currentTurn = value;
        }


        public Player p1 {
            get => this._p1;
            set => this._p1 = value;
        }

        public Player p2 {
            get => this._p2;
            set => this._p2 = value;
        }

        public Board board {
            get => this._board;
            set => this._board = value;
        }

        public GameState state {
            get => this._state;
            set => this._state = value;
        }

        public List<Move> moves {
            get => this._moves;
            set => this._moves = value;
        }
    }
}