using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaginationTestApp
{

    public partial class PaginationLayout : Form
    {
        private string cs = "Data Source = (local)\\SQLEXPRESS16; Initial Catalog=PaginationTestDesktop; Integrated Security = true";

        int _pageSize = 0;
        int _currentPage = 1;
        int _totalPage = 1;

        public PaginationLayout()
        {
            InitializeComponent();
            dgvRecords.AutoGenerateColumns = false;
            SetDefaultSettings();
            GetRecords(true);
        }

        private void SetDefaultSettings()
        {
            ddlPageSize.SelectedIndex = 1;
            _pageSize = int.Parse(ddlPageSize.Text);
            _currentPage = 1;
            _totalPage = 1;
        }

        private void GetPageNumbers()
        {
            flpPageNumbers.Controls.Clear();

            if (_totalPage > 4)
            {
                AddButton(1);
                AddButton(2);
                AddComboBox();
                AddButton(_totalPage - 1);
                AddButton(_totalPage);
            }
            else
            {
                for (int i = 1; i <= _totalPage; i++)
                {
                    AddButton(i);
                }
            }
        }

        private void AddButton(int page)
        {
            Button btnPage = new Button();
            btnPage.Size = new Size(30, 30);
            btnPage.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btnPage.Text = page.ToString();
            btnPage.Cursor = Cursors.Hand;
            btnPage.Click += BtnPage_Click;

            flpPageNumbers.Controls.Add(btnPage);
        }

        private void BtnPage_Click(object sender, EventArgs e)
        {
            Button pageclicked = sender as Button;

            int pageNumber = int.Parse(pageclicked.Text);

            if (pageNumber != _currentPage)
            {
                _currentPage = pageNumber;
                GetRecords();
            }
        }

        private void AddComboBox()
        {
            ComboBox cboPages = new ComboBox();
            cboPages.Width = 50;
            cboPages.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPages.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            cboPages.Items.Add("...");
            cboPages.Cursor = Cursors.Hand;

            for (int i = 3; i < _totalPage - 1; i++)
            {
                cboPages.Items.Add(i);
            }
            cboPages.SelectedIndex = 0;
            cboPages.SelectedIndexChanged += CboPages_SelectedIndexChanged;

            flpPageNumbers.Controls.Add(cboPages);
        }

        private void CboPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cboPage = sender as ComboBox;

            if (cboPage.SelectedIndex != 0)
            {
                int pageNumber = int.Parse(cboPage.Text);

                if (pageNumber != _currentPage)
                {
                    _currentPage = pageNumber;
                    GetRecords();
                }

            }
            
        }

        private void GetRecords(bool pageSizeAltered = false)
        {
            using (IDbConnection conxn = new SqlConnection(cs))
            {
                int offset = _pageSize * (_currentPage - 1);

                DynamicParameters paramList = new DynamicParameters();
                paramList.Add("@offset", offset);
                paramList.Add("@pagesize", _pageSize);
                List<UserInfo> lstUsers = SqlMapper.Query<UserInfo>(conxn, "usp_GetUserRecords", param: paramList, commandType: CommandType.StoredProcedure).ToList();

                if (lstUsers.Count > 0)
                {
                    if (pageSizeAltered)
                    {
                        int totalRows = lstUsers[0].total_rows;

                        _totalPage = (int)Math.Ceiling((decimal)totalRows / _pageSize);

                        if (_totalPage > 1)
                        {
                            GetPageNumbers();
                            flpPagination.Visible = true;
                        }
                        else
                        {
                            flpPagination.Visible = false;
                        }
                    }

                    btnPrev.Enabled = _currentPage > 1 ? true : false;
                    btnNext.Enabled = _currentPage < _totalPage ? true : false;

                }
                else
                {
                    SetDefaultSettings();
                    flpPagination.Visible = false;
                }

                dgvRecords.DataSource = lstUsers;
            }
        }
        
        private void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_pageSize == 0)
            {
                _pageSize = int.Parse(ddlPageSize.Text);
                return;
            }

            if (int.Parse(ddlPageSize.Text) != _pageSize)
            {
                _currentPage = 1;
                _pageSize = int.Parse(ddlPageSize.Text);
                GetRecords(true);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                GetRecords();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPage)
            {
                _currentPage++;
                GetRecords();
            }
        }
    }
}
