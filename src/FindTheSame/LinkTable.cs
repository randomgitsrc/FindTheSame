using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LianLianKan
{
    public class LinkTable
    {
        #region 字段
        int m_rownum = 0;//行数
        int m_colnum = 0;//列数
        int[][] m_table = null;//表格
        int m_CellTypesNum = 0;

        Rearrange m_Rearrange  = new Rearrange();
        public int CellTypesNum
        {
            get { return m_CellTypesNum; }
            private set { m_CellTypesNum = value; }
        }

        /// <summary>
        /// 获得表格
        /// </summary>
        public int[][] Table
        {
            get {
                if (!m_IsValidTable)
                    return null;
                return m_table; }
            set { m_table = value; }
        }
        bool m_IsValidTable = false;//有效表格标识 
        #endregion

        public LinkTable()
        {
           
               
        }
        #region 公开方法

        /// <summary>
        /// 按行列顺序填充，单元格无值则填充-1
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cellTypesNum"></param>
        /// <returns></returns>
        public bool SetTable(int[][] table, int cellTypesNum)
        {
            m_IsValidTable = false;
            if (table != null)
            {
                if (table.Length > 0)
                {
                    m_rownum = table.Length;//行数 
                    m_colnum = table[0].Length;//列数

                    if (m_colnum > 0)
                    {
                        if (m_rownum * m_colnum % 2 == 0) //数据必须为2的整数倍
                        {
                            if (cellTypesNum > 0)
                            {
                                m_CellTypesNum = cellTypesNum;
                                m_table = table;
                                m_IsValidTable = true;
                            }
                        }
                    }
                }
            }
            if (!m_IsValidTable)
            {
                m_CellTypesNum = 0;
                m_rownum = 0;
                m_colnum = 0;
                m_table = null;
            }
            return m_IsValidTable;
        }

        /// <summary>
        /// 判断两点之间是否可连
        /// </summary>
        public bool IsPointsLinked(int ax, int ay, int bx, int by)
        {
            if (Link_Line(ax, ay, bx, by))
                return true;
            if (Link_C(ax, ay, bx, by))
                return true; 
            if (Link_CC(ax, ay, bx, by))
                return true;
            return false;
        }

        /// <summary>
        /// 移除点
        /// </summary>
        public bool RemovePoint(int ax, int ay)
        {
            try
            {
                m_table[ax][ay] = 0;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获得下个可连接的两点
        /// </summary>
        public bool GetNextLinkedPoints(out int ax, out int ay, out int bx, out int by)
        {
            ax = ay = bx = by = -1;
            //获得所有相同点与其所在位置的字典
            Dictionary<int, List<Point>> _DictTable = new Dictionary<int, List<Point>>();
            for (int r = 0; r < m_rownum; r++)
            //for (int r = m_rownum - 1; r >= 0; r--)
            {
                for (int c = 0; c < m_colnum; c++)
                //for (int c = m_colnum - 1; c >= 0; c--)
                {
                    int _cell = GetCell(c, r);
                    if (_cell > 0)
                    {
                        if (!_DictTable.ContainsKey(_cell))
                            _DictTable.Add(_cell, new List<Point>());
                        List<Point> _poss = _DictTable[_cell];
                        _poss.Add(new Point(c, r));
                    }
                }
            }
            foreach (int cell in _DictTable.Keys)
            {
                Point[] _poss = _DictTable[cell].ToArray();
                if (_poss.Length < 2)
                    continue;
                //依次循环比较
                for (int i = 0; i < _poss.Length-1; i++)
                {
                    //for (int j = _poss.Length - 1; j >= i; j--)
                    for (int j = i; j < _poss.Length; j++)
                    {
                        if (IsPointsLinked(_poss[i].X, _poss[i].Y, _poss[j].X, _poss[j].Y))
                        {
                            ax = _poss[i].X;
                            ay = _poss[i].Y;
                            bx = _poss[j].X;
                            by = _poss[j].Y;
                            return true; 
                        }
                    }
                }
                
            }
            return false;
        }

        public int GetFilledCellNum()
        {
            if (!m_IsValidTable)
                return 0;
            
            int num = 0;
            int[][] _table = m_table;
            for (int r = 0; r < _table.Length; r++)
            {
                for (int c = 0; c < _table[r].Length; c++)
                {
                    if (_table[r][c] != 0)
                        num++;
                }
            }
            return num;
        }
        
        /// <summary>
        /// 对当前表重新生成新表格
        /// </summary>
        /// <returns></returns>
        public bool GetNextRandomTable()
        {
            if (!m_IsValidTable)
                return false; 

            Random ran = new Random(DateTime.Now.Millisecond);
            List<int> seedslst = new List<int>();
            int _num = GetFilledCellNum() / 2; //获得非空单元格的数量，计算生成的键值数据量

            for (int i = 0; i < _num; i++)
            {
                seedslst.Add(ran.Next(1, m_CellTypesNum - 1));
            }
            seedslst.AddRange(seedslst.ToArray());//确保生成的是成对键的列表

            if (seedslst.Count == 0)
                return false;

            int[][] _table = m_table;
            int _ranvalue = 0;
            if (_table != null)
            {
                for (int r = 0; r < _table.Length; r++)
                {
                    for (int c = 0; c < _table[r].Length; c++)
                    {
                        if (_table[r][c] != 0)
                        { 
                            int _pos_ = ran.Next(0, seedslst.Count - 1);
                            _ranvalue = seedslst[_pos_]; 
                            _table[r][c] = _ranvalue;
                            seedslst.RemoveAt(_pos_);
                        } 
                    }
                }
            }
            return true;
        }

        //按照规则重新排列数据
        public bool RearrangeByRule(string rule)
        {
            //rule = "R02";
            if (string.IsNullOrEmpty(rule))
                return false;
            int[][] _table = (m_Rearrange.GetRearrangedTable(m_table, rule));
            if(_table!=null)
            {
                m_table = _table;
                return true;
            }
            else
                return false;  
        } 
         
        #endregion

        #region 私有方法
        
        //获得某行列单元格值
        public int GetCell(int col, int row)
        {
            if (!m_IsValidTable)
                return int.MinValue;
            try
            {
                if (row < 0 || row > m_rownum - 1)
                    return 0;
                if (col < 0 || col > m_colnum - 1)
                    return 0;
                int _value = m_table[row][col];
                return _value;
            }
            catch(Exception ex)
            {
                return -1;
            }
        }
        
        #region 连接规则
        //直线连接
        bool Link_Line(int ax, int ay, int bx, int by)
        {
            if (ax == bx)
            {
                return Is_ylink(ax, ay, by);
            }
            else if (ay == by)
            {
                return Is_xlink(ax, bx, ay);
            }
            return false;
        }

        //规则：一次折线连接
        bool Link_C(int ax, int ay, int bx, int by)
        {
            //转折点
            int xmid = -1;
            int ymid = -1;
            //水平—垂直方向
            xmid = bx;
            ymid = ay;
            //判断转折点是否有效
            if (GetCell(xmid, ymid) == 0)
            {
                if (Link_Line(ax, ay, xmid, ymid) && Link_Line(xmid, ymid, bx, by))
                    return true;
            }
            //垂直—水平方向
            xmid = ax;
            ymid = by;
            if (GetCell(xmid, ymid) == 0)
            {
                if (Link_Line(ax, ay, xmid, ymid) && Link_Line(xmid, ymid, bx, by))
                    return true;
            }
            return false;
        }
          
        //规则：二次折线连接
        bool Link_CC(int ax, int ay, int bx, int by)
        {
            List<int> arrow = new List<int>();
             
            //先水平—再折线
            arrow.AddRange(Get_xarrow(ax, -1, ay));
            arrow.AddRange(Get_xarrow(ax, m_colnum, ay));
            foreach (int x in arrow)
            {
                //第二折点
                if (Link_C(x, ay, bx, by))
                    return true;
            }
            arrow.Clear();
             
            //先垂直-再折线 
            arrow.AddRange(Get_yarrow(ax, ay, -1));
            arrow.AddRange(Get_yarrow(ax, ay, m_rownum));
            foreach (int y in arrow)
            {
                //第二折点
                if (Link_C(ax, y, bx, by))
                    return true;
            } 
            return false;
        }
        #endregion
        
        #region 两点连接
        //在x方向，顺序增点连接，返回断点位置
        int[] Get_xarrow(int ax, int bx, int y)
        {
            List<int> lst = new List<int>();
            int nn = (bx - ax > 0) ? 1 : -1;
            for (int x = ax + nn; nn == 1 ? x <= bx : x >= bx; x += nn)
            {
                if (GetCell(x, y) != 0 && x != bx)
                    break;
                lst.Add(x);
            }
            return lst.ToArray();
        }
        //判断x方向是否可连接
        bool Is_xlink(int ax, int bx, int y)
        {
            int[] arrow = Get_xarrow(ax, bx, y);
            if (arrow == null) return false;
            if (arrow.Length == 0) return false;
            bool _link = (arrow[arrow.Length - 1] == bx);
            
            return _link;
        }

        //在y方向，顺序增点连接，返回断点位置
        int[] Get_yarrow(int x, int ay, int by)
        {
            List<int> lst = new List<int>();
            int nn = (by - ay > 0) ? 1 : -1; 
            for (int y = ay + nn; nn == 1 ? y <= by : y >= by; y += nn)
            { 
                if (GetCell(x, y) != 0 && y != by)
                    break;
                lst.Add(y);
            }
            return lst.ToArray();
        }
        //判断y方向是否可连接
        bool Is_ylink(int x, int ay, int by)
        {
            int[] arrow = Get_yarrow(x, ay, by);
            if (arrow == null) return false;
            if (arrow.Length == 0) return false;
            bool _link = (arrow[arrow.Length - 1] == by);
             
            return _link;
        }
        #endregion
         
        #endregion
    }
}

