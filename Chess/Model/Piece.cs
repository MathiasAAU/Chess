﻿﻿using System;

 namespace Chess.Model {
    public abstract class Piece {
        private bool _isKilled = false;
        private bool _isWhite = false;
        private bool _hasMoved = false;
        private int _value = -1;

        public Piece(bool white) {
            this.isWhite = white;
        }

        public abstract bool canMove(Board board, Square origin, Square dest);
        
        public abstract bool canAttack(Board board, Square origin, Square dest);

        public abstract string getUniCode();

        public bool isKilled {
            get => this._isKilled;
            set => this._isKilled = value;
        }

        public bool isWhite {
            get => this._isWhite;
            set => this._isWhite = value;
        }

        public int value {
            get => this._value;
            set => this._value = value;
        }

        public bool hasMoved {
            get => this._hasMoved;
            set => this._hasMoved = value;
        }
    }

    public class King : Piece {
        
        public King(bool white) : base(white) {
            this.value = 100; 
        }
        
        public override bool canMove(Board board, Square origin, Square dest) {

            // Returns false if moved to square with friendly piece which is not a rook
            if (dest.piece != null && dest.piece.isWhite == this.isWhite) {
                // Returns true if legal castle move
                return dest.piece.GetType() == typeof(Rook) && this.canCastle(dest.piece) && 
                       board.isFreeCastlePath(origin, dest);
            }

            // Move one square in every direction as long as its not in check
            var x = Math.Abs(origin.x - dest.x); 
            var y = Math.Abs(origin.y - dest.y);

            // Returns true if move is legal and updates [canCastle]
            if (x <= 1 && y <= 1 && !board.isChecked(this, dest)) {
                return true;
            }
            return false;
        }

        public override bool canAttack(Board board, Square origin, Square dest) {
            return this.canMove(board, origin, dest);
        }

        public override string getUniCode() {
            return this.isWhite ? "\u2654" : "\u265A";
        }

        // King can castle if the king and rook has not moved
        public bool canCastle(Piece rook) {
            return !this.hasMoved && !rook.hasMoved;
        }
    }
    
    public class Queen : Piece {
        public Queen(bool white) : base(white) {
            this.value = 9;
        }

        public override bool canMove(Board board, Square origin, Square dest) {
            if (dest.piece != null && dest.piece.isWhite == this.isWhite) {
                return false;
            }
            
            // Move diagonal
            if (Math.Abs(origin.x - dest.x) == Math.Abs(origin.y - dest.y)) {
                return board.isFreePath(origin, dest) && !board.moveCreatesCheck(origin, dest);
            }
            // Move one same row or column
            return (origin.x == dest.x || origin.y == dest.y) && board.isFreePath(origin, dest) &&
                   !board.moveCreatesCheck(origin, dest);
        }
        
        public override bool canAttack(Board board, Square origin, Square dest) {
            return this.canMove(board, origin, dest);
        }
        
        public override string getUniCode() {
            return this.isWhite ? "\u2655" : "\u265B";
        }
    }
    
    public class Rook : Piece {
        public Rook(bool white) : base(white) {
            this.value = 5;
        }

        public override bool canMove(Board board, Square origin, Square dest) {
            // Returns false if the end square has same color piece
            if (dest.piece != null && dest.piece.isWhite == this.isWhite) {
                return false;
            }
            
            // Returns false if moved in both directions
            if (origin.x != dest.x ^ origin.y != dest.y) {
                return board.isFreePath(origin, dest) && !board.moveCreatesCheck(origin, dest);
            }
            return false;
        }
        
        public override bool canAttack(Board board, Square origin, Square dest) {
            return this.canMove(board, origin, dest);
        }
        
        public override string getUniCode() {
            return this.isWhite ? "\u2656" : "\u265C";
        }
    }

    public class Bishop : Piece {
        public Bishop(bool white) : base(white) {
            this.value = 3;
        }

        public override bool canMove(Board board, Square origin, Square dest) {
            // Returns false if the end square has same color piece
            if (dest.piece != null && dest.piece.isWhite == this.isWhite) {
                return false;
            } 
            
            // Returns true if moved diagonal
            return (Math.Abs(origin.x - dest.x) == Math.Abs(origin.y - dest.y)) && board.isFreePath(origin, dest) && 
                   !board.moveCreatesCheck(origin, dest);
        }
        
        public override bool canAttack(Board board, Square origin, Square dest) {
            return this.canMove(board, origin, dest);
        }
        
        public override string getUniCode() {
            return this.isWhite ? "\u2657" : "\u265D";
        }
    }
    
    public class Knight : Piece {
        public Knight(bool white) : base(white) {
            this.value = 3;
        }

        public override bool canMove(Board board, Square origin, Square dest) {
            // Returns false if the end square has same color piece
            if (dest.piece != null && dest.piece.isWhite == this.isWhite) {
                return false;
            }
            
            var x = Math.Abs(origin.x - dest.x);
            var y = Math.Abs(origin.y - dest.y);
            return x * y == 2 && !board.moveCreatesCheck(origin, dest);
        }
        
        public override bool canAttack(Board board, Square origin, Square dest) {
            return this.canMove(board, origin, dest);
        }
        
        public override string getUniCode() {
            return this.isWhite ? "\u2658" : "\u265E";
        }

    }

    public class Pawn : Piece {
        public Pawn(bool white) : base(white) {
            this.value = 1;
        }

        public override bool canMove(Board board, Square origin, Square dest) {
            var direction = this.isWhite == true ? 1 : -1; // Direction that its allowed to move 
            
            // If you move in a straight line 
            if (origin.x == dest.x && dest.piece == null) {
                // You are able to move two squares on first move
                if (!this.hasMoved && board.isFreePath(origin, dest)) { 
                    return dest.y - origin.y == direction || dest.y - origin.y == direction * 2;
                }
                return dest.y - origin.y == direction && !board.moveCreatesCheck(origin, dest);
            }

            // If able to attack
            return this.canAttack(board, origin, dest);
        }
        
        // Move one spot diagonal in the right direction ONLY if there is an enemy piece
        public override bool canAttack(Board board, Square origin, Square dest) {
            var direction = this.isWhite == true ? 1 : -1; // Direction that its allowed to move 

            if (dest.piece != null && dest.piece.isWhite != this.isWhite) {
                if (Math.Abs(origin.x - dest.x) == 1 && dest.y - origin.y == direction) {
                    return !board.moveCreatesCheck(origin, dest);
                }
            }
            return false;
        }
        
        public override string getUniCode() {
            return this.isWhite ? "\u2659" : "\u265F";
        }
    }
    
}