﻿using System;
 using Newtonsoft.Json.Linq;

 namespace Chess.Model {
    public class Player {
        private bool _isWhite;
        private int _score;

        public Player(bool isWhite = false) {
            this._isWhite = isWhite;
        }

        public JObject toJson() {
            dynamic player = new JObject();
            player.isWhite = this._isWhite;
            player.score = this._score;
            return player;
        }

        public bool isWhite {
            get => this._isWhite;
            set => this._isWhite = value;
        }

        public int score {
            get => this._score;
            set => this._score = value;
        }
    }

    public class Move {
        private Player _player;
        private Square _origin;
        private Square _dest;
        private Piece _piece;
        private Piece _destPiece;
        private bool _isCastlingMove;

        public Move(Player player, Square origin, Square dest) {
            this._player = player;
            this._origin = origin;
            this._dest = dest;
            this._piece = origin.piece;
            this._destPiece = dest.piece;
            this._isCastlingMove = this.checkForCastle();
        }

        public JObject toJson() {
            dynamic move = new JObject();
            move.player = this._player.toJson();
            move.origin = this._origin.toJson();
            move.dest = this._dest.toJson();
            move.piece = this._piece.getUniCode();
            move.isCastleMove = this._isCastlingMove;
            return move;
        }

        // Returns true if it is a castle move
        private bool checkForCastle() {
            return this.piece != null && this.destPiece != null &&
                   this.piece.isWhite == this.destPiece.isWhite &&
                   this.piece.GetType() == typeof(King) && this.destPiece.GetType() == typeof(Rook);
        }
        
        
        public bool isCastlingMove => this._isCastlingMove;

        public Player player => this._player;

        public Square origin => this._origin;

        public Square dest => this._dest;

        public Piece piece => this._piece;

        public Piece destPiece => this._destPiece;
    }
}
