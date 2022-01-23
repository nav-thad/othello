// Navalan Thadchanamoorthy
// Objective: coding Othello game
// Date: 10/29/21

#nullable enable
using System;
using static System.Console;

namespace Bme121
{  
    class Player
    {
        public readonly string Colour;
        public readonly string Symbol;
        public readonly string Name;

        public Player( string Colour, string Symbol, string Name )
        {
            this.Colour = Colour;
            this.Symbol = Symbol;
            this.Name = Name;
        }
    }
    static class Program
    {
        static void Welcome( )  
        {
            WriteLine( "-=+-------------------------------------------------------------------+=-");
            WriteLine( "Welcome to Othello!" );
            WriteLine( "-=+-------------------------------------------------------------------+=-");
        }                                 

        static Player NewPlayer( string colour, string symbol, string defaultName ) 
        {
            Write( $" Enter the {colour} disc ({symbol}) player name, or press <return> for '{defaultName}': " );

            string name = ReadLine() !;

            if ( name.Length == 0 ) name = defaultName;

            return new Player( colour, symbol, name );
        }

        static int GetFirstTurn( Player[] players, int firstPlayer )  
        {
            while (true)
            {
                Write( "Select the player who will go first, or press <return> for {0}/{1}/{2}: ",
                    players[ firstPlayer ].Colour, players[ firstPlayer ].Symbol, 
                    players[ firstPlayer ].Name );
                
                string userInput = ReadLine() !;

                if ( userInput.Length == 0 ) return firstPlayer;

                for ( int i = 0; i < players.Length; i ++ )
                {
                    if ( players[ i ].Colour == userInput ) return i;
                    else if( players[ i ].Symbol == userInput ) return i;
                    else if( players[ i ].Name == userInput ) return i;
                }
                WriteLine( "Error: Please enter a valid response. " );
            }
        }

        static int GetBoardSize( string direction, int defaultSize )
        {
            while ( true )
            {
                Write( "Enter an even number of board {0} between 4-26, or press <return> for the default {1}:", 
                direction, defaultSize );

                string userInput = ReadLine() !;
                if (userInput.Length == 0 ) return defaultSize;

                int size = int.Parse( userInput );
                if ( size >= 4 && size <= 26 && size % 2 == 0 ) return size;

                WriteLine( "Error: Please enter an even board size between 4 and 26." );

            }
        }

        static string GetMove( Player player )
        {
            WriteLine();

            WriteLine( $"It is player {player.Colour}'s turn [ {player.Symbol} symbol {player.Name} ]" );

            WriteLine( "Select a move based on the row and column letters (ex. ab indicates row a and column b)." );
            WriteLine( "Alternatively, type 'skip' to forgo your turn, or 'quit' to end the game." );

            WriteLine();

            Write( "Enter your move: " );
            string inputedMove = ReadLine() !;

            return inputedMove;
        }

        static bool TryMove( string[ , ] board, Player player, string move)
        {
            if( move == "skip" ) return true;
            if( move.Length != 2 )
            {
                WriteLine( "Error: your move must be two letters indicating a row and column" );
                return false;
            }

            int inputedRow = IndexAtLetter( move.Substring(0, 1) );
            int inputedColumn = IndexAtLetter( move.Substring(1, 1) );

            if( inputedRow < 0 || inputedRow >= board.GetLength(1) ) 
            {
                WriteLine( "Error: the inputed row is not within the board size" );
                return false;
            }

            if( inputedColumn < 0 || inputedColumn >= board.GetLength(1) )
            {
                WriteLine( "Error: the inputed column is not within the board size" );
                return false;
            }

            if( board[ inputedRow, inputedColumn ] != " " )
            {
                WriteLine( "Error: the chosen cell already has a piece in it." );
                return false;            
            }
            else board[ inputedRow, inputedColumn ] = player.Symbol;

            bool isValid = false;

            isValid = TryDirection( board, player, inputedRow, -1, inputedColumn, 0 ) || isValid; // North
            isValid = TryDirection( board, player, inputedRow, 0, inputedColumn, 1 ) || isValid;  // East
            isValid = TryDirection( board, player, inputedRow, 1, inputedColumn, 0 ) || isValid;  // South
            isValid = TryDirection( board, player, inputedRow, 0, inputedColumn, -1 ) || isValid; // West

            isValid = TryDirection( board, player, inputedRow, -1, inputedColumn, 1 ) || isValid;  // Northeast
            isValid = TryDirection( board, player, inputedRow, -1, inputedColumn, -1 ) || isValid; // Northwest
            isValid = TryDirection( board, player, inputedRow, 1, inputedColumn, 1 ) || isValid;   // Southeast
            isValid = TryDirection( board, player, inputedRow, 1, inputedColumn, -1 ) || isValid;  // Southwest

            if( ! isValid ) board[ inputedRow, inputedColumn ] = " ";

            return isValid;
        }

