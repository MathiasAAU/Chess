import {Component, Inject} from '@angular/core';
import {HttpClient} from '@angular/common/http';


@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.scss']
})

export class GameComponent {

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<string>(baseUrl + 'game/status').subscribe(result => {
      this.game = JSON.parse(JSON.stringify(result));
    }, error => console.error(error));

    this.http = http;
    this.baseUrl = baseUrl;
    this.squares = [7, 6, 5, 4, 3, 2, 1, 0];
    this.blackSquare = 'square black';
    this.whiteSquare = 'square white';
    this.blueSquare = 'square blue';
    this.greenSquare = 'square green';
  }

  public game: Game;
  public http: HttpClient;
  public baseUrl: string;
  public squares: any;
  public pieceIsSelected: any;
  public selectedPieceId: any;
  public greenSquare: string;
  public blueSquare: string;
  public blackSquare: string;
  public whiteSquare: string;
  public displayMoves: Move[];

  public getSquareId(square: Square) {
    return square.x + ',' + square.y;
  }

  public selectSquare(clickedId: string) {
    const selected = document.getElementById(clickedId);
    // If a piece has been selected and the square clicked is the destination
    if (this.pieceIsSelected) {
      // API call to move the piece
      this.http.post<any>(this.baseUrl + 'game/' + this.selectedPieceId + '/' + clickedId + '/move', {}).subscribe(result => {
        // API call to get the new game status
        if (result) {
          this.http.get<string>(this.baseUrl + 'game/status').subscribe(status => {
            this.game = JSON.parse(JSON.stringify(status));
          }, getError => console.error(getError));
        }
        // Clears the square colors to black/white
        this.clearChoice();
      }, postError => console.error(postError));

    } else {
      // API calls that returns true if the square clicked has a piece that is the right color
      this.http.get<any>(this.baseUrl + 'game/' + clickedId + '/' + this.game.currentTurn + '/currentTurnPiece').subscribe(validSquare => {
        if (validSquare) {
          // Updates variables and changes color of the clicked square
          selected.className = this.blueSquare;
          this.pieceIsSelected = true;
          this.selectedPieceId = clickedId;

          // API calls that returns the squares the selected piece is able to move to
          this.http.get<string>(this.baseUrl + 'game/' + clickedId + '/possibleSquares').subscribe(squares => {
            this.showPossibleSquares(JSON.parse(JSON.stringify(squares)), clickedId);
          }, error => console.error(error));
        }
      }, error => console.error(error));

    }
  }

  // Changes a list of squares color to green
  private showPossibleSquares(squares: Square[], clicked: string) {
    squares.forEach(function (square) {
      document.getElementById(this.getSquareId(square)).className = this.greenSquare;
    }, this);
  }

  // Clears any choice a player has made
  public clearChoice() {
    this.pieceIsSelected = false;
    // Resets the color of all squares
    this.game.board.forEach(function (column) {
      column.forEach(function (square) {
        let classColor = this.whiteSquare;
        if ((square.x + square.y) % 2 === 0) { classColor = this.blackSquare; }
        document.getElementById(this.getSquareId(square)).className = classColor;
      }, this);
    }, this);
  }

  // Gets the color for a square given the [x] and [y] coord
  public getSquareColor(x: number, y: number) {
    let color = 'white';
    if ((x + y) % 2 === 0) {
      color = 'black';
    }
    return color;
  }

  // Creates a new game
  public newGame() {
    this.http.post<any>(this.baseUrl + 'game/newGame', {}).subscribe(result => {
      this.http.get<string>(this.baseUrl + 'game/status').subscribe(status => {
        this.game = JSON.parse(JSON.stringify(status));
      }, getError => console.error(getError));
    }, postError => console.error(postError));
    this.clearChoice();
  }

  public getPlayer(isWhite: boolean) {
    return this.game.player1.isWhite === isWhite ? this.game.player1 : this.game.player2;
  }

  public getKilledPieces(isWhite: boolean) {
    let string = '';
    this.game.deadPieces.forEach(function (piece) {
      if (piece.isWhite !== isWhite) {
        string += piece.piece;
      }
    });
    return string;
  }

  public currentTurnColor() {
    return this.game.currentTurn ? 'White' : 'Black';
  }

  public displaySquare(x: number, y: number) {
    const displayY = y + 1;
    let displayX = '';
    switch (x) {
      case 0: displayX = 'A'; break;
      case 1: displayX = 'B'; break;
      case 2: displayX = 'C'; break;
      case 3: displayX = 'D'; break;
      case 4: displayX = 'E'; break;
      case 5: displayX = 'F'; break;
      case 6: displayX = 'G'; break;
      case 7: displayX = 'H'; break;
      default: displayX = 'ERROR'; break;
    }
    return displayX + displayY;
  }

}

interface Game {
  player1:     Player;
  player2:     Player;
  currentTurn: boolean;
  state:       string;
  board:       Square[][];
  deadPieces:  DeadPiece[];
  moves:       Move[];
}

interface Square {
  x:          number;
  y:          number;
  piece:      string;
}

interface Player {
  name:       string;
  isWhite:    boolean;
  score:      number;
}

interface DeadPiece {
  isWhite:    boolean;
  piece:      string;
}

interface Move {
  player:     Player;
  origin:     Square;
  dest:       Square;
  piece:      string;
  isCastle:   boolean;
}
