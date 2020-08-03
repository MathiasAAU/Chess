﻿using System;
using System.Collections.Generic;
using Chess.Model;
using NUnit.Framework;

namespace UnitTests {
    public class BoardTest {
        private Board _board;
        private List<Piece> _pieces;

        [SetUp]
        public void setup() {
            this._board = new Board();
        }

        [Test]
        public void should_return_correct_square() {
            int x = 0;
            int y = 0;
            Square square = this._board.getSquare(x, y);
            Assert.AreEqual(x, square.x);
            Assert.AreEqual(y, square.y);

            x = y = 7;
            square = this._board.getSquare(x, y);
            Assert.AreEqual(x, square.x);
            Assert.AreEqual(y, square.y);
        }

        [Test]
        public void out_of_bounds_square_should_throw_exception() {
            var ex1 = Assert.Throws<Exception>(() => this._board.getSquare(8, 0));
            Assert.That(ex1.Message, Is.EqualTo("Index out of bounds"));
            
            var ex2 = Assert.Throws<Exception>(() => this._board.getSquare(0, 8));
            Assert.That(ex2.Message, Is.EqualTo("Index out of bounds"));

            var ex3 = Assert.Throws<Exception>(() => this._board.getSquare(-1, 0));
            Assert.That(ex3.Message, Is.EqualTo("Index out of bounds"));
            
            var ex4 = Assert.Throws<Exception>(() => this._board.getSquare(0, -1));
            Assert.That(ex4.Message, Is.EqualTo("Index out of bounds"));
        }
        

        [Test]
        public void board_should_be_cleared() {
            this._board.clearBoard();
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (this._board.squares[i][j].piece != null) {
                        Assert.Fail();
                    }
                }
            }
            Assert.True(this._board.deadPieces.Count == 0);
        }

        [Test]
        public void white_king_should_be_at_4_0() {
            Square kingSquare = this._board.getKingSquare(true);
            Assert.AreEqual(4, kingSquare.x);
            Assert.AreEqual(0, kingSquare.y);
        }
        
        [Test]
        public void black_king_should_be_at_4_7() {
            Square kingSquare = this._board.getKingSquare(false);
            Console.WriteLine(kingSquare.x);
            Assert.AreEqual(4, kingSquare.x);
            Assert.AreEqual(7, kingSquare.y);
        }
        
        [Test]
        public void white_king_should_not_be_in_check() {
            Square kingSquare = this._board.getKingSquare(true);
            Assert.False(this._board.isChecked((King) kingSquare.piece, kingSquare));
        }
        
        [Test]
        public void black_king_should_not_be_in_check() {
            Square kingSquare = this._board.getKingSquare(false);
            Assert.False(this._board.isChecked((King) kingSquare.piece, kingSquare));
        }
        
    }
}