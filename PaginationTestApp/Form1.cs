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
    public partial class Form1 : Form
    {
        private string cs= "Data Source = (local)\\SQLEXPRESS16; Initial Catalog=PaginationTestDesktop; Integrated Security = true";
        public Form1()
        {
            InitializeComponent();
            SetDefaults();
            BindRecords();
        }

        private void SetDefaults()
        {
            ddlPageSize.SelectedIndex = 1;
            txtCurrentPage.Text = txtTotalPage.Text = "1";
        }

        private void BindRecords()
        {
            int pageSize = int.Parse(ddlPageSize.Text);
            int currentPage = int.Parse(txtCurrentPage.Text);

            int offset = pageSize * (currentPage - 1);

            using (IDbConnection conxn = new SqlConnection(cs))
            {
                DynamicParameters paramList = new DynamicParameters();
                paramList.Add("@offset", offset);
                paramList.Add("@pagesize", pageSize);
                List<UserInfo> lstUsers = SqlMapper.Query<UserInfo>(conxn, "usp_GetUserRecords", param:paramList, commandType: CommandType.StoredProcedure).ToList();

                if (lstUsers.Count > 0)
                {
                    int totalRecords = lstUsers[0].total_rows;
                    txtTotalPage.Text = Math.Ceiling((decimal)totalRecords / pageSize).ToString();
                    dgvRecords.DataSource = lstUsers;
                }
            }
        }

        private void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCurrentPage.Text = "1";

            BindRecords();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            int currentPage = int.Parse(txtCurrentPage.Text);
            if (currentPage > 1)
            {
                txtCurrentPage.Text = (currentPage - 1).ToString();
                BindRecords();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int currentPage = int.Parse(txtCurrentPage.Text);
            if (currentPage < int.Parse(txtTotalPage.Text))
            {
                txtCurrentPage.Text = (currentPage + 1).ToString();
                BindRecords();
            }
        }

        private void txtCurrentPage_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                BindRecords();
            }
        }
    }


    public class UserInfo
    {
        public int total_rows { get; set; }
        public int SN { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string company_name { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string phone1 { get; set; }

    }
}