        static bool TryDirection( string[ , ] board, Player player, int moveRow, 
            int deltaRow, int moveCol, int deltaCol )
        {
            int row = moveRow + deltaRow;
            int column = moveCol + deltaCol;

            if( row < 0 | row >= board.GetLength(0) ) return false;
            if( column < 0 | column >= board.GetLength(1) ) return false;

            if( board[ row, column ] == " " || board[ row, column ] == player.Symbol ) return false;   // must be opponent symbol

            int count = 1;
            bool endLoop = false;

            while( ! endLoop ) // finding # of flips if any are needed
            {
                row += deltaRow; 
                column += deltaCol;

                if( row < 0 | row >= board.GetLength(0) ) return false;
                if( column < 0 | column >= board.GetLength(1) ) return false;

                if( board[ row, column ] == " " ) return false;
                if( board[ row, column ] != player.Symbol ) count += 1;
                else endLoop = true;
            }
            // reinitialize row and column values

            row = moveRow;
            column = moveCol;

            for( int i = 0; i < count; i ++ )  // executing the appropriate number of flips
            {
                row += deltaRow; 
                column += deltaCol;
                board[ row, column ] = player.Symbol; // flip
            }

            return true;
        }

        static int GetScore( string[ , ] board, Player player )
        {
            int score = 0;

            for( int i = 0; i < board.GetLength( 0 ); i++ )
            {
                for( int j = 0; j < board.GetLength( 1 ); j++ )
                {
                    if( board[ i, j ] == player.Symbol ) score += 1;
                }
            }
            return score;
        }

        static void DisplayScores( string[ , ] board, Player[] players ) 
        {
            int playerOneScore = GetScore( board, players[ 0 ] );
            int playerTwoScore = GetScore( board, players[ 1 ] );

            WriteLine();
            WriteLine( $"Player {players[ 0 ].Name}'s score is {playerOneScore} ");
            WriteLine( $"Player {players[ 1 ].Name}'s score is {playerTwoScore} ");
            WriteLine();
        }
        
        static void DisplayWinners( string[ , ] board, Player[] players ) 
        {
            WriteLine();
            if( GetScore( board, players[ 0 ]) > GetScore( board, players[ 1 ]) )
            {
                int difference = GetScore( board, players[ 0 ]) - GetScore( board, players[ 1 ]);
                WriteLine( $"The winner is {players[ 0 ].Name} by a total of {difference} ");
            }
            else if( GetScore( board, players[ 0 ]) < GetScore( board, players[ 1 ]) )
            {
                int difference = GetScore( board, players[ 1 ]) - GetScore( board, players[ 0 ]);
                WriteLine( $"The winner is {players[ 1 ].Name} by a total of {difference} ");          
            }
            else WriteLine( "The score of both players is tied -- there are no winners!" );
        }

        // -----------------------------------------------------------------------------------------
        // Return the single-character string "a".."z" corresponding to its index 0..25. 
        // Return " " for an invalid index.
        
        static string LetterAtIndex( int number )
        {
            if( number < 0 || number > 25 ) return " ";
            else return "abcdefghijklmnopqrstuvwxyz"[ number ].ToString( );
        }
        
        // -----------------------------------------------------------------------------------------
        // Return the index 0..25 corresponding to its single-character string "a".."z". 
        // Return -1 for an invalid string.
        
        static int IndexAtLetter( string letter )
        {
            if( letter.Length != 1 ) return -1;
            else return "abcdefghijklmnopqrstuvwxyz".IndexOf( letter[ 0 ] );
        }
        
        // -----------------------------------------------------------------------------------------
        // Create a new Othello game board, initialized with four pieces in their starting
        // positions. The counts of rows and columns must be no less than 4, no greater than 26,
        // and not an odd number. If not, the new game board is created as an empty array.
        
        static string[ , ] NewBoard( int rows, int cols )
        {
            const string blank = " ";
            const string white = "O";
            const string black = "X";
            
            if(    rows < 4 || rows > 26 || rows % 2 == 1
                || cols < 4 || cols > 26 || cols % 2 == 1 ) return new string[ 0, 0 ];
                
            string[ , ] board = new string[ rows, cols ];
            
            for( int row = 0; row < rows; row ++ )
            {
                for( int col = 0; col < cols; col ++ )
                {
                    board[ row, col ] = blank;
                }
            }
            
            board[ rows / 2 - 1, cols / 2 - 1 ] = white;
            board[ rows / 2 - 1, cols / 2     ] = black;
            board[ rows / 2,     cols / 2 - 1 ] = black;
            board[ rows / 2,     cols / 2     ] = white;
            
            return board;
        }

