using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LianLianKan
{
    public class Rearrange
    {
        public Rearrange()
        { 
        }
        /// <summary>
        /// 按照规则重排表格
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public int[][] GetRearrangedTable(int[][] table, string rule)
        {
            string method = "Rearrange_" + rule;
            System.Reflection.MethodInfo mi = this.GetType().GetMethod(method, new Type[] { typeof(int[][]) });
            if (mi != null)
            {
                var _table = mi.Invoke(this, new object[] { table });
                if (_table != null)
                    return (int[][])_table;
            }
            return null;
        }

        #region 重排规则
         
        /// <summary>
        /// 向下
        /// </summary>
        public int[][] Rearrange_R02(int[][] table)
        {
            Rearrange_Down(table);
            return table;
        }


        /// <summary>
        /// 向左
        /// </summary>
        public int[][] Rearrange_R03(int[][] table)
        {
            Rearrange_Left(table);
            return table;
        }
        /// <summary>
        /// 上下分离
        /// </summary>
        public int[][] Rearrange_R04(int[][] table)
        {
            List<int[]> _table = new List<int[]>(table);
            int midindex = _table.Count / 2;
            int[][] _table_up = _table.GetRange(0, midindex).ToArray();
            int[][] _table_down = _table.GetRange(midindex, _table.Count - midindex).ToArray();

            Rearrange_Up(_table_up);
            Rearrange_Down(_table_down);

            _table.Clear();
            _table.AddRange(_table_up);
            _table.AddRange(_table_down);
            return _table.ToArray();
        }
        /// <summary>
        /// 左右分离
        /// </summary>
        public int[][] Rearrange_R05(int[][] table)
        {
            //转制矩阵 
            int[][] _transtable = TransTable(table);

            _transtable = Rearrange_R04(_transtable);
            _transtable = TransTable(_transtable);
            _transtable = TransTable(_transtable);
            _transtable = TransTable(_transtable);
            return _transtable;

        }
        /// <summary>
        /// 上下集中
        /// </summary>
        public int[][] Rearrange_R06(int[][] table)
        {
            List<int[]> _table = new List<int[]>(table);
            int midindex = _table.Count / 2;
            int[][] _table_up = _table.GetRange(0, midindex).ToArray();
            int[][] _table_down = _table.GetRange(midindex, _table.Count - midindex).ToArray();

            Rearrange_Up(_table_down);
            Rearrange_Down(_table_up);

            _table.Clear();
            _table.AddRange(_table_up);
            _table.AddRange(_table_down);
            return _table.ToArray();
        }

        /// <summary>
        /// 左右集中
        /// </summary>
        public int[][] Rearrange_R07(int[][] table)
        {
            //转制矩阵 
            int[][] _transtable = TransTable(table);
            _transtable = Rearrange_R06(_transtable);
            _transtable = TransTable(_transtable);
            _transtable = TransTable(_transtable);
            _transtable = TransTable(_transtable);
            return _transtable;
        }

        /// <summary>
        /// 上左下右
        /// </summary>
        public int[][] Rearrange_R08(int[][] table)
        {
            List<int[]> _table = new List<int[]>(table);
            int midindex = _table.Count / 2;
            int[][] _table_up = _table.GetRange(0, midindex).ToArray();
            int[][] _table_down = _table.GetRange(midindex, _table.Count - midindex).ToArray();

            Rearrange_Up(_table_up);
            Rearrange_Left(_table_up);

            Rearrange_Down(_table_down);
            Rearrange_Right(_table_down);

            _table.Clear();
            _table.AddRange(_table_up);
            _table.AddRange(_table_down);
            return _table.ToArray();
        }
        /// <summary>
        /// 左下右上
        /// </summary>
        public int[][] Rearrange_R09(int[][] table)
        {
            //转制矩阵 
            int[][] _transtable = TransTable(table);
            List<int[]> _table = new List<int[]>(_transtable);

            int midindex = _table.Count / 2;
            int[][] _table_up = _table.GetRange(0, midindex).ToArray();
            int[][] _table_down = _table.GetRange(midindex, _table.Count - midindex).ToArray();

            Rearrange_Up(_table_up);
            Rearrange_Right(_table_up);

            Rearrange_Down(_table_down);
            Rearrange_Left(_table_down);

            _table.Clear();
            _table.AddRange(_table_up);
            _table.AddRange(_table_down); 

            _transtable = _table.ToArray();
            _transtable = TransTable(_transtable);
            _transtable = TransTable(_transtable);
            _transtable = TransTable(_transtable);
            return _transtable;
        }

        /// <summary>
        /// 向外扩散
        /// </summary>
        public int[][] Rearrange_R10(int[][] table)
        {
            table = Rearrange_R04(table);
            table = Rearrange_R05(table);
            return table;
        }
        /// <summary>
        /// 向内集中
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public int[][] Rearrange_R11(int[][] table)
        {
            table = Rearrange_R06(table);
            table = Rearrange_R07(table);
            return table;
        }
        #region 默认排序
        //转制矩阵
        int[][] TransTable(int[][] _table)
        {
            int row = 0, col = 0;
            if (!IsTableOK(_table, ref row, ref col)) return null;
            int[][] _newtable = new int[col][];
            for (int c = 0; c < col; c++)
            {
                _newtable[c] = new int[row];
                for (int r = 0; r < row; r++)
                {
                    _newtable[c][r] = _table[r][c];
                }
            }
            return _newtable;
        }
        //检查表格有效性
        bool IsTableOK(int[][] _table, ref int row, ref int col)
        {
            if (_table == null)
                return false;
            if (_table.Length == 0)
                return false;
            if (_table[0].Length == 0)
                return false;
            row = _table.Length;
            col = _table[0].Length;
            return true;
        }
        /// <summary>
        /// 向上 
        /// </summary>
        void Rearrange_Up(int[][] _table)
        {
            int row = 0, col = 0;
            if (!IsTableOK(_table, ref row, ref col)) return;
            for (int r = 0; r < row; r++)//从上往下取每一行
            {
                for (int c = 0; c < col; c++)//从左往右取每一列
                {
                    if (_table[r][c] == 0 && r < row - 1)//判断当前格为空
                    {
                        for (int _vert = r + 1; _vert < row; _vert++)//从上往下取非空值填充
                        {
                            if (_table[_vert][c] > 0)
                            {
                                _table[r][c] = _table[_vert][c];
                                _table[_vert][c] = 0;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 向下 
        /// </summary>
        public void Rearrange_Down(int[][] _table)
        {
            int row = 0, col = 0;
            if (!IsTableOK(_table, ref row, ref col)) return;
            for (int r = row - 1; r >= 0; r--)//从下往上取每一行
            {
                for (int c = 0; c < col; c++)//从左往右取每一列
                {
                    if (_table[r][c] == 0 && r > 0)//判断当前格为空
                    {
                        for (int _vert = r - 1; _vert >= 0; _vert--)//从下往上取非空值填充
                        {
                            if (_table[_vert][c] > 0)
                            {
                                _table[r][c] = _table[_vert][c];
                                _table[_vert][c] = 0;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 向左
        /// </summary>
        void Rearrange_Left(int[][] _table)
        {
            int row = 0, col = 0;
            if (!IsTableOK(_table, ref row, ref col)) return;
            for (int c = 0; c < col; c++)//从左往右取每一列
            {
                for (int r = 0; r < row; r++)//从上往下取每一行
                {
                    if (_table[r][c] == 0 && c < col - 1)//判断当前格为空
                    {
                        for (int _hori = c + 1; _hori < col; _hori++)//从左往右取非空值填充
                        {
                            if (_table[r][_hori] > 0)
                            {
                                _table[r][c] = _table[r][_hori];
                                _table[r][_hori] = 0;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 向右
        /// </summary>
        void Rearrange_Right(int[][] _table)
        {
            int row = 0, col = 0;
            if (!IsTableOK(_table, ref row, ref col)) return;
            for (int c = col - 1; c >= 0; c--)//从左往右取每一列
            {
                for (int r = 0; r < row; r++)//从上往下取每一行
                {
                    if (_table[r][c] == 0 && c > 0)//判断当前格为空
                    {
                        for (int _hori = c - 1; _hori >= 0; _hori--)//从右往左取非空值填充
                        {
                            if (_table[r][_hori] > 0)
                            {
                                _table[r][c] = _table[r][_hori];
                                _table[r][_hori] = 0;
                                break;
                            }
                        }
                    }
                }
            }
        }

        #endregion
        #endregion
    }
}

