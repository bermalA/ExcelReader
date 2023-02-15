using ExcelDataReader;
using System.Data;
using System.Data.SqlClient;
using Z.Dapper.Plus;

namespace Aran
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable table = tableCollection[comboBox1.SelectedItem.ToString()];
            if(table!= null)
            {
                List<MockData> datas = new List<MockData>();
                for(int i = 0; i < table.Rows.Count; i++)
                {
                    MockData mockData = new MockData();
                    mockData.id = Convert.ToInt32(table.Rows[i]["id"]);
                    mockData.first_name = table.Rows[i]["first_name"].ToString();
                    mockData.last_name = table.Rows[i]["last_name"].ToString();
                    mockData.email = table.Rows[i]["email"].ToString();
                    mockData.gender = table.Rows[i]["gender"].ToString();
                    mockData.ip_address = table.Rows[i]["ip_address"].ToString();
                    datas.Add(mockData);
                }
                mockDataBindingSource.DataSource = datas;
            }
        }

        DataTableCollection tableCollection;

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Excel 97-2003 Workbook|*.xls|ExcelWorkbook|*.xlsx" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = openFileDialog.FileName;
                    using(var stream = File.Open(openFileDialog.FileName,FileMode.Open, FileAccess.Read))
                    {
                            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                        using(IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                        {

                            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) =>new ExcelDataTableConfiguration(){UseHeaderRow= true}
                            });
                            tableCollection = result.Tables;
                            comboBox1.Items.Clear();
                            foreach(DataTable table in tableCollection)
                            {
                               comboBox1.Items.Add(table.TableName);
                            }
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string conn = "Server=localhost\\SQLEXPRESS;Database=DemoDatabase;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";
                DapperPlusManager.Entity<MockData>().Table("MockData");
                List<MockData> mockDatas = mockDataBindingSource.DataSource as List<MockData>;
                if(mockDatas != null)
                {
                    using (IDbConnection db = new SqlConnection(conn))
                    {
                        db.BulkInsert(mockDatas);
                    }
                }
                MessageBox.Show("Finish");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}