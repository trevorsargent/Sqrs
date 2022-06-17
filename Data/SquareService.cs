namespace Sqrs.Data;

public class SquareService
{

    const int SIZE = 16;
    private readonly int[,] board = new int[SIZE, SIZE];

    private readonly Random r;
    private readonly Dictionary<int, Square> squares;

    private int IdCount = 0;

    public SquareService()
    {
        r = new Random();
        squares = new Dictionary<int, Square>();

        for (int c = 0; c < SIZE; c++)
        {
            for (int r = 0; r < SIZE; r++)
            {
                board[r, c] = -1;
            }
        }
    }



    public Square[] GetSquares()
    {
        return squares.Values.ToArray();
    }

    private void WriteToBoard(Square s)
    {
        for (int col = s.Col; col < s.Col + s.Size; col++)
        {
            for (int row = s.Row; row < s.Row + s.Size; row++)
            {
                board[row, col] = s.Id;
            }
        }

    }

    private void RemoveFromBoard(Square s)
    {
        for (int col = s.Col; col < s.Col + s.Size; col++)
        {
            for (int row = s.Row; row < s.Row + s.Size; row++)
            {
                board[row, col] = -1;
            }
        }
    }

    private bool OverlapsAny(Square s)
    {
        for (int col = s.Col; col < s.Col + s.Size; col++)
        {
            for (int row = s.Row; row < s.Row + s.Size; row++)
            {
                if (board[row, col] > -1 && board[row, col] != s.Id)
                {
                    return true;
                }
            }
        }

        return false;
    }


    public void AddSquare()
    {

        if (!HasRoomForSize(1))
        {
            Console.WriteLine("NO ROOM!");
            return;
        }
        Square s;
        do
        {
            s = new Square(IdCount, r.Next(SIZE), r.Next(SIZE));
        } while (OverlapsAny(s));
        SetSquare(s);
        Publish();
        IdCount++;
    }

    public void GrowSquare(int id)
    {
        Square s = squares[id];
        RemoveFromBoard(s);

        s.Grow();

        if (!IsInBounds(s) || OverlapsAny(s))
        {
            s.Shrink();
        }

        WriteToBoard(s);
        Publish();

    }

    public void CombineWinners()
    {

        for (int r = 0; r < SIZE; r++)
        {
            for (int c = 0; c < SIZE; c++)
            {
                if (board[r, c] < 0)
                {
                    continue;
                }

                Square s = squares[board[r, c]];

                if (s.Col + s.Size >= SIZE || s.Row + s.Size >= SIZE)
                {
                    continue;
                }

                int rightId = board[s.Row, s.Col + s.Size];
                int downId = board[s.Row + s.Size, s.Col];
                int diagId = board[s.Row + s.Size, s.Col + s.Size];

                if (rightId < 0 || downId < 0 || diagId < 0)
                {
                    continue;
                }

                Square rightSquare = squares[rightId];
                Square downSquare = squares[downId];
                Square diagSquare = squares[diagId];

                if (rightSquare.Size != s.Size || downSquare.Size != s.Size || diagSquare.Size != s.Size)
                {
                    continue;
                }

                RemoveFromBoard(rightSquare);
                RemoveFromBoard(downSquare);
                RemoveFromBoard(diagSquare);

                squares.Remove(rightId);
                squares.Remove(downId);
                squares.Remove(diagId);

                GrowSquare(s.Id);

            }
        }
    }

    public void ShrinkSquare(int id)
    {
        Square s = squares[id];
        RemoveFromBoard(s);
        s.Shrink();
        WriteToBoard(s);
        Publish();
    }


    public async void ShiftSquare(int id, int r, int c)
    {
        Square s = squares[id];

        RemoveFromBoard(s);
        s.Shift(r, c);

        if (!IsInBounds(s) || OverlapsAny(s))
        {
            s.Shift(r * -1, c * -1);
        }

        WriteToBoard(s);
        Publish();

        CombineWinners();

    }
    public void Publish()
    {
        SquareUpdate?.Invoke(this, squares.Values.ToArray());
    }

    public void SetSquare(Square square)
    {
        squares.Add(square.Id, square);
        WriteToBoard(square);
    }

    public bool HasRoomForSize(int size)
    {
        for (int r = 0; r < SIZE; r++)
        {
            for (int c = 0; c < SIZE; c++)
            {
                if (board[r, c] < 0)
                {

                    for (int rr = r; rr < r + size; rr++)
                    {
                        for (int cc = c; cc < c + size; cc++)
                        {
                            if (board[rr, cc] > -1)
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsInBounds(Square s)
    {
        return s.Col >= 0 && s.Col + s.Size <= SIZE && s.Row >= 0 && s.Row + s.Size <= SIZE;
    }

    public event EventHandler<Square[]>? SquareUpdate;

}
