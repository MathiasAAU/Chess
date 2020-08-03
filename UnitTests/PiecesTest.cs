using System.Collections.Generic;
using System.Linq;
using Chess.Model;
using NUnit.Framework;

namespace UnitTests {
    public class PiecesTests {
        private Board _board;
        private List<Piece> _pieces;

        [SetUp]
        public void setup() {
            this._board = new Board();
            
            // Puts all the pieces into a list
            this._pieces = new List<Piece>();
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (this._board.squares[i][j].piece != null) {
                        this._pieces.Add(this._board.squares[i][j].piece);
                    }
                }
            }
        }

        [Test]
        public void white_pieces_should_be_in_correct_spots() {
            int y = 0;
            Assert.True(this._board.squares[0][y].piece.GetType() == typeof(Rook));
            Assert.True(this._board.squares[1][y].piece.GetType() == typeof(Knight));
            Assert.True(this._board.squares[2][y].piece.GetType() == typeof(Bishop));
            Assert.True(this._board.squares[3][y].piece.GetType() == typeof(Queen));
            Assert.True(this._board.squares[4][y].piece.GetType() == typeof(King));
            Assert.True(this._board.squares[5][y].piece.GetType() == typeof(Bishop));
            Assert.True(this._board.squares[6][y].piece.GetType() == typeof(Knight));
            Assert.True(this._board.squares[7][y].piece.GetType() == typeof(Rook));
        }
        
        [Test]
        public void black_pieces_should_be_in_correct_spots() {
            int y = 7;
            Assert.True(this._board.squares[0][y].piece.GetType() == typeof(Rook));
            Assert.True(this._board.squares[1][y].piece.GetType() == typeof(Knight));
            Assert.True(this._board.squares[2][y].piece.GetType() == typeof(Bishop));
            Assert.True(this._board.squares[3][y].piece.GetType() == typeof(Queen));
            Assert.True(this._board.squares[4][y].piece.GetType() == typeof(King));
            Assert.True(this._board.squares[5][y].piece.GetType() == typeof(Bishop));
            Assert.True(this._board.squares[6][y].piece.GetType() == typeof(Knight));
            Assert.True(this._board.squares[7][y].piece.GetType() == typeof(Rook));
        }

        [Test]
        public void white_pawns_should_be_at_y1() {
            for (int i = 0; i < 8; i++) {
                Piece piece = this._board.squares[i][1].piece;
                if (!(piece.GetType() == typeof(Pawn)) && !piece.isWhite) {
                    Assert.Fail();
                }
            }
            Assert.Pass();
        }
        
        [Test]
        public void black_pawns_should_be_at_y6() {
            for (int i = 0; i < 8; i++) {
                Piece piece = this._board.squares[i][6].piece;
                if (!(piece.GetType() == typeof(Pawn)) && piece.isWhite) {
                    Assert.Fail();
                }
            }
            Assert.Pass();
        }

        [Test]
        public void should_be_32_pieces() {
            int total = 32;
            Assert.AreEqual(total, this._pieces.Count);
        }
        
        [Test]
        public void should_be_equal_white_and_black_pieces() {
            int whiteCount = 0;
            int blackCount = 0;

            foreach (Piece piece in this._pieces) {
                if (piece.isWhite) {
                    whiteCount++;
                } else if (!piece.isWhite) {
                    blackCount++;
                }
            }
            Assert.AreEqual(whiteCount, blackCount);
        }

        [Test]
        public void should_be_16_pawns() {
            int total = 16;
            int count = this._pieces.Count(piece => piece != null && piece.GetType() == typeof(Pawn));

            Assert.AreEqual(total,count);
        }
        
        [Test]
        public void should_be_equal_white_and_black_pawns() {
            int whiteCount = 0;
            int blackCount = 0;

            foreach (Piece piece in this._pieces) {
                if (piece.isWhite && piece.GetType() == typeof(Pawn)) {
                    whiteCount++;
                } else if (!piece.isWhite && piece.GetType() == typeof(Pawn)) {
                    blackCount++;
                }
            }
            Assert.AreEqual(whiteCount, blackCount);
        }

        [Test]
        public void should_be_4_knights() {
            int total = 4;
            int count = this._pieces.Count(piece => piece.GetType() == typeof(Knight));
            Assert.AreEqual(total, count);
        }
        
        [Test]
        public void should_be_equal_white_and_black_knights() {
            int whiteCount = 0;
            int blackCount = 0;

            foreach (Piece piece in this._pieces) {
                if (piece.isWhite && piece.GetType() == typeof(Knight)) {
                    whiteCount++;
                } else if (!piece.isWhite && piece.GetType() == typeof(Knight)) {
                    blackCount++;
                }
            }
            Assert.AreEqual(whiteCount, blackCount);
        }
        
        [Test]
        public void should_be_4_bishops() {
            int total = 4;
            int count = this._pieces.Count(piece => piece.GetType() == typeof(Knight));
            Assert.AreEqual(total, count);
        }
        
        [Test]
        public void should_be_equal_white_and_black_bishops() {
            int whiteCount = 0;
            int blackCount = 0;

            foreach (Piece piece in this._pieces) {
                if (piece.isWhite && piece.GetType() == typeof(Bishop)) {
                    whiteCount++;
                } else if (!piece.isWhite && piece.GetType() == typeof(Bishop)) {
                    blackCount++;
                }
            }
            Assert.AreEqual(whiteCount, blackCount);
        }
        
        [Test]
        public void should_be_4_rooks() {
            int total = 4;
            int count = this._pieces.Count(piece => piece.GetType() == typeof(Knight));
            Assert.AreEqual(total, count);
        }
        
        [Test]
        public void should_be_equal_white_and_black_rooks() {
            int whiteCount = 0;
            int blackCount = 0;

            foreach (Piece piece in this._pieces) {
                if (piece.isWhite && piece.GetType() == typeof(Rook)) {
                    whiteCount++;
                } else if (!piece.isWhite && piece.GetType() == typeof(Rook)) {
                    blackCount++;
                }
            }
            Assert.AreEqual(whiteCount, blackCount);
        }
        
        [Test]
        public void should_be_2_queens() {
            int total = 4;
            int count = this._pieces.Count(piece => piece.GetType() == typeof(Knight));
            Assert.AreEqual(total, count);
        }
        
        [Test]
        public void should_be_equal_white_and_black_queens() {
            int whiteCount = 0;
            int blackCount = 0;

            foreach (Piece piece in this._pieces) {
                if (piece.isWhite && piece.GetType() == typeof(Queen)) {
                    whiteCount++;
                } else if (!piece.isWhite && piece.GetType() == typeof(Queen)) {
                    blackCount++;
                }
            }
            Assert.AreEqual(whiteCount, blackCount);
        }
        
        [Test]
        public void should_be_2_kings() {
            int total = 4;
            int count = this._pieces.Count(piece => piece.GetType() == typeof(Knight));
            Assert.AreEqual(total, count);
        }
        
        [Test]
        public void should_be_equal_white_and_black_kings() {
            int whiteCount = 0;
            int blackCount = 0;

            foreach (Piece piece in this._pieces) {
                if (piece.isWhite && piece.GetType() == typeof(King)) {
                    whiteCount++;
                } else if (!piece.isWhite && piece.GetType() == typeof(King)) {
                    blackCount++;
                }
            }
            Assert.AreEqual(whiteCount, blackCount);
        }
        
        
    }
}