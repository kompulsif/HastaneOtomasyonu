using System.Data.SqlClient;

namespace HastaneOtomasyonu
{
    class SQLBaglantisi
    {
        SqlConnection baglan;
        public SqlConnection baglanti()
        {
            baglan = new SqlConnection("Data Source=localhost;Initial Catalog=HastaneProje;Integrated Security=True");
            baglan.Open();
            return baglan;
        }
        public void kapat()
        {
            try
            {
                baglan.Close();
            }
            catch { }
        }
    }
}
