﻿﻿using System;
using System.Collections.Generic;
 using Newtonsoft.Json.Linq;

 namespace Chess.Model {
    /*                    BLACK
     *        A   B   C   D   E   F   G   H  
     * 
     *  8   |_R_|_H_|_B_|_Q_|_K_|_B_|_H_|_R_|  7
     *  7   |_P_|_P_|_P_|_P_|_P_|_P_|_P_|_P_|  6
     *  6   |___|___|___|___|___|___|___|___|  5
     *  5   |___|___|___|___|___|___|___|___|  4
     *  4   |___|___|___|___|___|___|___|___|  3
     *  3   |___|___|___|___|___|___|___|___|  2
     *  2   |_P_|_P_|_P_|_P_|_P_|_P_|_P_|_P_|  1
     *  1   |_R_|_H_|_B_|_Q_|_K_|_B_|_H_|_R_|  0
     *
     *        0   1   2   3   4   5   6   7
     * 
     *                    WHITE
     */
    public class Board {
        private Square[][] _squares = new Square[8][];
        private List<Piece> _deadPieces = new List<Piece>();

        public Board() {
            for (int i = 0; i < 8; i++) {
                this._squares[i] = new Square[8];
            }
            this.initializeBoard();
        }
        
        // Find a specific square
        public Square getSquare(int x, int y) {
            if (x < 0 || x > 7 || y < 0 || y > 7) {
                throw new Exception("Index out of bounds");
            }
            return this.squares[x][y];
        }

        private void initializeBoard() {
            this.initializePieces(true);  // White pieces
            this.initializePieces(false); // Black pieces

            // Empty squares
            for (int i = 0; i < 8; i++) {
                for (int j = 2; j < 6; j++) {
                    this.squares[i][j] = new Square(null, i, j);
                }
            }
        }
        
        public void resetBoard() {
            this.deadPieces.Clear();
            this.initializeBoard();
        }
        
        public JArray boardToJson() {
            JArray squaresX = new JArray();
            for (int i = 0; i < 8; i++) {
                JArray squaresY = new JArray();
                for (int j = 0; j < 8; j++) {
                    squaresY.Add(this._squares[i][j].toJSON());
                }
                squaresX.Add(squaresY);
            }
            return squaresX;
        }

        public List<Square> getAccessibleSquares(int x, int y) {
            List<Square> accessibleSquares = new List<Square>();
            Square origin = this.getSquare(x, y);
            if (origin.piece == null) return accessibleSquares;
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (origin.piece.canMove(this, origin, this.getSquare(i, j))) {
                        accessibleSquares.Add(this.getSquare(i, j));
                    }
                }
            }
            return accessibleSquares;
        }

        public bool isChecked(King king, Square kingSquare) {
            for (var i = 0; i < 8; i++) {
                for (var j = 0; j < 8; j++) {
                    if (this.squares[i][j].piece != null &&
                        this.squares[i][j].piece.isWhite != king.isWhite) {
                        if(this.squares[i][j].piece.canAttack(this, this.squares[i][j], kingSquare)) {
                            return true;
                        } 
                    }
                }
            }
            return false;
        }

        public bool isCheckMate(Player player) {
            bool isWhite = player.isWhite;
            King playerKing = this.getKing(isWhite);
            Square kingSquare = this.getKingSquare(isWhite);

            if (!this.isChecked(playerKing, kingSquare)) {
                return false;
            }

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (this.squares[i][j].piece != null && this.squares[i][j].piece.isWhite == isWhite &&
                        this.pieceCanStopCheck(kingSquare, this.squares[i][j], isWhite)) {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool pieceCanStopCheck(Square kingSquare, Square square, bool isWhite) {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (square.piece.canMove(this, square, this.squares[i][j]) &&
                        !this.canAnyPieceAttack(kingSquare, !isWhite)) {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool canAnyPieceAttack(Square square, bool isWhite) {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (this.squares[i][j].piece != null 
                        && this.squares[i][j].piece.canAttack(this, this.squares[i][j], square)){
                        return true;
                    }
                }
            }
            return false;
        }
        

        public bool moveCreatesCheck(Square origin, Square dest) {
            // Copies pieces and simulates outcome
            Piece oPiece = origin.piece;
            Piece dPiece = dest.piece;
            this.squares[origin.x][origin.y].piece = null;
            this.squares[dest.x][dest.y].piece = oPiece;
            
            // True: new move makes the king be in check
            bool isChecked = this.isChecked(this.getKing(oPiece.isWhite), this.getKingSquare(oPiece.isWhite));

            // Reverts the simulated state of the board
            this.squares[origin.x][origin.y].piece = oPiece;
            this.squares[dest.x][dest.y].piece = dPiece;
            
            return isChecked;
        }
        
        public Square getKingSquare(bool isWhite) {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (this.squares[i][j].piece != null && 
                        this.squares[i][j].piece.GetType() == typeof(King) &&
                        this.squares[i][j].piece.isWhite == isWhite) {
                        return this.squares[i][j];
                    }
                }
            }
            throw new Exception("No king found");
        }

        private King getKing(bool isWhite) {
            return (King) this.getKingSquare(isWhite).piece;
        }


        public bool isFreePath(Square origin, Square dest) {
            if (origin.piece == null) {
                throw new Exception("No piece selected");
            }
            Type type = origin.piece.GetType();
            var xPositive = origin.x < dest.x;
            var yPositive = origin.y < dest.y;
            var nextX = xPositive ? origin.x + 1 : origin.x - 1;
            var nextY = yPositive ? origin.y + 1 : origin.y - 1;
            
            // Diagonal move (Bishop or queen)
            if (type == typeof(Bishop) || type == typeof(Queen)) {
                if (origin.x != dest.x && origin.y != dest.y) {
                    while (nextX != dest.x && nextY != dest.y) {
                        if (this.getSquare(nextX, nextY).piece != null) {
                            return false;
                        }
                        nextX = xPositive ? nextX + 1 : nextX - 1;
                        nextY = yPositive ? nextY + 1 : nextY - 1;
                    }
                    return true;
                }
            }

            // Rook or queen move y direction (or king castle)
            if (origin.x == dest.x) {
                while (nextY != dest.y) {
                    if (this.getSquare(origin.x, nextY).piece != null) {
                        return false;
                    }
                    nextY = yPositive ? nextY + 1 : nextY - 1;
                }
                return true;
            }
            // Rook or queen move x direction (or king castle)
            if (origin.y == dest.y) {
                while (nextX != dest.x) {
                    if (this.getSquare(nextX, origin.y).piece != null) {
                        return false;
                    }
                    nextX = xPositive ? nextX + 1 : nextX - 1;
                }
                return true;
            }
            return false;
        }

        // Returns true if the two squares the kings moves during castle is NOT in control by an enemy piece and no friendly pieces are blocking
        public bool isFreeCastlePath(Square origin, Square dest) {
            int newKingX = origin.x < dest.x ? origin.x + 2 : origin.x - 2;
            int direction = newKingX > origin.x ? 1 : -1;
            int nextX = origin.x + direction;
            
            while (nextX != newKingX) {
                if (this.isChecked((King) origin.piece, this.getSquare(nextX, origin.y)) || this.getSquare(nextX, origin.y).piece != null) {
                    return false;
                }
                nextX += direction;
            }
            return true;
        }

        private void initializePieces(bool isWhite) {
            var pawnRow = 6;
            var kingRow = 7;
            
            if (isWhite) {
                pawnRow = 1;
                kingRow = 0;
            }
            
            this.squares[0][kingRow] = new Square(new Rook(isWhite), 0, kingRow);
            this.squares[1][kingRow] = new Square(new Knight(isWhite), 1, kingRow);
            this.squares[2][kingRow] = new Square(new Bishop(isWhite), 2, kingRow);
            this.squares[3][kingRow] = new Square(new Queen(isWhite), 3, kingRow);
            this.squares[4][kingRow] = new Square(new King(isWhite), 4, kingRow);
            this.squares[5][kingRow] = new Square(new Bishop(isWhite), 5, kingRow);
            this.squares[6][kingRow] = new Square(new Knight(isWhite), 6, kingRow);
            this.squares[7][kingRow] = new Square(new Rook(isWhite), 7, kingRow);

            for (int i = 0; i < 8; i++) {
                this.squares[i][pawnRow] = new Square(new Pawn(isWhite), i, pawnRow);
            }
        }

        public void clearBoard() {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    this.squares[i][j] = new Square(null, i, j);
                }
            }
            this.deadPieces.Clear();
        }

        public List<Piece> deadPieces {
            get => this._deadPieces;
            set => this._deadPieces = value;
        }


        public Square[][] squares {
            get => this._squares;
            set => this._squares = value;
        }
        
    }
}