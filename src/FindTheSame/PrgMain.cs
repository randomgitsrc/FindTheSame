using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO; 

namespace LianLianKan
{
    public class PrgMain
    {
        LinkTable m_link = new LinkTable();
        public PrgMain()
        {
            ReadRound();
            //RandomMap(12, 12, 10); 
            //AutoLink();
        }
        void ReadRound()
        {
            string _config = Environment.CurrentDirectory + @"\data\config.ini";
            string _round = SharpEx.IniOperator.GetIniValue(_config, "map", "round");
            _round = Path.GetDirectoryName(_config) + _round;
            if (!File.Exists(_round))
                throw new FileNotFoundException("文件未找到", _round);
            string[][] _roundcontent = SharpEx.FileParser.GetFileLinesSplitContent(_round, new char[] { ' ', '=' });
            for (int i = 0; i < _roundcontent.Length; i++)
            {
                string[] _line = _roundcontent[i];
                if (_line.Length != 4)
                    continue;
                string _map = Path.GetDirectoryName(_round) + "\\" + _line[0] + ".txt";
                if (!File.Exists(_map))//文件不存在
                    continue;
                int _difficult = 0;
                if (!int.TryParse(_line[1], out _difficult))
                    continue;
                string name = _line[3];
                SetMap(_map, _difficult);
                Console.WriteLine("\n\n\n--------------------- {0} -------------------------", name);
                AutoLink(_line[2]);
            }
        }
        void SetMap( string _map, int _difficult)
        {
            if (!File.Exists(_map))
                return;
            string[][] _strtable = SharpEx.FileParser.GetFileLinesSplitContent(_map, new char[] { ' ' });
            //转为int整形 
            int[][] _table = new int[_strtable.Length][];
            try
            {
                int _colnum = -1;
                for (int _row = 0; _row < _strtable.Length; _row++)
                {
                    int[] _line = new int[_strtable[_row].Length];

                    if (_colnum < 0)
                    {
                        _colnum = _strtable[_row].Length;
                    }
                    else
                    {
                        if (_line.Length != _colnum)
                            throw new ArgumentException();
                    }
                    _table[_row] = _line;
                    for (int _col = 0; _col < _strtable[_row].Length; _col++)
                    {
                        string _flag = _strtable[_row][_col];
                        _table[_row][_col] = (_flag == "#") ? 1 : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
            m_link.SetTable(_table, _difficult);
            m_link.GetNextRandomTable();

        }

        void RandomMap(int row, int col, int typenum)
        {
            //填充一张静态表
            if (typenum < 1) typenum = 1;
            if (row < 2) row = 2;
            if (col < 2) col = 2;
             
            int[][] _table = new int[row][];
            for (int r = 0; r < row; r++)
            {
                _table[r] = new int[col];
                for (int c = 0; c < col; c++)
                {
                    _table[r][c] = 1; 
                }
            }
            m_link.SetTable(_table, typenum);
            m_link.GetNextRandomTable();
        }
        void AutoLink(string _rearrange)
        {
            PrintTable();
            int ax, ay, bx, by;
            while (m_link.GetNextLinkedPoints(out ax, out ay, out bx, out by))
            {
                PrintTable(ax, ay, bx, by, false);
                RemovePoint(ax, ay, bx, by);
                m_link.RearrangeByRule(_rearrange);
            } 
            //PrintTable(); 
            if (m_link.GetNextRandomTable())
                AutoLink(_rearrange);

            //RandomMap(16, 16, 60);
            //AutoLink(); 
        }

        void PrintTable()
        {
            PrintTable(-1, -1, -1, -1, false);
        }

        void PrintTable(int ax, int ay, int bx, int by, bool easymode)
        { 
            int[][] _table = m_link.Table;
            //Console.WriteLine("\n-------------------------------------------------------------------\n");
            Console.WriteLine("\n");

            int _wlen = m_link.CellTypesNum.ToString().Length;//文本填充长度

            if (_table != null)
            {
                for (int r = 0; r < _table.Length; r++)
                {
                    Console.Write(" - ");

                    for (int c = 0; c < _table[r].Length; c++)
                    {
                        string _cell = _table[r][c].ToString();
                        if ((ax == c && ay == r) || (bx == c && by == r))
                        {
                            _cell = "x";
                            if (!easymode)
                                _cell = _cell.PadLeft(_wlen, 'x');
                        }
                        else
                        {
                            if (_cell == "0")
                                _cell = " ";
                            else
                            {
                                if (easymode)
                                {
                                    _cell = "o";
                                }
                            } 
                        } 
                        if (!easymode)
                        {
                            char _char = string.IsNullOrWhiteSpace(_cell) ? ' ': '0';
                            _cell = _cell.PadLeft(_wlen, _char);
                        }
                        Console.Write(_cell + " ");
                    }

                    Console.Write("-");
                    Console.WriteLine();
                }
            }
            //Console.WriteLine("\n-------------------------------------------------------------------\n");
        }

        void RemovePoint(int ax, int ay, int bx, int by)
        {
            m_link.RemovePoint(ay, ax);
            m_link.RemovePoint(by, bx);
        }

    } 
}
