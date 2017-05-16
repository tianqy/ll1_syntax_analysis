using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LL1
{
    public partial class Form1 : Form
    {
        Label lblTitle;
        Label lblGrammer;
        RichTextBox rtbGrammer;
        Label lblAnalysisTable;
        DataGridView dgvAnalysis;
        Label lblSymbolString;
        TextBox tbSymbolString;
        Button btnAnalysis;
        Label lblResult;
        DataGridView dgvResult;
        Label lblwarn;
        string symbolString="";    //输入串
        List<Process> processList = new List<Process>();

        public Form1()
        {
            InitializeComponent();
            initForm(); //初始化界面控件
            initData(); //初始化数据
            btnAnalysis.Click += new EventHandler(analysis);    //设置点击事件
        }

        //button点击事件，开始执行分析
        private void analysis(object sender, EventArgs e)
        {
            symbolString = tbSymbolString.Text.ToString();
            if (symbolString != "")
            {
                if (symbolString.ElementAt(symbolString.Length-1) != '#')
                {
                    symbolString += "#";
                }
            }
            else
            {
                MessageBox.Show("请先输入符号串");
                return;
            }

            processList.Clear();
            dgvResult.DataSource = null;
            //结果集DataGridView创建
            dgvResult.Location = new Point(470, 195);
            this.Controls.Add(dgvResult);
            //DataGridView属性
            dgvResult.Font = new Font("Consolas", 8, dgvAnalysis.Font.Style);
            dgvResult.RowHeadersVisible = false;

            Process process1 = new Process();
            process1.Index = 1;
            process1.SymbolStack = "#S";
            process1.InputString = symbolString;
            process1.Production = "S->A";
            processList.Add(process1);

            string stack_symbol = "#S";
            string input_str = symbolString;
            string production = "S->A";

            for (int i = 2; ; i++)
            {
                //得到新的符号栈stack_symbol
                if (production == "")
                {
                    //判断符号栈最末元素和输入串的初始元素是否相等，相等则规约，否则报错
                    if (stack_symbol[stack_symbol.Length - 1] == input_str[0])
                    {
                        stack_symbol = stack_symbol.Substring(0, stack_symbol.Length - 1);
                        input_str = input_str.Substring(1);
                    }
                    else
                    {
                        MessageBox.Show("查找失败");
                        showWarn(false);
                    }
                }
                else
                {
                    if (stack_symbol[stack_symbol.Length - 1] != '\'')
                    {
                        stack_symbol = stack_symbol.Substring(0, stack_symbol.Length - 1);
                    }
                    else stack_symbol = stack_symbol.Substring(0, stack_symbol.Length - 2);
                    string[] pro = production.Split('>');
                    stack_symbol += prod2symbol(pro[1]);
                }

                string symboltmp="";
                if(stack_symbol[stack_symbol.Length-1]!='\''){
                    symboltmp+=stack_symbol[stack_symbol.Length-1];
                } else symboltmp=stack_symbol[stack_symbol.Length-2]+""
                    +stack_symbol[stack_symbol.Length-1];
                string inputStrtmp="";
                inputStrtmp+=input_str[0];
                if (symboltmp == "#" && inputStrtmp == "#")
                {
                    production = "OK";
                    Process process = new Process();
                    process.Index = i;
                    process.SymbolStack = stack_symbol;
                    process.InputString = input_str;
                    process.Production = production;
                    processList.Add(process);
                    showWarn(true);
                    break;
                }
                else if (symboltmp == inputStrtmp)
                {
                    production = "";
                    Process process = new Process();
                    process.Index = i;
                    process.SymbolStack = stack_symbol;
                    process.InputString = input_str;
                    process.Production = production;
                    processList.Add(process);
                }
                else
                {
                    int r = 0, c = 0;
                    for (int row = 0; row <= 5; row++)
                    {
                        if (dgvAnalysis.Rows[row].Cells[0].Value.ToString().Equals(symboltmp))
                        {
                            r = row;
                            break;
                        }
                    }
                    for (int col = 1; col <= 6; col++)
                    {
                        if (dgvAnalysis.Columns[col].HeaderText.Equals(inputStrtmp))
                        {
                            c = col;
                            break;
                        }
                    }
                    if (r == 5 && c == 6)
                    {
                        MessageBox.Show("查找失败！");
                        showWarn(false);
                        break;
                    }
                    else
                    {
                        production = dgvAnalysis.Rows[r].Cells[c].Value.ToString();
                        if (production != "error")
                        {
                            Process process = new Process();
                            process.Index = i;
                            process.SymbolStack = stack_symbol;
                            process.InputString = input_str;
                            process.Production = production;
                            processList.Add(process);
                        }
                        else
                        {
                            Process process = new Process();
                            process.Index = i;
                            process.SymbolStack = stack_symbol;
                            process.InputString = input_str;
                            process.Production = "error!";
                            processList.Add(process);
                            MessageBox.Show("匹配失败");
                            showWarn(false);
                            break;
                        }
                    }
                }
            }
            if (processList.Count < 10)
            {
                int high = 24 * (processList.Count + 1)-processList.Count/2-1;
                dgvResult.Size = new Size(404, high);
            }
            else dgvResult.Size = new Size(420, 230);
            this.dgvResult.AllowUserToAddRows = false;
            this.dgvResult.DataSource = processList;
            dgvResult.Columns[0].HeaderText = "步骤";
            dgvResult.Columns[1].HeaderText = "符号栈S[i]";
            dgvResult.Columns[2].HeaderText = "输入串str[j]";
            dgvResult.Columns[3].HeaderText = "产生式";
            dgvResult.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private string prod2symbol(string p)
        {
            string stmp="";
            int len = p.Length;
            for (int i = len-1; i >=0; i--)
            {
                if (p[i] == '\'')
                {
                    stmp = stmp + p[i - 1] + p[i];
                    i--;
                }
                else if(p[i] != 'Ɛ')
                {
                    stmp += p[i];
                }
            }
            return stmp;
        }

        //窗体初始化
        void initForm()
        {
            this.BackColor = Color.FromArgb(0xee, 0xf7, 0xff);

            lblTitle = new Label();
            lblTitle.Size = new Size(175, 25);
            lblTitle.Location = new Point(270, 8);
            //lblTitle.Left = (this.ClientRectangle.Width - lblTitle.Width) / 2;
            //lblTitle.BringToFront();
            this.Controls.Add(lblTitle);

            lblGrammer = new Label();
            lblGrammer.Location = new Point(12, 36);
            lblGrammer.Size = new Size(40, 16);
            this.Controls.Add(lblGrammer);

            rtbGrammer = new RichTextBox();
            rtbGrammer.Location = new Point(12, 52);
            rtbGrammer.Size = new Size(200, 150);
            rtbGrammer.BackColor = Color.White;
            rtbGrammer.ReadOnly = true;
            this.Controls.Add(rtbGrammer);

            lblAnalysisTable = new Label();
            lblAnalysisTable.Location = new Point(12, 210);
            lblAnalysisTable.Size = new Size(56, 16);
            this.Controls.Add(lblAnalysisTable);

            dgvAnalysis = new DataGridView();
            dgvAnalysis.Location = new Point(12, 226);
            dgvAnalysis.Size = new Size(300, 140);
            this.Controls.Add(dgvAnalysis);

            lblSymbolString = new Label();
            lblSymbolString.Location = new Point(350, 36);
            lblSymbolString.Size = new Size(200, 16);
            this.Controls.Add(lblSymbolString);

            tbSymbolString = new TextBox();
            tbSymbolString.Location = new Point(350, 52);
            tbSymbolString.Size = new Size(250, 16);
            this.Controls.Add(tbSymbolString);

            btnAnalysis = new Button();
            btnAnalysis.Location = new Point(350, 90);
            btnAnalysis.Size = new Size(80, 24);
            btnAnalysis.BackColor = Color.White;
            btnAnalysis.ForeColor = Color.FromArgb(0x44, 0xaa, 0xff);
            this.Controls.Add(btnAnalysis);

            lblResult = new Label();
            lblResult.Location = new Point(350, 130);
            lblResult.Size = new Size(100, 16);
            this.Controls.Add(lblResult);

            dgvResult = new DataGridView();
            lblwarn = new Label();
            lblwarn.Size = new Size(60, 16);
            lblwarn.Location = new Point(470, 430);
        }

        //数据初始化
        void initData()
        {
            lblTitle.Font = new Font("Consolas", 18);
            lblTitle.Text = "LL(1)语法分析程序";
            this.Font = new Font("宋体", 12);
            lblGrammer.Text = "文法";
            rtbGrammer.Font = new Font("Consolas", 10, rtbGrammer.Font.Style);
            rtbGrammer.Text = "S->A\r\nA->BA'\r\nA'->iBA'|Ɛ\r\nB->CB'\r\nB'->+CB'|Ɛ\r\nC->)A*|(";
            lblAnalysisTable.Text = "分析表";
            dgvAnalysis.ColumnHeadersDefaultCellStyle.Font = new Font("Consolas", 8, dgvAnalysis.Font.Style);
            dgvAnalysis.RowsDefaultCellStyle.Font = new Font("Consolas", 8, dgvAnalysis.Font.Style);
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("(非)终结符", typeof(string)));
            dt.Columns.Add(new DataColumn("i", typeof(string)));
            dt.Columns.Add(new DataColumn("+", typeof(string)));
            dt.Columns.Add(new DataColumn("*", typeof(string)));
            dt.Columns.Add(new DataColumn("(", typeof(string)));
            dt.Columns.Add(new DataColumn(")", typeof(string)));
            dt.Columns.Add(new DataColumn("#", typeof(string)));
            DataRow dr;//行
            dr = dt.NewRow();
            dr[0] = "S";
            dr[1] = "error";
            dr[2] = "error";
            dr[3] = "error";
            dr[4] = "S->A";
            dr[5] = "S->A'";
            dr[6] = "error";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "A";
            dr[1] = "error";
            dr[2] = "error";
            dr[3] = "error";
            dr[4] = "A->BA'";
            dr[5] = "A->BA'";
            dr[6] = "error";
            dt.Rows.Add(dr); 

            dr = dt.NewRow();
            dr[0] = "A'";
            dr[1] = "A'->iBA'";
            dr[2] = "error";
            dr[3] = "A'->Ɛ";
            dr[4] = "error";
            dr[5] = "error";
            dr[6] = "A'->Ɛ";
            dt.Rows.Add(dr); 
            
            dr = dt.NewRow();
            dr[0] = "B";
            dr[1] = "error";
            dr[2] = "error";
            dr[3] = "error";
            dr[4] = "B->CB'";
            dr[5] = "B->CB'";
            dr[6] = "error";
            dt.Rows.Add(dr); 
            
            dr = dt.NewRow();
            dr[0] = "B'";
            dr[1] = "B'->Ɛ";
            dr[2] = "B'->+CB'";
            dr[3] = "B'->Ɛ";
            dr[4] = "error";
            dr[5] = "error";
            dr[6] = "B'->Ɛ";
            dt.Rows.Add(dr); 
            
            dr = dt.NewRow();
            dr[0] = "C";
            dr[1] = "error";
            dr[2] = "error";
            dr[3] = "error";
            dr[4] = "C->(";
            dr[5] = "C->(A*";
            dr[6] = "error";
            dt.Rows.Add(dr);
            dgvAnalysis.DataSource = dt;
            dgvAnalysis.Columns[0].Width = 55;
            dgvAnalysis.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            for (int i = 1; i < 7; i++)
            {
                dgvAnalysis.Columns[i].Width = 57;
                dgvAnalysis.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dgvAnalysis.RowHeadersVisible = false;
            dgvAnalysis.RowTemplate.Height = 25;
            dgvAnalysis.ColumnHeadersHeight = 35;
            dgvAnalysis.RowHeadersWidth = 50;
            dgvAnalysis.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAnalysis.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvAnalysis.AllowUserToAddRows = false; 
            dgvAnalysis.ReadOnly = true;

            lblSymbolString.Text = "请输入要分析的符号串，以#结束";

            btnAnalysis.Font = new Font(btnAnalysis.Font, FontStyle.Bold);
            btnAnalysis.Text = "开始分析";

            lblResult.Text = "分析结果:";

            
            
        }

        //提示查找失败label
        void showWarn(bool b){
            if (b)
            {
                lblwarn.Text = "成功!";
                lblwarn.ForeColor = Color.Green;
            }
            else
            {
                lblwarn.Text = "失败!";
                lblwarn.ForeColor = Color.Red;
            }
            this.Controls.Add(lblwarn);
        }
    }
}
