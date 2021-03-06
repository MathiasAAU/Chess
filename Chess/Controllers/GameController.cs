﻿using System;
using System.Collections.Generic;
using Chess.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;


namespace Chess.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase {
        private static Player _player1 = new Player(true);
        private static Player _player2 = new Player();
        private static Game _game = new Game(_player1, _player2);
        
        [HttpGet("status")]
        public string Get() {
            return _game.toJson().ToString();
        }
        
        [HttpGet("{id}/possibleSquares")]
        public string Get(string id) {
            string[] coords = id.Split(",");
            int x = int.Parse(coords[0]);
            int y = int.Parse(coords[1]);

            List<Square> squares = _game.board.getAccessibleSquares(x,y);
            bool chosenPieceIsWhite = _game.board.getSquare(x, y).piece.isWhite;
            if (squares.Count == 0 || chosenPieceIsWhite != _game.currentTurn.isWhite) return "";
            
            JArray array = new JArray();
            foreach(Square square in squares){
                array.Add(square.toJson());
            }
            return array.ToString();
        }
        
        // GET call that returns true if the piece matches the player
        [HttpGet("{id}/{currentTurn}/currentTurnPiece")]
        public bool Get(string id, bool currentTurn) {
            string[] coords = id.Split(",");
            int x = int.Parse(coords[0]);
            int y = int.Parse(coords[1]);

            return this.playerMatchPiece(x, y);
        }


        // POST call that performs a move and returns true if it is a legal move
        [HttpPost("{originId}/{destId}/move")]
        public bool Post(string originId, string destId) {
            string[] originCoords = originId.Split(",");
            int xOrigin = int.Parse(originCoords[0]);
            int yOrigin = int.Parse(originCoords[1]);
            
            string[] destCoords = destId.Split(",");
            int xDest = int.Parse(destCoords[0]);
            int yDest = int.Parse(destCoords[1]);
            
            if(!this.playerMatchPiece(xOrigin, yOrigin)) throw new Exception("Player and piece mismatch");
            
            return _game.move(_game.currentTurn, xOrigin, yOrigin, xDest, yDest);
        }

        // POST call that creates a new game
        [HttpPost("newGame")]
        public void Post() {
            _game.resetGame(_player1, _player2, false);
        }

        private bool playerMatchPiece(int x, int y) {
            return _game.board.getSquare(x, y).piece != null &&
                   _game.board.getSquare(x, y).piece.isWhite == _game.currentTurn.isWhite;
        }
        
        
    }
}