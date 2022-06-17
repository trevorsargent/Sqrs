namespace Sqrs.Data;

public class SquareService
{

    const int SIZE = 16;
    private int[,] board = new int[SIZE, SIZE];

    Random r;

    public SquareService()
    {
        r = new Random();
        for (int c = 0; c < SIZE; c++)
        {
            for (int r = 0; r < SIZE; r++)
            {
                board[r, c] = -1;
            }
        }
    }

    private Dictionary<int, Square> squares = new Dictionary<int, Square>();


    public Square[] GetSquares()
    {
        return squares.Values.ToArray();
    }

    private void writeToBoard(Square s)
    {
        for (int col = s.Col; col < s.Col + s.Size; col++)
        {
            for (int row = s.Row; row < s.Row + s.Size; row++)
            {
                board[row, col] = s.Id;
            }
        }

    }

    private void removeFromBoard(Square s)
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
            s = new Square(squares.Count, r.Next(SIZE), r.Next(SIZE));
        } while (OverlapsAny(s));
        SetSquare(s);
        Publish();
    }

    public void GrowSquare(int id)
    {
        Square s = squares[id];
        removeFromBoard(s);

        s.Grow();

        if (!IsInBounds(s) || OverlapsAny(s))
        {
            s.Shrink();
        }

        writeToBoard(s);

    }

    public void ShrinkSquare(int id)
    {
        Square s = squares[id];
        removeFromBoard(s);
        s.Shrink();
        writeToBoard(s);
    }
    public void Publish()
    {
        SquareUpdate?.Invoke(this, squares.Values.ToArray());
    }

    public void SetSquare(Square square)
    {
        squares.Add(square.Id, square);
        writeToBoard(square);
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
