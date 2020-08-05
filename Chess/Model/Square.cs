﻿﻿using System;
  using System.Text.Json.Serialization;
  using Newtonsoft.Json.Linq;

  namespace Chess.Model {
    public class Square {
        private Piece _piece;
        private int _x;
        private int _y;

        public Square(Piece piece, int x, int y) {
            this._piece = piece;
            this._x = x;
            this._y = y;
        }

        public JObject toJson() {
            dynamic square = new JObject();
            square.x = this._x;
            square.y = this._y;
            square.piece = this._piece == null ? "" : this._piece.getUniCode();
            return square;
        }

        
        public Piece piece {
            get => this._piece;
            set => this._piece = value;
        }

        public int x {
            get => this._x;
            set => this._x = value;
        }

        public int y {
            get => this._y;
            set => this._y = value;
        }
    }
}