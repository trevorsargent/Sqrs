namespace Sqrs.Data;

public class Square
{


    public Square(int id, int col, int row)
    {
        this.Id = id;
        this.Col = col;
        this.Row = row;
        this.Size = 1;
    }
    public int Id;

    public int Col { get; set; }

    public int Row { get; set; }

    public int Size { get; set; }

    public override string ToString()
    {
        return $"ID: {Id} - Row: {Row} - Col: {Col}";
    }

    public void Grow()
    {
        Size++;
    }

    public void Shrink()
    {
        if (Size > 1)
        {
            Size--;
        }
    }

    public string SizeClass()
    {
        return $"s{Size}";
    }

    public void Shift(int r, int c)
    {
        Col += c;
        Row += r;
    }

    public string SelectedClasses(int selected)
    {
        return selected == Id ? "selected" : "";
    }
}
