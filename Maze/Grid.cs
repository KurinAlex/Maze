using System.Text;
using System.Collections;

namespace Maze;

public class Grid
{
    internal const int Up = 0, Right = 1, Down = 2, Left = 3;

    private static readonly int[] OPPOSITE = { Down, Left, Up, Right };

    private static readonly int[] X = { 0, 1, 0, -1 };
    private static readonly int[] Y = { -1, 0, 1, 0 };

    internal const int NoVertex = -1;

    private readonly int _width;
    private readonly int _height;
    private readonly BitArray edges;

    public Grid(int width, int height)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        _width = width;
        _height = height;
        edges = new(width * height * 4);
    }

    private static int Bit(int v, int dir) => 4 * v + dir;

    private bool IsColInRange(int col) => col >= 0 && col < _width;
    private bool IsRowInRange(int row) => row >= 0 && row < _height;
    internal int Vertex(int col, int row) => IsColInRange(col) && IsRowInRange(row) ? _width * row + col : NoVertex;
    private int Col(int v) => v % _width;
    private int Row(int v) => v / _width;
    private bool HasEdge(int v, int dir) => v == NoVertex || edges[Bit(v, dir)];

    internal int Neighbor(int v, int dir)
    {
        int col = Col(v) + X[dir], row = Row(v) + Y[dir];
        return IsColInRange(col) && IsRowInRange(row) ? Vertex(col, row) : NoVertex;
    }

    internal void AddEdge(int v, int dir)
    {
        edges[Bit(v, dir)] = true;

        int neighbour = Neighbor(v, dir);
        if (neighbour != NoVertex)
        {
            edges[Bit(neighbour, OPPOSITE[dir])] = true;
        }
    }

    public void AddEdge(int col, int row, int dir)
    {
        if (!IsColInRange(col))
        {
            throw new ArgumentOutOfRangeException(nameof(col));
        }

        if (!IsRowInRange(row))
        {
            throw new ArgumentOutOfRangeException(nameof(row));
        }

        AddEdge(Vertex(col, row), dir);
    }

    readonly Dictionary<byte, char> chars = new()
    {
        [0b0000] = ' ', //nothing

        [0b0001] = '╵', // up
        [0b0010] = '╶', // right
        [0b0100] = '╷', // down
        [0b1000] = '╴', // left

        [0b0011] = '└', // up + right
        [0b0101] = '│', // up + down
        [0b1001] = '┘', // up + left
        [0b0110] = '┌', // right + down
        [0b1010] = '─', // right + left
        [0b1100] = '┐', // down + left

        [0b0111] = '├', // up + right + down
        [0b1011] = '┴', // up + right + left
        [0b1101] = '┤', // up + down + left
        [0b1110] = '┬', // right + down + left

        [0b1111] = '┼', // all
    };

    private char GetChar(int luv, int ruv, int rdv, int ldv)
    {
        byte flag = 0b1111;
        if (HasEdge(luv, Right) && HasEdge(ruv, Left))
        {
            flag &= 0b1110;
        }
        if (HasEdge(ruv, Down) && HasEdge(rdv, Up))
        {
            flag &= 0b1101;
        }
        if (HasEdge(rdv, Left) && HasEdge(ldv, Right))
        {
            flag &= 0b1011;
        }
        if (HasEdge(ldv, Up) && HasEdge(luv, Down))
        {
            flag &= 0b0111;
        }
        return chars[flag];
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (int upRow = -1, downRow = 0; upRow < _height; upRow++, downRow++)
        {
            for (int col = -1; col < _width; col++)
            {
                sb.Append(GetChar(Vertex(col, upRow), Vertex(col + 1, upRow),
                    Vertex(col + 1, downRow), Vertex(col, downRow)));
                if (col != _width - 1)
                {
                    sb.Append(HasEdge(Vertex(col + 1, upRow), Down) && HasEdge(Vertex(col + 1, downRow), Up)
                        ? ' ' : '─');
                }
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
