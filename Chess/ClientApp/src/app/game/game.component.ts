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
  }

  public game: Game;
  public http: HttpClient;
  public baseUrl: string;
  public squares: any;
  public pieceIsSelected: any;
  public selectedPieceId: any;

  private static getSquareId(square: Square) {
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
          selected.className = 'blue';
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
      document.getElementById(GameComponent.getSquareId(square)).className = 'green';
    });
  }

  // Clears any choice a player has made
  public clearChoice() {
    this.pieceIsSelected = false;
    // Resets the color of all squares
    this.game.board.forEach(function (column) {
      column.forEach(function (square) {
        let classColor = 'white';
        if ((square.x + square.y) % 2 === 0) { classColor = 'black'; }
        document.getElementById(GameComponent.getSquareId(square)).className = classColor;
      });
    });
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

}



interface Game {
  player1:     Player;
  player2:     Player;
  currentTurn: boolean;
  state:       string;
  board:       Square[][];
}

interface Square {
  x:     number;
  y:     number;
  piece: string;
}

interface Player {
  isWhite: boolean;
  score:   number;
}

interface DeadPiece {
  isWhite: boolean;
  piece:   string;
}