        // -----------------------------------------------------------------------------------------
        // Display the Othello game board on the Console.
        // All information about the game is held in the two-dimensional string array.
        
        static void DisplayBoard( string[ , ] board )
        {
            const string h  = "\u2500"; // horizontal line
            const string v  = "\u2502"; // vertical line
            const string tl = "\u250c"; // top left corner
            const string tr = "\u2510"; // top right corner
            const string bl = "\u2514"; // bottom left corner
            const string br = "\u2518"; // bottom right corner
            const string vr = "\u251c"; // vertical join from right
            const string vl = "\u2524"; // vertical join from left
            const string hb = "\u252c"; // horizontal join from below
            const string ha = "\u2534"; // horizontal join from above
            const string hv = "\u253c"; // horizontal vertical cross
            const string mx = "\u256c"; // marked horizontal vertical cross
            const string sp =      " "; // space

            // Nothing to display?
            if( board == null ) return;
            
            int rows = board.GetLength( 0 );
            int cols = board.GetLength( 1 );
            if( rows == 0 || cols == 0 ) return;
            
            // Display the board row by row.
            for( int row = 0; row < rows; row ++ )
            {
                if( row == 0 )
                {
                    // Labels above top edge.
                    for( int col = 0; col < cols; col ++ )
                    {
                        if( col == 0 ) Write( "   {0}{0}{1}{0}", sp, LetterAtIndex( col ) );
                        else Write( "{0}{0}{1}{0}", sp, LetterAtIndex( col ) );
                    }
                    WriteLine( );
                    
                    // Border above top row.
                    for( int col = 0; col < cols; col ++ )
                    {
                        if( col == 0 ) Write( "   {0}{1}{1}{1}", tl, h );
                        else Write( "{0}{1}{1}{1}", hb, h );
                        if( col == cols - 1 ) Write( "{0}", tr );
                    }
                    WriteLine( );
                }
                else
                {
                    // Border above a row which is not the top row.
                    for( int col = 0; col < cols; col ++ )
                    {
                        if(    rows > 5 && cols > 5 && row ==        2 && col ==        2 
                            || rows > 5 && cols > 5 && row ==        2 && col == cols - 2 
                            || rows > 5 && cols > 5 && row == rows - 2 && col ==        2 
                            || rows > 5 && cols > 5 && row == rows - 2 && col == cols - 2 )  
                            Write( "{0}{1}{1}{1}", mx, h );
                        else if( col == 0 ) Write( "   {0}{1}{1}{1}", vr, h );
                        else Write( "{0}{1}{1}{1}", hv, h );
                        if( col == cols - 1 ) Write( "{0}", vl );
                    }
                    WriteLine( );
                }
                
                // Row content displayed column by column.
                for( int col = 0; col < cols; col ++ ) 
                {
                    if( col == 0 ) Write( " {0,-2}", LetterAtIndex( row ) ); // Labels on left side
                    Write( "{0} {1} ", v, board[ row, col ] );
                    if( col == cols - 1 ) Write( "{0}", v );
                }
                WriteLine( );
                
                if( row == rows - 1 )
                {
                    // Border below last row.
                    for( int col = 0; col < cols; col ++ )
                    {
                        if( col == 0 ) Write( "   {0}{1}{1}{1}", bl, h );
                        else Write( "{0}{1}{1}{1}", ha, h );
                        if( col == cols - 1 ) Write( "{0}", br );
                    }
                    WriteLine( );
                }
            }
        }

        static void Main( )
        {
            Welcome( );
            
            Player[ ] players = new Player[ ] 
            {
                NewPlayer( colour: "black", symbol: "X", defaultName: "Black" ),
                NewPlayer( colour: "white", symbol: "O", defaultName: "White" ),
            };
            
            int turn = GetFirstTurn( players, 0 );
           
            int rows = GetBoardSize( direction: "rows",    defaultSize: 8 );
            int cols = GetBoardSize( direction: "columns", defaultSize: 8 );
            
            string[ , ] game = NewBoard( rows, cols );
            
            // Play the game.
            
            bool gameOver = false;
            while( ! gameOver )
            {
                Welcome( );
                DisplayBoard( game ); 
                DisplayScores( game, players );
                
                string move = GetMove( players[ turn ] );
                if( move == "quit" ) gameOver = true;
                else
                {
                    bool madeMove = TryMove( game, players[ turn ], move );
                    if( madeMove ) turn = ( turn + 1 ) % players.Length;
                    else 
                    {
                        Write( " Your choice didn't work!" );
                        Write( " Press <Enter> to try again." );
                        ReadLine( ); 
                    }
                }
            }
            
            // Show fhe final results.
            
            DisplayWinners( game, players );
            WriteLine( );
        }
    }
}